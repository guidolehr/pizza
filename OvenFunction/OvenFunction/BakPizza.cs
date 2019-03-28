using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PizzaModels;

namespace OvenFunction
{
    public static class BakPizza
    {
        [FunctionName("BakPizza")]
        public static async Task<Pizza> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            Pizza pizza = context.GetInput<Pizza>();

            pizza = await context.CallActivityAsync<Pizza>("BakPizza_Wachten", pizza);

            return pizza;
        }

        [FunctionName("BakPizza_Wachten")]
        public static async Task<Pizza> Wachten([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation("Wachten totdat pizza gebakken is");

            await Task.Delay(new Random().Next(30000, 90000)); // Wacht 30-90 seconden
            pizza.Status = Status.Gebakken;

            return pizza;
        }

        [FunctionName("BakPizza_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bak-pizza")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            var pizza = JsonConvert.DeserializeObject<Pizza>(requestBody);

            string instanceId = await starter.StartNewAsync("bak-pizza-orchestrator", pizza);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}