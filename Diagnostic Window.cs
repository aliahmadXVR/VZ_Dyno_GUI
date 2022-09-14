using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;


namespace DynoGUI_V1._0
{
    public partial class Form2 : Form 
    {
        private readonly SerialPort Port;
        public Form2(SerialPort port)
        // Form2()
        {
            InitializeComponent();
            this.Port = port;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            Port.Write(Serial_Data_Variables.Sensor_data_show_msg);
            Serial_Data_Variables.diagnostic_mode = true; //diagnostic mode
        }

              
        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = Serial_Data_Variables.dataSensor1;
            textBox2.Text = Serial_Data_Variables.dataSensor2;
            textBox3.Text = Serial_Data_Variables.dataSensor3;
            textBox4.Text = Serial_Data_Variables.dataSensor4;
            textBox5.Text = Serial_Data_Variables.dataSensor5;
            textBox6.Text = Serial_Data_Variables.dataSensor6;
            textBox7.Text = Serial_Data_Variables.dataSensor7;
            textBox8.Text = Serial_Data_Variables.dataSensor8;
            textBox9.Text = Serial_Data_Variables.dataSensor9;
            textBox10.Text = Serial_Data_Variables.dataSensor10;
        }

        private void Brake_button_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_Brake_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Brake msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

        }

        private void Lock_pins_button_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_LockPin_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("LockPins msg sent to Arduino-> \n");
            }

            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Unlock_pins_button_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_UnLockPin_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Un-LockPins msg sent to Arduino-> \n");
            }

            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Brake_disable_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_Disable_Brake_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Disable Brake msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Fan_on_but_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_Fan_ON_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("FAN ON msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Fan_off_but_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_Fan_oFF_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("FAN OFF msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Wheel_BaseR_Adj_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = true; //Only enabling the wheelbase adjust mode
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Wheelbase Rear adjust msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Adjust_wheelbase_button_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Wheel_BaseF_Adj_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = true; //Only enabling the wheelbase adjust mode
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Wheelbase Front adjust msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Wheel_BaseR_Adj_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = true; //Only enabling the wheelbase adjust mode
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Wheelbase Rear adjust msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Lock_pins_button_Click_1(object sender, EventArgs e)
        {

        }

        private void Unlock_pins_button_Click_1(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_UnLockPin_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Un-LockPins msg sent to Arduino-> \n");
            }

            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Brake_button_Click_1(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_Brake_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Brake msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Brake_disable_Click_1(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_Disable_Brake_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("Disable Brake msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Stop Showing sensors data
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_stop_msg);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /*private void Lock_pins_button_Click(object sender, EventArgs e)
        {
            try
            {
                Port.Write(Serial_Data_Variables.Dyno_LockPin_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                //richTextBox1.AppendText("LockPins msg sent to Arduino-> \n");
            }

            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }*/
    }
}
