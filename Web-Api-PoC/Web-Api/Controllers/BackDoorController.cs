using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApplicationWithMongoBackend.Data;
using RestApplicationWithMongoBackend.Models;

namespace RestApplicationWithMongoBackend.Controllers
{
  [Produces("application/json")]
  [Route("backdoor")]
  public class BackDoorController : Controller
  {
    private readonly IBackDoor backDoorData;
    private readonly ILogger logger;
    public BackDoorController(IBackDoor backDoorData, ILoggerFactory loggerFactory)
    {
      this.backDoorData = backDoorData;
      logger = loggerFactory.CreateLogger("BackDoorController");
    }

    // GET: http://localhost:8080/backdoor/
    [HttpGet]
    public IEnumerable<ParameterValue> Get()
    {
      logger.LogInformation("BackDoor Get");
      return backDoorData.GetType().GetProperties().Select(prop => new ParameterValue { Parameter = prop.Name, Value = prop.GetValue(backDoorData, null).ToString() }).ToArray();
    }



    // PUT: http://localhost:8080/backdoor/
    // Body:
    //{
    //	"parameter": "stopprocessing",
    //	"value": "True"
    //}
    [HttpPut()]
    public IActionResult Put([FromBody]ParameterValue parameterValue)
    {
      if (parameterValue == null)
      {
        logger.LogError("parameterValue == null");
        return NotFound();
      }
      if (parameterValue.Parameter == null)
      {
        logger.LogError("parameterValue.Parameter == null");
        return NotFound();
      }
      if (parameterValue.Value == null)
      {
        logger.LogError("parameterValue.Value == nul");
        return NotFound();
      }
      try
      {
        logger.LogInformation("BackDoor request Parameter {0} Value {1}", parameterValue.Parameter, parameterValue.Value);
        PropertyInfo propertyInfo = backDoorData.GetType().GetProperty(parameterValue.Parameter);
        propertyInfo.SetValue(backDoorData, Convert.ChangeType(parameterValue.Value, propertyInfo.PropertyType), null);
        return Ok();
      }
      catch (Exception)
      {
        logger.LogError("Parameter {0} Value {1} not found", parameterValue.Parameter, parameterValue.Value);
        return NotFound();
      }
    }
  }
}
