using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace L1
{
    /// <summary>
    /// A class designed to store data for Point.
    /// </summary>
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="x_"> Point's X coordinate </param>
        /// <param name="y_"> Point's Y coordinate </param>
        public Point(int x_, int y_)
        {
            X = x_;
            Y = y_;
        }

        /// <summary>
        /// Changes X and Y values for correct answer.
        /// </summary>
        /// <param name="n"> Map's height </param>
        public void TransformPoint(int n)
        {
            int xTemp = X;
            X = Y + 1;
            Y = n - xTemp;
        }
    }
}