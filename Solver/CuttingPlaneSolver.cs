using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace WinFormsApp1.Solver
{
    public class CuttingPlaneSolver : ILPSolver
    {
        private readonly double tol;
        public CuttingPlaneSolver(double tolerance = 1e-7)
        {
            tol = tolerance;
        }

        public SimplexResult Solve(LPProblem problem)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Cutting Plane Algorithm ===");

            // Initialize SimplexResult
            SimplexResult result = new SimplexResult();
            result.IsOptimal = false;
            result.IsUnbounded = false;
            result.IsFeasible = false; // Assume infeasible until integer solution found

            int n = problem.ObjectiveCoefficients.Length;
            int m = problem.Constraints.GetLength(0);

            // bounds
            double[] L = new double[n];
            double[] U = new double[n];
            for (int j = 0; j < n; j++)
            {
                if (problem.VariableTypes[j] == LPProblem.VarType.Binary)
                {
                    L[j] = 0; U[j] = 1;
                }
                else
                {
                    L[j] = 0; U[j] = double.PositiveInfinity;
                }
            }

            double[,] A = (double[,])problem.Constraints.Clone();
            double[] b = (double[])problem.RHS.Clone();
            double[] c = (double[])problem.ObjectiveCoefficients.Clone();
            bool maximize = problem.Maximize;

            if (!maximize)
                for (int j = 0; j < n; j++) c[j] = -c[j];

            int cutCount = 0;
            while (true)
            {
                var lp = SolveRelaxation(A, b, c, L, U, sb);
                if (lp.Status != LPStatus.Optimal)
                {
                    sb.AppendLine("No feasible LP relaxation.");
                    result.IsFeasible = false;
                    result.OutputLog = sb.ToString();
                    return result;
                }

                sb.AppendLine(string.Format("Iteration {0}, LP obj = {1}, x = [{2}]", cutCount, lp.Objective, string.Join(", ", lp.X.Select(v => v.ToString("0.###")))));

                // find fractional variable
                int fracIndex = -1;
                for (int j = 0; j < n; j++)
                {
                    if (problem.VariableTypes[j] != LPProblem.VarType.Continuous)
                    {
                        double frac = lp.X[j] - Math.Floor(lp.X[j]);
                        if (frac > tol && frac < 1 - tol)
                        {
                            fracIndex = j;
                            break;
                        }
                    }
                }

                if (fracIndex == -1)
                {
                    // All integer (within tolerance)
                    sb.AppendLine("Optimal integer solution found!");

                    // Round solution to nearest integers
                    double[] integerSolution = new double[n];
                    for (int j = 0; j < n; j++)
                    {
                        integerSolution[j] = Math.Round(lp.X[j]);
                    }

                    // Recalculate objective with integer solution
                    double finalObj = 0;
                    for (int j = 0; j < n; j++)
                    {
                        finalObj += problem.ObjectiveCoefficients[j] * integerSolution[j];
                    }

                    sb.AppendLine(string.Format("Objective = {0}", finalObj));
                    for (int j = 0; j < n; j++)
                    {
                        sb.AppendLine(string.Format("x{0} = {1}", j + 1, integerSolution[j]));
                    }

                    result.OptimalSolution = integerSolution;
                    result.OptimalObjectiveValue = finalObj;
                    result.IsOptimal = true;

                    // Feasibility Check with integer solution
                    sb.AppendLine("\n--- Feasibility Check ---");
                    bool allFeasible = true;
                    for (int i = 0; i < m; i++)
                    {
                        double lhs = 0;
                        for (int j = 0; j < n; j++)
                        {
                            lhs += problem.Constraints[i, j] * integerSolution[j];
                        }

                        bool constraintSatisfied = false;
                        switch (problem.ConstraintTypes[i])
                        {
                            case LPProblem.ConstraintType.LessThanOrEqual:
                                constraintSatisfied = lhs <= problem.RHS[i] + tol;
                                break;
                            case LPProblem.ConstraintType.Equal:
                                constraintSatisfied = Math.Abs(lhs - problem.RHS[i]) <= tol;
                                break;
                            case LPProblem.ConstraintType.GreaterThanOrEqual:
                                constraintSatisfied = lhs >= problem.RHS[i] - tol;
                                break;
                        }

                        if (!constraintSatisfied)
                        {
                            sb.AppendLine(string.Format("Constraint {0} is VIOLATED: {1} {2} {3}", i + 1, lhs, problem.ConstraintTypes[i], problem.RHS[i]));
                            allFeasible = false;
                        }
                        else
                        {
                            sb.AppendLine(string.Format("Constraint {0} is satisfied: {1} {2} {3}", i + 1, lhs, problem.ConstraintTypes[i], problem.RHS[i]));
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

                // Add Gomory fractional cut
                cutCount++;
                double fracVal = lp.X[fracIndex];
                sb.AppendLine(string.Format(" -> Cutting plane on x{0} = {1:0.###}", fracIndex + 1, fracVal));

                // new row: x_fracIndex <= floor(value)
                double[,] A2 = new double[A.GetLength(0) + 1, A.GetLength(1)];
                double[] b2 = new double[b.Length + 1];
                Array.Copy(A, A2, A.Length);
                Array.Copy(b, b2, b.Length);

                for (int j = 0; j < n; j++) A2[m, j] = (j == fracIndex ? 1.0 : 0.0);
                b2[m] = Math.Floor(fracVal);
                A = A2; b = b2; m++;
            }

            result.OutputLog = sb.ToString();
            return result;
        }

        // status enum
        private enum LPStatus { Optimal, Infeasible, Unbounded }
        private class LPResult { public LPStatus Status; public double Objective; public double[] X; }

        // --- LP Relaxation with bounds via variable shifting ---
        private LPResult SolveRelaxation(double[,] A, double[] b, double[] c, double[] L, double[] U, StringBuilder sb)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            // Shift variables: y = x - L  => y >= 0
            // A x <= b  =>  A(y + L) <= b  =>  A y <= b - A L
            double[] bShift = (double[])b.Clone();
            for (int i = 0; i < m; i++)
            {
                double sum = 0.0; for (int j = 0; j < n; j++) sum += A[i, j] * L[j];
                bShift[i] = b[i] - sum;
                if (bShift[i] < -1e-10)
                    return new LPResult { Status = LPStatus.Infeasible };
            }

            // Build A' and b' adding upper bounds y_j <= U_j - L_j where U finite
            var ubRows = new List<int>();
            for (int j = 0; j < n; j++) if (!double.IsPositiveInfinity(U[j])) ubRows.Add(j);
            int m2 = m + ubRows.Count;
            double[,] A2 = new double[m2, n];
            double[] b2 = new double[m2];

            // Copy Ay <= bShift
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) A2[i, j] = A[i, j];
                b2[i] = bShift[i];
            }
            // Add y_j <= U_j - L_j rows
            for (int r = 0; r < ubRows.Count; r++)
            {
                int j = ubRows[r];
                for (int k = 0; k < n; k++) A2[m + r, k] = 0.0;
                A2[m + r, j] = 1.0;
                double rhs = U[j] - L[j];
                if (rhs < -1e-12)
                    return new LPResult { Status = LPStatus.Infeasible };
                b2[m + r] = rhs;
            }

            // Solve max c^T x = c^T (y + L) = c^T y + c^T L
            var lp = RevisedSimplexNumericMax(A2, b2, c, tol, sb);
            if (lp.Status != LPStatus.Optimal) return lp;

            // Map back: x = y + L
            var x = new double[n];
            for (int j = 0; j < n; j++) x[j] = lp.X[j] + L[j];

            return new LPResult { Status = LPStatus.Optimal, Objective = lp.Objective + Dot(c, L), X = x };
        }

        // --- Lightweight Revised Simplex that returns numeric solution ---
        private LPResult RevisedSimplexNumericMax(double[,] A, double[] b, double[] c, double tol, StringBuilder sb)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            for (int i = 0; i < m; i++) if (b[i] < -1e-12) return new LPResult { Status = LPStatus.Infeasible };

            int N = n + m;
            double[,] Afull = new double[m, N];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) Afull[i, j] = A[i, j];
                Afull[i, n + i] = 1.0;
            }
            double[] cFull = new double[N];
            for (int j = 0; j < n; j++) cFull[j] = c[j];

            int[] basis = Enumerable.Range(n, m).ToArray();
            int iters = 0, maxIters = 2000;

            sb.AppendLine("  Solving LP relaxation with Revised Simplex:");

            while (true)
            {
                iters++;

                // Build B and Binv
                double[,] B = ExtractColumns(Afull, basis);
                double[,] Binv = Inverse(B);

                // Tableau Body
                double[,] T_body = new double[m, N];
                for(int i = 0; i < m; i++) {
                    for(int j = 0; j < N; j++) {
                        for(int k = 0; k < m; k++) {
                            T_body[i,j] += Binv[i,k] * Afull[k,j];
                        }
                    }
                }

                // RHS
                double[] rhs = MultiplyMatrixVector(Binv, b);

                // Reduced costs
                double[] cB = basis.Select(idx => cFull[idx]).ToArray();
                double[] yT = MultiplyVectorTranspose(cB, Binv);
                double[] reduced = new double[N];
                for (int j = 0; j < N; j++)
                {
                    if (basis.Contains(j)) { reduced[j] = 0; continue; }
                    double[] aj = GetColumn(Afull, j);
                    double yAj = 0.0; for (int i = 0; i < m; i++) yAj += yT[i] * aj[i];
                    reduced[j] = cFull[j] - yAj;
                }

                // Print Tableau
                sb.AppendLine(string.Format("    Simplex Iteration {0}", iters));
                var headerItems = new List<string>();
                headerItems.Add("Basis");
                for (int j = 0; j < n; j++) headerItems.Add(string.Format("x{0}", j + 1));
                for (int j = 0; j < m; j++) headerItems.Add(string.Format("s{0}", j + 1));
                headerItems.Add("RHS");
                sb.AppendLine("      " + string.Join(" | ", headerItems));

                for (int i = 0; i < m; i++)
                {
                    var rowItems = new List<string>();
                    rowItems.Add((basis[i] < n) ? string.Format("x{0}", basis[i] + 1) : string.Format("s{0}", basis[i] - n + 1));
                    for (int j = 0; j < N; j++)
                    {
                        rowItems.Add(T_body[i, j].ToString("0.###"));
                    }
                    rowItems.Add(rhs[i].ToString("0.###"));
                    sb.AppendLine("      " + string.Join(" | ", rowItems));
                }

                var reducedCostRowItems = new List<string>();
                reducedCostRowItems.Add("Cj-Zj");
                for (int j = 0; j < N; j++)
                {
                    reducedCostRowItems.Add(reduced[j].ToString("0.###"));
                }
                reducedCostRowItems.Add(""); // Empty for RHS
                sb.AppendLine("      " + string.Join(" | ", reducedCostRowItems));


                int entering = -1; double best = 0.0;
                for (int j = 0; j < N; j++) if (!basis.Contains(j) && reduced[j] > best + tol) { best = reduced[j]; entering = j; }
                if (entering == -1)
                {
                    // Optimal: x_B = Binv b
                    double[] xB = MultiplyMatrixVector(Binv, b);
                    for (int i = 0; i < m; i++) if (xB[i] < -1e-9) return new LPResult { Status = LPStatus.Infeasible };
                    double[] x = new double[n];
                    for (int i = 0; i < m; i++) if (basis[i] < n) x[basis[i]] = xB[i];
                    double obj = 0.0; for (int j = 0; j < n; j++) obj += c[j] * x[j];
                    sb.AppendLine("    Optimal point for relaxation found.");
                    return new LPResult { Status = LPStatus.Optimal, Objective = obj, X = x };
                }

                double[] aent = GetColumn(Afull, entering);
                double[] p = MultiplyMatrixVector(Binv, aent);

                double[] xBcur = MultiplyMatrixVector(Binv, b);
                double minRatio = double.PositiveInfinity; int leavePos = -1;
                for (int i = 0; i < m; i++) if (p[i] > tol)
                    {
                        double ratio = xBcur[i] / p[i];
                        if (ratio < minRatio - 1e-12) { minRatio = ratio; leavePos = i; }
                    }
                if (leavePos == -1)
                {
                    return new LPResult { Status = LPStatus.Unbounded };
                }
                
                sb.AppendLine(string.Format("      Entering: {0}, Leaving: {1}", entering, basis[leavePos]));

                basis[leavePos] = entering;
                if (iters > maxIters) return new LPResult { Status = LPStatus.Unbounded };
            }
        }

        // --- Linear algebra helpers ---
        private static double[,] ExtractColumns(double[,] M, int[] cols)
        {
            int m = M.GetLength(0);
            int k = cols.Length;
            var R = new double[m, k];
            for (int j = 0; j < k; j++)
                for (int i = 0; i < m; i++) R[i, j] = M[i, cols[j]];
            return R;
        }
        private static double[] GetColumn(double[,] M, int j)
        {
            int m = M.GetLength(0);
            var v = new double[m];
            for (int i = 0; i < m; i++) v[i] = M[i, j];
            return v;
        }
        private static double[] MultiplyMatrixVector(double[,] M, double[] v)
        {
            int m = M.GetLength(0), n = M.GetLength(1);
            var r = new double[m];
            for (int i = 0; i < m; i++)
            {
                double s = 0.0; for (int j = 0; j < n; j++) s += M[i, j] * v[j];
                r[i] = s;
            }
            return r;
        }
        private static double[] MultiplyVectorTranspose(double[] v, double[,] M)
        {
            int m = M.GetLength(0);
            var r = new double[m];
            for (int j = 0; j < m; j++)
            {
                double s = 0.0; for (int i = 0; i < m; i++) s += v[i] * M[i, j];
                r[j] = s;
            }
            return r;
        }
        private static double[,] Inverse(double[,] A)
        {
            int n = A.GetLength(0);
            if (n != A.GetLength(1)) throw new Exception("Only square matrices can be inverted");
            double[,] M = new double[n, 2 * n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) M[i, j] = A[i, j];
                for (int j = 0; j < n; j++) M[i, n + j] = (i == j) ? 1.0 : 0.0;
            }
            for (int col = 0; col < n; col++)
            {
                int piv = col; double best = Math.Abs(M[piv, col]);
                for (int r = col + 1; r < n; r++) { double v = Math.Abs(M[r, col]); if (v > best) { best = v; piv = r; } }
                if (Math.Abs(M[piv, col]) < 1e-12) throw new Exception("Matrix singular");
                if (piv != col) for (int j = 0; j < 2 * n; j++) { double t = M[col, j]; M[col, j] = M[piv, j]; M[piv, j] = t; }
                double p = M[col, col]; for (int j = 0; j < 2 * n; j++) M[col, j] /= p;
                for (int r = 0; r < n; r++) if (r != col)
                    {
                        double f = M[r, col]; if (Math.Abs(f) < 1e-15) continue;
                        for (int j = 0; j < 2 * n; j++) M[r, j] -= f * M[col, j];
                    }
            }
            var Inv = new double[n, n];
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) Inv[i, j] = M[i, n + j];
            return Inv;
        }
        private static double Dot(double[] a, double[] b)
        {
            double s = 0.0; for (int i = 0; i < a.Length; i++) s += a[i] * b[i]; return s;
        }
    }
}