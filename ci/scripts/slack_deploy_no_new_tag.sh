#!/bin/bash

: ${SLACK_WEBHOOK?}
: ${SLACK_THUMBNAIL:="https://img.icons8.com/ultraviolet/100/000000/playground.png"}

if [ -z "$SLACK_WEBHOOK" ]; then
    echo "NO SLACK WEBHOOK SET"
    echo "Please input your SLACK_WEBHOOK value either in the settings for this project, or as a parameter for this orb."
    exit 1
fi

text="*Project: \`$CIRCLE_PROJECT_USERNAME/$CIRCLE_PROJECT_REPONAME\` Branch: \`$CIRCLE_BRANCH\`*"

echo "BUILD_STATUS='$BUILD_STATUS'"
echo "DEPLOY_STATUS='$DEPLOY_STATUS'"

# Nothing to deploy
echo "Nothing to deploy 'eh?"
#color="#3AA3E3" # blue
color="" # slack defaults to gray
visitJobActionStyle="default" # gray
title="NOTHING TO DEPLOY"
text="$text\\n_No new tags_"
fallback="Nothing to deploy. No new tags."

data=" {
\"attachments\": [
    {
        \"fallback\": \"$fallback\",
        \"title\": \"$title\",
        \"footer\": \"$DEPLOY_CHANGESET\",
        \"text\": \"$text\",
        \"mrkdwn_in\": [\"text\"], 
        \"color\": \"$color\",
        \"thumb_url\": \"$SLACK_THUMBNAIL\",
        \"actions\": [
            {
                \"style\": \"$visitJobActionStyle\",
                \"type\": \"button\",
                \"text\": \"Visit Job #$CIRCLE_BUILD_NUM ($CIRCLE_STAGE)\",
                \"url\": \"$CIRCLE_BUILD_URL\"
            }
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
