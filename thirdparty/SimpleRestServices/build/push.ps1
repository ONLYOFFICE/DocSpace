. .\version.ps1

If ($Version.EndsWith('-dev')) {
	$host.ui.WriteErrorLine("Cannot push development version '$Version' to NuGet.")
	Exit 1
}

..\src\.nuget\NuGet.exe 'push' ".\nuget\SimpleRestServices.$Version.nupkg"
