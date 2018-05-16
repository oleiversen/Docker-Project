using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrafficGenerator
{
  class Program
  {
    private static HttpClient client;
    static void Main(string[] args)
    {
      if (args.Length != 3)
      {
        Console.WriteLine("Please provide input uri and count in the form: http://host.docker.internal:8080/api/values 1000 Tag");
        return;
      }
      Uri uri;
      if (!Uri.TryCreate(args[0], UriKind.Absolute, out uri))
      {
        Console.WriteLine("Please provide a valid Uri");
        return;
      }

      int requestCount;
      if (!int.TryParse(args[1], out requestCount))
      {
        Console.WriteLine("Please provide a valid integer");
        return;
      }

      string tag = args[2];

      Console.WriteLine("Start");
      Task.Run(async () =>
      {
        await MakeRestCallsAsync(uri, requestCount, tag);
      }).GetAwaiter().GetResult();

      Console.WriteLine("Stop");
    }

    private static async Task MakeRestCallsAsync(Uri uri, int count, string tag)
    {
      client = new HttpClient { MaxResponseContentBufferSize = 1000000 };

      for (int request = 0; request < count; request++)
      {
        var queryString = new Dictionary<string, string>()
                                        {
                                            { "id", Guid.NewGuid().ToString() },
                                            { "value", tag}
                                        };
        var content = new StringContent(JsonConvert.SerializeObject(queryString), Encoding.UTF8, "application/json");
        try
        {

          var result = await client.PostAsync(uri, content);
          Console.WriteLine("Result {0}", result.StatusCode);
        }
        catch (Exception e)
        {
          await Console.Error.WriteLineAsync(GetExceptionMessages(e));
        }
      }
    }

    private static string GetExceptionMessages(Exception e, string msgs = "")
    {
      if (e == null) return string.Empty;
      if (msgs == "") msgs = e.Message;
      if (e.InnerException != null)
        msgs += "\r\nInnerException: " + GetExceptionMessages(e.InnerException);
      return msgs;
    }
  }
}
