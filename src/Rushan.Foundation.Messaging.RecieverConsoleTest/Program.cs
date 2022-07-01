using Foundation.Messaging.Reciever;
using System;

namespace RecieverConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reciever = new Reciever();
            reciever.Recieve();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
