#!/bin/bash

commitRange="$CIRCLE_SHA1^...$CIRCLE_SHA1"
echo "Looking at range $commitRange"

if [ "${CIRCLE_API_KEY:-}" ]
then
    if [ "${CIRCLE_PREVIOUS_BUILD_NUM:-}" ]
    then
        echo "Got CircleCI API key and previous build. Let's find out the SHA1 of last commit..."
        curlResult="$(curl -su $CIRCLE_API_KEY: \
        https://circleci.com/api/v1.1/project/github/$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME/$CIRCLE_PREVIOUS_BUILD_NUM \
        | grep "\"vcs_revision\" :")"
        # $   "vcs_revision" : "c727a7309ff289d7c38465d7f07d7011658aa4b2",
        curlRegex='vcs_revision.*"(.+)"'
        
        if [[ $curlResult =~ $curlRegex ]] && [[ "${BASH_REMATCH[1]:-}" ]]
        then
            commitPrevSHA=${BASH_REMATCH[1]}
            echo "Found commit '$commitPrevSHA'"
            if [ "$commitPrevSHA" == "$CIRCLE_SHA1" ]
            then
                echo "Oh wait, it's the same commit. Leaving range as-is."
            else
                commitRange="$commitPrevSHA...$CIRCLE_SHA1"
                echo "Instead looking at range '$commitRange'"
                echo
            fi
        else
            echo "No match for previous commit."
            echo
        fi
    else
        echo "No previous build. Assuming new branch..."
        baseBranch=$(git show-branch -a \
        | grep '\*' \
        | grep -v $(git rev-parse --abbrev-ref HEAD) \
        | head -n1 \
        | sed 's/.*\[\(.*\)\].*/\1/' \
        | sed 's/[\^~].*//')
        echo "Found base branch '$baseBranch'"
        commitRange="$baseBranch...$CIRCLE_SHA1"
        echo "Instead looking at range '$commitRange'"
        echo
    fi
else
    echo "Add \$CIRCLE_API_KEY env var with personal CircleCI token for comparing commits since last build"
fi

echo "export COMMIT_RANGE='$commitRange'" >> $BASH_ENV
