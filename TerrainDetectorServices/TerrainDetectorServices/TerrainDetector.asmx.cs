using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using TerrainDetectorServices.Objects;

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

        public TerrainDetector()
        {
            terrains = new List<Tuple<Color, string>>();

            Color FARMLAND = Color.FromArgb(203, 220, 193);
            Color BUILT_UP = Color.FromArgb(233, 229, 220);
            Color FOREST = Color.FromArgb(201, 222, 169);

            terrains.Add(new Tuple<Color, string>(FARMLAND, "Farmland"));
            terrains.Add(new Tuple<Color, string>(BUILT_UP, "Built Up"));
            terrains.Add(new Tuple<Color, string>(FOREST, "Forest"));
            terrains.Add(new Tuple<Color, string>(Color.FromArgb(255, 241, 238, 230), "Beach"));
            terrains.Add(new Tuple<Color, string>(Color.FromArgb(173, 205, 253), "Water"));

        }

        [WebMethod]
        public string IdentifyTerrain(float lat, float lon)
        {
            Bitmap map = DownloadMap(lat, lon);

            //Go through the map and count the pixels!
            List<ColourCount> colours = new List<ColourCount>();

            for (int x = 0; x < map.Width; x++ )
            {
                for(int y=0; y < map.Height; y++)
                {
                    Color pixel = map.GetPixel(x, y);

                    var matched = colours.FirstOrDefault(c => c.Colour.Equals(pixel));

                    if (matched != null)
                    {
                        matched.Count++;
                    }
                    else
                    {
                        colours.Add(new ColourCount() { Colour = pixel });
                    }
                }
            }

            var pixelColour = colours.OrderByDescending(c => c.Count).Select(c => c.Colour).FirstOrDefault();

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

            var url = "https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lon + "&zoom=16&size=100x100&maptype=terrain&format=png&style=feature:all|element:labels|visibility:off&style=feature:road|element:all|visibility:off";

            //client.DownloadFile(url,"image.pg")

            using (Stream stream = client.OpenRead(url))
            {
                Bitmap bitmap = new Bitmap(stream);

                bitmap.Save("C:\\Temp\\image.png", ImageFormat.Png);

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
