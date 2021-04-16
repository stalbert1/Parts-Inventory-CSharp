using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
//added the below using statements
using Parts_Inventory_CSharp.Model;
using SQLite;


namespace Parts_Inventory_CSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Part> partsList;
        List<Part> filteredList;

        //This will be the returning part coming back from the add window.
        public Part returningPart;

        //This will keep hold of the currently selected part for updating or deleting
        private Part selectedPart;

        //added sqlite-net-pcl .nuget package right click references manage Nuget Packages.

        public MainWindow()
        {
            InitializeComponent();
            ReadDataBase();
            //btnDeletePart.IsEnabled = false;
            PartIsNotSelected();

        }

        //Created these 2 methods because when a part is selected or unselected will need to change 2 things
        private void PartIsSelected()
        {
            btnAddPart.Content = "Update Part";
            btnDeletePart.IsEnabled = true;

        }

        private void PartIsNotSelected()
        {
            btnAddPart.Content = "Add Part";
            btnDeletePart.IsEnabled = false;
        }

       

        private void ReadDataBase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))

            {
                connection.CreateTable<Part>();
                partsList = connection.Table<Part>().ToList();
                
                

                if (partsList != null)
                {
                    partsListView.ItemsSource = partsList;
                    lblSummary.Content = $"{partsList.Count} parts listed.";
                }

            }


        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

            filteredList = partsList.Where(p => p.PartDescription.ToLower().Contains(txtFilter.Text) ||
                                                p.ModelName.ToLower().Contains(txtFilter.Text) ||
                                                p.Comments.ToLower().Contains(txtFilter.Text)).ToList();
            partsListView.ItemsSource = filteredList;
            lblSummary.Content = $"{filteredList.Count} parts listed.";

            PartIsNotSelected();
        }

        private void AddPart()
        {
            AddPartWindow partWindow = new AddPartWindow(partsList);

            //partWindow.Show();
            partWindow.ShowDialog();

            //Now that it's back returning part may have a part to add
            if (returningPart != null)
            {
                if (Part.isPartInPartsList(partsList, returningPart) == false)
                {
                    //need to add to the database and then read the table
                    Console.WriteLine($"Part should be added as it's not in the list\n{returningPart.ToString()}");
                    using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
                    {
                        var tempPart = new Part();
                        tempPart.ModelName = returningPart.ModelName;
                        tempPart.PartNumber = returningPart.PartNumber;
                        tempPart.PartDescription = returningPart.PartDescription;
                        tempPart.Comments = returningPart.Comments;
                        //by using the using statement no need to close the database connection
                        connection.CreateTable<Part>();
                        //Once inserted into the database is where it will pickup the Id field.
                        connection.Insert(tempPart);

                        MessageBox.Show($"The {tempPart.PartDescription} was added to the database.");
                    }

                    ReadDataBase();

                }
                else
                {
                    //Console.WriteLine($"Part is already in the database\n{returningPart.ToString()}");
                    MessageBox.Show($"The Part is already in the database, or has an empty part number or model number.\n{returningPart.ToString()}");
                }

            }

            //if so can i set it back to null?
            returningPart = null;

        }

        private void UpdatePart()
        {
            Console.WriteLine("Update Part");
            AddPartWindow partWindow = new AddPartWindow(selectedPart);

            partWindow.ShowDialog();

            //If the database gets updated in the update part call will need to read
            ReadDataBase();

        }

        private void btnAddPart_Click(object sender, RoutedEventArgs e)
        {
            if (btnAddPart.Content.ToString() == "Add Part")
            {
                AddPart();
            }
            else
            {
                UpdatePart();
            }
            

        }

        private void btnImportCsv_Click(object sender, RoutedEventArgs e)
        {

            //Remember the import will not have the Id field, as it started life on the iphone app.

            //This will import a CSV file of parts and place them in the SQL lite database
            List<Part> partsListImport = new List<Part>();

            //need to import using statement to use OpenFileDialog
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;
                partsListImport = CSVPartParser.createPartList(filename);

            }


            //Will need to determine if the part already exist in the database before allowing entry
            using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
                {
                //create a variable to keep track of how many parts were added to the database
                var numPartsAdded = 0;

                    foreach (var part in partsListImport)
                    {

                    //Only add the part to the database if it is not in the partsList already, ensure partsList isn't null first.
                    if (partsList != null)
                    {
                        //Comparing the part in the imported parts list vs the actual partsList in memory.
                        if (Part.isPartInPartsList(partsList, part) == false)
                        {
                            var tempPart = new Part();
                            tempPart.ModelName = part.ModelName;
                            tempPart.PartNumber = part.PartNumber;
                            tempPart.PartDescription = part.PartDescription;
                            tempPart.Comments = part.Comments;
                            //by using the using statement no need to close the database connection
                            connection.CreateTable<Part>();
                            //Once inserted into the database is where it will pickup the Id field.

                            connection.Insert(tempPart);
                            numPartsAdded++;

                        }

                    }
  
                    }

                MessageBox.Show($"There were {numPartsAdded} parts added to the database");
                }

            ReadDataBase();


        }

        private void btnExportCsv_Click(object sender, RoutedEventArgs e)
        {
  
            using (StreamWriter sw = File.CreateText(App.csvFilePath))
            {
                //Will first write in the headers
                sw.WriteLine("Model Name,Part Description,Part Number,Comments");

                foreach (var part in partsList)
                {
                    //will write each part in seperated with commas.
                    sw.WriteLine(part.OutCSV());
                }

            }

            //should show window showing that the save was a success.
            MessageBox.Show("The CSV file was saved, it should be in your Documents Directory.");

        }

        private void btnDeletePart_Click(object sender, RoutedEventArgs e)
        {         
            //Will need to ensure that a part is selected first.
            if (selectedPart != null)
            {

                //give a selection to do this or not.
                string message = $"The {selectedPart.PartDescription} will be deleted from the database.";
                string caption = "Do you want to Delete this part?";
                var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
                    {
                        connection.CreateTable<Part>();
                        //Will need Id field to delete.
                        connection.Delete(selectedPart);
                    }

                }               
            }

            ReadDataBase();
            //btnDeletePart.IsEnabled = false;
            PartIsNotSelected();

        }

        private void partsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //btnDeletePart.IsEnabled = true;
            PartIsSelected();

            selectedPart = (Part)partsListView.SelectedItem;


        }
    }
}
