using Foundation.Messaging.Publisher;
using System;

namespace RecieverConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reciever = new Publisher();
            reciever.Publish();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
