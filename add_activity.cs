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
    public partial class add_activity : Form
    {
        // Public properties to access the input after dialog closes
        public double ObjCoeff { get; private set; }
        public double[] ConstraintCoeffs { get; private set; }
        public WinFormsApp1.Solver.LPProblem.VarType VarType { get; private set; } = WinFormsApp1.Solver.LPProblem.VarType.Continuous;
        public WinFormsApp1.Solver.LPProblem.VarSign VarSign { get; private set; } = WinFormsApp1.Solver.LPProblem.VarSign.NonNegative;

        public add_activity()
        {
            InitializeComponent();
        }

        private void okay_button_Click(object sender, EventArgs e)
        {
            ConstraintCoeffs = new double[2];

            // Validate and parse objective coefficient
            if (!double.TryParse(obj_fun_text.Text, out double objCoeff))
            {
                MessageBox.Show("Objective coefficient must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ObjCoeff = objCoeff;

            // Validate and parse constraint coefficient 1
            if (string.IsNullOrWhiteSpace(con1_text.Text))
            {
                MessageBox.Show("Constraint coefficient 1 is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!double.TryParse(con1_text.Text, out double coeff1))
            {
                MessageBox.Show("Constraint coefficient 1 must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ConstraintCoeffs[0] = coeff1;

            // Validate and parse constraint coefficient 2
            if (string.IsNullOrWhiteSpace(con2_text.Text))
            {
                MessageBox.Show("Constraint coefficient 2 is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!double.TryParse(con2_text.Text, out double coeff2))
            {
                MessageBox.Show("Constraint coefficient 2 must be a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ConstraintCoeffs[1] = coeff2;


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cance_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
