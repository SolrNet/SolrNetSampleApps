#!/usr/bin/env sh

solr_version=8.8.2
container_name=sample_solrnet_app_solr
this_ip=$(ip route get 8.8.8.8 | head -n1 | awk '{print $7}')

echo "Stopping previous container (if any)"
docker rm -f ${container_name} >/dev/null

echo Starting Solr
docker run -d -p 8983:8983 --name ${container_name} solr:${solr_version} \
  solr-precreate techproducts >/dev/null

echo Populating index

err=1
for i in $(seq 1 10); do
  output=$(docker run --rm -it -e SOLR=${this_ip} solr:${solr_version} /bin/bash -xc 'post -out yes -host $SOLR -c techproducts $(find | grep exampledocs | grep xml | grep -v manufacturers) 2>&1')
  # echo $output
  if ([ $? -eq 0 ] && (echo $output | grep -v -e '503' >/dev/null)); then
    # echo last output: $output
    err=0
    break
  fi
  sleep 1
done

if [ $err -ne 0 ]; then
  echo "Error posting example docs"
  exit 1
else
  echo "Solr available at http://${this_ip}:8983/solr/"
fi


