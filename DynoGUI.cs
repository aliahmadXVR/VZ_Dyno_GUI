using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic; //used for linkedList

    
namespace DynoGUI_V1._0
{

    public partial class DynoGUI : Form
    {
        Thread buttons_thread;
        /* 
         * Variables related to EMA on dT_sec 
         * ----------------------------------
         */
        double tickmultiplier;
        int     SmoothingWin_Size = 23;
        double  emaMultiplier;
        double  previous_EMA;
        int     first_time_Dt = 0; 
        int     first_RPM_check = 0;
        double  Inertia_Constant = 1.562500;
        double  ft_lb_conv_factor = 1.3556;
        double  DynoToEngine_RPM_Mul = 10;
        double  PI = 3.14159265359;
        int     shaft_dia = 32;
        double  speed_mul = 0;
        double  speed = 0;
        int     serial_index = 0;
        int     Acc_index_display = 0;

        double mph_conv_factor = 0.621371192;

        public DynoGUI()
        {
            InitializeComponent();
            Thread buttons_thread = new Thread(new ThreadStart(this.button_update_thread));
            buttons_thread.IsBackground = true;
            buttons_thread.Start();
        }

        /**
         * Returns the Saturated Vapour pressure Pws,
         * input is the temperature in Celcius.
         */
        static public double get_sat_vap_pressure()
        {
            double temp_K = actual_sensor_data._temp_C + 273.15;        //conversion to Kelvin
            double a = 77.3450 + (0.0057 * temp_K) - (7235 / temp_K);   //computing complex expression
            double b = Math.Pow(temp_K,8.2);
            double pws = (Math.Exp(a)) / b;                             //pws is saturation vapour pressure
            pws = pws * 0.01;                                           //Unit conversion to mbar 
            return pws;
        }

        /**
         * This function computes the Pw which 
         * is the Vapour Partial Pressure.
         */
        static public double cal_vap_par_pressure(double pws)  
        {    
            return ( (actual_sensor_data._rel_humidity/100) * pws ); //returns Pw 
        }

        static public double cal_dry_airPressure(double pw)
        {
            return (actual_sensor_data._pressure_baro - pw); // Psensor - Pw 
        }

        /*This is Serial Port Handler Function
         * All data recieved here is passed to ProcessData Function
         * */
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Serial_Data_Variables.serialDataIn = serialPort1.ReadLine();
            this.BeginInvoke(new EventHandler(ProcessData));
        }

        /* This Function Decodes the Data Coming on Serial Port
         * All the Serial Data decodeing, plotting is done here
         **/
        private void ProcessData(object sender, EventArgs e)
        {
            try
            {

                if (Serial_Data_Variables.diagnostic_mode == true)
                {
                    Serial_Data_Variables.indexOfA = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("A"));
                    Serial_Data_Variables.indexOfB = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("B"));
                    Serial_Data_Variables.indexOfC = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("C"));
                    Serial_Data_Variables.indexOfD = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("D"));
                    Serial_Data_Variables.indexOfE = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("E"));
                    Serial_Data_Variables.indexOfF = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("F"));
                    Serial_Data_Variables.indexOfG = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("G"));
                    Serial_Data_Variables.indexOfH = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("H"));
                    Serial_Data_Variables.indexOfI = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("I"));
                    Serial_Data_Variables.indexOfJ = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("J"));

                    Serial_Data_Variables.dataSensor1 = Serial_Data_Variables.serialDataIn.Substring(0, Serial_Data_Variables.indexOfA);
                    Serial_Data_Variables.dataSensor2 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfA + 1, (Serial_Data_Variables.indexOfB - Serial_Data_Variables.indexOfA) - 1);
                    Serial_Data_Variables.dataSensor3 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfB + 1, (Serial_Data_Variables.indexOfC - Serial_Data_Variables.indexOfB) - 1);
                    Serial_Data_Variables.dataSensor4 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfC + 1, (Serial_Data_Variables.indexOfD - Serial_Data_Variables.indexOfC) - 1);
                    Serial_Data_Variables.dataSensor5 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfD + 1, (Serial_Data_Variables.indexOfE - Serial_Data_Variables.indexOfD) - 1);
                    Serial_Data_Variables.dataSensor6 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfE + 1, (Serial_Data_Variables.indexOfF - Serial_Data_Variables.indexOfE) - 1);
                    Serial_Data_Variables.dataSensor7 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfF + 1, (Serial_Data_Variables.indexOfG - Serial_Data_Variables.indexOfF) - 1);
                    Serial_Data_Variables.dataSensor8 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfG + 1, (Serial_Data_Variables.indexOfH - Serial_Data_Variables.indexOfG) - 1);
                    Serial_Data_Variables.dataSensor9 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfH + 1, (Serial_Data_Variables.indexOfI - Serial_Data_Variables.indexOfH) - 1);
                    Serial_Data_Variables.dataSensor10 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfI + 1, (Serial_Data_Variables.indexOfJ - Serial_Data_Variables.indexOfI) - 1);

                }

                //This mode is enabled when the regular dT ticks are sent by the 
                //Arduino and the GUI has to Display Internal_Var.HP & Torque Graphs
                if (Serial_Data_Variables.dT_transmit_IN_mode == true)
                {
                    Serial_Data_Variables.indexOfA = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("A"));
                    Serial_Data_Variables.indexOfB = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("B"));
                    Serial_Data_Variables.indexOfC = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("C"));
                    Serial_Data_Variables.indexOfD = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("D"));
                    Serial_Data_Variables.indexOfE = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("E"));
                    Serial_Data_Variables.indexOfF = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("F"));
                    Serial_Data_Variables.indexOfG = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("G"));
                    Serial_Data_Variables.indexOfH = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("H"));
                    Serial_Data_Variables.indexOfI = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("I"));
                    Serial_Data_Variables.indexOfJ = Convert.ToSByte(Serial_Data_Variables.serialDataIn.IndexOf("J"));

                    Serial_Data_Variables.dataSensor1 = Serial_Data_Variables.serialDataIn.Substring(0, Serial_Data_Variables.indexOfA);
                    Serial_Data_Variables.dataSensor2 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfA + 1, (Serial_Data_Variables.indexOfB - Serial_Data_Variables.indexOfA) - 1);
                    Serial_Data_Variables.dataSensor3 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfB + 1, (Serial_Data_Variables.indexOfC - Serial_Data_Variables.indexOfB) - 1);
                    Serial_Data_Variables.dataSensor4 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfC + 1, (Serial_Data_Variables.indexOfD - Serial_Data_Variables.indexOfC) - 1);
                    Serial_Data_Variables.dataSensor5 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfD + 1, (Serial_Data_Variables.indexOfE - Serial_Data_Variables.indexOfD) - 1);
                    Serial_Data_Variables.dataSensor6 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfE + 1, (Serial_Data_Variables.indexOfF - Serial_Data_Variables.indexOfE) - 1);
                    Serial_Data_Variables.dataSensor7 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfF + 1, (Serial_Data_Variables.indexOfG - Serial_Data_Variables.indexOfF) - 1);
                    Serial_Data_Variables.dataSensor8 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfG + 1, (Serial_Data_Variables.indexOfH - Serial_Data_Variables.indexOfG) - 1);
                    Serial_Data_Variables.dataSensor9 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfH + 1, (Serial_Data_Variables.indexOfI - Serial_Data_Variables.indexOfH) - 1);
                    Serial_Data_Variables.dataSensor10 = Serial_Data_Variables.serialDataIn.Substring(Serial_Data_Variables.indexOfI + 1, (Serial_Data_Variables.indexOfJ - Serial_Data_Variables.indexOfI) - 1);

                    
                    //Calculating the Internal_Var.RPM
                    tickmultiplier = 0.0625 / 1000000; // 62.5nS(One Tick of Microcontroller)

                    Internal_Var.dT_sec = tickmultiplier * (Convert.ToInt64(Serial_Data_Variables.dataSensor1));  //This is the value coming from Arduino
                    Internal_Var.totalTime += Internal_Var.dT_sec; //Keep a record of totalTime

                    //Push then current total time till now in the Timevector.
                    Internal_Var.totalTimeVectot[serial_index] = Internal_Var.totalTime; 
                    
                    //Shift this calucation and sizing to general place afterwards
                    //0.1 currently works best. N =20 here.
                    emaMultiplier = 0.05;  //0.5//2 / (SmoothingWin_Size + 1); SEE THIS AFTERWARDS WHY NOT WORKING

                    if (first_time_Dt == 0)
                    {
                        previous_EMA = 0.01;
                        first_time_Dt = 1;
                    }

                    else
                    {
                        Internal_Var.TimeDelta_EMA = (Internal_Var.dT_sec * emaMultiplier) + (previous_EMA * (1.0 - emaMultiplier));
                        previous_EMA = Internal_Var.TimeDelta_EMA;
                    }

                    // Calculating Internal_Var.RPM
                    Internal_Var.RPM = 10 / Internal_Var.TimeDelta_EMA; //
                    //Internal_Var.RPM = 10 / Internal_Var.dT_sec; //

                    
                    //Internal_Var.RPM in Radians/sec
                    Internal_Var.RPM_Rad = ((10 * 2 * 3.14159) / Internal_Var.TimeDelta_EMA);  //
                    //Internal_Var.RPM_Rad = ((10 * 2 * 3.14159) / Internal_Var.dT_sec);  //

                    if (first_RPM_check == 0)
                    {
                        Internal_Var.Pre_RPM_Rad = 10;
                        first_RPM_check = 1; //Dont come into this check again
                    }

                    //Calcuate the Speed Here (kph)
                    //Vehicle Speed = Internal_Var.RPM * (Shaft Dia * Speed Mul)
                    // Speed Mul = (PI * 60) / 63360
                    speed_mul = (PI * 60) / 63360;
                    speed = (Internal_Var.RPM * (shaft_dia * speed_mul) ) + (Internal_Var.speed_scaler_user*5);

                    if(internal_flags._mph_conv_FLAG == true)
                    {
                        speed = speed * 0.62137; //convert the unit to mph from kph

                    }

                    // Calculating the Acceleration here
                    Internal_Var.Acc = (Internal_Var.RPM_Rad - Internal_Var.Pre_RPM_Rad) / Internal_Var.TimeDelta_EMA; //Internal_Var.TimeDelta_EMA
                    //Internal_Var.Acc = (Internal_Var.RPM_Rad - Internal_Var.Pre_RPM_Rad) / Internal_Var.dT_sec;
                    Internal_Var.Pre_RPM_Rad = Internal_Var.RPM_Rad;

                    //Calculate the Dyno SAE Correction Factor Here//
                    double pws = get_sat_vap_pressure();
                    double Pw = cal_vap_par_pressure(pws);
                    double Pdry = cal_dry_airPressure(Pw);

                    double a = (990 / Pdry);
                    double b = Math.Pow(((actual_sensor_data._temp_C + 273) / 298), 0.5);
                    double Cf = ( 1.176 * (a * b)  ) - 0.176;

                    richTextBox1.AppendText( Cf.ToString()  +"\n");
                    //

                    // Calculating the Torque
                    Internal_Var.Torque_lb = Inertia_Constant * Internal_Var.Acc * ft_lb_conv_factor; //Use EMA Accelelation Afterwards

                    // Calculating the Horespower 
                    Internal_Var.HP = Cf * ( (DynoToEngine_RPM_Mul * Internal_Var.RPM * Internal_Var.Torque_lb) / 5252);
                    /*
                    // Enable this for plotting RPM vs HP
                    chart1.Invoke((MethodInvoker)(() => chart1.Series["Series1"].Points.AddXY(Convert.ToInt32(Internal_Var.RPM), Convert.ToInt32(Internal_Var.HP))));

                    // Enable this for plotting RPM vs Torque
                    chart2.Invoke((MethodInvoker)(() => chart2.Series["Series1"].Points.AddXY(Convert.ToInt32(Internal_Var.RPM), Convert.ToInt32(Internal_Var.Torque_lb))));
                    */

                } // end if regular serial data


            }//end try

            catch (Exception error)
            {
                MessageBox.Show("Error Here in Serial Data");
                MessageBox.Show(error.Message);
            }
        }

        private void button_update_thread()
        {
            while (true)
            {
                if(serialPort1.IsOpen)
                {
                    button2.BackColor = System.Drawing.Color.GreenYellow;

                }
                else
                {
                    button2.BackColor = System.Drawing.Color.Red;
                }

                Thread.Sleep(1000);
            }
        }
        /*
         * This thread is for plotting the graphs (Currently Under Test)
         */
        private void ThreadTask() 
        {

            while (true)
            {
 
                // Enable this for plotting RPM vs HP
                if ( (double.IsInfinity(Internal_Var.HP) ==false) && (double.IsInfinity(Internal_Var.Torque_lb) == false) )
                {
                    chart1.Invoke((MethodInvoker)(() => chart1.Series["Series1"].Points.AddXY(Convert.ToInt32(Internal_Var.RPM), Convert.ToInt32(Internal_Var.HP))));
                    chart1.Invoke((MethodInvoker)(() => chart1.Series["Series2"].Points.AddXY(Convert.ToInt32(Internal_Var.RPM),Convert.ToInt32(Internal_Var.Torque_lb))));

                    // Enable this for plotting AFR Value
                    //chart3.Invoke((MethodInvoker)(() => chart3.Series["Series1"].Points.AddY(Convert.ToInt32(Internal_Var.RPM))));
                    chart3.Invoke((MethodInvoker)(() => chart3.Series["Series1"].Points.AddXY(Convert.ToInt32(Internal_Var.RPM),Internal_Var.AFR_value)));


                    // Update the Speed Guages//
                    aGauge1.Invoke((MethodInvoker)(() => aGauge1.Value = Convert.ToSingle(Internal_Var.RPM)));
                    aGauge2.Invoke((MethodInvoker)(() => aGauge2.Value = Convert.ToSingle(speed)));
                }

                Thread.Sleep(110);
            }
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "DynoGUI" + Convert.ToChar(169) + " Developed By Ali Ahmad";///169 is copyright symbol
            string title = "Help";
            MessageBox.Show(message, title);
        }

       
        private void ComportViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 to = new Form1(this.serialPort1);
            to.Show();   
        }

        private void DiagnosticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //First send the Show sensors data msg to the 
            //Arduino then open new window.
            /*try
            {
                serialPort1.Write(Serial_Data_Variables.Sensor_data_show_msg);
                Serial_Data_Variables.diagnostic_mode = true;

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }*/
            Form2 to = new Form2(serialPort1);
            to.Show();
            //this.Hide();
        }

        private void DynoGUI_Load(object sender, EventArgs e)
        {
       
            start_dyno_button.Enabled = true;
            

            if (internal_flags._mph_conv_FLAG == true)
            {
                
                AGaugeLabel aGaugeLabel2 = new AGaugeLabel();
                aGaugeLabel2.Color = System.Drawing.Color.Yellow;
                aGaugeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                aGaugeLabel2.Name = "GaugeLabel1";
                aGaugeLabel2.Position = new System.Drawing.Point(80, 150);
                aGaugeLabel2.Text = "Speed (mph)";
                
                aGauge2.GaugeLabels.Clear();
                aGauge2.GaugeLabels.Add(aGaugeLabel2);

            }

        }

        //Closing the Serial Port If user Exits the main GUI//
        private void DynoGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Close();
            try
            {
                serialPort1.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        // Exit button Closes the main GUI and Quits the Serial port//
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            try
            {
                serialPort1.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

  
        //This button Starts the Dyno 'dT' Transmission
        private void start_dyno_button_Click(object sender, EventArgs e)
        {
            //Init the thread for plotting graph (For testing purpose now)
            Thread trd = new Thread(new ThreadStart(this.ThreadTask));
            trd.IsBackground = true;
            trd.Start();
            try
            {
                serialPort1.Write(Serial_Data_Variables.Dyno_start_msg);
                Serial_Data_Variables.diagnostic_mode = false;  //This is done to tell the ProcessData Function that now Main DYno data will come instead of Sensors data//
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = true;
                richTextBox1.AppendText("dT transmit msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            
            try
            {
                serialPort1.Close();
                richTextBox1.AppendText("Serial Port Closed \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(Serial_Data_Variables.Wheel_BaseF_Adj_msg);
                Serial_Data_Variables.diagnostic_mode = false;  
                Serial_Data_Variables.WheelBaseAdj_mode = true; //Only enabling the wheelbase adjust mode
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                richTextBox1.AppendText("Wheelbase Front adjust msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

    
        // This Fucntion transmits the stop dyno msg
         private void Stop_dyno_button_Click(object sender, EventArgs e)
           {
               try
               {
                    serialPort1.Write(Serial_Data_Variables.Dyno_stop_msg);
                    Serial_Data_Variables.diagnostic_mode = false;
                    Serial_Data_Variables.WheelBaseAdj_mode = false; 
                    Serial_Data_Variables.dT_transmit_IN_mode = false;
                    richTextBox1.AppendText("dT stop msg sent to Arduino-> \n");
                }
               catch (Exception error)
               {
                   MessageBox.Show(error.Message);
               }
           }


        private void defaultValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Add Default Values form here// To be done afterwards.
            //this.Hide();
            Form4 to = new Form4();
            to.Show();
        }


        /*
         * This function sends configuration msg
         * to the Arduino
         */
        private void config_button_Click(object sender, EventArgs e)
        {
            try
            { 
                //serialPort1.Write(Serial_Data_Variables.Dyno_config_msg);
                serialPort1.Write("$,50,2,3,4.4,5,$");
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false; 
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                richTextBox1.AppendText("Config data sent to Arduino-> \n");
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
                serialPort1.Write(Serial_Data_Variables.Dyno_Fan_ON_msg);
                Serial_Data_Variables.diagnostic_mode = false;   
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                richTextBox1.AppendText("FAN ON msg sent to Arduino-> \n");
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
                serialPort1.Write(Serial_Data_Variables.Dyno_Fan_oFF_msg);
                Serial_Data_Variables.diagnostic_mode = false;
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = false;
                richTextBox1.AppendText("FAN OFF msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }


        private void debugDataWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Send the Debug Data 'Char' to the Arduino Here,
            //then switch the window to the new form (Debug Form)
            Form3 to = new Form3();
            to.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void calibrationSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 to = new Form5(this.serialPort1);
            to.Show();
        }

        /**
         * This button will send the sample_data_simulate message 
         * to Arduino.
         * */
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Write(Serial_Data_Variables.Dyno_sample_msg);
                Serial_Data_Variables.diagnostic_mode = false;  //This is done to tell the ProcessData Function that now Main DYno data will come instead of Sensors data//
                Serial_Data_Variables.WheelBaseAdj_mode = false;
                Serial_Data_Variables.dT_transmit_IN_mode = true;
                richTextBox1.AppendText("dT transmit msg sent to Arduino-> \n");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }

}

/**
 * Global Variable class for Serial Data In
 */
public static class Serial_Data_Variables
{
    public static bool serialConnected;
    public static string serialDataIn;
    public static sbyte indexOfA, indexOfB, indexOfC, indexOfD, indexOfE, indexOfF, indexOfG, indexOfH, indexOfI, indexOfJ;
    public static string dataSensor1, dataSensor2, dataSensor3, dataSensor4, dataSensor5, dataSensor6, dataSensor7, dataSensor8, dataSensor9, dataSensor10;
    public static string Dyno_start_msg = "#";
    public static string Dyno_stop_msg = "A";
    public static string Dyno_sample_msg = "S";
    public static string Wheel_BaseF_Adj_msg = "W";
    public static string Wheel_BaseR_Adj_msg = "R";

    public static string Sensor_data_show_msg = "D";
    public static string Dyno_config_msg = "C";
    public static string Dyno_Brake_msg = "B";
    public static string Dyno_Disable_Brake_msg = "V";
    public static string Dyno_LockPin_msg = "L";
    public static string Dyno_UnLockPin_msg = "U";
    public static string Dyno_Fan_ON_msg = "F";
    public static string Dyno_Fan_oFF_msg = "N";

    public static bool diagnostic_mode = false; //Diagnostic Mode
    public static bool dT_transmit_IN_mode = false;
    public static bool WheelBaseAdj_mode = false;

}

public static class actual_sensor_data
{
    public static double _temp_C = 25;           //temp in Celcius
    public static double _pressure_baro = 1013.2;    //Units (mBar) Default value 1013.2 @ Sea level
    public static double _rel_humidity = 50;     //units %
}

public static class Pspline_Var
{

    public static double[] time_vector = new double[500];
    public static double[] Pspline_vector = new double[500];
    public static int pspline_N = 1;
    public static int index = 0;
    public static double rho = 50;
    public static int info;
    public static double Acc_Pspline; //Computed Accerlations Next
    public static double Spline_sliding_counter = 1;
}

/**
 * Internal Variables used overall the scope of this
 * application
 */
public static class Internal_Var
{
    public static double totalTime;
    public static double[] totalTimeVectot = new double[500];
    public static double TimeDelta_EMA;
    public static double RPM;
    public static double RPM_Rad;
    public static double Pre_RPM_Rad;
    public static double Torque_lb;
    public static double HP;
    public static double dT_sec;
    public static double Acc;
    public static double[] AccVector = new double[1000] ;
    public static double AFR_value = 11; //Attach this variable to the Data coming on serial against the afr sensor value//

    public static double  speed_scaler_user = 0;

}

/*
 * This class contains flags which 
 * are used for user selectable commands
 * These are used across various forms
 */
public static class internal_flags
{
    public static bool _mph_conv_FLAG = false;
}


