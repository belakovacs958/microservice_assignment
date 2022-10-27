// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Builder;
using ProductApi.Infrastructure;



var builder = WebApplication.CreateBuilder(args);
string cloudAMQPConnectionString =
   "host=hawk.rmq.cloudamqp.com;virtualHost=aaqlhcqa;username=aaqlhcqa;password=dmojgvjNDOFGSV9WtVqJjcql0wnG6AtP";

var app = builder.Build();

Task.Factory.StartNew(() =>
    new MessageListener(app.Services, cloudAMQPConnectionString).Start());

app.Run();