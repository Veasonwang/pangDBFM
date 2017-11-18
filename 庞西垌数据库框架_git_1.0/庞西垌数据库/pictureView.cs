using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.IO;
//图片查看器模块，因性能过低而不用。
//图片查看方式调用当前系统默认的图片查看器来查看
namespace 庞西垌数据库
{
    public partial class pictureView : Form
    {
        string str;
        public Point mouseDownPoint;
        public bool isSelected;
        public static Image img ;
        public pictureView()
        {
            InitializeComponent();
        }
        public pictureView(string _str,string path)
        {
            InitializeComponent();
            str = path+_str;
            isSelected = false;
        }

        private void pictureView_Load(object sender, EventArgs e)
        {
            img= Image.FromFile(@str);
            pictureBox1.Image = img;
            this.MouseWheel += pictureView_MouseWheel;

            //  resource.图1_文地实际材料图;
        }
        void pictureView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta >= 0)
            {
                pictureBox1.Width = (int)(pictureBox1.Width * 1.1);//因为Widthh和Height都是int类型，所以要强制转换一下-_-||  
                pictureBox1.Height = (int)(pictureBox1.Height * 1.1);
            }
            else
            {
                pictureBox1.Width = (int)(pictureBox1.Width * 0.9);
                pictureBox1.Height = (int)(pictureBox1.Height * 0.9);
            }

        }
       

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;  //注：全局变量mouseDownPoint前面已定义为Point类型  

                mouseDownPoint.Y = Cursor.Position.Y;
                isSelected = true;
            }
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected )
            {
                this.pictureBox1.Left = this.pictureBox1.Left + (Cursor.Position.X - mouseDownPoint.X);
                this.pictureBox1.Top = this.pictureBox1.Top + (Cursor.Position.Y - mouseDownPoint.Y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isSelected = false;
        }

        private void pictureView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;  //注：全局变量mouseDownPoint前面已定义为Point类型  

                mouseDownPoint.Y = Cursor.Position.Y;
                isSelected = true;
            }
        }

        private void pictureView_MouseUp(object sender, MouseEventArgs e)
        {
            isSelected = false;
        }

        private void pictureView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected)
            {
                this.pictureBox1.Left = this.pictureBox1.Left + (Cursor.Position.X - mouseDownPoint.X);
                this.pictureBox1.Top = this.pictureBox1.Top + (Cursor.Position.Y - mouseDownPoint.Y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }
    }
}
