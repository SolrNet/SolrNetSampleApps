#!/usr/bin/env sh

set -e

docker run --rm -it \
  --network host \
  -e DOCKER_HOST \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -w /work -v $PWD/SampleSolrApp:/work \
  mcr.microsoft.com/dotnet/sdk:5.0.300-alpine3.12 \
  dotnet run -v minimal

