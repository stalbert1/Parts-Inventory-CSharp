using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Parts_Inventory_CSharp.Model
{
    public class Part
    {

        private string modelName;
        private string partDescription;
        private string partNumber;
        private string comments;

        //The next 2 lines is for the SQL lite DB and a unique field that autoupdates..
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //ctor double tab writes the stub for you
        public Part(string model, string partDesc, string partNum, string commt)
        {
            //Could call the property ModelName and then the constructor will use the setter, otherwise the setter of the propery will not get called.
            modelName = model;
            partDescription = partDesc;
            partNumber = partNum;
            comments = commt;
        }

        //created this to get rid of error while trying to read the database from the begginning.
        public Part()
        {
            modelName = "";
            partDescription = "";
            partNumber = "";
            comments = "";
        }


        public string ModelName
        {
            get { return modelName; }
            //new value and old value
            set 
            {
                modelName = value;
            }
        }

        public string PartDescription
        {
            get { return partDescription; }
            set { partDescription = value; }
        }

        public string PartNumber
        {
            get { return partNumber; }
            set { partNumber = value; }
        }

        public string Comments
        {
            get 
            { 
                if (comments == "No comments")
                {
                    return "";
                }
                else
                {
                    return comments;
                }
               
            }
            set { comments = value; }
        }

        //static method that returns boolean takes (partlist, part) will add the modelname to the partnumber to determine if the part that is 
        //passed in is in the list that is passed in
        public static bool isPartInPartsList(List<Part> partsList, Part partIn)
        {
            bool partsInList = false;

            foreach (var part in partsList)
            {
                //Want to catch a case where the part number or the model name is empty, only need to look at the part in
                if (partIn.ModelName == "" || partIn.PartNumber == "")
                {
                    partsInList = true;
                }

                if ((partIn.ModelName + partIn.PartNumber) == (part.ModelName + part.PartNumber))
                {
                    partsInList = true;
                }

            }

            return partsInList;

        }

        public string OutCSV()
        {
            //Model Name, Part Description, Part Number, Comments
            return $"{ModelName},{PartDescription},{PartNumber},{Comments}";
        }


        public override string ToString()
        {
            return $"Model Name = {modelName}, Part Description = {partDescription}, Part Number = {partNumber}, Comments = {comments}.";
        }

    }
}
