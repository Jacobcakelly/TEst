#! /bin/sh

echo "Deleting unwanted folders"
rm -rf ../build/
rm -rf ../dist/
rm -rf ../jfrog_python_example.egg-info/

echo "START : Delete Builds : "
jf rt curl -XPOST /api/build/delete -H "Content-Type: application/json" -d @json/delete-python-build.json
echo "\n--> Complete : Delete Builds : "

