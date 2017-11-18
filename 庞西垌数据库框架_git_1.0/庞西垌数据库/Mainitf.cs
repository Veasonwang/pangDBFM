using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Resources;
using System.Reflection;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
/*
 * 主窗口功能实现，包括动态创建tabpage、调整其属性，动态切换datagridview控件，实现关键字查询功能，查询的结果导出等功能
 * 核心思路：对每一次连接，分别为SqlDataAdapter 对象、DataSet、创建一个3*n的二维数组，为切换标签页建立事件，根据其标签的Index标号，选择对应的表格，并将其加载进入相应的datagridview即可。导出同理。
 * 
*/
namespace 庞西垌数据库
{
    public partial class Mainitf : Form
    {
        public string sqlstr1;                                                                 //三个数据库的连接字符串
        public string sqlstr2;
        public string sqlstr3;
                                                                            
        SqlConnection cnnuser1;
        SqlConnection cnnuser2;
        SqlConnection cnnuser3;

        DataTable[,] dt_ = new DataTable[3, 20];
        BindingSource[,] bs = new BindingSource[3, 20];
        SqlDataAdapter[,] dataAdapter = new SqlDataAdapter[3, 20];

        string str = "";
        DataSet[] dst = new DataSet[3];

        Point pst;                                                                            //当前选项卡数据
        power pwr = new power();                                                              //权限控制模块
        public event EventHandler login_close;
        string path;
        public static bool contine;
        public static progressbar pb;


        public Mainitf(power _pwr)                                                            //构造函数，传入参数为权限控制变量
        {
            InitializeComponent();

            Connection_set c_set = new Connection_set();
            c_set=c_set.readfile();
                                                                                              //读入配置数据文件
            sqlstr1 = "Data Source =" + c_set.get_inital() + "; User ID = " + c_set.get_user() + "; Password = " + c_set.get_psd() + "; Initial Catalog = " + c_set.get_s_d();
            sqlstr2 = "Data Source =" + c_set.get_inital() + "; User ID = " + c_set.get_user() + "; Password = " + c_set.get_psd() + "; Initial Catalog = " + c_set.get_t_d();
            sqlstr3 = "Data Source =" + c_set.get_inital() + "; User ID = " + c_set.get_user() + "; Password = " + c_set.get_psd() + "; Initial Catalog = " + c_set.get_w_d();
            cnnuser1 = new SqlConnection();         //水系数据
            cnnuser2 = new SqlConnection();         //土壤数据
            cnnuser3 = new SqlConnection();         //物探数据
            pst.Y = 0;
            path = "";
            pwr = _pwr;
            contine = new bool();
        }
        private int connectsql()
        {
            cnnuser1.ConnectionString = sqlstr1;
            cnnuser2.ConnectionString = sqlstr2;
            cnnuser3.ConnectionString = sqlstr3;
            cnnuser1.Open();
            cnnuser2.Open();
            cnnuser3.Open();
                                                                                                //连接数据库
            int error = 0;
            if (cnnuser1.State != ConnectionState.Open)
            {
                error++;
            }
            if (cnnuser2.State != ConnectionState.Open)
            {
                error++;
            }
            if (cnnuser3.State != ConnectionState.Open)
            {
                error++;
            }
            return error;
        }

        void f2_accept(object sender, EventArgs e)                                               //创建事件供查询窗口调用
        {
            str = sender.ToString();
            if (pst.X == 0)
                Loadtable(cnnuser1);
            if (pst.X == 1)
                Loadtable(cnnuser2);
            if (pst.X == 2)
                Loadtable(cnnuser3);
        }
        /*
        public Connection_set readfile()
        {
            FileStream fs = new FileStream("eky.dll", FileMode.Open, FileAccess.Read); //使用第6个构造函数  
            BinaryFormatter bf = new BinaryFormatter();  //创建一个序列化和反序列化类的对象  
            Connection_set c_s = (Connection_set)bf.Deserialize(fs);  //调用反序列化方法，从文件userInfo.exe中读取对象信息  
            fs.Close();   //关闭文件流  
            return c_s;
        }
        */
        private void Mainitf_Load(object sender, EventArgs e)
        {
            int i = 0;
            //pwr.delete = false;
            //pwr.sd = false;
            物探数据ToolStripMenuItem.Enabled = pwr.wd;
            土壤数据ToolStripMenuItem.Enabled = pwr.td;
            水系数据ToolStripMenuItem.Enabled = pwr.sd;
            连接设置ToolStripMenuItem.Enabled = pwr.set;
            bindingNavigatorDeleteItem.Enabled = pwr.delete;
            toolStripButton1.Enabled = pwr.serch;
            toolStripButton2.Enabled = pwr.delete;
            
            //遍历资源文件
            // DirectoryInfo TheFolder = new DirectoryInfo("Resources");
            //foreach (FileInfo NextFile in TheFolder.GetFiles())
            //     this.toolStripComboBox1.Items.Add(NextFile.Name);

            try
            {
                i = connectsql();
                水系数据ToolStripMenuItem_Click("", EventArgs.Empty);
            }
            catch
            {
                MessageBox.Show("数据库连接失败或图片资源框架不存在,请检查Resources文件夹是否在程序目录下或参阅帮助文档与管理员联系"); 
            }
        }


        //获取数据库所有的表
        private SqlDataReader GetTables_SystemTable(SqlConnection sqlcn)
        {
            //使用信息架构视图 
            SqlCommand sqlcmd = new SqlCommand("SELECT OBJECT_NAME (id) FROM sysobjects WHERE xtype = 'U' AND OBJECTPROPERTY (id, 'IsMSShipped') = 0", sqlcn);
            SqlDataReader dr = sqlcmd.ExecuteReader();
            return dr;
        }


        private void Loadtable(SqlConnection sqlcn)
        {
            SqlDataReader dr = GetTables_SystemTable(sqlcn);
            DataTable dt1 = new DataTable();
            DataSet ds = new DataSet();
            dt1.Columns.Add("name");
            while (dr.Read())
            {
                dt1.Rows.Add(dr.GetString(0));
            }
            dr.Close();
            try {
                SqlCommand cmd = new SqlCommand("select * from " + dt1.Rows[pst.Y][0].ToString() + str, sqlcn);                 //创建相应的SQL语句来载入对应的数据表
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(ds, dt1.Rows[pst.Y][0].ToString());
                bs[pst.X, pst.Y].DataSource = ds.Tables[0];
                dt_[pst.X, pst.Y] = ds.Tables[0];
            }
            catch(Exception ex)                                                                                                 //异常处理
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 水系数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                toolStripComboBox1.Items.Clear();                                                                                //载入相应的图片资源
                toolStripComboBox1.Text = "图片";
                path = @"Resources\water\";
                DirectoryInfo TheFolder = new DirectoryInfo(@"Resources\water");
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                    this.toolStripComboBox1.Items.Add(NextFile.Name);

                水系数据ToolStripMenuItem.Checked = true;                                                                        //前面的钩控制部分
                物探数据ToolStripMenuItem.Checked = false;
                土壤数据ToolStripMenuItem.Checked = false;

                pst.X = 0;                                                                                                       //二维数组定位
                int tabCount = this.tabControl1.TabCount;
                for (int i = tabCount - 1; i >= 0; i--)
                {
                    this.tabControl1.TabPages.RemoveAt(i);
                }

                SqlDataReader dr = GetTables_SystemTable(cnnuser1);                                                              //获取相应数据库的表的数目
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("name");
                while (dr.Read())
                {
                    dt1.Rows.Add(dr.GetString(0));
                }
                dr.Close();

                dst[0] = new DataSet();
                for (int i = 0; i < dt1.Rows.Count; i++)                                                                         //动态创建TabPage并将控件设置好并加入进去
                {

                    TabPage tp = new TabPage();
                    tp.Name = "TabPage0" + (i + 1).ToString();
                    tp.Text = dt1.Rows[i][0].ToString();
                    //SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    DataGridView dgv1 = new DataGridView();
                    dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dgv1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dgv1.Dock = DockStyle.Fill;
                    dgv1.Name = "dgv1" + (i + 1).ToString();
                    dgv1.Parent = tp;
                    // dgv1.ReadOnly = true;

                    SqlCommand cmd = new SqlCommand("select * from " + dt1.Rows[i][0].ToString(), cnnuser1);
                    dataAdapter[0, i] = new SqlDataAdapter();
                    dataAdapter[0, i].SelectCommand = cmd;
                    dataAdapter[0, i].Fill(dst[0], dt1.Rows[i][0].ToString());
                    //导出变量                                                                                                   //导出的表的拷贝，作为导出表的传递参数
                    dt_[0, i] = dst[0].Tables[i];   

                    dgv1.AutoGenerateColumns = true;
                    bs[0, i] = new BindingSource();
                    bs[0, i].DataSource = dst[0].Tables[i];
                    bindingNavigator1.BindingSource = bs[0, i];
                    dgv1.DataSource = bs[0, i];
                    tp.Controls.Add(dgv1);
                    tabControl1.Controls.Add(tp);
                }
                bindingNavigator1.BindingSource = bs[0, 0];                                                                      //与数据库导航栏关联
            }
            catch
            {
                MessageBox.Show("连接失败或图片资源框架不存在，请与管理员联系");
            }
        }
        private void 土壤数据ToolStripMenuItem_Click(object sender, EventArgs e)                                                 //土壤数据选定事件，注释见水系数据
        {
            try
            {
                //toolStripComboBox1.Text = "";
                toolStripComboBox1.Items.Clear();
                toolStripComboBox1.Text = "图片";
                path = @"Resources\soil\";
                DirectoryInfo TheFolder = new DirectoryInfo(@"Resources\soil");
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                    this.toolStripComboBox1.Items.Add(NextFile.Name);

                水系数据ToolStripMenuItem.Checked = false;
                土壤数据ToolStripMenuItem.Checked = true;
                物探数据ToolStripMenuItem.Checked = false;
                pst.X = 1;
                int tabCount = this.tabControl1.TabCount;
                for (int i = tabCount - 1; i >= 0; i--)
                {
                    this.tabControl1.TabPages.RemoveAt(i);
                }
                SqlDataReader dr = GetTables_SystemTable(cnnuser2);
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("name");
                while (dr.Read())
                {
                    dt1.Rows.Add(dr.GetString(0));
                }
                dr.Close();
                dst[1] = new DataSet();
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    dataAdapter[1, i] = new SqlDataAdapter();
                    TabPage tp = new TabPage();
                    tp.Name = "TabPage1" + (i + 1).ToString();
                    tp.Text = dt1.Rows[i][0].ToString();
                    //  SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    DataGridView dgv1 = new DataGridView();
                    dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dgv1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dgv1.Dock = DockStyle.Fill;
                    dgv1.Name = "dgv2" + (i + 1).ToString();
                    //dgv1.ReadOnly = true;

                    SqlCommand cmd = new SqlCommand("select * from " + dt1.Rows[i][0].ToString(), cnnuser2);
                    dataAdapter[1, i].SelectCommand = cmd;
                    dataAdapter[1, i].Fill(dst[1], dt1.Rows[i][0].ToString());
                    dt_[1, i] = dst[1].Tables[i];
                    dgv1.AutoGenerateColumns = true;
                    bs[1, i] = new BindingSource();
                    bs[1, i].DataSource = dst[1].Tables[i];
                    bindingNavigator1.BindingSource = bs[1, i];
                    dgv1.DataSource = bs[1, i];
                    tp.Controls.Add(dgv1);
                    tabControl1.Controls.Add(tp);
                }
                bindingNavigator1.BindingSource = bs[1, 0];
            }
            catch
            {
                MessageBox.Show("连接失败或图片资源框架不存在，请与管理员联系");
            }
        }
        private void 物探数据ToolStripMenuItem_Click(object sender, EventArgs e)                                                 //物探数据选定事件，注释见水系数据
        {
            try
            {
                //toolStripComboBox1.Text = "";
                toolStripComboBox1.Items.Clear();
                toolStripComboBox1.Text = "图片";
                path = @"Resources\magnet\";
                DirectoryInfo TheFolder = new DirectoryInfo(@"Resources\magnet");
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                    this.toolStripComboBox1.Items.Add(NextFile.Name);


                水系数据ToolStripMenuItem.Checked = false;
                土壤数据ToolStripMenuItem.Checked = false;
                物探数据ToolStripMenuItem.Checked = true;

                pst.X = 2;
                int tabCount = this.tabControl1.TabCount;
                for (int i = tabCount - 1; i >= 0; i--)
                {
                    this.tabControl1.TabPages.RemoveAt(i);
                }
                SqlDataReader dr = GetTables_SystemTable(cnnuser3);
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("name");
                while (dr.Read())
                {
                    dt1.Rows.Add(dr.GetString(0));
                }
                dr.Close();
                dst[2] = new DataSet();
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    TabPage tp = new TabPage();
                    tp.Name = "TabPage2" + (i + 1).ToString();
                    tp.Text = dt1.Rows[i][0].ToString();
                    //SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    DataGridView dgv1 = new DataGridView();
                    dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dgv1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dgv1.Dock = DockStyle.Fill;
                    dgv1.Name = "dgv3" + (i + 1).ToString();
                    // dgv1.ReadOnly = true;

                    SqlCommand cmd = new SqlCommand("select * from " + dt1.Rows[i][0].ToString(), cnnuser3);
                    dataAdapter[2, i] = new SqlDataAdapter();
                    dataAdapter[2, i].SelectCommand = cmd;
                    dataAdapter[2, i].Fill(dst[2], dt1.Rows[i][0].ToString());
                    //导出变量
                    dt_[2, i] = dst[2].Tables[i];
                    dgv1.AutoGenerateColumns = true;
                    bs[2, i] = new BindingSource();
                    bs[2, i].DataSource = dst[2].Tables[i];
                    bindingNavigator1.BindingSource = bs[2, i];
                    dgv1.DataSource = bs[2, i];
                    tp.Controls.Add(dgv1);
                    tabControl1.Controls.Add(tp);
                }
                bindingNavigator1.BindingSource = bs[2, 0];
            }
            catch
            {
                MessageBox.Show("连接失败或图片资源框架不存在，请与管理员联系");
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)                                                  //选项卡切换时间
        {   
            // string a = e.TabPage.Name;
            if (e.TabPage != null)
            {
                string str = e.TabPage.Name;
                int i = Convert.ToInt32(str.Substring(str.Length - 2, 1));
                int j = Convert.ToInt32(str.Substring(str.Length - 1, 1)) - 1;
                bindingNavigator1.BindingSource = bs[i, j];
                pst.X = i;
                pst.Y = j;
                str = "";
            }

        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable _dt = new DataTable();
                _dt = (DataTable)bs[pst.X, pst.Y].DataSource;

                Qry f2 = new Qry(_dt);
                f2.accept += new EventHandler(f2_accept);
                f2.Show();
            }
            catch
            {

            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认保存？", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter[pst.X, pst.Y]);
                try
                {
                    foreach(DataGridView c in tabControl1.TabPages[pst.Y].Controls)
                    {
                            c.CurrentCell = null;
                    }
                    
                    dataAdapter[pst.X, pst.Y].Update(dst[pst.X].Tables[pst.Y]);
                    MessageBox.Show("修改已保存");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)                                                               //更改账户属性的二次鉴权窗口
        {
            二次鉴权 dcj = new 二次鉴权(pwr);
            dcj.Show();

        }

        private void 连接设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _setting set = new _setting();
            set.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
           // Thread thread1 = new Thread(DataGridViewToExcel(dt_[pst.X, pst.Y]));
           // DataGridViewToExcel(dt_[pst.X,pst.Y]);
          // dttocsv(dt_[pst.X, pst.Y]);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.Show();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            //pictureView pcv1 = new pictureView();
            //  pcv1.Show();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             //pictureView pcv = new pictureView(toolStripComboBox1.SelectedItem.ToString(), path);
             //pcv.Show();
            
            try {
                string str = toolStripComboBox1.SelectedItem.ToString();
                System.Diagnostics.Process.Start(path+str);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
         //   System.Diagnostics.Process.Start(@"Resources\water\图3 塘蓬实际材料图.JPG");
         
        }

        private void Mainitf_ImeModeChanged(object sender, EventArgs e)
        {

        }

        private void Mainitf_FormClosed(object sender, FormClosedEventArgs e)
        {
            login_close("", EventArgs.Empty);
        }
        
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }
        void dttocsv(DataTable dt)                                                                                  //导出到.CSV文件，
        {
            //申明保存对话框   
            SaveFileDialog dlg = new SaveFileDialog();
            //默然文件后缀   
            dlg.DefaultExt = "csv";
            //文件后缀列表   
            dlg.Filter = "CSV文件(*.csv)|*.csv";
            //默然路径是系统当前路径   
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            //打开保存对话框   
            if (dlg.ShowDialog() == DialogResult.Cancel) return;
            //返回文件路径   
            string fileNameString = dlg.FileName;
            //验证strFileName是否为空或值无效   
            if (fileNameString.Trim() == " ")
            { return; }
            //定义表格内数据的行数和列数   
            int rowscount = dt.Rows.Count;
            int colscount = dt.Columns.Count;
            //行数必须大于0   
            if (rowscount <= 0)
            {
                MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //列数必须大于0   
            if (colscount <= 0)
            {
                MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //行数不可以大于65536   
            if (rowscount > 65536)
            {
                MessageBox.Show("数据记录数太多(最多不能超过65536条)，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //列数不可以大于255   
            if (colscount > 255)
            {
                MessageBox.Show("数据记录行数太多，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //验证以fileNameString命名的文件是否存在，如果存在删除它   
            FileInfo file = new FileInfo(fileNameString);
            if (file.Exists)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "删除失败 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            byte[] bye = new byte[255];
            try
            {
                StreamWriter fs= new StreamWriter(fileNameString, true, Encoding.GetEncoding("gb2312"));
                //向CSV中写入表格的表头   
                int displayColumnsCount = 1;
                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    fs.Write(dt.Columns[i].ColumnName.Trim());
                    if(i!= dt.Columns.Count - 1)
                        fs.Write(",");
                    displayColumnsCount++;
                }
                fs.Write("\r\n");
                //设置进度条   
                pb = new progressbar(dt.Rows.Count + 5);
                pb.Show();
                //向CSV中逐行逐列写入表格中的数据   

                
                for (int row = 0; row <= dt.Rows.Count - 1; row++)                                                  //导出数据操作
                {
                    displayColumnsCount = 1;
                    pb.progressBar1.Value++;
                    //  pb.label1.Text = "已经导出";
                    for (int col = 0; col < colscount; col++)
                    {
                        try
                        {
                            //objExcel.Cells[row + 2, displayColumnsCount] = dt.Rows[row][col].ToString().Trim();
                            fs.Write(dt.Rows[row][col].ToString().Trim());
                            if (col != colscount - 1)  fs.Write(",");
                            displayColumnsCount++;                                                                  //进度条指示变量
                            

                        }
                        catch (Exception)
                        {

                        }
                    }

                    fs.Write("\r\n");

                }
                pb.Hide();
                fs.Close();
                MessageBox.Show(fileNameString.ToString() + "导出成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           

        }  
        /* 导出到excel模块，现在不用
        public static void DataGridViewToExcel(DataTable dt)
        {


            #region   验证可操作性  
            //申明保存对话框   
            SaveFileDialog dlg = new SaveFileDialog();
            //默然文件后缀   
            dlg.DefaultExt = "xls";
            //文件后缀列表   
            dlg.Filter = "EXCEL文件(*.xls)|*.xls";
            //默然路径是系统当前路径   
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            //打开保存对话框   
            if (dlg.ShowDialog() == DialogResult.Cancel) return;
            //返回文件路径   
            string fileNameString = dlg.FileName;
            //验证strFileName是否为空或值无效   
            if (fileNameString.Trim() == " ")
            { return; }
            //定义表格内数据的行数和列数   
            int rowscount = dt.Rows.Count;
            int colscount = dt.Columns.Count;
            //行数必须大于0   
            if (rowscount <= 0)
            {
                MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //列数必须大于0   
            if (colscount <= 0)
            {
                MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //行数不可以大于65536   
            if (rowscount > 65536)
            {
                MessageBox.Show("数据记录数太多(最多不能超过65536条)，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //列数不可以大于255   
            if (colscount > 255)
            {
                MessageBox.Show("数据记录行数太多，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //验证以fileNameString命名的文件是否存在，如果存在删除它   
            FileInfo file = new FileInfo(fileNameString);
            
            if (file.Exists)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "删除失败 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            //中止控制变量
            contine = true;

            #endregion
            Excel.Application objExcel = null;
            Excel.Workbook objWorkbook = null;
            Excel.Worksheet objsheet = null;
            try
            {
                //申明对象   
                objExcel = new Microsoft.Office.Interop.Excel.Application();
                objWorkbook = objExcel.Workbooks.Add(Missing.Value);
                objsheet = (Excel.Worksheet)objWorkbook.ActiveSheet;
                //设置EXCEL不可见   
                objExcel.Visible = false;

                //向Excel中写入表格的表头   
                int displayColumnsCount = 1;
                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                   // if (dt.Columns[i].Visible == true)
                   // {
                        objExcel.Cells[1, displayColumnsCount] = dt.Columns[i].ColumnName.Trim();
                        displayColumnsCount++;
                  //  }
                }
                //设置进度条   
                
                pb.Show();
                //向Excel中逐行逐列写入表格中的数据   
                //  var stopWatch = new Stopwatch();
                //
                    for (int row = 0; row <= dt.Rows.Count - 1; row++)
                    {
                        displayColumnsCount = 1;
                        pb.progressBar1.Value++;
                        for (int col = 0; col < colscount; col++)
                        {
                            try
                            {
                                objExcel.Cells[row + 2, displayColumnsCount] = dt.Rows[row][col].ToString().Trim();
                                displayColumnsCount++;
                                pb.label1.Text = "已经导出" + row.ToString() + "行/" + (dt.Rows.Count - 1).ToString() + "行";

                            }
                            catch (Exception)
                            {

                            }
                        }
                    if (contine == false) break;
                    }
                
                //保存文件  
                pb.Hide(); 
                objWorkbook.SaveAs(fileNameString, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
        Missing.Value, Excel.XlSaveAsAccessMode.xlShared, Missing.Value, Missing.Value, Missing.Value,
        Missing.Value, Missing.Value);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "警告 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                //关闭Excel应用   
                if (objWorkbook != null) objWorkbook.Close(Missing.Value, Missing.Value, Missing.Value);
                if (objExcel.Workbooks != null) objExcel.Workbooks.Close();
                if (objExcel != null) objExcel.Quit();

                objsheet = null;
                objWorkbook = null;
                objExcel = null;
            }
            MessageBox.Show(fileNameString + "导出完毕! ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        */
        private void 导出到csvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dttocsv(dt_[pst.X, pst.Y]);
            }
            catch
            {

            }
        }
        /* 不导出到EXCEl，不使用
        private void 到处到excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                pb = new progressbar(dt_[pst.X, pst.Y].Rows.Count + 5);
                pb.et += new EventHandler(edout);
                Thread tr = new Thread(new ThreadStart(delegate () { DataGridViewToExcel(dt_[pst.X, pst.Y]); }));
                tr.SetApartmentState(ApartmentState.STA);
                this.WindowState = FormWindowState.Minimized;
                tr.Start();
            }
            catch 
            {
                
            }
           
            // DataGridViewToExcel(dt_[pst.X,pst.Y]);
            //dttocsv(dt_[pst.X, pst.Y]);
        }
        */
        public static void edout(object obj,EventArgs e)
        {
            contine = false;
        }

        private void Mainitf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
              if (MessageBox.Show("退出？", "确认?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Close();
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }
    }
}

