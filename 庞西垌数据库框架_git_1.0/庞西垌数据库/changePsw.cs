using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace 庞西垌数据库
{
    public partial class changePsw : Form
    {
        SqlConnection cnnuser;
        power pr;
        public changePsw(SqlConnection _cnnuser,power _pr)
        {
            InitializeComponent();
            cnnuser = _cnnuser;
            pr = _pr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != textBox2.Text)
            {
                MessageBox.Show("两次输入密码不一样，请重新输入");
            }
            else
            {
                try
                {
                    cnnuser.Open();
                    var cmd = new SqlCommand("UPDATE 用户权限 SET psw = '" + textBox1.Text + "' where ID = '" + pr.id + "';",cnnuser);
                    cmd.ExecuteNonQuery();
                    cnnuser.Close();
                    this.Close();

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                
            }
        
        }

        private void changePsw_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
