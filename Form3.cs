using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DynoGUI_V1._0
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;   //Set timer true when window loads
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = Convert.ToString(Internal_Var.dT_sec);
            textBox2.Text = Convert.ToString(Internal_Var.TimeDelta_EMA);
            textBox3.Text = Convert.ToString(Internal_Var.RPM);
            textBox4.Text = Convert.ToString(Internal_Var.Pre_RPM_Rad);
            textBox5.Text = Convert.ToString(Internal_Var.RPM_Rad);
            textBox6.Text = Convert.ToString(Internal_Var.Acc);
            textBox7.Text = Convert.ToString(Internal_Var.Torque_lb);
            textBox8.Text = Convert.ToString(Internal_Var.HP);
            textBox9.Text = Convert.ToString(Pspline_Var.Acc_Pspline);

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
