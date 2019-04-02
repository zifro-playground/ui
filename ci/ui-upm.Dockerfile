
FROM ubuntu:16.04

#FROM gableroux/unity3d:2018.3.11f1-webgl


# Install git
RUN apt-get update && \
    # apt-get install mono-devel -y \
    apt-get install git -y

COPY scripts/generate_metafiles.sh /usr/local/bin/generate_metafiles.sh