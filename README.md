## Porting Assistant for .NET
Porting Assistant for .NET is an analysis tool that scans .NET Framework applications and generates a .NET Core compatibility assessment, helping customers port their applications to Linux faster.

Porting Assistant for .NET quickly scans .NET Framework applications to identify incompatibilities with .NET Core, finds known replacements, and generates detailed compatibility assessment reports. This reduces the manual effort involved in modernizing the applications to Linux.

### Porting Assistant Dotnet DataStore
This repository contains the location of data store in S3. The data store contains data files that show package and API compatibility with .NET Core and .NET Standard. 

* Porting assistant uses these data files for finding the compatibility of packages and their APIs
* For each NuGet package in nuget.org, a separate Json file is defined. Each file captures compatibility information of all its packaged versions.
* It shows compatibility information for all the NuGet packages available in nuget.org - 211k unique packages, 2.5 millions packaged versions (The numbers could change over time).

### Format of the data file (Json format)
#### Package Details
| Attribute Name | Description |
| :----------- | :---------- |
| Name | Name of the package. <br/> Example: `"Name": "AWSSDK.EC2"`|
| Format | Version of the Json document.<br/>Example: `"Format": "1.0"` |
| Versions |  List of packaged versions of this package. <br/>Example: `"Versions": [ "4.0","4.4"]` |
| Targets |  Targets: Compatible targets based on packaged versions. <br/>Example: `{ "netcoreapp3.1" => [ "4.0", "4.1"] }, { "netcoreapp5.0 => ["4.0"] }`. <br/>"netcoreapp3.1" is supported for package versions - 4.0 and 4.1. |
| API details | List of APIs. Refer API Details. | 
| License details | License details. Refer License Details |


#### API Details
| Attribute Name | Description |
| :------ | :---------- |
| Name | Name of the Api. <br/>Example: `"Name": ".ctor"` |
| Signature | Signature of the Api. <br/>Example: `"Signature": "AutoMapper.AutoMapperConfigurationException.AutoMapperConfigurationException(string)"`  |
| Namespace | Namespace.  <br/>Example: `"NameSpace": "AutoMapper"` |
| ClassName | Class name.  <br/>Example: `"ClassName": "AutoMapper.AutoMapperConfigurationException"` |
| Parameters | Parameters. <br/>Example: `"Parameters": [ "string" ]`  |
| ReturnType | Return type. <br/>Example: `"ReturnType": "Void"` |
| Targets |  Targets: Compatible targets based on packaged versions. <br/>Example: `{ "netcoreapp3.1" => [ "4.0", "4.1"] }, { "netcoreapp5.0 => ["4.0"] }`. <br/>"netcoreapp3.1" is supported for package versions - 4.0 and 4.1. |

#### License Details
| Attribute Name | Description |
| :------ | :---------- |
| License | License type. <br/>Example: `"License": { "MIT": [ "4.0.0" ], "Apache": [ "4.4.0" ] }`  |
| Title |  Title of the package. <br/>Example: `"Title": { "This package is used to analyse C# code": [ "4.0.0", "4.4.0" ] }` |
| Url |  Url of the license. <br/>Example: `"Url": { "https://licenses.nuget.org/MIT": [ "4.0.0", "4.4.0" ] }` |
| Description | Description of the package. <br/>Example: `"Details": { "This package is used to analyse C# code": [ "4.0.0", "4.4.0" ] }  |

## Contributing
* [Adding Recommendations](https://github.com/aws/porting-assistant-dotnet-datastore/blob/master/RECOMMENDATIONS.md)

## Security
* See [CONTRIBUTING](CONTRIBUTING.md#security-issue-notifications) for more information.

## License

This project is licensed under the Apache-2.0 License.

