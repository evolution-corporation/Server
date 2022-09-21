// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

var client = new HttpClient();
var message = new HttpRequestMessage(HttpMethod.Get, "http://62.84.125.238:8000/api/204");
client.Send(message);