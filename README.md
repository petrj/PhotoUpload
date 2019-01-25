# PhotoUpload

Command line application for uploading photos and videos to Google Photos

- Directories on file system uploaded as albums
- Recursive uploading

- Mono & .NET
- Linux & Windows compatibile
- Powered by GAPI (Mono & .NET Google API)

- Usage:

  `PhotoUpload.exe directoryName`

    - for uploading directoryName recursively

  `PhotoUpload.exe directoryName --reupload`

    - for reuploading directoryName recursively
    - directories in journal.json are reuploaded, other directories are uploaded as usually

  `PhotoUpload.exe --info`

    - for showing Google account informations such as name, email and free size on drive

  `PhotoUpload.exe --info --json`

    - for showing all Google account informations in json format

  `PhotoUpload.exe --help`

    - for showing help and exit


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

- OAuth2 credentials `client_id` and `client_secret` can be generated on Google API Console Credentials page (https://console.developers.google.com/apis/credentials)

