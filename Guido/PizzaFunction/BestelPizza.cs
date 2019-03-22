using IngredientFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IngredientFunction
{
    public static class BestelPizza
    {
        [FunctionName("BestelPizza")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "GET", "POST")] HttpRequestMessage req, [OrchestrationClient] DurableOrchestrationClient starter, ILogger log)
        {
            // Get request body
            dynamic eventData = await req.Content.ReadAsAsync<object>();

            // Start orchestration
            string instanceId = await starter.StartNewAsync("BestelPizzaOrchestration", eventData);
            

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            TimeSpan timeout = TimeSpan.FromSeconds(30);
            TimeSpan retryInterval = TimeSpan.FromSeconds(1);

            /*return await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(
                req,
                instanceId,
                timeout,
                retryInterval);
            */

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("BestelPizzaOrchestration")]
        public static async Task<Pizza> RunOrchestration([OrchestrationTrigger] DurableOrchestrationContextBase context, TraceWriter log)
        {
            Pizza pizza = new Pizza();

            // Maak Pizza
            pizza = await context.CallActivityAsync<Pizza>("Tomaat", pizza);
            pizza = await context.CallActivityAsync<Pizza>("Kaas", pizza);

            pizza.Status = Status.Gebakken;
            
            // Bezorg Pizza
            pizza = await context.CallActivityAsync<Pizza>("Bezorgen", pizza);
            
            return pizza;
        }
    }
}
