using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
//新增用户界面的逻辑设计
namespace 庞西垌数据库
{
    public partial class 新增用户 : Form
    {
        public event EventHandler fh;
        public SqlConnection cnnuser;
        public DataTable dt;
        public 新增用户(SqlConnection _cnnuser,DataTable _dt)
        {
            InitializeComponent();
            cnnuser = _cnnuser;
            dt = _dt;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                if (MessageBox.Show("确认不授予查询权限？","确认？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    checkBox1.Checked = false;
                }
                else
                {
                    checkBox1.Checked = true;
                }
            }
        }
        int bti(bool a)
        {
            if (a == true)
                return 1;
            else return 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("两次输入密码不一致");
            }
            else
            {
                
                try
                {
                    cnnuser.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO 用户权限(ID,psw,query,del,[Save],[set]) VALUES('" +
                                                     textBox1.Text + "','" +
                                                     textBox2.Text + "','" +
                                                     bti(checkBox1.Checked) + "','" +
                                                     bti(checkBox2.Checked) + "','" +
                                                     bti(checkBox3.Checked) + "','" +
                                                     bti(checkBox4.Checked) + "')"
                                                     , cnnuser);
                    cmd.ExecuteNonQuery();
                    cnnuser.Close();
                    fh("", EventArgs.Empty);
                }
                catch 
                {
                    MessageBox.Show("请不要与已有的用户名重复！");
                    cnnuser.Close();
                }

                
            }
        }

        private void 新增用户_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
