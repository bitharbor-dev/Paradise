#!/bin/bash

apt-get update && apt-get install -y \
curl

curl --fail http://localhost:80 || exit 1