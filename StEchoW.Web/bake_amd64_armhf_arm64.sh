#!/usr/bin/env sh
# https://docs.docker.com/engine/reference/commandline/buildx_bake/
TAG=latest docker buildx bake -f bake_amd64_armhf_arm64.hcl --print 
