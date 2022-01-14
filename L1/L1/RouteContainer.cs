using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace L1
{
    /// <summary>
    /// Class designed to store data of various Routes.
    /// </summary>
    public class RouteContainer
    {
        Route[] routes;
        public int Count { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"> Max size for array </param>
        public RouteContainer(int size)
        {
            routes = new Route[size];
            Count = 0;
        }

        /// <summary>
        /// Adds a Route to array
        /// </summary>
        /// <param name="route"> Route to add </param>
        public void Add(Route route)
        {
            routes[Count++] = route;
        }

        /// <summary>
        /// Returns a Route
        /// </summary>
        /// <param name="index"> Index of the Route </param>
        /// <returns> Route by index </returns>
        public Route Get(int index)
        {
            return routes[index];
        }

        /// <summary>
        /// Returns a Route which has the lowest positive value of PathValue.
        /// </summary>
        /// <returns> Most optimal Route </returns>
        public Route GetBest()
        {
            Route route = routes[0];

            for (int i = 1; i < Count; i++)
            {
                if(routes[i].PathValue > 0 &&
                   route.PathValue > routes[i].PathValue)
                {
                    route = routes[i];
                }
            }

            return route;
        }
    }
}