using IngredientFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;

namespace IngredientFunction
{
    public static class Kaas
    {
        [FunctionName("Kaas")]
        public static async Task<Pizza> AddKaas([ActivityTrigger] Pizza pizza, TraceWriter log)
        {
            log.Info("Adding Kaas to Pizza..");

            await Task.Delay(TimeSpan.FromSeconds(5));

            pizza.Ingredienten.Add("Kaas");

            return pizza;
        }
    }
}
