using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1.Solver
{
    public partial class Add_con : Form
    {
        public double X1 { get; private set; }
        public double X2 { get; private set; }
        public double X3 { get; private set; }
        public string Sign { get; private set; }
        public double Rhs { get; private set; }

        public Add_con()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okay_button_Click(object sender, EventArgs e)
        {
            // Null/empty value error checking for each input
            if (string.IsNullOrWhiteSpace(x1_text.Text) ||
                string.IsNullOrWhiteSpace(x2_text.Text) ||
                string.IsNullOrWhiteSpace(x3_text.Text) ||
                sign_box.SelectedItem == null ||
                string.IsNullOrWhiteSpace(rhs_text.Text))
            {
                MessageBox.Show("All fields must be filled in.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Parse and validate numeric values
            if (!double.TryParse(x1_text.Text, out double x1))
            {
                MessageBox.Show("X1 must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!double.TryParse(x2_text.Text, out double x2))
            {
                MessageBox.Show("X2 must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!double.TryParse(x3_text.Text, out double x3))
            {
                MessageBox.Show("X3 must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!double.TryParse(rhs_text.Text, out double rhs))
            {
                MessageBox.Show("RHS must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Save values to properties
            X1 = x1;
            X2 = x2;
            X3 = x3;
            Sign = sign_box.SelectedItem.ToString();
            Rhs = rhs;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
