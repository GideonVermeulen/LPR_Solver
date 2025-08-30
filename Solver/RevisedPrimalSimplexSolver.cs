using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Solver
{
    public class RevisedPrimalSimplexSolver : ILPSolver
    {
        public SimplexResult Solve(LPProblem problem)
        {
            var sb = new StringBuilder();

            // Initialize SimplexResult
            SimplexResult result = new SimplexResult();
            result.IsOptimal = false;
            result.IsUnbounded = false;
            result.IsFeasible = true; // Assume feasible until proven otherwise

            double[] c = (double[])problem.ObjectiveCoefficients.Clone();
            double[,] A = problem.Constraints;
            double[] b = (double[])problem.RHS.Clone();
            bool maximize = problem.Maximize;

            if (!maximize)
            {
                for (int i = 0; i < c.Length; i++) c[i] = -c[i];
            }

            int m = A.GetLength(0);
            int n = A.GetLength(1);

            double[,] Aaug = new double[m, n + m];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) Aaug[i, j] = A[i, j];
                Aaug[i, n + i] = 1.0;
            }

            double[] cFull = new double[n + m];
            Array.Copy(c, cFull, n);

            int[] basis = Enumerable.Range(n, m).ToArray();
            int iter = 0;
            const int maxIters = 500;
            const double tol = 1e-9;

            while (iter < maxIters)
            {
                iter++;
                sb.AppendLine(string.Format("--- Revised Simplex Iteration {0} ---", iter));

                double[,] B = GetBasisMatrix(Aaug, basis);
                double[,] Binv = Invert(B);
                double[] cB = basis.Select(bv => cFull[bv]).ToArray();

                double[] pi = MultiplyVectorByMatrixTranspose(cB, Binv);
                sb.AppendLine("Price-out (pi = c_B^T * B^-1):");
                sb.AppendLine(string.Join("\t", pi.Select(v => Math.Round(v, 6))));
                sb.AppendLine();

                double[] reduced = new double[n + m];
                for (int j = 0; j < n + m; j++)
                {
                    double piAj = 0.0;
                    for (int i = 0; i < m; i++) piAj += pi[i] * Aaug[i, j];
                    reduced[j] = cFull[j] - piAj;
                }

                result.ReducedCosts = reduced; // Store reduced costs

                sb.AppendLine("Reduced costs (c_j - pi*a_j):");
                sb.AppendLine(string.Join("\t", reduced.Select(v => Math.Round(v, 6))));
                sb.AppendLine();

                int entering = -1;
                double bestVal = 0.0;
                for (int j = 0; j < n + m; j++)
                {
                    if (!basis.Contains(j) && reduced[j] > bestVal + tol)
                    {
                        bestVal = reduced[j];
                        entering = j;
                    }
                }

                if (entering == -1)
                {
                    double[] xB = MultiplyMatrixVector(Binv, b);
                    double[] xFull = new double[n + m];
                    for (int i = 0; i < m; i++) xFull[basis[i]] = xB[i];
                    double obj = 0.0;
                    for (int j = 0; j < n; j++) obj += problem.ObjectiveCoefficients[j] * xFull[j];

                    // Correct the sign for minimization problems
                    if (!problem.Maximize)
                        obj = -obj;

                    sb.AppendLine("Optimal solution found.");
                    sb.AppendLine(string.Format("Objective = {0}", Math.Round(obj, 6)));
                    for (int j = 0; j < n; j++) sb.AppendLine(string.Format("x{0} = {1}", j + 1, Math.Round(xFull[j], 6)));
                    sb.AppendLine();

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

                    if (allFeasible)
                    {
                        sb.AppendLine("All constraints are satisfied.");
                    }
                    else
                    {
                        sb.AppendLine("WARNING: Solution is not feasible.");
                    }

                    result.OptimalObjectiveValue = obj;
                    result.OutputLog = sb.ToString();
                    return result;
                }

                string enteringVar = (entering < n) ? string.Format("x{0}", entering + 1) : string.Format("s{0}", entering - n + 1);
                sb.AppendLine(string.Format("Entering column: {0}, reduced = {1}", enteringVar, Math.Round(reduced[entering], 6)));

                double[] a_enter = GetColumn(Aaug, entering);
                double[] d = MultiplyMatrixVector(Binv, a_enter);
                sb.AppendLine("Direction d = B^-1 * a_enter:");
                sb.AppendLine(string.Join("\t", d.Select(v => Math.Round(v, 6))));
                sb.AppendLine();

                double[] xBvec = MultiplyMatrixVector(Binv, b);
                double minRatio = double.PositiveInfinity;
                int leaving_idx = -1;
                for (int i = 0; i < m; i++)
                {
                    if (d[i] > tol)
                    {
                        double ratio = xBvec[i] / d[i];
                        if (ratio < minRatio - tol)
                        {
                            minRatio = ratio;
                            leaving_idx = i;
                        }
                    }
                }

                if (leaving_idx == -1)
                {
                    result.IsUnbounded = true;
                    sb.AppendLine("Problem is unbounded (no positive entries in direction).");
                    result.OutputLog = sb.ToString();
                    return result;
                }

                string leavingVar = (basis[leaving_idx] < n) ? string.Format("x{0}", basis[leaving_idx] + 1) : string.Format("s{0}", basis[leaving_idx] - n + 1);
                sb.AppendLine(string.Format("Leaving basis at basis row {0} (variable {1}). Step length = {2}", leaving_idx, leavingVar, Math.Round(minRatio, 6)));

                basis[leaving_idx] = entering;
                if (iter > maxIters)
                {
                    sb.AppendLine("Iteration limit reached");
                    result.OutputLog = sb.ToString();
                    return result;
                }
            }
            sb.AppendLine("Iteration limit reached");
            result.OutputLog = sb.ToString();
            return result;
        }

        static double[,] GetBasisMatrix(double[,] Aaug, int[] basis) { int m = Aaug.GetLength(0); var B = new double[m, m]; for (int c = 0; c < m; c++) for (int r = 0; r < m; r++) B[r, c] = Aaug[r, basis[c]]; return B; }
        static double[] GetColumn(double[,] M, int c) { int r = M.GetLength(0); var col = new double[r]; for (int i = 0; i < r; i++) col[i] = M[i, c]; return col; }
        static double[] MultiplyMatrixVector(double[,] M, double[] v) { int r = M.GetLength(0), c = M.GetLength(1); var res = new double[r]; for (int i = 0; i < r; i++) for (int j = 0; j < c; j++) res[i] += M[i, j] * v[j]; return res; }
        static double[] MultiplyVectorByMatrixTranspose(double[] vec, double[,] M) { int m = M.GetLength(0); var res = new double[m]; for (int j = 0; j < m; j++) for (int i = 0; i < m; i++) res[j] += vec[i] * M[i, j]; return res; }
        static double[,] Invert(double[,] M) { int n = M.GetLength(0); var A = (double[,])M.Clone(); var I = new double[n, n]; for (int i = 0; i < n; i++) I[i, i] = 1.0; for (int i = 0; i < n; i++) { int p = i; for (int j = i + 1; j < n; j++) if (Math.Abs(A[j, i]) > Math.Abs(A[p, i])) p = j; for (int k = 0; k < n; k++) { double t = A[i, k]; A[i, k] = A[p, k]; A[p, k] = t; t = I[i, k]; I[i, k] = I[p, k]; I[p, k] = t; } double div = A[i, i]; for (int k = 0; k < n; k++) { A[i, k] /= div; I[i, k] /= div; } for (int j = 0; j < n; j++) { if (i == j) continue; double mult = A[j, i]; for (int k = 0; k < n; k++) { A[j, k] -= mult * A[i, k]; I[j, k] -= mult * I[i, k]; } } } return I; }
    }
}