using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TicTacToeClasses;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace TicTacToeOnline
{
    public static class WebPage
    {
        [FunctionName(nameof(LoadPage))]
        public static async Task<IActionResult> LoadPage(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string responseMessage = LoadBoard().Result;
            return new OkObjectResult(responseMessage);
        }

        [FunctionName(nameof(LoadBoard))]
        public static async Task<string> LoadBoard()
        {
            return GameController.CreateBoard();
        }


    }
}
