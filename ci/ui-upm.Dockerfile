
FROM ubuntu:16.04

# Install git
RUN apt-get update && \
    # apt-get install mono-devel -y \
    apt-get install git -y

COPY scripts/generate_metafiles.sh /usr/local/bin/generate_metafiles.sh