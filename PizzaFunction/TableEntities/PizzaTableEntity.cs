using System;
using IngredientFunction.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace IngredientFunction.TableEntities
{
    public class PizzaTableEntity : TableEntity
    {
        public string Ingredienten { get; set; }

        public string Status { get; set; }
        
        public PizzaTableEntity(Status status, string ingredienten) : base("primary", Guid.NewGuid().ToString())
        {
            this.Status = status.ToString();
            this.Ingredienten = ingredienten;
        }
    }
}
