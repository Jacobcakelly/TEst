#!/usr/bin/env sh

# Delete a Watch
jf xr curl -XDELETE /api/v2/watches/packagepython-watch -H 'Content-Type: application/json'
echo "\n"


# Delete the policies
jf xr curl -XDELETE /api/v2/policies/packagepython-license-policy -H 'Content-Type: application/json'
echo "\n"
jf xr curl -XDELETE /api/v2/policies/packagepython-oprisk-policy -H 'Content-Type: application/json'
echo "\n"
jf xr curl -XDELETE /api/v2/policies/packagepython-security-policy -H 'Content-Type: application/json'
echo "\n"