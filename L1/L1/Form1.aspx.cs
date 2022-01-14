using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace L1
{
    public partial class Form1 : System.Web.UI.Page
    {
        public const int CMaxNumberOfRoutes = 50;
        // Maximum number of routes
        public char friendChar = 'D';
        // Friend tile in map
        public char meetChar = 'S';
        // Meeting place tile in map
        public char pizzaChar = 'P';
        // Pizzeria tile in map
        public char blockChar = 'X';
        // Blocking tile in map
        public char pathChar = '.';
        // Free movement tile in map
        public const int CMaxN = 20;
        // Maximum number of map height
        public const int CMaxM = 20;
        // Maximum number of map width
        public const int StartingNumber = 65;
        // Constant number to determine various path values

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string fileName = "App_Data/U3.txt";
            string resultFile = "App_Data/ResultFile.txt";
            int n, m;
            char[,] map;

            ReadData(fileName, out n, out m, out map);

            RouteContainer routes;
            List<Point> friends;

            FindTilesByType(out friends, map, n, m, friendChar);

            FindRoutes(map, n, m, out routes);

            CountPathValues(map, routes, friends, n, m, StartingNumber);

            Route bestRoute = routes.GetBest();

            DisplayAnswers(friends, bestRoute, n);

            PutMap(map, n, m);

            PrintDataToTextFile(map, n, m, friends, bestRoute, resultFile);
        }

        /// <summary>
        /// Prints starting and ending data to file
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <param name="friends"> List of friends coordinates </param>
        /// <param name="route"> Most optimal route </param>
        /// <param name="fileName"> Name of file </param>
        void PrintDataToTextFile(char[,] map, int n, int m, 
                                 List<Point> friends, Route route, 
                                 string fileName)
        {
            using(StreamWriter writer = 
                  new StreamWriter(Server.MapPath(fileName)))
            {
                int lineLength = 2 * m + 1;
                string line = new string('-', lineLength);
                writer.WriteLine(line);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        writer.Write("|" + map[i, j]);
                    }
                    writer.Write("|");
                    writer.WriteLine();
                    writer.WriteLine(line);
                }
                writer.WriteLine();
                foreach(Point point in friends)
                {
                    writer.WriteLine(point.X + " " + point.Y);
                }
                if(route.PathValue < 0)
                {
                    writer.WriteLine("Neįmanoma");
                }
                else
                {
                    writer.WriteLine("Susitikimo vieta " + route.Meet.X + 
                                     " " + route.Meet.Y);
                    writer.WriteLine("Picerija " + route.Pizza.X + 
                                     " " + route.Pizza.Y);
                    writer.WriteLine("Nueita " + route.PathValue);
                }
            }
        }

        /// <summary>
        /// Reads file data
        /// </summary>
        /// <param name="fileName"> Name of file </param>
        /// <param name="n"> Map's height, returned by argument </param>
        /// <param name="m"> Map's width, returned by argument </param>
        /// <param name="map"> Char array of map elements, 
        ///                    returned by argument </param>
        void ReadData(string fileName, out int n, out int m, out char[,] map)
        {            string[] lines = File.ReadAllLines(Server.MapPath(fileName));            string[] values = lines[0].Split(' ');            n = int.Parse(values[0]);            m = int.Parse(values[1]);            map = new char[CMaxN, CMaxM];            for (int i = 0; i < n; i++)
            {
                string line = lines[i + 1];
                for (int j = 0; j < m; j++)
                {
                    map[i, j] = line[j];
                }
            }
        }

        /// <summary>
        /// Finds all points in map by given type
        /// </summary>
        /// <param name="points"> List of found points 
        ///                       to be returned by argument </param>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <param name="type"> Char of tile to look for </param>
        void FindTilesByType(out List<Point> points, char[,] map, 
                             int n, int m, char type)
        {
            points = new List<Point>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if(map[i, j] == type)
                    {
                        points.Add(new Point(i, j));
                    }
                }
            }
        }

        /// <summary>
        /// Finds all the routes in the map
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <param name="routes"> Newly created route container 
        ///                       returned through argument </param>
        void FindRoutes(char[,] map, int n, int m, out RouteContainer routes)
        {
            routes = new RouteContainer(CMaxNumberOfRoutes);

            List<Point> meets;
            List<Point> pizzas;

            FindTilesByType(out meets, map, n, m, meetChar);
            FindTilesByType(out pizzas, map, n, m, pizzaChar);

            // Creates all routes, which contains 
            // one meeting place and one pizzeria
            foreach(Point meet in meets)
            {
                foreach(Point pizza in pizzas)
                {
                    Route route = new Route(meet.X, meet.Y, pizza.X, pizza.Y);
                    routes.Add(route);
                }
            }
        }

        /// <summary>
        /// Counts path values for each route
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="routes"> Route container </param>
        /// <param name="friends"> List of all of friends coordinates </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <param name="number"> Constant which determines length </param>
        void CountPathValues(char[,] map, RouteContainer routes, 
                             List<Point> friends, int n, 
                             int m, int number)
        {
            bool t = true;

            for (int i = 0; i < routes.Count; i++)
            {
                Route route = routes.Get(i);
                int value = 0;

                foreach(Point point in friends)
                {
                    // Finds shortest distance between 
                    // friend and a meeting place
                    int value1 = 
                        ShortestDistanceBetweenPoints(map, n, m, point, 
                                                      route.Meet, number);

                    // If a friend can go there, his value will be positive.
                    if(value1 <= 0)
                    {
                        // Sets bool value to false, later the whole route will
                        // be ignored.
                        t = false;
                        continue;
                    }

                    // Friend goes to the meeting place and back, hence
                    // multiplying value by 2
                    value += 2 * value1;
                }

                // Finds shortest distance between 
                // routes meeting place and pizzeria
                int value2 = 
                    ShortestDistanceBetweenPoints(map, n, m, route.Meet, 
                                                  route.Pizza, number);

                // If path is not valid or at least one friend can't get there,
                // we move on to another route.
                if(value2 <= 0 || !t)
                {
                    continue;
                }

                // Each friend goes to the pizzeria and back to meeting place,
                // hence multiplying by 2, and all of them takes the same route,
                // hence multiplying by friends count
                value += 2 * value2 * friends.Count;

                // Sets routes path value. If the route is not valid,
                // path value will stay as -1 by default action in the
                // constructor
                routes.Get(i).PathValue = value;
            }
        }

        /// <summary>
        /// Finds value of shortest distance between two given points
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <param name="a"> First given point </param>
        /// <param name="b"> Second given point </param>
        /// <param name="number"> Constant which determines length </param>
        /// <returns> Value of shortest distance </returns>
        int ShortestDistanceBetweenPoints(char[,] map, int n, int m, 
                                          Point a, Point b, int number)
        {
            char[,] mapCopy = new char[CMaxN, CMaxM];

            // Creates map copy, because we don't want to change the
            // original map
            mapCopy = Copy(map, n, m);
            TransformMap(mapCopy, n, m, a, b);

            // Adds starting point to list, which will be used
            // as a center of the wave created in the Path method.
            List<Point> points = new List<Point>();
            points.Add(a);
            
            Path(mapCopy, points, b, number, n, m);

            // Starting constant, subtracted from ending point's value
            // will evaluate to distance between 2 given points.
            return mapCopy[b.X, b.Y] - number;
        }

        /// <summary>
        /// Copies all of char array's elements to other
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <returns> A copy of given char array </returns>
        char[,] Copy(char[,] map, int n, int m)
        {
            char[,] copy = new char[CMaxN, CMaxM];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    copy[i, j] = map[i, j];
                }
            }
            return copy;
        }

        /// <summary>
        /// Changes blocking tiles to 1's and other to 0's
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        /// <param name="a"> First point of interest </param>
        /// <param name="b"> Second point of interest </param>
        void TransformMap(char[,] map, int n, int m, Point a, Point b)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if(map[i, j] == blockChar ||
                       map[i, j] == pizzaChar)
                    {
                        map[i, j] = '1';
                    }
                    if(map[i, j] == friendChar ||
                       map[i, j] == meetChar ||
                       map[i, j] == pathChar)
                    {
                        map[i, j] = '0';
                    }
                }
            }
            map[a.X, a.Y] = '0';
            map[b.X, b.Y] = '0';
        }

        /// <summary>
        /// Recursive method, which changes map values
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="points"> List of points which are being used </param>
        /// <param name="end"> End coordinate </param>
        /// <param name="number"> Number which is used 
        ///                       to change map values </param>
        /// <param name="n"> Map's height </param>
        /// <param name="m"> Map's width </param>
        void Path(char[,] map, List<Point> points, Point end, 
                  int number, int n, int m)
        {
            List<Point> nextPoints = new List<Point>();

            // Checks if ending point in a map is changed, or
            // there aren't any points in a list to 
            // spread the wave in a map.
            if(map[end.X, end.Y] > StartingNumber ||
               points.Count == 0)
            {
                return;
            }

            // For each point in list, we're checking 4 neighbours
            // and if their values haven't changed or they can still
            // be improved, it changes to set number, which will
            // change by one after each wave movement.
            foreach(Point point in points)
            {
                if(point.X + 1 < n &&
                   (map[point.X + 1, point.Y] == '0' ||
                   map[point.X + 1, point.Y] > number))
                {
                    map[point.X + 1, point.Y] = (char)(number + 1);
                    nextPoints.Add(new Point(point.X + 1, point.Y));
                }

                if (point.Y - 1 >= 0 &&
                   (map[point.X, point.Y - 1] == '0' ||
                   map[point.X, point.Y - 1] > number))
                {
                    map[point.X, point.Y - 1] = (char)(number + 1);
                    nextPoints.Add(new Point(point.X, point.Y - 1));
                }

                if (point.X - 1 >= 0 &&
                   (map[point.X - 1, point.Y] == '0' ||
                   map[point.X - 1, point.Y] > number))
                {
                    map[point.X - 1, point.Y] = (char)(number + 1);
                    nextPoints.Add(new Point(point.X - 1, point.Y));
                }

                if (point.Y + 1 < m &&
                   (map[point.X, point.Y + 1] == '0' ||
                   map[point.X, point.Y + 1] > number))
                {
                    map[point.X, point.Y + 1] = (char)(number + 1);
                    nextPoints.Add(new Point(point.X, point.Y + 1));
                }
            }

            // Calls same method with next set of points in a wave and a
            // one added to number to indicate that it is the next wave
            Path(map, nextPoints, end, number + 1, n, m);
        }

        /// <summary>
        /// Displays answers
        /// </summary>
        /// <param name="friends"> List of all of friends coordinates </param>
        /// <param name="route"> Most optimal route </param>
        /// <param name="n"> Map's height </param>
        void DisplayAnswers(List<Point> friends, Route route, int n)
        {
            foreach(Point point in friends)
            {
                point.TransformPoint(n);
            }
            friends = SortPointList(friends);

            // Puts each friends coordinates
            foreach (Point point in friends)
            {
                TableCell cell = new TableCell();
                TableRow row = new TableRow();
                cell.Text = point.X + " " + point.Y;
                row.Cells.Add(cell);
                Table1.Rows.Add(row);
            }

            // Puts answer if none of the routes work
            if (route.PathValue <= 0)
            {
                TableCell cell = new TableCell();
                TableRow row = new TableRow();
                cell.Text = "Neįmanoma";
                row.Cells.Add(cell);
                Table1.Rows.Add(row);
            }
            // Puts answer for the best route
            else
            {
                TableCell cell = new TableCell();
                TableRow row = new TableRow();

                route.Meet.TransformPoint(n);
                route.Pizza.TransformPoint(n);

                cell.Text = "Susitikimo vieta " + 
                            route.Meet.X + " " + 
                            route.Meet.Y;
                row.Cells.Add(cell);
                Table1.Rows.Add(row);

                TableCell cell1 = new TableCell();
                TableRow row1 = new TableRow();
                cell1.Text = "Picerija " + 
                             route.Pizza.X + " " + 
                             route.Pizza.Y;
                row1.Cells.Add(cell1);
                Table1.Rows.Add(row1);

                TableCell cell2 = new TableCell();
                TableRow row2 = new TableRow();
                cell2.Text = "Nueita " + route.PathValue;
                row2.Cells.Add(cell2);
                Table1.Rows.Add(row2);
            }
        }

        /// <summary>
        /// Sorts Point list.
        /// </summary>
        /// <param name="points"> List of points </param>
        /// <returns> Sorted list of points </returns>
        List<Point> SortPointList(List<Point> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].X > points[j].X ||
                       (points[i].X == points[j].X &&
                       points[i].Y > points[j].Y))
                    {
                        Point temp = points[i];
                        points[i] = points[j];
                        points[j] = temp;
                    }
                }
            }
            return points;
        }
        
        /// <summary>
        /// Prints map
        /// </summary>
        /// <param name="map"> Char array of map elements </param>
        /// <param name="n"> Maps height </param>
        /// <param name="m"> Map width </param>
        void PutMap(char[,] map, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                TableRow row = new TableRow();
                {
                    TableCell cell = new TableCell();
                    cell.Width = 20;
                    cell.Height = 20;
                    cell.Text = (n - i).ToString();
                    cell.VerticalAlign = VerticalAlign.Middle;
                    cell.HorizontalAlign = HorizontalAlign.Center;
                    row.Cells.Add(cell);
                }
                for (int j = 0; j < m; j++)
                {
                    TableCell cell = new TableCell();
                    cell.Width = 20;
                    cell.Height = 20;
                    cell.Text = map[i, j].ToString();
                    cell.VerticalAlign = VerticalAlign.Middle;
                    cell.HorizontalAlign = HorizontalAlign.Center;
                    row.Cells.Add(cell);
                }
                Table2.Rows.Add(row);
            }

            TableCell cell0 = new TableCell();
            TableRow row0 = new TableRow();
            cell0.Text = "";
            cell0.Width = 20;
            cell0.Height = 20;
            cell0.VerticalAlign = VerticalAlign.Middle;
            cell0.HorizontalAlign = HorizontalAlign.Center;
            row0.Cells.Add(cell0);
            for (int i = 0; i < m; i++)
            {
                TableCell cell = new TableCell();
                cell.Text = (i + 1).ToString();
                cell.Width = 20;
                cell.Height = 20;
                cell.VerticalAlign = VerticalAlign.Middle;
                cell.HorizontalAlign = HorizontalAlign.Center;
                row0.Cells.Add(cell);
            }
            Table2.Rows.Add(row0);

            Label1.Text = "P - picerija";
            Label2.Text = "D - vieno iš draugų pradinė vieta";
            Label3.Text = "S - galima susitikimo vieta";
            Label4.Text = ". - praeinamas langelis";
        }
    }
}