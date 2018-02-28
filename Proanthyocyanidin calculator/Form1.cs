using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Proanthyocyanidin_calculator
{
    public partial class Form1 : Form
    {
        String pathToDb = null;
        OleDbConnection con;
        Dictionary<string, FoodObject> dictionary = new Dictionary<string, FoodObject>();
       
        public Form1()
        {
            InitializeComponent();
            listBox1.DisplayMember = "display";
            listBox2.DisplayMember = "display";
        }

        private void loadDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String path ="";

            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Access database files (*.accdb)|*.accdb;";
            if (file.ShowDialog() == DialogResult.OK)
            {
                path = file.FileName;
                checkConnectionToDb(path);
                pathToDb = path;
            }
            createObjectQuery();
            int counter = 1;
            listBox1.Items.Clear();
            foreach (KeyValuePair<string, FoodObject> entry in dictionary)
            {
                entry.Value.counter = counter;
                listBox1.Items.Add(entry.Value);
                counter++;
            }
            Console.WriteLine(path);
        }

        private void checkConnectionToDb(String path)
        {
            try
            {
                con = perfomConnection(path);
                con.Open();
                MessageBox.Show("Connected to db");
                label2.Text = "Succesfuly connected to database";
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex);
            }
        }
        private OleDbConnection perfomConnection(String path)
        {
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Jet OLEDB:Database Password=MyDbPassword;";
            return connection;
        }


        private void createObjectQuery()
        {
            con = perfomConnection(pathToDb);
            con.Open();
            OleDbDataReader reader = null;
            var cmd = new OleDbCommand("SELECT * FROM PA_DAT;", con);
            reader = cmd.ExecuteReader();
            String prevId = "";
            while (reader.Read())
            {
                string id = reader["NDB No"].ToString();
                FoodObject food;

                //jesli juz istnieje w slowniku obiekt wyciagam ze slownika
                if(prevId.Equals(id))
                {
                    food = dictionary[id];
                }
                //jesli nie o robie nowu
                else
                {
                    food = new FoodObject(id);
                    prevId = id;
                }
                food.SumOfMers += Double.Parse(reader["Flav_Val"].ToString());
                food.SumOfMersWithInputWeight = food.SumOfMers;
                if (!dictionary.ContainsKey(id))
                {
                    OleDbDataReader reader2 = null;
                    var cmd2 = new OleDbCommand("SELECT * FROM FOOD_DES WHERE FOOD_DES.[NDB No] like '" + id + "'", con);
                    reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        food.Name = reader2["Long_Desc"].ToString();
                        if (id.Equals("97020"))
                            food.Name = "Babyfood, fruit, apple, strawberry, banana";
                        dictionary.Add(id, food);
                    }
                }
            }
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (con == null)
            {
                MessageBox.Show("Please go to settings and select accdb file", "Error");
            }
            else
            {
                int counter = 1;
                if (textBox1.Text == null || textBox1.Text.Equals(""))
                {
                    listBox1.Items.Clear();
                    foreach (KeyValuePair<string, FoodObject> entry in dictionary)
                    {
                        entry.Value.counter = counter;
                        listBox1.Items.Add(entry.Value);
                        counter++;
                    }
                }
                else
                {
                    string name = textBox1.Text;
                    var r = from x in dictionary
                            where x.Value.Name.ToLower().Contains(name.ToLower())
                            select x;
                    listBox1.Items.Clear();
                    foreach (KeyValuePair<string, FoodObject> entry in r)
                    {
                        entry.Value.counter = counter;
                        listBox1.Items.Add(entry.Value);
                        counter++;
                    }
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1.PerformClick();
        }


        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && !listBox2.Items.Contains(listBox1.SelectedItem) && listBox1.SelectedItems.Count < 2)
            {
                var temp = listBox1.SelectedItem as FoodObject;
                using (var form2 = new Form2(this, temp))
                {
                    form2.ShowDialog();

                    listBox2.Items.Add(temp2);
                    listBox1.SelectedItems.Clear();
                }
            }
            calculateSum();
        }
        FoodObject temp2;
        public void setObj(FoodObject food)
        {
            temp2 = food;
        }
        private void button3_Click(object sender, EventArgs e)
        {

            listBox2.Items.Clear();
            calculateSum();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count < 1)
                MessageBox.Show("You cannot delete 0 items", "Error");
            else
            {
                var selectedItems = listBox2.SelectedItems;

                for(int i = listBox2.SelectedItems.Count - 1; i >= 0; i--)
                {
                    var foodObj = selectedItems[i] as FoodObject;
                    listBox2.Items.Remove(selectedItems[i]);
                }
            }
            calculateSum();
        }

        private void calculateSum()
        {
            double sum = 0;
            foreach(var item in listBox2.Items)
            {
                var foodObject = item as FoodObject ;
                sum += foodObject.SumOfMersWithInputWeight;
            }
            textBox2.Text = sum.ToString();
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if(listBox2.SelectedItems.Count < 2)
                listBox2.Items.Remove(listBox2.SelectedItem);
            else
            {
                MessageBox.Show("Cannot perform removing via double click on multiple elements, please use provided Button", "Operation error");
                listBox2.SelectedItems.Clear();
            }
            calculateSum();
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            listBox2.SelectedItems.Clear();
        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            listBox1.SelectedItems.Clear();
        }
    }
    public class FoodObject
    {
        private string id;
        private string name;
        public int counter;
        private string foodGroupId;
        private double sumOfMers;
        private double sumOfMersWithDifferentWeight;
        private double weight = 100;
        public FoodObject(string id)
        {
            this.id = id;
        }
        public override string ToString()
        {
            return counter + ". " + name + " " + sumOfMersWithDifferentWeight + " / " + weight+"g"; 
        }

        public string Name { get => name; set => name = value; }
        public string FoodGroupId { get => foodGroupId; set => foodGroupId = value; }
        public double SumOfMers { get => sumOfMers; set => sumOfMers = value; }
        public double Weight { get => weight; set => weight = value; }
        public double SumOfMersWithInputWeight { get => sumOfMersWithDifferentWeight; set => sumOfMersWithDifferentWeight = value; }
    }
}
