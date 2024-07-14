#!/usr/bin/env sh

set -e

docker run --rm -it \
  --network host \
  -e DOCKER_HOST \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -w /work -v $PWD/SampleSolrApp:/work \
  mcr.microsoft.com/dotnet/sdk:8.0-alpine \
  dotnet run -v minimal

