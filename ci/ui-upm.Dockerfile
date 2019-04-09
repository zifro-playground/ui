
FROM ubuntu:16.04

# Add trusted sources
RUN mkdir -p ~/.ssh \
    # Fingerprint to github.com
    && echo "|1|/olfPnpeGUgKkhLlwJSy4ro3Hb8=|tXqFHMrM0PWDFuQmABAg7zYndWc= ssh-rsa AAAAB3NzaC1yc2EAAAABIwAAAQEAq2A7hRGmdnm9tUDbO9IDSwBK6TbQa+PXYPCPy6rbTrTtw7PHkccKrpp0yVhp5HdEIcKr6pLlVDBfOLX9QUsyCOV0wzfjIJNlGEYsdlLJizHhbn2mUjvSAHQqZETYP81eFzLQNnPHt4EVVUh7VfDESU84KezmD5QlWpXLmvU31/yMf+Se8xhHTvKSCZIFImWwoG6mbUoWf9nzpIoaSjB+weqqUUmpaaasXVal72J+UX2B+2RPW3RcT0eOzQgqlJL3RKrTJvdsjE3JEAvGq3lGHSZXy28G3skua2SmVi/w4yCE6gbODqnTWlg7+wC604ydGXA8VJiS5ap43JXiUFFAaQ==" \
        >> ~/.ssh/known_hosts

# Install utils
RUN apt-get update && \
    apt-get install --no-install-recommends -y \
        git=1:2.7.4* \
        ssh=1:7.2p2* \
        curl=7.47.0* \
        jq=1.5+dfsg* \
    # Cleanup
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY scripts/git_login.sh /usr/local/bin/git_login.sh
COPY scripts/generate_metafiles.sh /usr/local/bin/generate_metafiles.sh
COPY scripts/get_ui_version.sh /usr/local/bin/get_ui_version.sh
COPY scripts/slack_notify_github_deploy.sh /usr/local/bin/slack_notify_github_deploy.sh
COPY scripts/deploy_upm.sh /usr/local/bin/deploy_upm.sh
COPY scripts/move_upm_files.sh /usr/local/bin/move_upm_files.sh