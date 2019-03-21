using IngredientFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;

namespace IngredientFunction
{
    public static class Tomaat
    {
        [FunctionName("Tomaat")]
        public static async Task<Pizza> AddTomaat([ActivityTrigger] Pizza pizza, TraceWriter log)
        {
            log.Info("Adding Tomaat to Pizza..");

            await Task.Delay(TimeSpan.FromSeconds(5));

            pizza.Ingredienten.Add("Tomaat");

            return pizza;
        }
    }
}
