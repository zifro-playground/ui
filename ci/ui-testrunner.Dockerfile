
FROM gableroux/unity3d:2018.3.11f1-unity

# Install utils
RUN apt-get update \
    && apt-get install -y \
        git \
        curl \
        apt-transport-https \
        xmlstarlet

# Install powershell
RUN wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y powershell

COPY scripts/unity_login.sh /usr/local/bin/unity_login.sh
COPY scripts/unity_test.sh /usr/local/bin/unity_test.sh
COPY scripts/slack_notify_testrunner.sh /usr/local/bin/slack_notify_testrunner.sh
ADD https://raw.githubusercontent.com/nunit/nunit-transforms/master/nunit3-junit/nunit3-junit.xslt /usr/local/lib/nunit3-junit.xslt
COPY scripts/nunit2junit.ps1 /usr/local/bin/nunit2junit.ps1
COPY scripts/nunit2junit.sh /usr/local/bin/nunit2junit.sh