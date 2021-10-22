FROM ubuntu:latest

RUN DEBIAN_FRONTEND=noninteractive apt-get update -y

# The side-car uses python-boto3 for the side-car program, krb5 for kerberos and msktutil for keytab creation
# msktutil requires dnsutils, ldap support requires ldap3, gssapi and libkrb5-dev
RUN DEBIAN_FRONTEND=noninteractive apt-get install -y python3-pip krb5-user msktutil dnsutils libkrb5-dev python3-gssapi
RUN DEBIAN_FRONTEND=noninteractive apt-get autoremove

RUN pip3 install boto3 subprocess.run ldap3 gssapi dnspython

COPY krb5.conf /etc/krb5.conf

# Side-car source code
COPY krb_side_car.py /
RUN chmod +x /krb_side_car.py

VOLUME ["/var/scratch"]

#ENV CREDENTIALS_SECRET_ARN="Enter ARN value starting arn:aws:secretsmanager:..."
ENV CREDENTIALS_SECRET_ARN={REPLACE_WITH_AWS_SECRET_MANAGER_ARN_STRING}
#ENV DOMAIN_NAME="Enter domain such as EXAMPLE.COM"
ENV DOMAIN_NAME={REPLACE_WITH_DOMAIN_NAME_STRING}

# Keep this for flushing logs to CloudWatch
ENV PYTHONUNBUFFERED=1

#ENV SERVICE_PRINCIPAL_NAME="SPN such as HTTP/ip-172-31-30-50.example.com"
ENV SERVICE_PRINCIPAL_NAME={REPLACE_WITH_SERVICE_PRINCIPAL_NAME_STRING}

#ENV KRB_DIR="Directory kerberos tickets and keytab are saved in this directory such as /var/scratch"
ENV_KRB_DIR="/var/scratch"
# this must be the same directory for default_ccache_name and default_keytab_name in krb5.conf.
# This directory must also be shared with the app container"

#ENV KRB_TICKET_REFRESH_PERIOD_IN_SECS="Refresh kerberos tickets every hour or less, such as "60*45""
ENV KRB_TICKET_REFRESH_PERIOD_IN_SECS="60*45"


CMD ["python3", "/krb_side_car.py"]
