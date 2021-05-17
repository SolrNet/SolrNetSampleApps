#!/usr/bin/env sh

set -e

./run_solr.sh

docker run --rm -it \
  --network host \
  -w /work -v $PWD/SampleSolrApp:/work \
  mcr.microsoft.com/dotnet/sdk:5.0.300-alpine3.12 \
  dotnet run -v minimal

