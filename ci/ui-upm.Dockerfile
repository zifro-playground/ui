
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
        ca-certificates=20170717~16.04.2 \
    # Cleanup
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
