using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Solver;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private SimplexResult _lastSimplexResult;
        private LPProblem _lastLPProblem;

        public Form1()
        {
            InitializeComponent();
            // Populate the algorithm ComboBox with all available solvers
            algorithmComboBox.Items.Add("Primal Simplex");
            algorithmComboBox.Items.Add("Revised Primal Simplex");
            algorithmComboBox.Items.Add("Branch and Bound");
            algorithmComboBox.Items.Add("Cutting Plane");
            algorithmComboBox.Items.Add("Knapsack");
            algorithmComboBox.SelectedIndex = 0;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        inputTextBox.Text = File.ReadAllText(openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void solveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                MessageBox.Show("Please load an LP problem first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var lines = new List<string>(inputTextBox.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                LPProblem problem = LPParser.Parse(lines);

                ILPSolver solver = null;
                string selectedAlgorithm = algorithmComboBox.SelectedItem.ToString();

                // --- Validation for Sign Restrictions (Mandatory for marks, except Knapsack) ---
                if (selectedAlgorithm != "Knapsack" && !problem.HasSignRestrictionLine)
                {
                    MessageBox.Show("For this project, all algorithms (except Knapsack) require explicit variable type/sign restrictions (e.g., int, bin, +, -, urs) in the input file.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- Algorithm-Specific Validation ---
                if (selectedAlgorithm == "Cutting Plane")
                {
                    if (!problem.VariableTypes.Any(vt => vt == LPProblem.VarType.Integer || vt == LPProblem.VarType.Binary))
                    {
                        MessageBox.Show("Cutting Plane algorithm requires at least one integer or binary variable.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                // Add more algorithm-specific validations here if needed
                if (selectedAlgorithm == "Branch and Bound")
                {
                    if (!problem.VariableTypes.Any(vt => vt == LPProblem.VarType.Integer || vt == LPProblem.VarType.Binary))
                    {
                        MessageBox.Show("Branch and Bound algorithm requires at least one integer or binary variable.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                switch (selectedAlgorithm)
                {
                    case "Primal Simplex":
                        solver = new PrimalSimplexSolver();
                        break;
                    case "Revised Primal Simplex":
                        solver = new RevisedPrimalSimplexSolver();
                        break;
                    case "Branch and Bound":
                        solver = new BranchAndBound();
                        break;
                    case "Cutting Plane":
                        solver = new CuttingPlaneSolver();
                        break;
                    case "Knapsack":
                        solver = new KnapsackSolver();
                        break;
                    default:
                        MessageBox.Show("Please select a valid algorithm.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                SimplexResult result = solver.Solve(problem);
                _lastSimplexResult = result;
                _lastLPProblem = problem;
                outputTextBox.Text = result.OutputLog;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during solving: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void performAnalysisButton_Click(object sender, EventArgs e)
        {
            if (_lastSimplexResult == null || !_lastSimplexResult.IsOptimal)
            {
                MessageBox.Show("Please solve an LP problem first and ensure an optimal solution is found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_lastLPProblem == null)
            {
                MessageBox.Show("LP Problem data is not available for sensitivity analysis.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SensitivityAnalyzer analyzer = new SensitivityAnalyzer(_lastLPProblem, _lastSimplexResult);
            string analysisResult = "";

            if (shadowPricesRadioButton.Checked)
            {
                analysisResult = analyzer.DisplayShadowPrices();
            }
            else if (nonBasicVarRangeRadioButton.Checked)
            {
                if (int.TryParse(analysisInputTextBox.Text, out int varIndex))
                {
                    analysisResult = analyzer.AnalyzeNonBasicVariableRange(varIndex - 1); // Adjust for 0-based index
                }
                else
                {
                    MessageBox.Show("Please enter a valid variable index for Non-Basic Variable Range.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (basicVarRangeRadioButton.Checked)
            {
                if (int.TryParse(analysisInputTextBox.Text, out int varIndex))
                {
                    analysisResult = analyzer.AnalyzeBasicVariableRange(varIndex - 1); // Adjust for 0-based index
                }
                else
                {
                    MessageBox.Show("Please enter a valid variable index for Basic Variable Range.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (rhsRangeRadioButton.Checked)
            {
                if (int.TryParse(analysisInputTextBox.Text, out int constraintIndex))
                {
                    analysisResult = analyzer.AnalyzeRHSConstraintRange(constraintIndex - 1); // Adjust for 0-based index
                }
                else
                {
                    MessageBox.Show("Please enter a valid constraint index for RHS Constraint Range.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (dualProblemRadioButton.Checked)
            {
                analysisResult = analyzer.SolveDualProblem();
            }
            else
            {
                MessageBox.Show("Please select a sensitivity analysis type.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            outputTextBox.Text = analysisResult;
        }

        private void export_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(outputTextBox.Text))
            {
                MessageBox.Show("No output to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FileName = "LP_Solution.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, outputTextBox.Text);
                        MessageBox.Show("Output exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void add_con_button_Click(object sender, EventArgs e)
        {
            if (_lastLPProblem == null || _lastSimplexResult == null)
            {
                MessageBox.Show("Please load and solve an LP problem first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var addConForm = new WinFormsApp1.Solver.Add_con();
            if (addConForm.ShowDialog() == DialogResult.OK)
            {
                // 1. Check if the current optimal solution satisfies the new constraint
                double[] x = _lastSimplexResult.OptimalSolution;
                double lhs = addConForm.X1 * x[0] + addConForm.X2 * x[1] + addConForm.X3 * x[2];
                double rhs = addConForm.Rhs;
                string sign = addConForm.Sign;
                bool satisfied = false;

                switch (sign)
                {
                    case "<=":
                        satisfied = lhs <= rhs + 1e-8; // small tolerance for floating point
                        break;
                    case "=":
                        satisfied = Math.Abs(lhs - rhs) <= 1e-8;
                        break;
                    case ">=":
                        satisfied = lhs >= rhs - 1e-8;
                        break;
                }

                if (satisfied)
                {
                    MessageBox.Show("The current optimal solution still satisfies the new constraint. No need to re-solve.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2. If not satisfied, add the constraint to the LP and re-solve
                var problem = _lastLPProblem;

                int oldRows = problem.Constraints.GetLength(0);
                int cols = problem.Constraints.GetLength(1);
                var newConstraints = new double[oldRows + 1, cols];
                for (int i = 0; i < oldRows; i++)
                    for (int j = 0; j < cols; j++)
                        newConstraints[i, j] = problem.Constraints[i, j];
                newConstraints[oldRows, 0] = addConForm.X1;
                newConstraints[oldRows, 1] = addConForm.X2;
                newConstraints[oldRows, 2] = addConForm.X3;

                var newRHS = new double[problem.RHS.Length + 1];
                problem.RHS.CopyTo(newRHS, 0);
                newRHS[problem.RHS.Length] = addConForm.Rhs;

                var newTypes = problem.ConstraintTypes.ToList();
                LPProblem.ConstraintType type = LPProblem.ConstraintType.LessThanOrEqual;
                switch (addConForm.Sign)
                {
                    case "<=": type = LPProblem.ConstraintType.LessThanOrEqual; break;
                    case "=": type = LPProblem.ConstraintType.Equal; break;
                    case ">=": type = LPProblem.ConstraintType.GreaterThanOrEqual; break;
                }
                newTypes.Add(type);

                problem.Constraints = newConstraints;
                problem.RHS = newRHS;
                problem.ConstraintTypes = newTypes.ToArray();

                // Re-solve
                ILPSolver solver = null;
                string selectedAlgorithm = algorithmComboBox.SelectedItem.ToString();
                switch (selectedAlgorithm)
                {
                    case "Primal Simplex":
                        solver = new PrimalSimplexSolver();
                        break;
                    case "Revised Primal Simplex":
                        solver = new RevisedPrimalSimplexSolver();
                        break;
                    case "Branch and Bound":
                        solver = new BranchAndBound();
                        break;
                    case "Cutting Plane":
                        solver = new CuttingPlaneSolver();
                        break;
                    case "Knapsack":
                        solver = new KnapsackSolver();
                        break;
                }
                if (solver != null)
                {
                    var result = solver.Solve(problem);
                    _lastSimplexResult = result;
                    outputTextBox.Text = result.OutputLog;
                }
            }
        }

        private void add_act_button_Click(object sender, EventArgs e)
        {
            if (_lastLPProblem == null)
            {
                MessageBox.Show("Please load and solve an LP problem first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var addActForm = new WinFormsApp1.Solver.add_activity();
            if (addActForm.ShowDialog() == DialogResult.OK)
            {
                var problem = _lastLPProblem;

                int m = problem.Constraints.GetLength(0); // number of constraints
                int n = problem.Constraints.GetLength(1); // number of variables

                // 1. Expand Constraints matrix (add a column)
                var newConstraints = new double[m, n + 1];
                for (int i = 0; i < m; i++)
                    for (int j = 0; j < n; j++)
                        newConstraints[i, j] = problem.Constraints[i, j];
                // Add new activity's coefficients
                for (int i = 0; i < m; i++)
                    newConstraints[i, n] = addActForm.ConstraintCoeffs[i];

                // 2. Expand ObjectiveCoefficients
                var newObjCoeffs = new double[n + 1];
                for (int j = 0; j < n; j++)
                    newObjCoeffs[j] = problem.ObjectiveCoefficients[j];
                newObjCoeffs[n] = addActForm.ObjCoeff;

                // 3. Expand VariableTypes and VariableSigns if needed
                var newVarTypes = problem.VariableTypes?.ToList() ?? Enumerable.Repeat(LPProblem.VarType.Continuous, n).ToList();
                newVarTypes.Add(addActForm.VarType);

                var newVarSigns = problem.VariableSigns?.ToList() ?? Enumerable.Repeat(LPProblem.VarSign.NonNegative, n).ToList();
                newVarSigns.Add(addActForm.VarSign);

                // 4. Update the problem
                problem.Constraints = newConstraints;
                problem.ObjectiveCoefficients = newObjCoeffs;
                problem.VariableTypes = newVarTypes.ToArray();
                problem.VariableSigns = newVarSigns.ToArray();

                // 5. Re-solve the problem
                ILPSolver solver = null;
                string selectedAlgorithm = algorithmComboBox.SelectedItem.ToString();
                switch (selectedAlgorithm)
                {
                    case "Primal Simplex":
                        solver = new PrimalSimplexSolver();
                        break;
                    case "Revised Primal Simplex":
                        solver = new RevisedPrimalSimplexSolver();
                        break;
                    case "Branch and Bound":
                        solver = new BranchAndBound();
                        break;
                    case "Cutting Plane":
                        solver = new CuttingPlaneSolver();
                        break;
                    case "Knapsack":
                        solver = new KnapsackSolver();
                        break;
                }
                if (solver != null)
                {
                    var result = solver.Solve(problem);
                    _lastSimplexResult = result;
                    outputTextBox.Text = result.OutputLog;
                }
            }
        }
    }
}