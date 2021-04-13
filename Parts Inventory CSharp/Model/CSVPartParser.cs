using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parts_Inventory_CSharp.Model
{
    public class CSVPartParser
    {
        public static List<Part> createPartList(string CSVFileNamePath)
        {
            var lines = File.ReadAllLines(CSVFileNamePath);

            //This will be the item that this function returns.
            var partsList = new List<Part>();

            for (int i = 0; i < lines.Count(); i++)
            {
                //Will ignore the first line as this is just the header
                if (i > 0)
                {
                    var columns = lines[i].Split(',');

                    if (columns.Count() > 3)
                    {
                        var tempPart = new Part(columns[0], columns[1], columns[2], columns[3]);
                        partsList.Add(tempPart);
                    }

                    

                }
            }

            return partsList;
        }
    }
}
