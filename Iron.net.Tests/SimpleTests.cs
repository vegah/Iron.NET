using Fantasista.IronDotNet.Config;

namespace Iron.net.Tests;

public class SimpleTests
{
    [Fact]
    public void It_is_reversable()
    {
        var iron = new Fantasista.IronDotNet.Iron();
        var GAPI_KEY = "blah123123123#######123123123123123123123";
        var stringToSeal = "TestStringToSeal";
        var b = iron.Seal(stringToSeal,GAPI_KEY,Fantasista.IronDotNet.Config.IronConfig.Default);
        var c = iron.Unseal(b,GAPI_KEY,Fantasista.IronDotNet.Config.IronConfig.Default);
        Assert.Equal(stringToSeal,c);
    }

    [Fact]
    public void It_unseals_like_the_js_package()
    {
        var password = "some_not_random_password_that_is_also_long_enough";
        var iron = new Fantasista.IronDotNet.Iron();
        var sealedString =
            "Fe26.2**0cdd607945dd1dffb7da0b0bf5f1a7daa6218cbae14cac51dcbd91fb077aeb5b*aOZLCKLhCt0D5IU1qLTtYw*g0ilNDlQ3TsdFUqJCqAm9iL7Wa60H7eYcHL_5oP136TOJREkS3BzheDC1dlxz5oJ**05b8943049af490e913bbc3a2485bee2aaf7b823f4c41d0ff0b7c168371a3772*R8yscVdTBRMdsoVbdDiFmUL8zb-c3PQLGJn4Y8C-AqI";
        var result = iron.Unseal(sealedString,password, IronConfig.Default);
        Assert.Equal("{\"a\":1,\"b\":2,\"c\":[3,4,5],\"d\":{\"e\":\"f\"}}",result);
        
    }
}