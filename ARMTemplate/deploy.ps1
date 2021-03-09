Connect-AzAccount 
Set-AzContext
Get-AzContext
$deploymentName="udai-boston-dep1"
$resourceGroupName="rg-udai-boston"
$location="eastus2"
New-AzResourceGroup -Name $resourceGroupName -Location $location
   
New-AzResourceGroupDeployment -Name $deploymentName -ResourceGroupName $resourceGroupName -TemplateFile "<template file path>" -param1 "val1" -paramn "valn"

New-AzResourceGroupDeployment -Name $deploymentName -ResourceGroupName $resourceGroupName -TemplateFile "<template file path>" -TemplateParameterFile "<template parameter file path>"

New-AzResourceGroupDeployment -Name $deploymentName -ResourceGroupName $resourceGroupName -TemplateUri "<http template url>" -TemplateParameterUri "<http parameter template url>"

New-AzTemplateSpec -Name "<NAME>" -Version "1.0" -ResourceGroupName $resourceGroupName -Location $location -TemplateFile "TEMPLATESPECT MAIN TEMPLATE"

$id = (Get-AzTemplateSpec -Name "<NAME OF SPEC>" -ResourceGroupName $resourceGroupName -Version "<VERSION>").Versions.Id
New-AzResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateSpecId $id