using System;

namespace PublisherConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reciever = new Foundation.Messaging.Reciever.PublisherConsoleTest();
            reciever.Recieve();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
