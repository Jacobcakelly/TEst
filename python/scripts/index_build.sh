#! /bin/sh

echo "\n\nindexing Build\n"
jf xr curl -XPUT /api/v1/binMgr/1/builds -H "Content-Type: application/json" -d @json/index-python-build.json

