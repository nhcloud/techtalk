$ssrsFileName='SQLServerReportingServices.exe'
$ssrsUrl='https://download.microsoft.com/download/1/a/a/1aaa9177-3578-4931-b8f3-373b24f63342/SQLServerReportingServices.exe'
Invoke-WebRequest -Uri $ssrsUrl -OutFile "$HOME/$ssrsFileName"
Start-Process $HOME/$ssrsFileName  -ArgumentList "/quiet /norestart /IAcceptLicenseTerms"
