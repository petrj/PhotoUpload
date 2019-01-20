# PhotoUpload

CLI Application for uploading media to Google Photo

- Directories on file system uploaded as Google Photo albums 
- Mono & .NET
- Linux & Windows compatibile
- Powered by GAPI (Mono & .NET Google API)

- Using:

	PhotoUpload.exe directoryName

	In the same directory as PhotoUpload.exe must be file authInfo.json:

	```
	{
	  "client_id": "xxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.apps.googleusercontent.com",
	  "client_secret": "xxxxxxxxxxxxxxxxxxxxxxxx",
	  "scopes": [
	    "https://www.googleapis.com/auth/photoslibrary",
	    "https://www.googleapis.com/auth/photoslibrary.readonly",
	    "https://www.googleapis.com/auth/photoslibrary.readonly.appcreateddata",
	    ]
	}
	```
	
- Recurse uploading

- Already uploaded directories are stored in journal.json

- Acces token is stored in token.json

- Building:

	`msbuild PhotoUpload.sln`
