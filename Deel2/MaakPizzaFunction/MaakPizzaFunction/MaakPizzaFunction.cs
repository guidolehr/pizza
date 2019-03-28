using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PizzaModels;

namespace MaakPizzaFunction
{
    public static class MaakPizzaFunction
    {
        [FunctionName("MaakPizzaFunction")]
        public static async Task<Pizza> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            Pizza pizza = new Pizza();

            pizza = await context.CallActivityAsync<Pizza>("MaakPizzaFunction_ToevoegenPizzabodem", pizza);
            pizza = await context.CallActivityAsync<Pizza>("MaakPizzaFunction_ToevoegenTomatensaus", pizza);

            return pizza;
        }

        [FunctionName("MaakPizzaFunction_ToevoegenPizzabodem")]
        public static async Task<Pizza> ToevoegenPizzabodem([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation($"Toevoegen Pizzabodem");

            using (var client = new HttpClient())
            {
                string functionUri = "https://pizzabodem-func-euw-o.azurewebsites.net/api/toevoegen-pizzabodem";

                var response = await client.PostAsJsonAsync(functionUri, pizza);

                return await response.Content.ReadAsAsync<Pizza>();
            }
        }

        [FunctionName("MaakPizzaFunction_ToevoegenTomatensaus")]
        public static async Task<Pizza> ToevoegenTomatensaus([ActivityTrigger] Pizza pizza, ILogger log)
        {
            log.LogInformation($"Toevoegen Tomatensaus");

            using (var client = new HttpClient())
            {
                string functionUri = "https://tomatensaus-func-euw-o.azurewebsites.net/api/toevoegen-tomatensaus";

                var response = await client.PostAsJsonAsync(functionUri, pizza);

                return await response.Content.ReadAsAsync<Pizza>();
            }
        }

        [FunctionName("MaakPizzaFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("MaakPizzaFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}