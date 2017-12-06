using System;

namespace Fantasista.IronDotNET.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            var iron = new Fantasista.IronDotNet.Iron();
            var b = iron.Seal("{\"TestStringToSeal\":14}",key,Fantasista.IronDotNet.Config.IronConfig.Default);
            var c = iron.Unseal(b,key,Fantasista.IronDotNet.Config.IronConfig.Default);
            Console.WriteLine(b);
            Console.WriteLine(c);
        }
    }
}
