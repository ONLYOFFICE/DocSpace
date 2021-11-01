$DocsPath = "..\src\Documentation\Help"

# make sure the script was run from the expected path
If (!(Test-Path $DocsPath)) {
	$host.ui.WriteErrorLine('The script was run from an invalid working directory.')
	Exit 1
}

function Upload-Folder() {
	param([string]$LocalPath, [string]$ArtifactName)

	[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem')
	[System.IO.Compression.ZipFile]::CreateFromDirectory((Join-Path (pwd) $LocalPath), (Join-Path (pwd) $ArtifactName))
	Push-AppveyorArtifact (Join-Path (pwd) $ArtifactName)
}

Upload-Folder -LocalPath "$DocsPath\v4.0" -ArtifactName "docs.zip"
