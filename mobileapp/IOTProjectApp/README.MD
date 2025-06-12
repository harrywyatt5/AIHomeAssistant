## CM2211 Internet of Things Project - Mobile App

This is the mobile app side of our group project, meant to be installed on an Android phone.
The app itself has been created in .NET MAUI.

## Installation

These instructions are for Windows devices, but it can also be installed and ran on MacOS and Linux

1. Install [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) and [Visual Studio 2022](https://visualstudio.microsoft.com/en/vs/)
2. When installing Visual Studio 2022, ensure to download the MAUI workload. This can be found under 'Desktop & Mobile' workloads.
2. 3.Install Java 17. The Microsoft distribution can be found [here](https://learn.microsoft.com/en-us/java/openjdk/download#openjdk-17)

   **Note**: This must be Java 17 (no newer versions). You are welcome to use your own Java distributions

4. Ensure `java.exe` has been added to the PATH variable.
5. Use [this guide](https://learn.microsoft.com/en-us/dotnet/maui/get-started/first-app?view=net-maui-8.0&tabs=vswin&pivots=devices-android) to install an Android emulator and run the Android app. Note that you must use an API version greater than 34 (we recommend downloading the Google Pixel 6a emulator)