using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace OSEmulator3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        void Start()
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //这里是程序的真正入口
            OSDriver osd = new OSDriver();
            osd.Start(this);
        }
        //点击开始按钮，启动机器开始执行程序
        private void SwitchBtn_Click(object sender, EventArgs e)
        {
            //下面是启动代码，为避免在计算过程中界面卡着不能动，另启动一个线程来计算
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            ThreadStart ts = new ThreadStart(Start);
            Thread t = new Thread(ts);
            t.Start();
        }

        //用于右侧的表格显示行号
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, this.dataGridView1.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dataGridView1.RowHeadersDefaultCellStyle.Font, rectangle, this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
    }
}
