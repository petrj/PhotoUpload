# PhotoUpload

Command line application for uploading photos and videos to Google Photos

- Directories on file system uploaded as albums
- Recurse uploading

- Mono & .NET
- Linux & Windows compatibile
- Powered by GAPI (Mono & .NET Google API)

- Using:

	`PhotoUpload.exe directoryName`

	`PhotoUpload.exe directoryName --reupload`


- Already uploaded directories are stored in journal.json
- Access token is stored in token.json

- Building:

	`msbuild PhotoUpload.sln`

- Create configuration file authInfo.json:

	```
	{
	  "client_id": "xxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.apps.googleusercontent.com",
	  "client_secret": "xxxxxxxxxxxxxxxxxxxxxxxx",
	  "scopes": [
	    "https://www.googleapis.com/auth/photoslibrary",
	    "https://www.googleapis.com/auth/drive.photos.readonly"
	    ]
	}
	```

- OAuth2 credentials `client_id` and `client_secret` generate on Google API Console Credentials page (https://console.developers.google.com/apis/credentials)

