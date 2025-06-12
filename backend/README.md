# CM2211 Backend

This is the backend code for our IoT project. This should be installed on the Raspberry Pi and the LED button should be plugged into port D4 on the GrovePi+ board

## Installation

Requires Python 3.6+

1. Run the install script `bash install.sh` (Do not run it as superuser, as this can cause issues)
2. Download the ffmpeg package. `sudo apt-get install ffmpeg`
3. Run the code by executing the run script `bash run.sh`


## Troubleshooting

Sometimes, configuring a local venv can fail. If this is the case, attempt to install the requirements to your global Python installation as well as the GrovePi library. More information on how to configure the GrovePi library can be found here <https://www.dexterindustries.com/GrovePi/get-started-with-the-grovepi/>