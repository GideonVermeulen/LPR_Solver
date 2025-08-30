using System;
using System.Text; // Added
using WinFormsApp1.Solver;
using System.Linq; // Added

namespace WinFormsApp1.Solver
{
    public class SensitivityAnalyzer
    {
        private LPProblem _problem;
        private SimplexResult _simplexResult;

        public SensitivityAnalyzer(LPProblem problem, SimplexResult simplexResult)
        {
            _problem = problem;
            _simplexResult = simplexResult;
        }

        // Placeholder for sensitivity analysis methods
        public string AnalyzeNonBasicVariableRange(int varIndex)
        {
            if (_simplexResult == null || !_simplexResult.IsOptimal)
            {
                return "Cannot analyze non-basic variable range: No optimal solution found.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"=== Non-Basic Variable x{varIndex + 1} Sensitivity ===");

            // Check if the variable is non-basic
            if (_simplexResult.FinalBasis.Contains(varIndex))
            {
                return $"Variable x{varIndex + 1} is a basic variable. Use AnalyzeBasicVariableRange instead.";
            }

            // The allowable increase is the absolute value of its reduced cost.
            // The allowable decrease is infinity (or a very large number).
            // This assumes a maximization problem. If it was originally minimization,
            // the interpretation of reduced costs might be flipped.
            // However, since the solver converts to maximization, we can use the reduced costs directly.

            double reducedCost = _simplexResult.ReducedCosts[varIndex];
            double currentCoefficient = _problem.ObjectiveCoefficients[varIndex];

            // For a non-basic variable in a maximization problem,
            // its coefficient can increase by its reduced cost before it becomes basic.
            // It can decrease infinitely without changing the optimal solution.
            // The range is [current_coefficient - infinity, current_coefficient + reduced_cost]

            sb.AppendLine($"Current Coefficient (c{varIndex + 1}): {currentCoefficient:F4}");
            sb.AppendLine($"Reduced Cost (Cj-Zj for x{varIndex + 1}): {reducedCost:F4}");
            sb.AppendLine($"Allowable Increase: {reducedCost:F4}");
            sb.AppendLine("Allowable Decrease: Infinity");
            sb.AppendLine($"Range: [-Infinity, {currentCoefficient + reducedCost:F4}]");

            return sb.ToString();
        }

        public string AnalyzeBasicVariableRange(int varIndex)
        {
            return "Basic Variable Range Analysis for x" + (varIndex + 1) + " (Not yet implemented)";
        }

        public string AnalyzeRHSConstraintRange(int constraintIndex)
        {
            return "RHS Constraint Range Analysis for constraint " + (constraintIndex + 1) + " (Not yet implemented)";
        }

        public string DisplayShadowPrices()
        {
            if (_simplexResult == null || !_simplexResult.IsOptimal)
            {
                return "Cannot display shadow prices: No optimal solution found.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Shadow Prices ===");

            int numOriginalVars = _problem.ObjectiveCoefficients.Length;
            int numConstraints = _problem.RHS.Length;

            // Shadow prices are the negative of the reduced costs of the slack variables.
            // Slack variables are at indices numOriginalVars to numOriginalVars + numConstraints - 1
            for (int i = 0; i < numConstraints; i++)
            {
                double shadowPrice = -_simplexResult.ReducedCosts[numOriginalVars + i];
                sb.AppendLine($"Constraint {i + 1}: {shadowPrice:F4}");
            }

            return sb.ToString();
        }

        public string SolveDualProblem()
        {
            // Only works for standard form: maximize c^T x, Ax <= b, x >= 0
            // Dual: minimize b^T y, A^T y >= c, y >= 0

            int m = _problem.Constraints.GetLength(0); // number of constraints
            int n = _problem.Constraints.GetLength(1); // number of variables

            // Build dual problem data
            double[] dualObj;
            double[,] dualA = new double[n, m]; // A^T
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    dualA[j, i] = _problem.Constraints[i, j];

            double[] dualRHS;

            // Determine dual variable signs and constraints types based on primal problem
            LPProblem.ConstraintType[] dualConstraintTypes;
            LPProblem.VarSign[] dualVarSigns;

            bool primalIsMax = _problem.Maximize;

            if (primalIsMax)
            {
                // Primal: max c^T x, Ax <= b
                // Dual: min b^T y, A^T y >= c
                dualObj = (double[])_problem.RHS.Clone();
                dualRHS = (double[])_problem.ObjectiveCoefficients.Clone();
                dualConstraintTypes = Enumerable.Repeat(LPProblem.ConstraintType.GreaterThanOrEqual, n).ToArray();
                dualVarSigns = Enumerable.Repeat(LPProblem.VarSign.NonNegative, m).ToArray();
            }
            else
            {
                // Primal: min c^T x, Ax >= b
                // Dual: max b^T y, A^T y <= c
                dualObj = (double[])_problem.RHS.Clone();
                dualRHS = (double[])_problem.ObjectiveCoefficients.Clone();
                dualConstraintTypes = Enumerable.Repeat(LPProblem.ConstraintType.LessThanOrEqual, n).ToArray();
                dualVarSigns = Enumerable.Repeat(LPProblem.VarSign.NonNegative, m).ToArray();
            }

            // Create dual LPProblem
            var dualProblem = new LPProblem
            {
                ObjectiveCoefficients = dualObj,
                Constraints = dualA,
                RHS = dualRHS,
                ConstraintTypes = dualConstraintTypes,
                VariableSigns = dualVarSigns,
                Maximize = !primalIsMax // Dual sense is opposite
            };

            // Solve dual using PrimalSimplexSolver (or any available solver)
            var solver = new PrimalSimplexSolver();
            var dualResult = solver.Solve(dualProblem);

            // Output dual formulation and solution
            var sb = new StringBuilder();
            sb.AppendLine("=== Dual Problem Formulation ===");
            sb.AppendLine("Minimize: " + string.Join(" + ", dualObj.Select((v, i) => $"{v}y{i + 1}")));
            for (int i = 0; i < n; i++)
            {
                var terms = new List<string>();
                for (int j = 0; j < m; j++)
                    terms.Add($"{dualA[i, j]}y{j + 1}");
                sb.AppendLine(string.Join(" + ", terms) + $" >= {dualRHS[i]}");
            }
            sb.AppendLine("y >= 0");
            sb.AppendLine();
            sb.AppendLine("=== Dual Solution ===");
            sb.AppendLine(dualResult.OutputLog);

            return sb.ToString();
        }

        private double[,] TransposeMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] transposed = new double[cols, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    transposed[j, i] = matrix[i, j];
                }
            }
            return transposed;
        }
    }
}
