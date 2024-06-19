#!/usr/bin/env sh


# Create Policies
echo "Creating Policies "
jf xr curl -XPOST /api/v2/policies -H 'Content-Type: application/json' -d @json/packagepython-sec-policy.json
echo "\n"
jf xr curl -XPOST /api/v2/policies -H 'Content-Type: application/json' -d @json/packagepython-lic-policy.json
echo "\n"
jf xr curl -XPOST /api/v2/policies -H 'Content-Type: application/json' -d @json/packagepython-oprisk-policy.json
echo "\n"


# Create a Watch
echo "Creating Watch"
jf xr curl -XPOST /api/v2/watches -H 'Content-Type: application/json' -d @json/packagepython-watch.json
echo "\n"

