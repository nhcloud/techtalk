Install-WindowsFeature -Name Web-Server -IncludeManagementTools
$urlReWriter21FileName='rewrite_amd64_en-US.msi'
$urlReWriter21Url='https://download.microsoft.com/download/1/2/8/128E2E22-C1B9-44A4-BE2A-5859ED1D4592/rewrite_amd64_en-US.msi'
Invoke-WebRequest -Uri $urlReWriter21Url -OutFile "$HOME/$urlReWriter21FileName"
Start-Process $HOME/$urlReWriter21FileName  -ArgumentList "/quiet"
#Get-Disk | Where-Object OperationalStatus -eq 'Offline'|
Get-Disk | Where-Object Number -eq 2|
         Initialize-Disk -PartitionStyle GPT -PassThru |
            New-Volume -FileSystem NTFS -DriveLetter F -FriendlyName 'Data'
