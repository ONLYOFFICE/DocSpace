$WorkDir = "$(Split-Path -Parent $PSScriptRoot)\run"

workflow StartServices-Runbook
{
param(
        [string]$WorkDir
    )
	
    foreach -parallel($s in Get-ChildItem -Path $WorkDir | ForEach-Object -Process {[System.IO.Path]::GetFileNameWithoutExtension($_)}) {
	 Start-Service -InputObject $(Get-Service -Name "Onlyoffice$s")
  	 Write-Output "Onlyoffice$s service has been started" 
    }
}

#Write-Output  "Starting services at time: $(Get-Date -Format HH:mm:ss)"
#Write-Output  ""
StartServices-Runbook $WorkDir
#Write-Output  ""
#Write-Output  "End start services at time: $(Get-Date -Format HH:mm:ss)"
