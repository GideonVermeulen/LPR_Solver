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

            sb.AppendLine($"Current Coefficient (c{varIndex + 1}): {currentCoefficient:F4}");
            sb.AppendLine($"Reduced Cost (Cj-Zj for x{varIndex + 1}): {reducedCost:F4}");
            sb.AppendLine($"Allowable Increase: {reducedCost:F4}");
            sb.AppendLine("Allowable Decrease: Infinity");
            sb.AppendLine($"Range: [-Infinity, {currentCoefficient + reducedCost:F4}]");

            return sb.ToString();
        }

        public string AnalyzeBasicVariableRange(int varIndex)
        {
            if (_simplexResult == null || !_simplexResult.IsOptimal)
            {
                return "Cannot analyze basic variable range: No optimal solution found.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"=== Basic Variable x{varIndex + 1} Sensitivity ===");

            // Check if variable is basic
            int basicRow = Array.IndexOf(_simplexResult.FinalBasis, varIndex);
            if (basicRow == -1)
            {
                return $"Variable x{varIndex + 1} is non-basic. Use AnalyzeNonBasicVariableRange instead.";
            }

            double currentCoeff = _problem.ObjectiveCoefficients[varIndex];
            double reducedCost = 0;

            double allowableIncrease = double.PositiveInfinity;
            double allowableDecrease = double.PositiveInfinity;

            int numVars = _simplexResult.FinalTableau.GetLength(1) - 1; // exclude RHS
            int numRows = _simplexResult.FinalTableau.GetLength(0);

            for (int j = 0; j < numVars; j++)
            {
                // Skip if j is a basic variable
                if (_simplexResult.FinalBasis.Contains(j))
                    continue;

                double zjCoeff = _simplexResult.FinalTableau[basicRow, j];
                double rc = _simplexResult.ReducedCosts[j];

                if (Math.Abs(zjCoeff) < 1e-8)
                    continue;

                double ratio = rc / zjCoeff;

                if (zjCoeff > 0)
                {
                    allowableDecrease = Math.Min(allowableDecrease, ratio);
                }
                else
                {
                    allowableIncrease = Math.Min(allowableIncrease, -ratio);
                }
            }

            double lowerBound = currentCoeff - (double.IsPositiveInfinity(allowableDecrease) ? 0 : allowableDecrease);
            double upperBound = currentCoeff + (double.IsPositiveInfinity(allowableIncrease) ? 0 : allowableIncrease);

            sb.AppendLine($"Current Coefficient (c{varIndex + 1}): {currentCoeff:F4}");
            sb.AppendLine($"Reduced Cost: {reducedCost:F4}");
            sb.AppendLine($"Allowable Increase: {(double.IsPositiveInfinity(allowableIncrease) ? "Infinity" : allowableIncrease.ToString("F4"))}");
            sb.AppendLine($"Allowable Decrease: {(double.IsPositiveInfinity(allowableDecrease) ? "Infinity" : allowableDecrease.ToString("F4"))}");
            sb.AppendLine($"Range: [{(double.IsNegativeInfinity(lowerBound) ? "-Infinity" : lowerBound.ToString("F4"))}, {(double.IsPositiveInfinity(upperBound) ? "Infinity" : upperBound.ToString("F4"))}]");

            return sb.ToString();
        }

        public string AnalyzeRHSConstraintRange(int constraintIndex)
        {
            if (_simplexResult == null || !_simplexResult.IsOptimal)
            {
                return "Cannot analyze constraint range: No optimal solution found.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"=== Constraint {constraintIndex + 1} RHS Sensitivity ===");

            int numConstraints = _simplexResult.FinalTableau.GetLength(0);
            int numVars = _simplexResult.FinalTableau.GetLength(1) - 1; // exclude RHS

            // Get the shadow price (dual value) for this constraint
            double shadowPrice = _simplexResult.FinalTableau[numConstraints - 1, constraintIndex];

            // Get the current RHS value
            double currentRHS = _simplexResult.FinalTableau[constraintIndex, numVars];

            // Estimate allowable increase/decrease (simplified)
            double allowableIncrease = double.PositiveInfinity;
            double allowableDecrease = double.PositiveInfinity;

            for (int i = 0; i < numConstraints - 1; i++) // skip objective row
            {
                if (i == constraintIndex)
                    continue;

                double coeff = _simplexResult.FinalTableau[i, constraintIndex];
                double rhs = _simplexResult.FinalTableau[i, numVars];

                if (coeff == 0)
                    continue;

                double ratio = rhs / coeff;

                if (coeff > 0)
                    allowableIncrease = Math.Min(allowableIncrease, ratio);
                else
                    allowableDecrease = Math.Min(allowableDecrease, -ratio);
            }

            double lowerBound = currentRHS - (double.IsPositiveInfinity(allowableDecrease) ? 0 : allowableDecrease);
            double upperBound = currentRHS + (double.IsPositiveInfinity(allowableIncrease) ? 0 : allowableIncrease);

            sb.AppendLine($"Current RHS: {currentRHS:F4}");
            sb.AppendLine($"Shadow Price: {shadowPrice:F4}");
            sb.AppendLine($"Allowable Increase: {(double.IsPositiveInfinity(allowableIncrease) ? "Infinity" : allowableIncrease.ToString("F4"))}");
            sb.AppendLine($"Allowable Decrease: {(double.IsPositiveInfinity(allowableDecrease) ? "Infinity" : allowableDecrease.ToString("F4"))}");
            sb.AppendLine($"Range: [{lowerBound:F4}, {upperBound:F4}]");

            return sb.ToString();
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
