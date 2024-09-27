using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Visualization.Util
{
    public class ConfigManager
    {
        public static double GetConfigProperty(string name)
        {
            string? propertyString = ConfigurationManager.AppSettings.Get(name);
            int property;
            if (int.TryParse(propertyString, out property))
            {
                return (double)property / 10;
            }
            throw new Exception($"Invalid configuration for {name}, must be an integer.");
        }
        public static IBrush GetConfigColor(string name)
        {
            string? colorString = ConfigurationManager.AppSettings.Get(name);
            if (colorString != null)
            {
                return Brush.Parse(colorString);
            }
            throw new Exception($"Invalid configuration for {name}, must be hex color.");
        }
        public static LinkColorType GetLinkColorType()
        {
            string? colorString = ConfigurationManager.AppSettings.Get("LinkColor");
            if (colorString != null)
            {
                if (colorString == "byTargetBook")
                {
                    return LinkColorType.byTargetBook;
                }
                else if (colorString == "bySourceBook")
                {
                    return LinkColorType.bySourceBook;
                }
                else if (colorString == "byOrder")
                {
                    return LinkColorType.byOrder;
                }
            }
            throw new Exception("Invalid LinkColor configuration.");
        }

        public static List<int> GetBookList(string name)
        {
            string? booksIncluded = ConfigurationManager.AppSettings.Get(name);
            if (booksIncluded != null)
            {
                return booksIncluded.Split(',').Select(int.Parse).ToList();
            }
            throw new Exception("Invalid list of books, must be integers separated by commas");
        }
    }
}
