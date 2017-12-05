# Iron.Net

A .Net port of the npm package Iron.  

## Install
Nuget Package Manager
```
Install-Package Fantasista.Iron.Net
```

Dotnet core:
```
dotnet add package Fantasista.Iron.Net
```


## Usage
```
            var iron = new Fantasista.IronDotNet.Iron();

            var b = iron.Seal("TestStringToSeal",GAPI_KEY,Fantasista.IronDotNet.Config.IronConfig.Default);
            var c = iron.Unseal(b,GAPI_KEY,Fantasista.IronDotNet.Config.IronConfig.Default);

```

## Notes
This is still work in progress, but it is usable for Seal and Unseal.  Should be refactored and some exceptions should be moved etc.




