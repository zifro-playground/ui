
# Generate SSH key

The SSH key is required for Unity Package Manager to pull the private dependencies.
See [README.md on upm branch](https://github.com/zardan/ui/blob/upm/README.md) for more details.

1. Go to <https://circleci.com/gh/zardan/ui/edit#checkout> and add a user key from an account that also can connect to the other repos from link above.
2. Add this step to the config.yml:

```yml
steps:
  - add_ssh_keys:
      fingerprints:
        - "b7:35:a6:4e:9b:0d:6d:d4:78:1e:9a:97:2a:66:6b:be"
        # Change to fingerprint of your new key
```

# Unity license

Gableroux on GitLab has a good tutorial. Follow it here to obtain a `.ulf` license file.  
You can follow the instructions on here: <https://gitlab.com/gableroux/unity3d-gitlab-ci-example/tree/master#how-to-activate>

With the `.ulf` downloaded, convert it to base64 and add it to the `UNITY_LICENSE_CONTENT_B64` environment key in the [env settings on CircleCI](https://circleci.com/gh/zardan/ui/edit#env-vars).

```sh
$ base64 $UNITY_ULF_KEY_PATH | clip
# paste into UNITY_LICENSE_CONTENT_B64 env var in circleci
```

# Add Slack webhook for notifications

To use an existing integration, head to  
[https://your_domain.slack.com/apps/manage](https://slack.com/apps/manage)

To create a new integration via CircleCI, head to  
[https://your_domain.slack.com/apps/new/A0F7VRE7N-circleci](https://slack.com/apps/new/A0F7VRE7N-circleci)

Finally, paste the webhook url into the `SLACK_WEBHOOK` environment variable.

# Add CircleCI API key

For further detailed slack messages the script uses CircleCI's API.

Create a personal API token at:  
<https://circleci.com/account/api>

Paste the key into `CIRCLE_API_TOKEN` in the [env settings on CircleCI](https://circleci.com/gh/zardan/ui/edit#env-vars).

# Summary: All env vars

List of all environment variables, to check if one is missing in CircleCI

Key                         | Description
--------------------------- | -----------
`UNITY_LICENSE_CONTENT_B64` | Unity_lic.ulf in base 64 format
`SLACK_WEBHOOK`             | Slack webhook url, for use in Slack integration
`CIRCLE_API_TOKEN`          | CircleCI personal API token, for use in Slack integration
