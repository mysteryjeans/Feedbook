using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScrambleUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != null)
            {
                var scrambleBytes = Scramble(Encoding.UTF8.GetBytes(this.textBox1.Text));
                this.textBox2.Text = "{ " + string.Join(", ", scrambleBytes.Select(b => string.Format("0x{0:X}",b))) + " }";
            }
        }

        public static byte[] Scramble(byte[] scrambleBytes)
        {
            if (scrambleBytes == null)
                throw new ArgumentNullException("scrambleBytes");

            byte[] bytes = new byte[scrambleBytes.Length];

            for (int i = 0; i < scrambleBytes.Length; i += 2)
            {
                if (i + 1 < scrambleBytes.Length)
                {
                    bytes[i] = scrambleBytes[i + 1];
                    bytes[i + 1] = scrambleBytes[i];
                }
                else
                    bytes[i] = scrambleBytes[i];
            }

            return bytes;
        }
    }
}
