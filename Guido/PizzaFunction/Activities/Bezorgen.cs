using IngredientFunction.Models;
using IngredientFunction.TableEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace IngredientFunction
{
    public static class Bezorgen
    {
        [FunctionName("Bezorgen")]
        public static async Task<Pizza> Run([ActivityTrigger] Pizza pizza, [Table("Pizzas")] CloudTable cloudTable, TraceWriter log)
        {
            log.Info("Pizza bezorgen..");

            cloudTable.CreateIfNotExists();

            var tableEntity = new PizzaTableEntity(pizza.Status, string.Join(",", pizza.Ingredienten));
            TableOperation tableOperation = TableOperation.InsertOrReplace(tableEntity);
            cloudTable.Execute(tableOperation);

            await Task.Delay(TimeSpan.FromSeconds(10));

            pizza.Status = Status.Bezorgd;

            return pizza;
        }
    }
}
