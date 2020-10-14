# Recommendations
These are recommended actions to apply for - APIs, namespaces and packages - that are not supported on a .NET platform to migrate into .NET Core. "The Porting Assistant for .NET" tool consumes these recommendations dynamically, so the latest recommendations are used.

## Adding Recommendations
1.  Use the template: [Template](namespace.json) to create a new recommendation JSON file.
1.  Fill the details as shown in the example.
1.  Rename the file with "namespace".json (Please use lower case letters)
1.  Place the file in **"porting-assistant-dotnet-datastore/recommendation/"** folder.
1.  Create a pull request!

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


