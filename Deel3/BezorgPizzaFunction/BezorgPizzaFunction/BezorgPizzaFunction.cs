using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PizzaModels;

namespace BezorgPizzaFunction
{
    public static class BezorgPizzaFunction
    {
        [FunctionName("BezorgPizzaFunction")]
        public static async Task<Pizza> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            Pizza pizza = new Pizza();

            pizza = await context.CallActivityAsync<Pizza>("BezorgPizzaFunction_ToevoegenPizzabodem", pizza);
            pizza = await context.CallActivityAsync<Pizza>("BezorgPizzaFunction_ToevoegenTomatensaus", pizza);
            pizza = await context.CallActivityAsync<Pizza>("BezorgPizzaFunction_BakPizza", pizza);
            pizza = await context.CallActivityAsync<Pizza>("BezorgPizzaFunction_BezorgPizza", pizza);

            return pizza;
        }

        [FunctionName("BezorgPizzaFunction_ToevoegenPizzabodem")]
        public static async Task<Pizza> ToevoegenPizzabodem([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation($"Toevoegen Pizzabodem");

            using (var client = new HttpClient())
            {
                string functionUri = "https://pizzabodem-func-euw-o.azurewebsites.net/api/ToevoegenPizzabodem";

                var response = await client.PostAsJsonAsync(functionUri, pizza);

                return await response.Content.ReadAsAsync<Pizza>();
            }
        }

        [FunctionName("BezorgPizzaFunction_ToevoegenTomatensaus")]
        public static async Task<Pizza> ToevoegenTomatensaus([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation($"Toevoegen Tomatensaus");

            using (var client = new HttpClient())
            {
                string functionUri = "https://tomatensaus-func-euw-o.azurewebsites.net/api/ToevoegenTomatensaus";

                var response = await client.PostAsJsonAsync(functionUri, pizza);

                return await response.Content.ReadAsAsync<Pizza>();
            }
        }

        [FunctionName("BezorgPizzaFunction_BakPizza")]
        public static async Task<Pizza> BakPizza([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation($"Pizza bakken in oven");

            using (var client = new HttpClient())
            {
                var functionUri = new Uri("https://oven-func-euw-o.azurewebsites.net/api/BakPizza_HttpStart");

                var response = await client.PostAsJsonAsync(functionUri, pizza);

                while (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    response = await client.GetAsync(response.Headers.Location);
                }

                var ovenStatus = await response.Content.ReadAsAsync<OvenStatus>();
                return ovenStatus.Output;
            }
        }

        [FunctionName("BezorgPizzaFunction_BezorgPizza")]
        public static async Task<Pizza> BezorgPizza([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation($"Pizza bezorgen");

            var endpointUri = new Uri("https://pizza-cm-euw-o.documents.azure.com:443/");
            var accessKey = "j4NE0BoZZiNyODyRV2wpiHAVp1rqIKrEH4aQKtiI7WKGXR12DMZ0JzUILoTHvxhKu22dfrKQYwDr42Hb2AH7dA==";

            var documentClient = new DocumentClient(endpointUri, accessKey);

            await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Pizza", "Pizzas"), pizza);

            return pizza;
        }

        [FunctionName("BezorgPizzaFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("BezorgPizzaFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}