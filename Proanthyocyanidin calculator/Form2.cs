using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proanthyocyanidin_calculator
{
    public partial class Form2 : Form
    {
        FoodObject food;
        Form1 form;
        public Form2(Form1 form1,FoodObject obj)
        {
            form = form1;
            InitializeComponent();
            label1.Text += obj.Name;
            food = obj;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            food.SumOfMersWithInputWeight = (food.SumOfMers / 100.0 ) * getValue();
            food.Weight = getValue();
            form.setObj(food);
            this.Dispose();
        }
        public double getValue()
        {
            return Double.Parse(weightTb.Text);
        }
        public FoodObject getFood()
        {
            return food;
        }

        private void weightTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1.PerformClick();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("toDispose");
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("closing");
        }
    }
}
