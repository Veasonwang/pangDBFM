using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 庞西垌数据库
{
    public partial class Qry : Form
    {
        public DataTable dt;
        //int column;
        public Qry()
        {
            InitializeComponent();
        }
        public Qry(DataTable _dt)
        {
            InitializeComponent();
            dt = _dt;
            //column = 0;
        }
        public event EventHandler accept;
        private void Button1_Click(object sender, EventArgs e)
        {
            string str;
            if (radioButton1.Checked == true)
            {
                str = "  WHERE "+comboBox1.Text+"='" + TextBox1.Text + "'";
                accept(str, EventArgs.Empty);
            }
            else if(radioButton2.Checked == true)
            {
                str = " WHERE "+ comboBox1.Text + ">= '" + TextBox4.Text + "' AND "+ comboBox1.Text + "<= '" + TextBox5.Text + "'";
                accept(str, EventArgs.Empty);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked==true)
            {
                radioButton2.Checked = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                radioButton1.Checked = false;
            }
        }
        private void Button5_Click(object sender, EventArgs e)
        {
            TextBox1.Text= "";
            TextBox4.Text = "";
            TextBox5.Text = "";

            accept("", EventArgs.Empty);
        }

        private void Qry_Load(object sender, EventArgs e)
        {
            for(int i=0;i<dt.Columns.Count;i++)
            {
                comboBox1.Items.Add(dt.Columns[i].ColumnName);
            }
            comboBox1.Text = dt.Columns[0].ColumnName;
            groupBox1.Text = comboBox1.Text;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox1.Text = comboBox1.Text;
        }
    }
    
    
}
