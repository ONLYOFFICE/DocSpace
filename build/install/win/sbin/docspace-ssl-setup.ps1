# runas administrator
if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
  Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
  exit
}

function Test-RegistryValue($RegistryKey, $RegistryName)
{
  $exists = Get-ItemProperty -Path "$RegistryKey" -Name "$RegistryName" -ErrorAction SilentlyContinue
  if (($exists -ne $null) -and ($exists.Length -ne 0)) { return $true }
  return $false
}

$certbot_path = if ((Test-RegistryValue "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Certbot" "InstallLocation") -eq $true )
{
  (Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Certbot" -ErrorAction Stop).InstallLocation
}
elseif ((Test-RegistryValue "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Certbot" "InstallLocation") -eq $true )
{
  (Get-ItemProperty -Path "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Certbot" -ErrorAction Stop).InstallLocation
}

if ( -not $certbot_path )
{
  Write-Output " Attention! Certbot is not installed on your computer. "
  Write-Output " Certbot could be downloaded by this link 'https://github.com/certbot/certbot/releases/latest/download/certbot-beta-installer-win_amd64_signed.exe' "
  exit
}

if ( $args.Count -ge 2 )
{
  $letsencrypt_root_dir = "$env:SystemDrive\Certbot\live"
  $app = Resolve-Path -Path ".\..\"
  $root_dir = "${app}\letsencrypt"
  $nginx_conf_dir = "$env:SystemDrive\OpenResty\conf"
  $nginx_conf = "onlyoffice-proxy.conf"
  $nginx_tmpl = "onlyoffice-proxy-ssl.conf.tmpl"
  $proxy_service = "OpenResty"
  $appsettings_config_path = "${app}\config\appsettings.production.json"

  if ($args[0] -eq "-f") {
    $ssl_cert = $args[1]
    $ssl_key = $args[2]
  }

  else {
    $letsencrypt_mail = $args[0]
    $letsencrypt_domain = $args[1]

    [void](New-Item -ItemType "directory" -Path "${root_dir}\Logs" -Force)

    "certbot certonly --expand --webroot -w `"${root_dir}`" --noninteractive --agree-tos --email ${letsencrypt_mail} -d ${letsencrypt_domain}" > "${app}\letsencrypt\Logs\le-start.log"
    cmd.exe /c "certbot certonly --expand --webroot -w `"${root_dir}`" --noninteractive --agree-tos --email ${letsencrypt_mail} -d ${letsencrypt_domain}" > "${app}\letsencrypt\Logs\le-new.log"

    pushd "${letsencrypt_root_dir}\${letsencrypt_domain}"
        $ssl_cert = (Resolve-Path -Path (Get-Item "${letsencrypt_root_dir}\${letsencrypt_domain}\fullchain.pem").Target).ToString().Replace('\', '/')
        $ssl_key = (Resolve-Path -Path (Get-Item "${letsencrypt_root_dir}\${letsencrypt_domain}\privkey.pem").Target).ToString().Replace('\', '/')
    popd
  }

  if ( [System.IO.File]::Exists($ssl_cert) -and [System.IO.File]::Exists($ssl_key) -and [System.IO.File]::Exists("${nginx_conf_dir}\${nginx_tmpl}"))
  {
    Copy-Item "${nginx_conf_dir}\${nginx_tmpl}" -Destination "${nginx_conf_dir}\${nginx_conf}"
    ((Get-Content -Path "${nginx_conf_dir}\${nginx_conf}" -Raw) -replace '/usr/local/share/ca-certificates/tls.crt', $ssl_cert) | Set-Content -Path "${nginx_conf_dir}\${nginx_conf}"
    ((Get-Content -Path "${nginx_conf_dir}\${nginx_conf}" -Raw) -replace '/etc/ssl/private/tls.key', $ssl_key) | Set-Content -Path "${nginx_conf_dir}\${nginx_conf}"

    if ($letsencrypt_domain)
    {
        $acl = Get-Acl -Path "$env:SystemDrive\Certbot\archive\${letsencrypt_domain}"
        $acl.SetSecurityDescriptorSddlForm('O:LAG:S-1-5-21-4011186057-2202358572-2315966083-513D:PAI(A;;0x1200a9;;;WD)(A;;FA;;;SY)(A;OI;0x1200a9;;;LS)(A;;FA;;;BA)(A;;FA;;;LA)')
        Set-Acl -Path $acl.path -ACLObject $acl
    }
  }

  Restart-Service -Name $proxy_service

  "certbot renew >> `"${app}\letsencrypt\Logs\le-renew.log`"" > "${app}\letsencrypt\letsencrypt_cron.bat"
  "net stop $proxy_service" >> "${app}\letsencrypt\letsencrypt_cron.bat"
  "net start $proxy_service" >> "${app}\letsencrypt\letsencrypt_cron.bat"

  $day = (Get-Date -Format "dddd").ToUpper().SubString(0, 3)
  $time = Get-Date -Format "HH:mm"
  cmd.exe /c "SCHTASKS /F /CREATE /SC WEEKLY /D $day /TN `"Certbot renew`" /TR `"${app}\letsencrypt\letsencrypt_cron.bat`" /ST $time"
}
else
{
  Write-Output " This script provided to automatically get Let's Encrypt SSL Certificates for DocSpace "
  Write-Output " usage: "
  Write-Output "   docspace-ssl-setup.ps1 EMAIL DOMAIN "
  Write-Output "      EMAIL       Email used for registration and recovery contact. Use "
  Write-Output "                  comma to register multiple emails, ex: "
  Write-Output "                  u1@example.com,u2@example.com. "
  Write-Output "      DOMAIN      Domain name to apply "
  Write-Output " "
  Write-Output " Using your own certificates via the -f parameter: "
  Write-Output " usage: "
  Write-Output "  docspace-ssl-setup.ps1 -f CERTIFICATE PRIVATEKEY "
  Write-Output "    CERTIFICATE   Path to the certificate file for the domain."
  Write-Output "    PRIVATEKEY    Path to the private key file for the certificate."
}
