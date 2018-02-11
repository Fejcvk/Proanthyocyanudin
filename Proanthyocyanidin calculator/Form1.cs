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
            foreach (KeyValuePair<string, FoodObject> entry in dictionary)
            {
                richTextBox1.Text += entry.Value.Name + " " + entry.Value.SumOfMers + "\n";
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
                }
                food.SumOfMers += Double.Parse(reader["Flav_Val"].ToString());
                if (!dictionary.ContainsKey(id))
                {
                    OleDbDataReader reader2 = null;
                    var cmd2 = new OleDbCommand("SELECT * FROM FOOD_DES WHERE FOOD_DES.[NDB No]='" + id + "'", con);
                    reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        food.Name = reader2["Long_Desc"].ToString();
                        food.FoodGroupId = reader2["Long_Desc"].ToString();
                        dictionary.Add(id, food);
                    }
                }
                prevId = id;
            }
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == null || textBox1.Text.Equals(""))
            {
                MessageBox.Show("Name of product cannot be blank!", "Error");
            }
            else
            {
                string name = textBox1.Text;
                var r = from x in dictionary
                        where x.Value.Name.ToLower().Contains(name.ToLower())
                        select x;
                richTextBox1.Clear();
                foreach(KeyValuePair<string, FoodObject> obj in r)
                {
                    richTextBox1.Text += obj.Value.ToString() + "\n";
                }
            }
        }
    }
    public class FoodObject
    {
        private string id;
        private string name;
        private string foodGroupId;
        private double sumOfMers;
        public FoodObject(string id)
        {
            this.id = id;
        }
        public String ToString()
        {
            String ret = this.name + " " + this.SumOfMers.ToString();
            return ret;
        }
        public string Name { get => name; set => name = value; }
        public string FoodGroupId { get => foodGroupId; set => foodGroupId = value; }
        public double SumOfMers { get => sumOfMers; set => sumOfMers = value; }
    }
}
