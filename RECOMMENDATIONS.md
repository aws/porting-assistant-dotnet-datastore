# Recommendations
These are recommended actions to apply for - APIs, namespaces and packages - that are not supported on a .NET platform to migrate into .NET Core. "The Porting Assistant for .NET" tool consumes these recommendations dynamically, so the latest recommendations are used.

## Recommendation Files

Recommendations files are used to create mappings that can be used when porting a .NET framework project to .NET core. Each namespace will have its own file, and the data from the namespace file will be used when analyzing the compatibility and possible replacements for artifacts inside that namespace.

**The recommendation file will have the following properties:**

| Property Name | Description |
| :----------- | :---------- |
| Name | The namespace this file applies to. |
| Packages	| The list of packages that this namespace belongs to.	|
| Version	| The version of Porting Assistant this applies to.	|
| Recommendations	| The list of recommendations that are applicable under this namespace	|
  
  
**The Recommendations section is where we specify the changes needed to convert to .net core.** Each recommendation will have the below structure:

| Property Name | Description |
| :----------- | :---------- |
| Type	| The node to look for in the code for this recommendation. This can be  one of the following values: Namespace, Class, Attribute, Method	|
| Name	| The name of the node to look for. For example, if we’re looking for a  class, name can be the name of the class. For example, **SelectList** would be a name of class we look for	|
| Value	| The value of the node to look for. This is usually the name, in  addition to the namespace and type. For example, the value of **SelectList** would be **System.Web.Mvc.SelectList**	|
| KeyType	| This is the type of key we’re looking for. By default, this looks for  the name of the node. However, there are other options to find nodes by.  Below is a list of these options: <br/> Namespace: Name <br/> Class: BaseClass, ClassName, Identifier <br/> Attribute: Name Method: Name	|

  
**RecommendedActions**
This section describes the actions needed to migrate the node found  using the properties above
 
The recommendation section has the below properties:

| Property Name | Description |
| :----------- | :---------- |
| Source	| The source of the recommendation. This can be from the open source community  (OpenSource) or Amazon (Amazon)	|
| TargetFrameworks	| This section describes what version of .net core this recommendation  applies to. Some recommendations might be applicable to .net core 3.1, but  not 2.0.	|
| Description	| This is the description of the recommendation. It can be a list of  steps needed to migrate, or	|

### Example
In the example below, we have a recommendation to migrate from "System.Data.SqlClient" namespace that is not compatible in .NET Core to the compatible namespace.

**The top section is the namespace and package information. We use this to group recommendations together:**

```javascript
  "Name": "System.Data.SqlClient",
  "Version": "1.0.0",
  "Packages": [
    {
      "Name": "System.Data",
      "Type": "SDK"
    }
  ],
```

**The Recommendations section tells the system what to look for in the code file.** In this example, we are looking for a namespace called “System.Data.SqlClient”:

```javascript
   "Type": "Namespace",
   "Value": "System.Data.SqlClient",
```

**The description field will summarize all the required steps to migrate to .net core.** In this example, there are three steps:

1. Add a package reference to Microsoft.Data.SqlClient
1. Remove all using statements System.Data.SqlClient 
1. Add a using statement for Microsoft.Data.SqlClient

```javascript      
"Description": "Add reference to Microsoft.Data.SqlClient\r\nAdd Microsoft.Data.SqlClient namespace\r\n Remove System.Data.SqlClient namespace\r\n"
```

Below is the full recommendation file

```javascript
{
  "Name": "System.Data.SqlClient",
  "Version": "1.0.0",
  "Packages": [
    {
      "Name": "System.Data",
      "Type": "SDK"
    }
  ],
  "Recommendations": [
    {
      "Type": "Namespace",
      "Value": "System.Data.SqlClient",
      "RecommendedActions": [
        {
          "Source": "OpenSource",
          "TargetFrameworks": [
            "netcoreapp3.1"
          ],
          "Description": "Add reference to Microsoft.Data.SqlClient\r\nAdd Microsoft.Data.SqlClient namespace\r\n Remove System.Data.SqlClient namespace\r\n",          
        }
      ]
    }
  ]
}
```

## Adding Recommendations
1.  Use the template: [Template](namespace.json) to create a new recommendation JSON file.
1.  Fill the details as shown in the example.
1.  Rename the file with "namespace".json (Please use lower case letters)
1.  Place the file in **"porting-assistant-dotnet-datastore/recommendation/"** folder.
1.  Create a pull request!

