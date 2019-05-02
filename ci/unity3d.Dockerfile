
ARG UNITY_VERSION=2018.3.11f1-unity
FROM gableroux/unity3d:${UNITY_VERSION}

# Add trusted sources
RUN mkdir -p ~/.ssh \
    # Fingerprint to github.com
    && echo "|1|/olfPnpeGUgKkhLlwJSy4ro3Hb8=|tXqFHMrM0PWDFuQmABAg7zYndWc= ssh-rsa AAAAB3NzaC1yc2EAAAABIwAAAQEAq2A7hRGmdnm9tUDbO9IDSwBK6TbQa+PXYPCPy6rbTrTtw7PHkccKrpp0yVhp5HdEIcKr6pLlVDBfOLX9QUsyCOV0wzfjIJNlGEYsdlLJizHhbn2mUjvSAHQqZETYP81eFzLQNnPHt4EVVUh7VfDESU84KezmD5QlWpXLmvU31/yMf+Se8xhHTvKSCZIFImWwoG6mbUoWf9nzpIoaSjB+weqqUUmpaaasXVal72J+UX2B+2RPW3RcT0eOzQgqlJL3RKrTJvdsjE3JEAvGq3lGHSZXy28G3skua2SmVi/w4yCE6gbODqnTWlg7+wC604ydGXA8VJiS5ap43JXiUFFAaQ==" \
        >> ~/.ssh/known_hosts

# Install utils
RUN echo "\n>>> Installing tools\n" \
    && apt-get update \
    && apt-get install --no-install-recommends -y \
        git=1:2.7.4* \
        ssh=1:7.2p2* \
        curl=7.47.0* \
        apt-transport-https=1.2.31 \
        xmlstarlet=1.6.1* \
        jq=1.5+dfsg* \
        ca-certificates=20170717~16.04.2 \
    # Special requirement for Unity 2019.1 and above
    && ([ -z "$(echo '${UNITY_VERSION}' | egrep '2018.*|2017.*|5.*')" ] || exit 0 \
        && echo "\n>>> Unity 2019.1.0f2 or above, installing additional libraries\n" \
        && apt-get install --no-install-recommends -y \
            libunwind-dev=1.1-4.1 \
    ) \
    # Cleanup
    && echo "\n>>> Cleaning up apt-get cache\n" \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Install powershell
# /Requires apt-transport-https/
RUN echo "\n>>> Installing PowerShell\n" \
    && wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install --no-install-recommends -y \
        powershell=6.2.0* \
    # Cleanup
    && echo "\n>>> Cleaning up apt-get cache\n" \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
