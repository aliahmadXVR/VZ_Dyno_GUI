using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace DynoGUI_V1._0
{
    public partial class Form1 : Form
    {
        private readonly SerialPort Port;
       
        public Form1(SerialPort port)
        {
            InitializeComponent();
            this.Port = port;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button_start.Enabled = true;
            comboBox2_baud.Text = "115200";
            string[] portLists = SerialPort.GetPortNames();
            Serial_Data_Variables.serialConnected = false;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(portLists);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Hide this form
            this.Hide(); 
        }


        private void button_start_Click(object sender, EventArgs e)
        {
            try
            {
                Port.PortName = comboBox1.Text;
                Port.BaudRate = Convert.ToInt32(comboBox2_baud.Text);
                Port.Open();
                button_start.Enabled = false;
                Serial_Data_Variables.serialConnected = true;
                progressBar1.Value = 100;

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        
        //Closing the form and going back to Main GUI//
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Hide this form
            this.Hide();
        }

    }
}
