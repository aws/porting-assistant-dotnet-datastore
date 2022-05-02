#!/usr/bin/env python3

"""
Copyright 2020 Amazon.com, Inc. or its affiliates. All Rights Reserved.
SPDX-License-Identifier: MIT-0

This file contains the logic for kerberos ticket renewal
as well as keytab file creation
IAM role to access Secrets Manager is required
"""

import sys
import traceback

import subprocess
import time
import json
import os
import re
import socket
from datetime import datetime
import boto3
from botocore.exceptions import ClientError
from ldap3 import Connection, SASL, KERBEROS
from ldap3.core.rdns import ReverseDnsSetting
import dns.resolver
import base64

"""
Constants
"""
DEFAULT_KERBEROS_REFRESH = 60 * 45
KEYTAB_FILE_NAME = "krb5.keytab"
CONF_FILE_NAME = "/etc/krb5.conf"
SECRET_ARN = "secret_arn"
DIRECTORY_NAME = "directory_name"
KRB5_CONF = "krb5.conf"
REGION_NAME = "region_name"
SERVICE_PRINCIPAL_NAME = "service_principal_name"
KRB_TICKET_REFRESH_PERIOD = "krb_ticket_refresh_period"
KRB_DIR = "krb_dir"
USERNAME_KEY = "username"
PASSWORD_KEY = "password"
MAX_FAILURES = 24
FAILURE_RETRY_PERIOD = "failure_retry_period"
DEFAULT_FAILURE_RETRY_PERIOD = 30


def get_secret(region_name_arg, secret_arn_arg):
    """
    Get secret from AWS Secrets Manager using IAM role,
    see  https://boto3.amazonaws.com/v1/documentation/api/latest/guide/secrets
    -manager.html
    :param region_name_arg: Region name of current container
    :type region_name_arg: basestring such as "us-west-1"
    :param secret_arn_arg: Secret ARN for AWS Secrets Manager secret
    :type secret_arn_arg: basestring such as "arn:aws:secretsmanager:us-west-1...
    :return: username, password, domain in str or None if there is an error
    :rtype: tuple of 3 str or 3 None
    """

    session = boto3.session.Session()
    client = session.client(
        service_name='secretsmanager',
        region_name=region_name_arg,
    )

    try:
        get_secret_value_response = client.get_secret_value(
            SecretId=secret_arn_arg
        )
        # Secrets Manager decrypts the secret value using the associated KMS CMK
        # Depending on whether the secret was a string or binary, only one of
        # these fields will be populated
        if 'SecretString' in get_secret_value_response:
            text_secret_data = get_secret_value_response['SecretString']
            secret = text_secret_data
        else:
            binary_secret_data = get_secret_value_response['SecretBinary']
            secret = binary_secret_data

        try:
            secret_dict = json.loads(secret)
        except ValueError as _:
            print("ERROR* Secret doesn't contain valid JSON", flush=True)
        try:
            username = secret_dict[USERNAME_KEY]
        except KeyError as _:
            print("ERROR* Secret doesn't contain username", flush=True)
        try:
            password = secret_dict[PASSWORD_KEY]
        except KeyError as _:
            print("ERROR* Secret doesn't contain password", flush=True)
        domain = secret_dict.get(DIRECTORY_NAME)
        krb5_conf = secret_dict.get(KRB5_CONF)
        # Missing values are handled in the caller
        return username, password, domain, krb5_conf
    except ClientError as e:
        if e.response['Error']['Code'] == 'ResourceNotFoundException':
            print("The requested secret " + secret_arn_arg + " was not found",
                  flush=True)
            # Retry this because the secret can be created later
            return None, None, None, None
        elif e.response['Error']['Code'] == 'InvalidRequestException':
            print("The request was invalid due to:", e, flush=True)
            raise  # there is no point to retry because there is nothing that can change
        elif e.response['Error']['Code'] == 'InvalidParameterException':
            print("The request had invalid params:", e, flush=True)
            raise  # there is no point to retry because there is nothing that can change
        elif e.response['Error']['Code'] == 'DecryptionFailure':
            print(
                "The requested secret can't be decrypted using the provided KMS "
                "key:",
                e, flush=True)
            raise  # there is no point to retry because there is nothing that can change
        elif e.response['Error']['Code'] == 'InternalServiceError':
            print("An error occurred on service side:", e, flush=True)
            # Retry this, the service can fix itself
            return None, None, None, None
        elif e.response['Error']['Code'] == 'AccessDeniedException':
            print(f"Access denied when reading secret {secret_arn_arg}. Check your container execution role:",
                  e, flush=True)
            # Retry this, they can fix the role without restarting
            return None, None, None, None
        # All other exceptions will be caught in the caller
        raise


def get_dc_server_names(directory_name_arg):
    """
    Get DNS servers that resolve to directory name
    :param directory_name_arg: Directory name such as example.com
    :type directory_name_arg: basestring
    :return: list of DNS server names
    :rtype: basestring
    """
    # Find IP of DNS server
    ip_list = dns.resolver.resolve(directory_name_arg)
    # Get server name of DNS server, such as server.example.com
    if ip_list is None or len(ip_list) == 0:
        print("**ERROR DNS resolution failed for %s" % directory_name_arg)
        raise NameError("**ERROR DNS resolution failed for %s" % directory_name_arg)

    server_names = []
    for ip in ip_list:
        name, _, _ = socket.gethostbyaddr(str(ip))
        server_names.append(name)

    return server_names


def create_keytab(username_arg, password_arg, directory_name_arg,
                  spn_arg, keytab_filename):
    """
    Creates kerberos keytab file in krb_dir_arg/krb5.keytab, such as
    /var/scratch/krb5.keytab
    Keytab file must be protected.
    Throws exception if keytab creation fails.
    :param username_arg: Username in Active Directory domain
    :type username_arg: basestring
    :param password_arg: Plain text password of above user
    :type password_arg: basestring
    :param directory_name_arg: Directory name of AD domain such as example.com
    :type directory_name_arg: basestring
    :param spn_arg: SPN such as HTTP://<hostname>:<port>
    :type spn_arg: basestring
    :param keytab_filename: file location of keytab
    :type keytab_filename: basestring
    :rtype: nothing
    """

    server_names = get_dc_server_names(directory_name_arg)

    keytab_creation_status = False
    for server in server_names:
        # Create keytab file
        print("Server name = " + server, flush=True)
        with subprocess.Popen(
                [
                    "msktutil", "create", "--use-service-account", "--service",
                    spn_arg,
                    "--account-name", username_arg,
                    "--server", server,
                    "-N", "--dont-change-password",
                    "--old-account-password", password_arg,
                    "--password", password_arg, "-k",
                    keytab_filename,
                    "--realm", directory_name_arg.upper()
                    # "--verbose"
                ],
                stdin=subprocess.PIPE,
                stdout=subprocess.PIPE,
                encoding="utf-8",
                shell=False
        ) as proc:
            output, error = proc.communicate(timeout=30)
            if proc.returncode != 0:
                print(
                    "keytab file create failed %d %s %s %s" % (
                        proc.returncode, error,
                        output, server), flush=True)
                continue
            else:
                print("keytab file created " + output, flush=True)
                with subprocess.Popen(
                        [
                            "kinit", "-kt", keytab_filename,
                            "-S",
                            spn_arg,
                            username_arg + "@" + directory_name_arg.upper()
                        ],
                        stdin=subprocess.PIPE,
                        stdout=subprocess.PIPE,
                        encoding="utf-8",
                        shell=False
                ) as proc:
                    output, error = proc.communicate(timeout=30)
                    if proc.returncode != 0:
                        print(
                            "keytab file verification failed %d %s %s %s" % (
                                proc.returncode, error,
                                output, server), flush=True)
                        raise NameError("ERROR** Keytab verification failed")
                    else:
                        print("Keytab file validated", flush=True)
                        keytab_creation_status = True
                        break

    if not keytab_creation_status:
        raise NameError("ERROR** keytab creation failed")
    return


def update_krb5_conf(directory_name_arg: str, krb5_conf_filename: str) -> None:
    """
    Replaces realm and domain names in krb5.conf with the parameter
    Throws exception if keytab creation fails.
    :param directory_name_arg: Directory name of AD domain such as example.com
    :param krb5_conf_filename: file location of krb5.conf
    """
    with open(krb5_conf_filename, 'r') as krb5_conf_file:
        lines = krb5_conf_file.readlines()
    for i, line in enumerate(lines):
        lines[i] = line.replace(
            "{REPLACE_WITH_DEFAULT_REALM}", directory_name_arg.upper()).replace(
                "{REPLACE_WITH_DOMAIN_NAME}", directory_name_arg)
    with open(krb5_conf_filename, 'r') as krb5_conf_file:
        krb5_conf_file.writelines(lines)


def execute_kinit_cmd(username_arg, password_arg, directory_name_arg):
    """
    Get kerberos tickets by executing the kinit command
    Kerberos tickets get saved to ticket cache specified by default_ccache_name in
    krb5.conf
    Kerberos tickets must be protected

    :param username_arg: Username in Active Directory domain
    :type username_arg: basestring
    :param password_arg: Plain text password of above user
    :type password_arg: basestring
    :param directory_name_arg: Directory name of AD domain such as example.com
    :type directory_name_arg: basestring
    :return: Kinit output visible in console
    :rtype: Nothing, also does not throw exceptions to allow retries
    """
    with subprocess.Popen(
            [
                "kinit", "-V",
                "%s@%s" % (username_arg, directory_name_arg.upper()),
            ],
            stdin=subprocess.PIPE,
            stdout=subprocess.PIPE,
            encoding="utf-8",
            shell=False,
    ) as proc:
        output, error = proc.communicate(input="%s\n" % password_arg, timeout=15)
        if proc.returncode != 0:
            print("ERROR** kinit failed %d %s %s" % (proc.returncode, error,
                                                     output), flush=True)
            raise NameError("ERROR** kinit failed")

    return


def read_env():
    """
    Environment variables can be set in Dockerfile
    :return: Environment variables
    :rtype: Dictionary
    """
    secret_arn = os.environ.get('CREDENTIALS_SECRET_ARN')
    directory_name = os.environ.get('DOMAIN_NAME')
    region_name = os.environ.get('AWS_REGION')
    service_principal_name = os.environ.get('SERVICE_PRINCIPAL_NAME')
    try:
        krb_ticket_refresh_period = int(os.environ.get("KRB_TICKET_REFRESH_PERIOD_IN_SECS"))
    except (TypeError, ValueError) as _:
        print(f"Invalid value for env KRB_TICKET_REFRESH_PERIOD_IN_SECS, using default {DEFAULT_KERBEROS_REFRESH}")
        krb_ticket_refresh_period = DEFAULT_KERBEROS_REFRESH
    try:
        failure_retry_period = int(os.environ.get("FAILURE_RETRY_PERIOD"))
    except (TypeError, ValueError) as _:
        print(f"Invalid value for env FAILURE_RETRY_PERIOD, using default {DEFAULT_FAILURE_RETRY_PERIOD}")
        failure_retry_period = DEFAULT_FAILURE_RETRY_PERIOD
    krb_dir = os.getenv("KRB_DIR")

    if not secret_arn or not directory_name or not region_name or not krb_dir \
            or not service_principal_name:
        print("*ERROR* : All parameters are not filled out", flush=True)
        print("secret_arn = {0}, directory_name = {1}, region_name = {2}, krb_dir "
              "= {3}, service_principal_name = {4}".format(secret_arn,
                                                           directory_name,
                                                           region_name, krb_dir,
                                                           service_principal_name),
              flush=True)
        raise NameError("*ERROR* : All parameters are not filled out")

    # Protect against script injection
    invalid_chars = re.compile(r"[$`()]")
    if invalid_chars.search(secret_arn) or invalid_chars.search(directory_name) or \
            invalid_chars.search(region_name) or \
            invalid_chars.search(krb_dir):
        print("*ERROR* Invalid characters detected", flush=True)
        print("secret_arn = {0}, directory_name = {1}, region_name = {2}, krb_dir "
              "= {3}".format(secret_arn,
                             directory_name,
                             region_name, krb_dir), flush=True)
        raise NameError("*ERROR* : Invalid characters detected")

    print('secret arn = ' + secret_arn, flush=True)
    print('directory name = ' + directory_name, flush=True)
    print('region name = ' + region_name, flush=True)
    print('kerberos ticket renewal time = ' + str(krb_ticket_refresh_period),
          flush=True)
    print('kerberos directory = ' + krb_dir, flush=True)

    env_vars = {SECRET_ARN: secret_arn, DIRECTORY_NAME: directory_name,
                REGION_NAME: region_name,
                SERVICE_PRINCIPAL_NAME: service_principal_name,
                KRB_TICKET_REFRESH_PERIOD: krb_ticket_refresh_period,
                FAILURE_RETRY_PERIOD: failure_retry_period,
                KRB_DIR: krb_dir}
    return env_vars


def check_ldap_info(env_vars):
    """
    Sanity check of user info from LDAP
    :param env_vars: Environment variables
    :type env_vars: Dictionary
    :return: Prints warnings, no return value
    :rtype: nothing
    """

    ldap_check_status = False
    # Kerberos ticket must be available since the query is over Kerberos
    print("Start LDAP check", flush=True)

    servers = get_dc_server_names(env_vars[DIRECTORY_NAME])
    servers += [env_vars[DIRECTORY_NAME]]

    for server in servers:
        server = "ldap://" + server
        print("DNS server name = " + server, flush=True)
        try:
            conn = Connection(server, sasl_credentials=(
                ReverseDnsSetting.REQUIRE_RESOLVE_ALL_ADDRESSES,),
                              authentication=SASL,
                              sasl_mechanism=KERBEROS, auto_bind=True)
            ldap_filter = '(objectclass=group)'
            ldap_attrs = ["cn", "sn", "givenName"]
            distinguished_name_list = env_vars[DIRECTORY_NAME].split('.')
            distinguished_name = ""
            for name in distinguished_name_list:
                distinguished_name += "dc=" + name + ','
            distinguished_name = distinguished_name.strip(',')
            print("Getting LDAP info for DN = " + distinguished_name)
            conn.search(distinguished_name, ldap_filter, attributes=ldap_attrs)
            for entry in conn.entries:
                if re.findall("Administrator", str(entry)):
                    print("WARNING: User in administrator group: " + str(entry),
                          flush=True)
            print("LDAP check done", flush=True)
            ldap_check_status = True
            break
        except Exception as _:
            print("LDAP check failed using DNS server = " + server, flush=True)
            continue
    if not ldap_check_status:
        print("Warning** LDAP check failed", flush=True)
        raise NameError("Warning**: LDAP check failed")
    else:
        print("LDAP check succeeded")
    return


def main():
    """
    Entrypoint of kerberos sidecar
    :return: Will return only if there is error
    :rtype: Exceptions on error
    """
    env_vars = read_env()

    username = None
    password = None
    domain = None

    """
    Kerberos ticket refresh every DEFAULT_KERBEROS_REFRESH
    The grace period for Kerberos even if passwords change, is about an hour
    DEFAULT_KERBEROS_REFRESH is set to 45 minutes
    """
    keytab_filename = env_vars[KRB_DIR] + "/" + KEYTAB_FILE_NAME
    num_failures = 0
    while True:
        if num_failures > MAX_FAILURES:
            print("ERROR** Max failures reached, exiting", flush=True)
            sys.exit(1)
        try:
            # get_secret returns None for username and/or password in cases where retry makes sense, like
            # secret not found, and returns None for username and password
            username_new, password_new, domain_new, krb5_conf = get_secret(env_vars[REGION_NAME], env_vars[SECRET_ARN])
            print(f"Got username {username_new} password {password_new} and domain name {domain_new} from secret")

            # Write krb5.conf if provided via secret
            if krb5_conf:
                with open(CONF_FILE_NAME, "w") as f:
                    f.write(base64.b64decode(krb5_conf)

            if username_new is not None and password_new is not None:
                if domain_new is not None:
                    env_vars[DIRECTORY_NAME] = domain_new
                if domain != env_vars[DIRECTORY_NAME]:
                    domain = env_vars[DIRECTORY_NAME]
                    update_krb5_conf(CONF_FILE_NAME, env_vars[DIRECTORY_NAME])
                execute_kinit_cmd(username_new, password_new, env_vars[DIRECTORY_NAME])

                # Only create keytab if service principal is set in env
                if env_vars[SERVICE_PRINCIPAL_NAME] and (username_new != username or password_new != password):
                    print(
                        "Credentials change detected at " + str(datetime.now()) +
                        "creating a new keytab file", flush=True)
                    if os.path.isfile(keytab_filename):
                        os.remove(keytab_filename)
                    username = username_new
                    password = password_new
                    create_keytab(username, password, env_vars[DIRECTORY_NAME],
                                  env_vars[SERVICE_PRINCIPAL_NAME], keytab_filename)
                num_failures = 0
                time.sleep(env_vars[KRB_TICKET_REFRESH_PERIOD])
            else:
                time.sleep(env_vars[FAILURE_RETRY_PERIOD])
        except Exception as _:
            num_failures = num_failures + 1
            print("ERROR** JSON error while loading secrets from secrets manager",
                  flush=True)
            exc_type, exc_value, exc_traceback = sys.exc_info()
            traceback.print_tb(exc_traceback, limit=1, file=sys.stdout)
            traceback.print_exception(exc_type, exc_value, exc_traceback,
                                      limit=5, file=sys.stdout)
            traceback.print_exc(limit=5, file=sys.stdout)
            time.sleep(env_vars[FAILURE_RETRY_PERIOD])


if __name__ == "__main__":
    main()
