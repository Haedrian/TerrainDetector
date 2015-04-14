using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;

namespace TerrainDetectorServices
{
    /// <summary>
    /// Summary description for TerrainDetector
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TerrainDetector : System.Web.Services.WebService
    {
        private readonly List<Tuple<Color, string>> terrains;

        private static readonly Color FARMLAND = Color.FromArgb(212, 229, 202);
        private static readonly Color BUILT_UP = Color.FromArgb(240, 237, 228);
        private static readonly Color FOREST = Color.FromArgb(201, 222, 169);

        public TerrainDetector()
        {
            terrains = new List<Tuple<Color, string>>();

            Color FARMLAND = Color.FromArgb(212, 229, 202);
            Color BUILT_UP = Color.FromArgb(240, 237, 228);
            Color FOREST = Color.FromArgb(201, 222, 169);

            terrains.Add(new Tuple<Color, string>(FARMLAND, "Farmland"));
            terrains.Add(new Tuple<Color, string>(BUILT_UP, "Built Up"));
            terrains.Add(new Tuple<Color, string>(FOREST, "Forest"));

        }

        [WebMethod]
        public string IdentifyTerrain(float lat, float lon)
        {
            Bitmap map = DownloadMap(lat, lon);

            //Go through the map and count the pixels!
            List<Tuple<Color, int>> colours = new List<Tuple<Color, int>>();

            for (int x = 0; x < map.Width; x++ )
            {
                for(int y=0; y < map.Height; y++)
                {
                    Color pixel = map.GetPixel(x, y);

                    var matched = colours.FirstOrDefault(c => c.Item1.Equals(pixel));

                    if (matched != null)
                    {
                        matched = new Tuple<Color, int>(pixel, matched.Item2 + 1); //Apparently tuples are immutable. Who knew...
                    }
                    else
                    {
                        colours.Add(new Tuple<Color, int>(pixel, 1));
                    }
                }
            }

            var pixelColour = colours.OrderByDescending(c => c.Item2).Select(c => c.Item1).FirstOrDefault();

            var matchedString = terrains.FirstOrDefault(t => t.Item1.Equals(pixelColour));

            if (matchedString != null)
            {
                return matchedString.Item2;
            }
            else
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Downloads the map from google's service
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        private Bitmap DownloadMap(float lat, float lon)
        {
            WebClient client = new WebClient();

            using (Stream stream = client.OpenRead("https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lon + "&zoom=16&size=100x100&maptype=terrain"))
            {
                Bitmap bitmap = new Bitmap(stream);
                return bitmap;
            }
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
