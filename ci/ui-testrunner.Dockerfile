
FROM gableroux/unity3d:2018.3.11f1-unity

# Add trusted sources
RUN mkdir -p ~/.ssh \
    # Fingerprint to github.com
    && echo "|1|/olfPnpeGUgKkhLlwJSy4ro3Hb8=|tXqFHMrM0PWDFuQmABAg7zYndWc= ssh-rsa AAAAB3NzaC1yc2EAAAABIwAAAQEAq2A7hRGmdnm9tUDbO9IDSwBK6TbQa+PXYPCPy6rbTrTtw7PHkccKrpp0yVhp5HdEIcKr6pLlVDBfOLX9QUsyCOV0wzfjIJNlGEYsdlLJizHhbn2mUjvSAHQqZETYP81eFzLQNnPHt4EVVUh7VfDESU84KezmD5QlWpXLmvU31/yMf+Se8xhHTvKSCZIFImWwoG6mbUoWf9nzpIoaSjB+weqqUUmpaaasXVal72J+UX2B+2RPW3RcT0eOzQgqlJL3RKrTJvdsjE3JEAvGq3lGHSZXy28G3skua2SmVi/w4yCE6gbODqnTWlg7+wC604ydGXA8VJiS5ap43JXiUFFAaQ==" \
        >> ~/.ssh/known_hosts

# Install utils
RUN apt-get update \
    && apt-get install -y \
        git \
        curl \
        apt-transport-https \
        xmlstarlet \
        jq
        
# Install powershell
# /Requires apt-transport-https/
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
COPY scripts/unity_update_deps.sh /usr/local/bin/unity_update_deps.sh
