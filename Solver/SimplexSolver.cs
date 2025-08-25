using System;
using System.Linq;
using System.Text;

namespace WinFormsApp1.Solver
{
    public class PrimalSimplexSolver : ILPSolver
    {
        public SimplexResult Solve(LPProblem problem)
        {
            double[] c = (double[])problem.ObjectiveCoefficients.Clone();
            double[,] A = problem.Constraints;
            double[] b = problem.RHS;
            bool maximize = problem.Maximize;

            if (!maximize)
                for (int i = 0; i < c.Length; i++) c[i] = -c[i];

            int m = A.GetLength(0);
            int n = A.GetLength(1);
            int numTotalVars = n + m;
            int numTableauCols = numTotalVars + 1; // +1 for RHS
            double[,] T = new double[m, numTableauCols];

            // Initialize tableau with A, I, and b
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) T[i, j] = A[i, j];
                T[i, n + i] = 1.0; // Slack variables
                T[i, numTableauCols - 1] = b[i]; // RHS
            }

            int[] basis = Enumerable.Range(n, m).ToArray(); // Initial basis are slack variables
            double[] cFull = new double[numTotalVars];
            Array.Copy(c, cFull, n);

            var sb = new StringBuilder();
            sb.AppendLine("=== Primal Simplex Solution ===");
            int iter = 0;
            double tol = 1e-9;
            int maxIters = 2000;

            // Initialize SimplexResult
            SimplexResult result = new SimplexResult();
            result.IsOptimal = false;
            result.IsUnbounded = false;
            result.IsFeasible = true; // Assume feasible until proven otherwise

            // --- Helper for formatting ---
            const int colWidth = 10; // Consistent column width
            string FormatCell(object value) => Convert.ToString(value).PadRight(colWidth);

            string CreateHorizontalLine(int numberOfColumns) =>
                "+" + string.Join("+", Enumerable.Repeat(new string('-', colWidth), numberOfColumns)) + "+";

            while (iter < maxIters)
            {
                iter++;
                sb.AppendLine(CreateHorizontalLine(numTotalVars + 2)); // +2 for Basis and RHS columns
                sb.AppendLine(FormatCell("Iteration") + "|" + FormatCell(iter) + string.Join("|", Enumerable.Repeat(FormatCell(""), numTotalVars)) + "|");
                sb.AppendLine(CreateHorizontalLine(numTotalVars + 2));

                // --- Calculate Reduced Costs (Cj - Zj) ---
                double[] reduced = new double[numTotalVars];
                for (int j = 0; j < numTotalVars; j++)
                {
                    double z_j = 0;
                    for (int i = 0; i < m; i++)
                    {
                        z_j += cFull[basis[i]] * T[i, j];
                    }
                    reduced[j] = cFull[j] - z_j;
                }
                result.ReducedCosts = reduced; // Store reduced costs

                // --- Print Tableau Header ---
                var headerItems = new List<string>();
                headerItems.Add(FormatCell("Basis"));
                for (int j = 0; j < n; j++) headerItems.Add(FormatCell(string.Format("x{0}", j + 1)));
                for (int j = 0; j < m; j++) headerItems.Add(FormatCell(string.Format("s{0}", j + 1)));
                headerItems.Add(FormatCell("RHS"));
                sb.AppendLine(string.Join("|", headerItems) + "|");
                sb.AppendLine(CreateHorizontalLine(numTotalVars + 2));

                // --- Print Tableau Rows ---
                for (int i = 0; i < m; i++)
                {
                    var rowItems = new List<string>();
                    rowItems.Add(FormatCell((basis[i] < n) ? string.Format("x{0}", basis[i] + 1) : string.Format("s{0}", basis[i] - n + 1)));
                    for (int j = 0; j < numTableauCols; j++)
                    {
                        rowItems.Add(FormatCell(Math.Round(T[i, j], 3)));
                    }
                    sb.AppendLine(string.Join("|", rowItems) + "|");
                    sb.AppendLine(CreateHorizontalLine(numTotalVars + 2)); // Horizontal line after each row
                }

                // --- Print Cj-Zj Row ---
                var reducedCostRowItems = new List<string>();
                reducedCostRowItems.Add(FormatCell("Cj-Zj"));
                for (int j = 0; j < numTotalVars; j++)
                {
                    reducedCostRowItems.Add(FormatCell(Math.Round(reduced[j], 3)));
                }
                reducedCostRowItems.Add(FormatCell("")); // Empty for RHS
                sb.AppendLine(string.Join("|", reducedCostRowItems) + "|");
                sb.AppendLine(CreateHorizontalLine(numTotalVars + 2));


                // --- Check for Optimality ---
                int entering = -1;
                double maxReduced = tol;
                for (int j = 0; j < numTotalVars; j++)
                {
                    if (reduced[j] > maxReduced)
                    {
                        maxReduced = reduced[j];
                        entering = j;
                    }
                }

                if (entering == -1)
                {
                    // Optimal solution found
                    double[] xFull = new double[numTotalVars];
                    for (int i = 0; i < m; i++) xFull[basis[i]] = T[i, numTableauCols - 1];
                    double obj = 0;
                    for (int j = 0; j < n; j++)
                    {
                        obj += problem.ObjectiveCoefficients[j] * xFull[j];
                    }

                    result.OptimalSolution = xFull.Take(n).ToArray(); // Only original variables
                    result.OptimalObjectiveValue = obj;
                    result.FinalTableau = T;
                    result.FinalBasis = basis;
                    result.IsOptimal = true;


                    sb.AppendLine("\n=== Optimal Solution Found ===");
                    sb.AppendLine(string.Format("Objective Value = {0}", Math.Round(obj, 4)));
                    for (int j = 0; j < n; j++) sb.AppendLine(string.Format("x{0} = {1}", j + 1, Math.Round(xFull[j], 4)));

                    // Feasibility Check
                    sb.AppendLine("\n--- Feasibility Check ---");
                    bool allFeasible = true;
                    for (int i = 0; i < m; i++)
                    {
                        double lhs = 0;
                        for (int j = 0; j < n; j++)
                        {
                            lhs += A[i, j] * xFull[j];
                        }

                        bool constraintSatisfied = false;
                        switch (problem.ConstraintTypes[i])
                        {
                            case LPProblem.ConstraintType.LessThanOrEqual:
                                constraintSatisfied = lhs <= b[i] + tol;
                                break;
                            case LPProblem.ConstraintType.Equal:
                                constraintSatisfied = Math.Abs(lhs - b[i]) <= tol;
                                break;
                            case LPProblem.ConstraintType.GreaterThanOrEqual:
                                constraintSatisfied = lhs >= b[i] - tol;
                                break;
                        }

                        if (!constraintSatisfied)
                        {
                            sb.AppendLine(string.Format("Constraint {0} is VIOLATED: {1} {2} {3}", i + 1, lhs, problem.ConstraintTypes[i], b[i]));
                            allFeasible = false;
                        }
                        else
                        {
                            sb.AppendLine(string.Format("Constraint {0} is satisfied: {1} {2} {3}", i + 1, lhs, problem.ConstraintTypes[i], b[i]));
                        }
                    }

                    result.IsFeasible = allFeasible;

                    if (allFeasible)
                    {
                        sb.AppendLine("All constraints are satisfied.");
                    }
                    else
                    {
                        sb.AppendLine("WARNING: Solution is not feasible.");
                    }

                    result.OutputLog = sb.ToString();
                    return result;
                }

                // --- Ratio Test ---
                double minRatio = double.PositiveInfinity;
                int leaving = -1;
                for (int i = 0; i < m; i++)
                {
                    if (T[i, entering] > tol)
                    {
                        double ratio = T[i, numTableauCols - 1] / T[i, entering];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            leaving = i;
                        }
                    }
                }
                if (leaving == -1)
                {
                    result.IsUnbounded = true;
                    sb.AppendLine("--- UNBOUNDED PROBLEM ---");
                    sb.AppendLine("The solution is unbounded and can be increased infinitely.");
                    result.OutputLog = sb.ToString();
                    return result;
                }
                
                string enteringVar = (entering < n) ? string.Format("x{0}", entering + 1) : string.Format("s{0}", entering - n + 1);
                string leavingVar = (basis[leaving] < n) ? string.Format("x{0}", basis[leaving] + 1) : string.Format("s{0}", basis[leaving] - n + 1);
                sb.AppendLine(string.Format("\nEntering Variable: {0}", enteringVar));
                sb.AppendLine(string.Format("Leaving Variable: {0}\n", leavingVar));


                // --- Pivot Operation ---
                double piv = T[leaving, entering];
                for (int j = 0; j < numTableauCols; j++) T[leaving, j] /= piv;

                for (int i = 0; i < m; i++)
                {
                    if (i == leaving) continue;
                    double factor = T[i, entering];
                    for (int j = 0; j < numTableauCols; j++)
                    {
                        T[i, j] -= factor * T[leaving, j];
                    }
                }
                basis[leaving] = entering;

                if (iter > maxIters)
                {
                    sb.AppendLine("--- ITERATION LIMIT REACHED ---");
                    result.OutputLog = sb.ToString();
                    return result;
                }
            }
            result.OutputLog = sb.ToString();
            return result;
        }
    }
}