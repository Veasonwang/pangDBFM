using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Resources;
using System.Reflection;
//用户权限设置相应模块
namespace 庞西垌数据库
{
    public partial class powerset : Form
    {
        DataSet ds;
        DataTable dt;
        power pr;
        DataRow[] dtr;
        SqlConnection cnnuser;
        public powerset(power _pr,DataSet _ds ,SqlConnection _cnnuser)
        {
            InitializeComponent();
            pr = _pr;
            dt = _ds.Tables[0];
            dtr = new DataRow[dt.Rows.Count];
            cnnuser = _cnnuser;
        }
        private void powerset_Load(object sender, EventArgs e)
        {
            ds = new DataSet();
            cnnuser.Open();
            var da = new SqlDataAdapter();
            var cmd = new SqlCommand("select * from 用户权限", cnnuser);
            da.SelectCommand = cmd;
            da.Fill(ds, "数据");
            dt = ds.Tables[0];
            cnnuser.Close();
            for(int i=0;i<dt.Rows.Count;i++)
            {
                dtr[i] = dt.Rows[i];
                listBox1.Items.Add(dtr[i][0].ToString());
            }
            label2.Text = pr.id;
            button2.Enabled = pr.set;
            button3.Enabled = pr.set;
            button1.Enabled = pr.set;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                if (MessageBox.Show("确认删除用户" + listBox1.SelectedItem.ToString(), "确认？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        cnnuser.Open();
                        var cmd = new SqlCommand("DELETE FROM 用户权限 WHERE ID = '" + listBox1.SelectedItem.ToString() + "'", cnnuser);
                        cmd.ExecuteNonQuery();
                        cnnuser.Close();
                        listBox1.Items.Clear();
                        ds = new DataSet();
                        cnnuser.Open();
                        var da = new SqlDataAdapter();
                        cmd = new SqlCommand("select * from 用户权限", cnnuser);
                        da.SelectCommand = cmd;
                        da.Fill(ds, "数据");
                        dt = ds.Tables[0];
                        cnnuser.Close();
                        dtr = new DataRow[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dtr[i] = dt.Rows[i];
                            listBox1.Items.Add(dtr[i][0].ToString());
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }


                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            新增用户 xz = new 新增用户(cnnuser,ds.Tables[0]);
            xz.fh += new EventHandler(fh_);
            xz.Show();
        }
        private void fh_(object obj,EventArgs e)
        {
            listBox1.Items.Clear();
            ds = new DataSet();
            cnnuser.Open();
            var da = new SqlDataAdapter();
            var cmd = new SqlCommand("select * from 用户权限", cnnuser);
            da.SelectCommand = cmd;
            da.Fill(ds, "数据");
            dt = ds.Tables[0];
            cnnuser.Close();
            dtr = new DataRow[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtr[i] = dt.Rows[i];
                listBox1.Items.Add(dtr[i][0].ToString());
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        bool int_to_b(int i)
        {
            if (i == 1)
                return true;
            else
                return false;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i=0;i<listBox1.Items.Count;i++)
            {
                if (listBox1.SelectedItem != null && listBox1.SelectedItem.ToString() == dtr[i][0].ToString())
                {
                    checkBox1.Checked = int_to_b((int)dtr[i][2]);
                    checkBox2.Checked = int_to_b((int)dtr[i][3]);
                    checkBox3.Checked = int_to_b((int)dtr[i][4]);
                    checkBox4.Checked = int_to_b((int)dtr[i][5]);
                    if (listBox1.SelectedItem.ToString() == pr.id || pr.set == false)

                    {
                        button3.Enabled = false;
                        button2.Enabled = false;
                    }
                    else
                    {
                        button2.Enabled = true;
                        button3.Enabled = true;
                    }
                
                  }
            }
        }
        int bti(bool a)
        {
            if (a == true)
                return 1;
            else return 0;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                if (MessageBox.Show("确认改变权限？", "确认？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        cnnuser.Open();
                        var cmd = new SqlCommand("UPDATE 用户权限 SET query= " + bti(checkBox1.Checked).ToString()+ "WHERE ID= '" +
                                                                               listBox1.SelectedItem.ToString() + "';", cnnuser);
                        cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("UPDATE 用户权限 SET del= " + bti(checkBox2.Checked).ToString() + "WHERE ID= '" +
                                                                               listBox1.SelectedItem.ToString() + "';", cnnuser);
                        cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("UPDATE 用户权限 SET [Save]= " + bti(checkBox3.Checked).ToString() + "WHERE ID= '" +
                                                                               listBox1.SelectedItem.ToString() + "';", cnnuser);
                        cmd.ExecuteNonQuery();
                        cmd = new SqlCommand("UPDATE 用户权限 SET [set]= " + bti(checkBox4.Checked).ToString() + "WHERE ID= '" +
                                                                               listBox1.SelectedItem.ToString() + "';", cnnuser);
                        cmd.ExecuteNonQuery();
                        cnnuser.Close();
                        listBox1.Items.Clear();
                        fh_("", EventArgs.Empty);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        cnnuser.Close();
                    }
                   

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            changePsw cpw = new changePsw(cnnuser, pr);
            cpw.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
