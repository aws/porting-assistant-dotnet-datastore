#!/usr/bin/env python3

import sys
import krb_side_car

# Basic tests of utility functions in krb_side_car.py

got_exception = False
try:
  krb_side_car.get_secret("us-west-1","non_secret_arn")
except:
  got_exception = True

if not got_exception:
   print("**ERROR** [Test] get_secret exception test failed")
   sys.exit(1)

print("[Test] get_secret exception test passed")

hostname = krb_side_car.get_dc_server_name("info.cern.ch")
if hostname != 'webafs706.cern.ch':
   print("**ERROR** [Test] failed")
   sys.exit(1)

print("[Test] get_dc_server_name hostname check passed")

got_exception = False
try:
   krb_side_car.create_keytab("username", "nonexistent_password", "non-existent-dir",
                  "non-existent-spn", "/tmp/mykeytabfile")
except:
   got_exception = True

if not got_exception:
   print("**ERROR** [Test] create_keytab exception test failed")
   sys.exit(1)

print("[Test] create_keytab exception test passed")

got_exception = False
try:
   krb_side_car.execute_kinit_cmd("username", "non-existent-password", "non-existent-dir")
except:
   got_exception = True

if not got_exception:
   print("**ERROR** [Test] execute_kinit_cmd exception test failed")
   sys.exit(1)

print("[Test] execute_kinit_cmd exception test passed")


env_vars = {}
if krb_side_car.check_ldap_info(env_vars):
   print("**ERROR [Test] check_ldap_info exception test failed")
   sys.exit(1)

print("[Test] check_ldap_info exception test passed")

print("All tests passed")
sys.exit(0)
