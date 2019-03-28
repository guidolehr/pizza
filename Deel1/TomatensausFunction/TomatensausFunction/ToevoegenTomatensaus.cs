using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using PizzaModels;

namespace PizzabodemFunction
{
    public static class ToevoegenTomatensaus
    {
        [FunctionName("ToevoegenTomatensaus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Adding tomatensaus to pizza");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var pizza = JsonConvert.DeserializeObject<Pizza>(requestBody);

            pizza.Ingredienten.Add("tomatensaus");
            
            return new OkObjectResult(pizza);
        }
    }
}
