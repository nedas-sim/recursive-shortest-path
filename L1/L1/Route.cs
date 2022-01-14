using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace L1
{
    /// <summary>
    /// Class designed to store data for individual Route.
    /// </summary>
    public class Route
    {
        public Point Meet { get; set; }
        public Point Pizza { get; set; }
        public int PathValue { get; set; }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="meetX"> Meeting place's X coordinate </param>
        /// <param name="meetY"> Meeting place's Y coordinate </param>
        /// <param name="pizzaX"> Pizzeria's X coordinate </param>
        /// <param name="pizzaY"> Pizzeria's Y coordinate </param>
        public Route(int meetX, int meetY, int pizzaX, int pizzaY)
        {
            Meet = new Point(meetX, meetY);
            Pizza = new Point(pizzaX, pizzaY);
            PathValue = -1; 
            //sets distance as -1, will change if the path is valid
        }
    }
}