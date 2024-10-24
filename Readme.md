# Iron.Net

A .Net port of the npm package Iron.  It should be able to unseal messages which have been sealed with the npm package and the npm package should be able to unseal whatever is sealed with this library.  

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
 * Npm :  https://www.npmjs.com/package/iron  
 * Github:  https://github.com/hueniverse/iron  
  
According to Iron's own documentation, the purpose is:  
> iron is a cryptographic utility for sealing a JSON object using symmetric key encryption with message integrity verification. Or in other words, it lets you encrypt an object, send it around (in cookies, authentication credentials, etc.), then receive it back and decrypt it. The algorithm ensures that the message was not tampered with, and also provides a simple mechanism for password rotation.

## Notes
This is still work in progress, but it is usable for Seal and Unseal.  

## License
The code is distributed under the Apache 2 license.  

## Version history
|Version| Comment                             |Date|
|-------|-------------------------------------|----|
|0.1.2| Updated to dotnet 8<br/>Added tests |2024-10-24|
|0.1.1  | Just some small refactoring         |2017-12-05|
|0.1.0  | First version                       |2017-12-04|