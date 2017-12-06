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


## What is it?
Iron.NET is a .Net implementation of the npm package Iron, which can be found here :
Npm :  https://www.npmjs.com/package/iron  
Github:  https://github.com/hueniverse/iron  
According to their own documentation, the purpose is:  
> iron is a cryptographic utility for sealing a JSON object using symmetric key encryption with message integrity verification. Or in other words, it lets you encrypt an object, send it around (in cookies, authentication credentials, etc.), then receive it back and decrypt it. The algorithm ensures that the message was not tampered with, and also provides a simple mechanism for password rotation.

## Notes
This is still work in progress, but it is usable for Seal and Unseal.  




