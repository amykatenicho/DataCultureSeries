$hbaseClusterName = "<HBase cluster name>"
$clusterSize = 1
$location = "<location>" #i.e. "West US"
$storageAccountName = "<storage account name>"
$storageContainerName = "hbase"
$vnetName = "Vnet1"
$subnetName = "Subnet-1"

# Check if the storage account exists.  If not create the storage account.
If (!(Test-AzureName -Storage $storageAccountName)) 
{
    New-AzureStorageAccount -StorageAccountName $storageAccountName -Location $Location 
}

# Set the storage account context
$storageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey (Get-AzureStorageKey $storageAccountName ).Primary

# Check if the container exists.  If not, create the new container.
if (!(Get-AzureStorageContainer -Context $storageContext -Name $storageContainerName))
{New-AzureStorageContainer -Permission Container -Name $storageContainerName -Context $storageContext}

# Create the HBase cluster
New-AzureHDInsightCluster -Name $hbaseClusterName `
    -ClusterType HBase `
    -Version 3.1 `
    -Location $location `
    -ClusterSizeInNodes $clusterSize `
    -DefaultStorageAccountName "$storageAccountName.blob.core.windows.net" `
    -DefaultStorageAccountKey (Get-AzureStorageKey $storageAccountName ).Primary `
    -DefaultStorageContainerName $storageContainerName `
    -VirtualNetworkId (Get-AzureVNetSite $vnetName).Id `
    -SubnetName $subnetName 
