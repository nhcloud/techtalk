Function Copy-AzMarketPlaceVhd([string]$storageAccount,[string]$storageKey,[string]$srcUri,[string]$destStorageAccount,[string]$destStorageKey,[string]$destContainerName,[string]$destFileName)
{
    $sourceContext = New-AzureStorageContext  -StorageAccountName $storageAccount -StorageAccountKey $storageKey  
    $destContext = New-AzureStorageContext  -StorageAccountName $destStorageAccount -StorageAccountKey $destStorageKey  
    ### Target Container Name
    $destContainerName = "publishedvhds"

    ### Start the Asynchronous Copy ###
    $blob1 = Start-AzureStorageBlobCopy -srcUri $srcUri -DestContainer $destContainerName -DestBlob $destFileName -SrcContext $sourceContext -DestContext $destContext
    $status = $blob1 | Get-AzureStorageBlobCopyState   
    While($status.Status -eq "Pending")
    {  
        $status = $blob1 | Get-AzureStorageBlobCopyState   
        Start-Sleep 10   
        $status 
    }
}
Function Publish-AzMarketPlaceVhd([string]$ImageName,[string]$Label,[string]$Description,[string]$osMediaLink,[string]$publicCertPath)
{
    # Image Parameters to Specify
    $osCaching='ReadWrite'
    $os = 'Windows'
    $state = 'Generalized'
    $dataCaching='None'
    $lun='1'
    $SrvMngtEndPoint='https://management.core.windows.net'
    $subscription = Get-AzureSubscription -Current -ExtendedDetails
    $certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($publicCertPath) 
    $SubId = $subscription.SubscriptionId
    $body =  "<VMImage xmlns=`"http://schemas.microsoft.com/windowsazure`" xmlns:i=`"http://www.w3.org/2001/XMLSchema-instance`">" + "<Name>" + $ImageName + "</Name><Label>" + $Label + "</Label><Description>" + $Description + "</Description>" + "<OSDiskConfiguration><HostCaching>" + $osCaching + "</HostCaching><OSState>" + $state + "</OSState><OS>" + $os + "</OS><MediaLink>" + $osMediaLink + "</MediaLink></OSDiskConfiguration></VMImage>"
    $uri = $SrvMngtEndPoint + "/" + $SubId + "/" + "services/vmimages" 
    $headers = @{"x-ms-version"="2014-06-01"}
    $response = Invoke-WebRequest -Uri $uri -ContentType "application/xml" -Body $body -Certificate $certificate -Headers $headers -Method POST
    if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300)
    {
        echo "Accepted"
    } 
    else
    {
        echo "Not Accepted" 
    }
    $opId = $response.Headers.'x-ms-request-id'
    $uri2 = $SrvMngtEndPoint + "/" + $SubId + "/" + "operations" + "/" + $opId 
    $response2 = Invoke-WebRequest -Uri $uri2 -ContentType "application/xml" -Certificate $certificate -Headers $headers -Method GET
    $response2.RawContent
}
