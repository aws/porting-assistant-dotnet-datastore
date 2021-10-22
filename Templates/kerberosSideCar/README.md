# Kerberos Side-Car Container

This container creates keytab files, refreshes kerberos tickets in a docker volume that can be shared with an application container and a LDAP sanity check of the AD user.
Keytab files and kerberos tickets are used for Windows Authentication in Active Directory.

## Container support

Only Docker is supported, this container is tested on AWS ECS.

## Prerequisites

AD user in Active Directory, like AWS Directory Service.
Entry for AD username and password must be created in AWS Secrets Manager, container must have IAM role sufficient to read the entry.
Container must be launched in same VPC as Active Directory.
DNS (DHCP option-set or otherwise) must be setup so that the side-car container can reach the Active Directory domain.

## Installation

Create a multi-container docker application with a web application container and this side-car container.
A shared volume is required to share keytab file and kerberos ticket between the web application container
and this side-car container as shown in https://aws.amazon.com/blogs/containers/using-windows-authentication-with-linux-containers-on-amazon-ecs/.
Environment variables in the Dockerfile and krb5.conf must be modified before launching docker containers.

## License

Copyright 2021 Amazon.com, Inc. or its affiliates. All Rights Reserved.
SPDX-License-Identifier: MIT-0
