// See https://aka.ms/new-console-template for more information
using Rushan.Foundation.Messaging.Helpers;

Console.WriteLine("Hello, World!");
var actual = ApplicationHelper.GetApplicationName();
Console.WriteLine(actual);
Console.ReadKey();