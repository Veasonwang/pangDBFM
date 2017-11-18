using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
/*连接参数设置部分
 * 通过一种加密算法向文件中写入相关的连接数据，然后再在登陆窗口进行解密来读取该文件获得相应数据进行连接。 
 */
namespace 庞西垌数据库
{
    public partial class _setting : Form
    {
        int m = 0;
        public _setting()
        {
            InitializeComponent();
        }
        public _setting(int i)
        {
            InitializeComponent();
            m = i;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            /*
            textBox1.Text = "(local)";
            textBox2.Text = "sa";
            textBox3.Text = "123456";
            textBox4.Text = "123456";
            textBox5.Text = "水系数据";
            textBox6.Text = "土壤数据";
            textBox7.Text = "磁测数据";
            */
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" || textBox2.Text != "" || textBox3.Text != "" || textBox4.Text != "" || textBox5.Text != "" || textBox6.Text != "" || textBox7.Text != "" )
            {
                if (textBox3.Text == textBox4.Text)
                {
                    Connection_set c_set = new Connection_set(textBox1.Text, textBox2.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text);    //创建该对象
                    c_set.savefile();                                                                                                                       //保存文件
                    MessageBox.Show("设置已保存");
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("两次输入密码不一致，请重新输入密码");
                }
            }
            else
            {
                MessageBox.Show("请输入完整参数");
            }
        }

        private void _setting_Load(object sender, EventArgs e)
        {
            if(m!=0)
            {
                button2.Enabled = true;
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
            }
        }

        private void _setting_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                button1_Click(" ", EventArgs.Empty);
            }
        }
    }
}
