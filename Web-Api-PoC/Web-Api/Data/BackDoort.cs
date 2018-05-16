using System;
namespace RestApplicationWithMongoBackend.Data
{
  public class BackDoor : IBackDoor
  {
    public bool StopProcessing { get; set; }
  }
}
