using System.Collections.Generic;

namespace PizzaModels
{
    public class Pizza
    {
        public List<string> Ingredienten { get; }

        public Status Status { get; set; }

        public Pizza()
        {
            this.Ingredienten = new List<string>();
            this.Status = Status.Ongebakken;
        }
    }
}