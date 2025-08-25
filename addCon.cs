using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class addCon : Form
    {
        public double RHS { get; private set; }
        public string ConstraintText { get; private set; }

        public addCon()
        {
            InitializeComponent();
            buttonCancel.DialogResult = DialogResult.Cancel;
        }

        //inputTextBox.Text <---

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Check for empty values
            if (string.IsNullOrWhiteSpace(x1.Text) ||
                string.IsNullOrWhiteSpace(x2.Text) ||
                string.IsNullOrWhiteSpace(x3.Text) ||
                string.IsNullOrWhiteSpace(txtRhs.Text) ||
                comboSign.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields and select a constraint type.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get values from text boxes
                string x1Value = x1.Text;
                string x2Value = x2.Text;
                string x3Value = x3.Text;
                string signValue = comboSign.SelectedItem.ToString();
                string rhsValue = txtRhs.Text;

                // Parse to double
                double c1 = double.Parse(x1Value);
                double c2 = double.Parse(x2Value);
                double c3 = double.Parse(x3Value);
                double rhs = double.Parse(rhsValue);

                // Now you have the values in variables:
                // c1, c2, c3, signValue, rhs

                ConstraintText = $"{c1} {c2} {c3} {signValue} {rhs}";

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter valid numeric values for all fields.");
            }
        }
    }
}
