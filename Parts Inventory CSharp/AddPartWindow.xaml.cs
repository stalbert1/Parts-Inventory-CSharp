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

namespace Parts_Inventory_CSharp
{
    /// <summary>
    /// Interaction logic for AddPartWindow.xaml
    /// </summary>
    public partial class AddPartWindow : Window
    {

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

        private void btnAddPart_Click(object sender, RoutedEventArgs e)
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
