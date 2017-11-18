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
    public partial class progressbar : Form
    {
        public event EventHandler et;
        public progressbar(int max)
        {
            InitializeComponent();
            progressBar1.Maximum = max;
            progressBar1.Minimum = 1;
            progressBar1.Value = 1;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void progressbar_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(MessageBox.Show("关闭将会中止导出，确认关闭？","确认?",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                et("", EventArgs.Empty);
            }
        }

        private void progressbar_Load(object sender, EventArgs e)
        {

        }

        private void progressbar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (MessageBox.Show("关闭将会中止导出，确认关闭？", "确认?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                
                    et("", EventArgs.Empty);
                }
            }
        }
    }
}
