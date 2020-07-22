$subscriptionId=""
$definition = New-AzPolicyDefinition -Name "Virtual-Machine-Name-pattern" -DisplayName "Virtual Machine Name pattern" -description "Virtual Machine Name pattern" -Policy 'matchPattern.rules.json' -Parameter 'matchPattern.parameters.json' -SubscriptionId $subscriptionId -Mode All
$definition
#$assignment = New-AzPolicyAssignment -Name <assignmentname> -Scope <scope> -PolicyDefinition $definition
#$assignment 
