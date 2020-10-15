# Introduction

> Recommendation file folder
https://github.com/aws/porting-assistant-dotnet-datastore/tree/master/recommendation

> Manifest file
https://github.com/aws/porting-assistant-dotnet-datastore/blob/master/data/namespaces.recommendation.lookup.json
	
## contribute to recommendation

1. add or modify recommendation in the same file data structure in RecommedationPOJO.cs
2. for new recommendation file, add namespace -> file name in the lookup file
https://github.com/aws/porting-assistant-dotnet-datastore/blob/master/data/namespaces.recommendation.lookup.json


## Requirements

* dotnet

## Technology
* C#

## run

```shell
runnable/RecommendationValidator.exe -f file-to-recommendation-path
```
for example

success response

```shell
runnable/RecommendationValidator.exe -f system.io.recommendation.json
```

error response

```shell
runnable/RecommendationValidator.exe -f foo.json

```

