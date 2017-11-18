using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
//using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
//using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Resources;
//using System.Reflection;
using 庞西垌数据库;
//using System.Threading;
namespace 庞西垌数据库
{
    public partial class login : Form
    {
        public  string sqlstr1;
        SqlConnection cnnuser1;
        public login()
        {
            InitializeComponent();
        }
        power pr = new power();

        public Connection_set readfile()                                                                                        //读取配置参数
        {
            FileStream fs = new FileStream("eky.dll", FileMode.Open, FileAccess.Read); //使用第6个构造函数  
            BinaryFormatter bf = new BinaryFormatter();  //创建一个序列化和反序列化类的对象  
            Connection_set c_s = (Connection_set)bf.Deserialize(fs);  //调用反序列化方法，从文件userInfo.exe中读取对象信息  
            fs.Close();   //关闭文件流  
            return c_s;
        }

        private void Mainitf_Load(object sender, EventArgs e)
        {
            
            //textBox2.PasswordChar='*';
        }
        bool int_to_b(int i)
        {
            if (i == 1)
                return true;
            else
                return false;
        }
        private void Showwatting()
        {
           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Thread th1 = new Thread(new ThreadStart(Showwatting));
            try
            {
               //读取配置参数，若不存在则提示并载入参数设置窗口来设置连接参数
               // th1.Start();
                Connection_set c_set = readfile();
                sqlstr1 = "Data Source =" + c_set.get_inital() + "; User ID = " + c_set.get_user() + "; Password = " + c_set.get_psd() + "; Initial Catalog = 用户数据;";
                cnnuser1 = new SqlConnection();//用户数据
                cnnuser1.ConnectionString = sqlstr1;
                cnnuser1.Open();
               // th1.Abort();

            }
            catch
            {
                //wt1.Close();
               // th1.Abort();
                MessageBox.Show("连接参数错误或者不存在,请设置参数");
                //载入参数设置窗口
                 _setting set = new _setting(0);
                 set.Show();
                
            }
            //
            try
            {
                //th1.Abort();
                //连接数据库获取用户的表，以此来鉴定当前输入的用户信息是否合法并获取当前用户的权限信息
                SqlCommand cmd = new SqlCommand("select * from 用户权限", cnnuser1);
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                DataSet dst = new DataSet();
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dst, "用户权限");
                DataTable dt = new DataTable();
                dt = dst.Tables[0];
                cnnuser1.Close();
                int k = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (textBox1.Text == dt.Rows[i][0].ToString() && textBox2.Text == dt.Rows[i][1].ToString())
                    {
                        pr.serch = int_to_b((int)dt.Rows[i][2]);
                        pr.delete = int_to_b((int)dt.Rows[i][3]);
                        pr.save = int_to_b((int)dt.Rows[i][4]);
                        pr.set = int_to_b((int)dt.Rows[i][5]);
                        pr.id = textBox1.Text;
                        Mainitf f1 = new Mainitf(pr);
                        f1.login_close += new EventHandler(L_close);
                        f1.Show();
                        this.Hide();
                        k = 1;
                        break;
                    }
                }
               // wt1.Close();
                if (k == 0)
                    MessageBox.Show("用户名或密码错误，请检查", "提示");
            }
            catch 
            {
                //th1.Abort();
               
            }
            //wt1.Close();
           // th1.Abort();

        }
        void L_close(object obj,EventArgs e)
        {
            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //_setting a = new _setting();
            // a.show();
            // _setting set = new _setting(1);
            // set.Show();
            this.Close();

        }

        private void login_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                button1_Click(" ", EventArgs.Empty);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
