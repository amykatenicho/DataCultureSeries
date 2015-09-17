$vnetName = "Vnet1"
$subnetName = "Subnet-1"
$location = "<location>"

#Get the current azure config
$currentVNetConfig = get-AzureVNetConfig
[xml]$workingVnetConfig = $currentVNetConfig.XMLConfiguration
$virtNetCfg = $workingVnetConfig.GetElementsByTagName("VirtualNetworkSites")

#Add a new virtual network
$newNetwork = $workingVnetConfig.CreateElement("VirtualNetworkSite","http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration")
$newNetwork.SetAttribute("name",$vnetName)
$newNetwork.SetAttribute("Location",$location)
$Network = $virtNetCfg.appendchild($newNetwork)

#Add an address space
$newAddressSpace = $workingVnetConfig.CreateElement("AddressSpace","http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration")
$AddressSpace = $Network.appendchild($newAddressSpace)
$newAddressPrefix = $workingVnetConfig.CreateElement("AddressPrefix","http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration")
$newAddressPrefix.InnerText="10.0.0.0/8"
$AddressSpace.appendchild($newAddressPrefix)

#Add a subnet
$newSubnets = $workingVnetConfig.CreateElement("Subnets","http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration")
$Subnets = $Network.appendchild($newSubnets)
$newSubnet = $workingVnetConfig.CreateElement("Subnet","http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration")
$newSubnet.SetAttribute("name",$subnetName)
$Subnet = $Subnets.appendchild($newSubnet)
$newAddressPrefix = $workingVnetConfig.CreateElement("AddressPrefix","http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration")
$newAddressPrefix.InnerText="10.0.0.0/11"
$Subnet.appendchild($newAddressPrefix)

#Write to file and use that file
$tempFileName = $env:TEMP + "\azurevnetconfig.netcfg"
$workingVnetConfig.save($tempFileName)

set-AzureVNetConfig -configurationpath $tempFileName