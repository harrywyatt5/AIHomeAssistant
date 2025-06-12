#!/usr/bin/env bash

# Auto installing the environment
python -m venv appenv
source appenv/bin/activate
pip install -r requirements.txt

# Extra steps for GrovePi
curl -kL dexterindustries.com/update_grovepi | bash -s -- --bypass-gui-installation --env-local
echo REMEMBER TO MAKE A FILE FOR THE OPENAI API KEY!!!