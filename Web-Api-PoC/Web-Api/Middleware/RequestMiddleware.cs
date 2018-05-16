using Microsoft.AspNetCore.Http;
using RestApplicationWithMongoBackend.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestApplicationWithMongoBackend.Middleware
{
  public class RequestMiddleware
  {
    private readonly RequestDelegate next;
    private readonly IBackDoor backDoor;


    public RequestMiddleware(RequestDelegate next, IBackDoor backDoor)
    {
      this.next = next;
      this.backDoor = backDoor;
    }


    public async Task Invoke(HttpContext context)
    {
      if (backDoor.StopProcessing)
      {
        Console.WriteLine("Ups backDoor.Stopprocessing = true stopping");
        while (true)
        {
          Thread.Sleep(60000);
        }
      }
      await next.Invoke(context);
    }
  }
}
