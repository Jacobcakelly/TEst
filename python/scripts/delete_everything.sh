#! /bin/sh

echo "Deleting watch policies"
sh delete_policy_and_watch.sh

echo "Deleting Repos"
sh delete_repos.sh

#echo "Deleting build"
sh delete_build.sh

echo "purge PIP cache"
pip cache purge

#pip uninstall click jfrog-python-example nltk tqdm yolk joblib regex Werkzeug