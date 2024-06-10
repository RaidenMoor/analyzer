using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            tabControl1.TabPages.Remove(tabPage3);
            //tabPage3.Hide();
            button1.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void analise_Click(object sender, EventArgs e)
        {
            bool flag;
            if (userInput.Text != "")
            {
                StringBuilder ids;
                StringBuilder consts;
                Analyze.Analysis(userInput.Text, errorMessage, userInput, out flag, out ids, out consts);
                if (flag)
                {
                    tabPage3.Show();
                    button1.Show();
                    tabControl1.TabPages.Insert(2, tabPage3);
                    string[] idArray = ids.ToString().Split(',');
                    string[] constArray = consts.ToString().Split('\n');
                    idsGrid.DataSource = idArray;
                    constsGrid.DataSource = constArray;
                    stringOfUserInput.Text = userInput.Text;
                                       
                }

            }
            else
            {
                errorMessage.ForeColor = Color.Red;
                errorMessage.Text = "Строка не может быть пустой.";
            }
            
        }

        private void userInput_TextChanged(object sender, EventArgs e)
        {
            if (userInput.Text.Length > 1000)
            {
                MessageBox.Show("Вы превысили лимит ввода символов!");
                userInput.Text = string.Empty;
            }
            if (errorMessage.ForeColor == System.Drawing.Color.Black)
            {
                if ((userInput.Text.Length > 1) && (userInput.Text.Length < 150))
                {
                    tabControl1.TabPages.Remove(tabPage3);
                    
                    button1.Hide();
                    errorMessage.Text = " ";
                    errorMessage.ForeColor = Color.Red;

                }
            }

            
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            userInput.Text = string.Empty;
            
            tabControl1.TabPages.Remove(tabPage3);
            errorMessage.Text = string.Empty;
            errorMessage.ForeColor = System.Drawing.Color.IndianRed;
            button1.Visible = false;

        }     

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void FormingDataGrid(int rows, int columns)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
   
}
