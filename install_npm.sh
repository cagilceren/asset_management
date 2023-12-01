#!/bin/sh
apt update
apt --yes install libnss3 xvfb libxi6 libgconf-2-4 unzip gnupg
curl -fsSL https://deb.nodesource.com/setup_18.x | bash - &&\
apt update
apt-get --yes install nodejs
