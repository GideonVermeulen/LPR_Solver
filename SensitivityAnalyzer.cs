using System;
using System.Text; // Added
using WinFormsApp1.Solver;

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
            return "Dual Problem Solution (Not yet implemented)";
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
