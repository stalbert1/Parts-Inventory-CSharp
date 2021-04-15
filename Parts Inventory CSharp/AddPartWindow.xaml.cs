using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Parts_Inventory_CSharp.Model;
using SQLite;

namespace Parts_Inventory_CSharp
{
    /// <summary>
    /// Interaction logic for AddPartWindow.xaml
    /// </summary>
    public partial class AddPartWindow : Window
    {

        //this will be used to update a part if passed in from the constructor.
        private Part part;

        //private string test;

        ////good example of using a setter.
        //public string Test
        //{
        //    get { return test; }

        //    set 
        //    { 
        //        //should I pass the entire list which is only a ref

        //        test = value;
        //        //now lets set a text box to the value
        //        txtComments.Text = test;
        //    }
        //}



        //When the window instance is called will want a string for this CTOR
        public AddPartWindow(List<Part> partsList)
        {
            InitializeComponent();

            //a sorted set will only keep unique items.
            var uModelNames = new SortedSet<string>();
            foreach (var part in partsList)
            {
                uModelNames.Add(part.ModelName);
            }

            uModelNames.Add("Add New Model Name");

            cboModelName.ItemsSource = uModelNames;

        }

        //This is the constructor that will be called when you want to update a part in the database
        public AddPartWindow(Part partToUpdate)
        {
            InitializeComponent();
            //Make it so we will have a txt box for model instead of the combo box
            cboModelName.Visibility = Visibility.Hidden;
            txtModelName.Visibility = Visibility.Visible;

            //Take the part that was passed in and fill in the text boxes
            txtModelName.Text = partToUpdate.ModelName;
            txtPartDescription.Text = partToUpdate.PartDescription;
            txtPartNumber.Text = partToUpdate.PartNumber;
            txtComments.Text = partToUpdate.Comments;

            //Will we create an update button or repurpose the add button?
            btnAddPart.Content = "Update Part";

            this.part = partToUpdate;

        }

        //This is the constructor of the class and is called during construction...
        public AddPartWindow()
        {
            InitializeComponent();


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(test);
            this.Close();

        }

        private void AddPart()
        {
            //Need to determine if the model name is coming from the combo box or the text box.
            var modelName = "";

            if (cboModelName.IsVisible)
            {
                //test if selected index is -1, will be like this if nothing is selected.
                if (cboModelName.SelectedIndex != -1)
                {
                    modelName = cboModelName.Items.GetItemAt(cboModelName.SelectedIndex).ToString();
                }

            }
            else
            {
                modelName = txtModelName.Text;
            }

            //Pass the part back to the other window to save.
            var returningPart = new Part(modelName, txtPartDescription.Text, txtPartNumber.Text, txtComments.Text);

            //Now how to return the part to the other window. This will allow to get the instance of the MainWindow memory.
            ((MainWindow)Application.Current.MainWindow).returningPart = returningPart;

            this.Close();

        }

        private void UpdatePart()
        {
            //Can we update the part from here and just have the database read when returned?
            //If so can simplify the call back on the part

            this.part.ModelName = txtModelName.Text;
            this.part.PartDescription = txtPartDescription.Text;
            this.part.PartNumber = txtPartNumber.Text;
            this.part.Comments = txtComments.Text;

            using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
            {
                connection.CreateTable<Part>();
                connection.Update(part);
            }

            this.Close();

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

        

        private void cboModelName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Selection changed was triggered.");
            var cboBox = sender as ComboBox;
            var i = cboBox.SelectedIndex;
            var modelSelected = cboModelName.Items.GetItemAt(i).ToString();
            Console.WriteLine(modelSelected);

            if (modelSelected == "Add New Model Name")
            {
                Console.WriteLine("Take action");
                //Will make it so the text box will need to be used to enter the new model name.
                cboModelName.Visibility = Visibility.Hidden;
                txtModelName.Visibility = Visibility.Visible;
            }

        }
    }
}
