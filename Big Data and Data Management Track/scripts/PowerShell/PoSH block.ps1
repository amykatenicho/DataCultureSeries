# Do not change these variables.  These are set to the class subscription and certificate.
$subscriptionName = "<subscription name>"

# Update this variable to the storage account supplied in class.
$storageAccount = "<storage account name>"

# Set-AzureSubscription will set the configuration values for the subscription used in class
Set-AzureSubscription -SubscriptionName $subscriptionName -CurrentStorageAccount $storageAccount

Select-AzureSubscription -SubscriptionName $subscriptionName -Current $subscriptionName

# Execute to view information about the default subscription.
Get-AzureSubscription –Current


