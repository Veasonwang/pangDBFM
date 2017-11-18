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
    public partial class 二次鉴权 : Form
    {
        public power pr=new power();
        SqlConnection cnnuser = new SqlConnection();

        public 二次鉴权(power _pr)
        {
            InitializeComponent();
            pr = _pr;
        }

        private void 二次鉴权_Load(object sender, EventArgs e)
        {
            textBox1.Text = pr.id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connection_set c_set = new Connection_set();
            c_set=c_set.readfile();
            string sqlstr1 = "Data Source =" + c_set.get_inital() + "; User ID = " + c_set.get_user() + "; Password = " + c_set.get_psd() + "; Initial Catalog = 用户数据";
            cnnuser.ConnectionString = sqlstr1;
            cnnuser.Open();
            SqlCommand cmd = new SqlCommand("select * from 用户权限",cnnuser);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds, "用户权限");
            cnnuser.Close();
            int k = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
              if (textBox1.Text == ds.Tables[0].Rows[i][0].ToString() && textBox2.Text == ds.Tables[0].Rows[i][1].ToString())
               {
                    k = 1;
                    powerset pws = new powerset(pr, ds,cnnuser);
                    pws.Show();
                    Close();
                }
             }
            
            if(k==0)
            MessageBox.Show("用户名或密码错误！");

        }

        private void 二次鉴权_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                button1_Click(1, EventArgs.Empty);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
