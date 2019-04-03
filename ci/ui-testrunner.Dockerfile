
FROM gableroux/unity3d:2018.3.11f1-unity

# Install git
RUN apt-get update && \
    apt-get install git -y

COPY scripts/unity_login.sh /usr/local/bin/unity_login.sh
COPY scripts/unity_test.sh /usr/local/bin/unity_test.sh
COPY scripts/slack_notify_testrunner.sh /usr/local/bin/slack_notify_testrunner.sh