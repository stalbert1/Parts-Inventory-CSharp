using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Parts_Inventory_CSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Will use this to establish a path to the sqllite database
        private static string dataBaseName = "Parts.db";
        //This will get the my docs folder path so it can be used for the database.
        private static string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //should be able to refer to this as App.databasePath
        public static string dataBasePath = System.IO.Path.Combine(folderPath, dataBaseName);

        //this will be used as a place to save CSV files
        private static string csvFileName = "Parts_CSV.csv";
        public static string csvFilePath = System.IO.Path.Combine(folderPath, csvFileName);

    }
}
   
