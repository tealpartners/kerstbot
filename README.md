# kerstbot
Holiday slackbot that allows a webcam to auto join any meeting.

## Installation
### Prepare your browser
Make sure that your browser is configured to auto-join a meeting when it tries to open a URL.

For Chrome, you can install a plugin [Google Meet Enhancement Suite](https://chrome.google.com/webstore/detail/google-meet-enhancement-s/ljojmlmdapmnibgflmmminacbjebjpno)

### Run the application
Make sure you have dotnet installed.

Download code and compile: ```dotnet build```
Or run the code directly: ```dotnet run <token>```

Where __token__ is the token you retrieve from the Slack documentation.

## Usage
Just say ```@<botname> https://meet.google.com/<code>``` to make him auto-join your meeting.

Edit ```messages.txt``` to make kerstbot say some random messages every now and then.
Edit ```replies.txt``` to make kerstbot reply to certain words.