echo "Creating Necessary repos "
sh create_local_repos.sh
sh create_remote_repos.sh
sh create_virtual_repos.sh

sh index_build.sh

sh create_policy_and_watch.sh

