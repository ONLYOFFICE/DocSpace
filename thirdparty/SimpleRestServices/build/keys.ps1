# Note: these values may only change during major release
# Also, if the values change, assembly binding redirection will not work between the affected releases.

If ($Version.Contains('-')) {

	# Use the development keys
	$Keys = @{
		'v3.5' = '579ef5a5d8b3751c'
		'v4.0' = '579ef5a5d8b3751c'
	}

} Else {

	# Use the final release keys
	$Keys = @{
		'v3.5' = '541c51dcbcf0ec3c'
		'v4.0' = '541c51dcbcf0ec3c'
	}

}

function Resolve-FullPath() {
	param([string]$Path)
	[System.IO.Path]::GetFullPath((Join-Path (pwd) $Path))
}
