namespace RestApplicationWithMongoBackend.Data
{
  public interface IBackDoor
  {
    bool StopProcessing { get; set; }
  }
}