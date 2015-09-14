# DeviceSender Usage Instructions #
##Introduction##
The DeviceSender C# console application provides a set of mechanisms to generate event data.  The generated data can be optionally published to an Azure Event Hub or local file.  A local file can be uploaded to blob storage.  A source file in blob storage can be used to replay previously generated data and publish it to an Azure Event Hub.  The application simulates data being generated from 4 device types/sensors .across multiple rooms.

- Temperature (degrees F)
- Humidity (0-100%)
- Lighting (Lumens)
- Energy (kwh)

##Pre-Requisites##

The Event Hub should be configured with a policy that has Manage and Send permissions.

##Capabilities##
The application provides the following capabilities;

- Generate device event data, publish to Azure Event Hub
- Generate device event data, publish to file, Upload local file to blob storage
- Generate device event data, publish to file
- Upload local file to blob storage
- Generate Consumer Group(s) in the Azure Event Hub

##Usuage##
###Available Commands###

  - **GenerateDataToEventHub**     
    Generate data and upload to Event Hub

	`DeviceSender GenerateDataToEventHub -n serviebusnamespace -e eventhubname -p policyname -k poliykey`
 
  - **ReplayFileFromBlockBlob**    
    Replay a set of messages from Blob Storage by sending them to the Event Hub

	`DeviceSender ReplayFileFromBlockBlob -n serviebusnamespace -e eventhubname -p policyname -k poliykey -q storageaccountname -l storageaccountkey -c container -r my/root/folder/ -a my/root/backup/ -f filename`

  - **GenerateLocalFileUpload**    
    Generate a dataset locally, upload to blob storage

	`DeviceSender.exe GenerateLocalFileUpload -q storageaccountname -l storageaccountkey -c container -r my/root/folder/ -a my/root/backup/ -f c:\\\\tmp\\localfilename`

  - **GenerateLocalFile**          
    Generate a dataset locally

	`DeviceSender.exe GenerateLocalFile -f c:\\\\tmp\\localfilename `

  - **UploadLocalFile**            
    Upload a dataset to blob storage

	`DeviceSender.exe UploadLocalFile -q storageaccountname -l storageaccountkey -c container -r my/root/folder/ -a my/root/backup/ -f c:\\\\tmp\\localfilename`

  - **CreateConsumerGroup**        
    Create an Event Hub consumer group

	`DeviceSender.exe CreateConsumerGroup -n serviebusnamespace -e eventhubname -p policyname -k poliykey -g consumergroupname -i 10`

###Help###
DeviceSender -h|--help

### GenerateDataToEventHub ####

DeviceSender GenerateDataToEventHub [-h] -n|--EventHubNamespace namespace -e|--EventHubName name -p|--EventHubSasPolicy policyname -k|--EventHubSasKey policykey

###ReplayFileFromBlockBlob###

DeviceSender ReplayFileFromBlockBlob [-h] -n|--EventHubNamespace namespace -e|--EventHubName name -p|--EventHubSasPolicy policyname -k|--EventHubSasKey policykey -q|--BlobStoragePolicyName accountname -l|--BlobStoragePolicyKey accountkey -c|--BlobStorageContainer container -r|--BlobStorageRootFolder root -a|--BlobStorageArchiveFolder archive -f|--Filename filename

###GenerateLocalFileUpload###

DeviceSender GenerateLocalFileUpload [-h] -q|--BlobStoragePolicyName accountname -l|--BlobStoragePolicyKey accountkey -c|--BlobStorageContainer container -r|--BlobStorageRootFolder root -a|--BlobStorageArchiveFolder archive -f|--Filename filename

###GenerateLocalFile###

DeviceSender GenerateLocalFile [-h] -f|--Filename filename

###UploadLocalFile###

DeviceSender UploadLocalFile [-h] -q|--BlobStoragePolicyName accountname -l|--BlobStoragePolicyKey accountkey -c|--BlobStorageContainer container -r|--BlobStorageRootFolder root -a|--BlobStorageArchiveFolder archive -f|--Filename filename

###CreateConsumerGroup###

DeviceSender CreateConsumerGroup [-h] -n|--EventHubNamespace namespace -e|--EventHubName name -p|--EventHubSasPolicy policyname -k|--EventHubSasKey policykey -g|--ConsumerGroupName groupname [-i|--NumberOfInstances numberOfInstances]


##Configuration options##
Some additional configuration options exist for the app itself, these are located in app.config.  These control the amount of data that is generated and the bounds for the various devices.  The following shows the available options and the default values.

	NumberOfRooms = 5
    IterationSeconds = 240
    NumberOfDevices = 2
    TemperatureMax = 28.9
    TemperatureMin = 19.6
    HumidityMin = 40.0
    HumidityMax = 80.0
    EnergyMin = 2000.0
    EnergyMax = 4900.0
    LightMin = 10.0
    LightMax = 1800.0
    MillisecondDelay = 1000

A simple calculation can be performed to determine the number of messages generated

NumberOfSensors\*NumberOfRooms\*NumberOfDevices\*IterationSeconds

Where NumberOfSensors = 4 (non-configurable)

Given the defaults

4*5*2*240 = 9600 messages

Given the nature of the abs its best to keep the number of devices/room small so messages over a longer period can be generated.

Choosing larger values increases the number of messages considerably e.g. 10 rooms, 50 devices, 30 seconds

4\*10\*50\*30 = 60000 Messages

This number of messages can become unwieldy in certain labs.  