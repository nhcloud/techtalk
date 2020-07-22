#Install-Module -Name Az.Blueprint
#Import-Module -Name Az.Blueprint

#Get-Command -Module 'Az.Blueprint' -CommandType 'Cmdlet'
#Get-Help  Import-AzBlueprintWithArtifact
#Import-AzBlueprintWithArtifact -Name "MyBlueprint" -InputPath  "."

# Get the blueprint we just created
#$myBluePrint = Get-AzBlueprint -Name "MyBlueprint" 

# Publish version 1.0
#Publish-AzBlueprint -Blueprint $myBluePrint -Version 1.0

#subscriptionId=""
#Get-AzBlueprintAssignment -SubscriptionId $subscriptionId