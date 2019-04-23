#!/bin/bash

: ${SLACK_WEBHOOK?}

: ${PLAYGROUND_UI_VERSION?}

: ${DEPLOY_STATUS:="fail"}
: ${DEPLOY_TAG_PLAYGROUND_UI:=}
: ${DEPLOY_CHANGESET:=}
: ${BUILD_STATUS:="fail"}

if [ -z "$SLACK_WEBHOOK" ]; then
    echo "NO SLACK WEBHOOK SET"
    echo "Please input your SLACK_WEBHOOK value either in the settings for this project, or as a parameter for this orb."
    exit 1
fi

author=""
text="*Project: \`$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME\` Branch: \`$CIRCLE_BRANCH\`*"
fields=""
actions=""
footer="$DEPLOY_CHANGESET"

echo "BUILD_STATUS='$BUILD_STATUS'"
echo "DEPLOY_STATUS='$DEPLOY_STATUS'"

if [[ "$BUILD_STATUS" == "success" ]]
then
    if [[ "$DEPLOY_STATUS" == "success" ]]
    then
        # Deploy success
        echo "Got successful deployment"
        color="#1CBF43" # green
        visitJobActionStyle="primary" # green
        title=":tada: DEPLOYED TO GITHUB"
        fallback="Deployed to GitHub successfully, new tag: $DEPLOY_TAG_PLAYGROUND_UI"
        actions=",
            {
                \"type\": \"button\",
                \"text\": \"Visit release $DEPLOY_TAG_COMBINED\",
                \"url\": \"https://github.com/$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME/releases/tag/$DEPLOY_TAG_COMBINED\"
            }
        "
        if [[ "$DEPLOY_TAG_PLAYGROUND_UI" ]]; then
            echo "Recording new ui tag: '$DEPLOY_TAG_PLAYGROUND_UI'"
            uiTagText="_(tag: <https://github.com/$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME/releases/tag/$DEPLOY_TAG_PLAYGROUND_UI|$DEPLOY_TAG_PLAYGROUND_UI>)_"
        else
            uiTagText="_(no new tag)_"
        fi
        fields="{
                \"title\": \"Zifro Playground UI\",
                \"value\": \"\`\`\`$DEPLOY_TAG_PLAYGROUND_UI\`\`\`\\n$uiTagText\",
                \"short\": true
            }"

        if [[ "${GITHUB_USER_ID:-}" ]]
        then
            echo "Looking up commit author $GITHUB_USER_ID on github..."

            curlResult="$(curl -s https://api.github.com/users/$GITHUB_USER_ID \
            | grep "\"avatar_url\":")"
            curlRegex='avatar_url.*"(.+)"'

            if [[ $curlResult =~ $curlRegex ]] && [[ "${BASH_REMATCH[1]:-}" ]]
            then
                authorIcon=${BASH_REMATCH[1]}
                echo "Found author profile picture: $authorIcon"

                author="
                \"author_name\": \"deployed by $GITHUB_USER_ID\",
                \"author_icon\": \"$authorIcon\",
                \"author_link\": \"https://github.com/$GITHUB_USER_ID\",
                "
            else
                echo "No profile picture found."
                autor="
                \"author_name\": \"deployed by $GITHUB_USER_ID\",
                \"author_link\": \"https://github.com/$GITHUB_USER_ID\",
                "
            fi
        fi

    else
        # Nothing to deploy
        echo "Nothing to deploy 'eh?"
        #color="#3AA3E3" # blue
        color="" # slack defaults to gray
        visitJobActionStyle="default" # gray
        title="NOTHING TO DEPLOY"
        text="$text\\n_No new tags_"
        fallback="Nothing to deploy. No new tags."
    fi
else
    # Build failed
    echo "Build failed"
    color="#ed5c5c" # red
    visitJobActionStyle="danger" # red
    title=":no_entry_sign: DEPLOYMENT JOB FAILED"
    text="$text\\n_Visit CircleCI for further details._"
    footer=""
    fallback="Deployment failed. Unknown error during the build."
fi

data=" {
\"attachments\": [
    {
        $author
        \"fallback\": \"$fallback\",
        \"title\": \"$title\",
        \"footer\": \"$footer\",
        \"text\": \"$text\",
        \"mrkdwn_in\": [\"fields\", \"text\"], 
        \"color\": \"$color\",
        \"thumb_url\": \"https://img.icons8.com/ultraviolet/100/000000/playground.png\",
        \"fields\": [
            $fields
        ],
        \"actions\": [
            {
                \"style\": \"$visitJobActionStyle\",
                \"type\": \"button\",
                \"text\": \"Visit Job #$CIRCLE_BUILD_NUM ($CIRCLE_STAGE)\",
                \"url\": \"$CIRCLE_BUILD_URL\"
            }
            $actions
        ]
    }
] }"

echo
response="$(curl -X POST -H 'Content-type: application/json' --data "$data" $SLACK_WEBHOOK)"
if [[ "$response" == "ok" ]]
then
    echo "Job completed successfully. Alert sent."
else
    echo "Something went wrong in the webhook..."
fi

echo "Payload:"
echo
echo "$data"
