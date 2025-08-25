using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsApp1.Solver
{
    public class BranchAndBound : ILPSolver
    {
        private readonly double _tol;
        private readonly int _maxNodes;

        public BranchAndBound(double tolerance = 1e-7, int maxNodes = 10000)
        {
            _tol = tolerance;
            _maxNodes = maxNodes;
        }

        private class Node
        {
            public double[] L;   // lower bounds per variable (length n)
            public double[] U;   // upper bounds per variable (length n), use +inf for none
            public int Depth;
            public override string ToString()
            {
                string lb = string.Join(", ", L.Select(v => double.IsNegativeInfinity(v) ? "-inf" : v.ToString()));
                string ub = string.Join(", ", U.Select(v => double.IsPositiveInfinity(v) ? "+inf" : v.ToString()));
                return string.Format("Depth={0} | L=[{1}] | U=[{2}]", Depth, lb, ub);
            }
        }

        private enum LPStatus { Optimal, Infeasible, Unbounded }

        private struct LPResult
        {
            public LPStatus Status;
            public double Objective;
            public double[] X; // length n (+ slacks ignored by caller)
        }

        public SimplexResult Solve(LPProblem problem)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Branch & Bound (Simplex-based) ===");

            // Initialize SimplexResult
            SimplexResult result = new SimplexResult();
            result.IsOptimal = false;
            result.IsUnbounded = false; // Branch and Bound typically doesn't result in unbounded, but LP relaxations can be
            result.IsFeasible = false; // Assume infeasible until integer solution found

            int n = problem.ObjectiveCoefficients.Length;
            int m = problem.Constraints.GetLength(0);

            // clone inputs; convert to maximization if needed
            double[] cOrig = (double[])problem.ObjectiveCoefficients.Clone();
            bool maximize = problem.Maximize;
            if (!maximize) for (int j = 0; j < n; j++) cOrig[j] = -cOrig[j];

            double[,] A = (double[,])problem.Constraints.Clone();
            double[] b = (double[])problem.RHS.Clone();

            // initial bounds: 0 <= x < +inf
            var root = new Node
            {
                L = Enumerable.Repeat(0.0, n).ToArray(),
                U = Enumerable.Repeat(double.PositiveInfinity, n).ToArray(),
                Depth = 0
            };

            double bestObj = double.NegativeInfinity;
            double[] bestX = null;

            var stack = new Stack<Node>();
            stack.Push(root);

            int explored = 0;
            while (stack.Count > 0 && explored < _maxNodes)
            {
                var node = stack.Pop();
                explored++;
                sb.AppendLine(string.Format("-- Node {0} :: {1}", explored, node));

                // Solve LP relaxation at this node
                var lp = SolveRelaxation(cOrig, A, b, node.L, node.U, _tol);

                if (lp.Status == LPStatus.Infeasible)
                {
                    sb.AppendLine("  -> Infeasible. Prune.");
                    continue;
                }
                if (lp.Status == LPStatus.Unbounded)
                {
                    sb.AppendLine("  -> Unbounded relaxation. Prune (no better bounded integer solution). ");
                    continue;
                }

                double lpObj = lp.Objective;
                double[] x = lp.X; // length n
                sb.AppendLine(string.Format("  LP value = {0} | x = [" + string.Join(", ", x.Select(Round)) + "]", Round(lpObj)));

                // Bound: if LP is worse than incumbent, prune
                if (lpObj <= bestObj + _tol)
                {
                    sb.AppendLine("  -> Bound not improving incumbent. Prune.");
                    continue;
                }

                // Check integrality
                int fracIndex = -1;
                double maxFrac = 0.0;
                for (int j = 0; j < n; j++)
                {
                    double frac = Math.Abs(x[j] - Math.Round(x[j]));
                    if (frac > _tol && frac > maxFrac)
                    {
                        maxFrac = frac; fracIndex = j;
                    }
                }

                if (fracIndex == -1)
                {
                    // Integer feasible
                    bestObj = lpObj;
                    bestX = (double[])x.Clone();
                    sb.AppendLine("  -> Integer feasible. Update incumbent.");
                    continue;
                }

                // Branch on fracIndex
                double val = x[fracIndex];
                double floorVal = Math.Floor(val);
                double ceilVal = Math.Ceiling(val);

                sb.AppendLine(string.Format("  Branch on x{0} = {1} => x{2} <= {3}  OR  x{4} >= {5}", fracIndex + 1, Round(val), fracIndex + 1, floorVal, fracIndex + 1, ceilVal));

                // Child 1: x_k <= floorVal  (tighten upper bound)
                if (floorVal >= 0) // keep feasibility with nonnegativity
                {
                    var child1 = new Node { L = (double[])node.L.Clone(), U = (double[])node.U.Clone(), Depth = node.Depth + 1 };
                    child1.U[fracIndex] = Math.Min(child1.U[fracIndex], floorVal);
                    stack.Push(child1);
                }

                // Child 2: x_k >= ceilVal (tighten lower bound)
                var child2 = new Node { L = (double[])node.L.Clone(), U = (double[])node.U.Clone(), Depth = node.Depth + 1 };
                child2.L[fracIndex] = Math.Max(child2.L[fracIndex], ceilVal);
                stack.Push(child2);
            }

            sb.AppendLine();
            if (bestX == null)
            {
                sb.AppendLine("*** No integer-feasible solution found (within node/search limits). ***");
                result.IsOptimal = false; // No optimal integer solution found
                result.IsFeasible = false; // No feasible integer solution found
            }
            else
            {
                double reportObj = bestObj;
                if (!maximize) reportObj = -reportObj; // back to user sense
                sb.AppendLine("*** Best integer solution ***");
                sb.AppendLine("Objective = " + Round(reportObj));
                for (int j = 0; j < n; j++) sb.AppendLine(string.Format("x{0} = {1}", j + 1, Round(bestX[j])));

                result.OptimalSolution = bestX;
                result.OptimalObjectiveValue = reportObj;
                result.IsOptimal = true;

                // Feasibility Check
                sb.AppendLine("\n--- Feasibility Check ---");
                bool allFeasible = true;
                for (int i = 0; i < m; i++)
                {
                    double lhs = 0;
                    for (int j = 0; j < n; j++)
                    {
                        lhs += A[i, j] * bestX[j];
                    }

                    bool constraintSatisfied = false;
                    switch (problem.ConstraintTypes[i])
                    {
                        case LPProblem.ConstraintType.LessThanOrEqual:
                            constraintSatisfied = lhs <= b[i] + _tol;
                            break;
                        case LPProblem.ConstraintType.Equal:
                            constraintSatisfied = Math.Abs(lhs - b[i]) <= _tol;
                            break;
                        case LPProblem.ConstraintType.GreaterThanOrEqual:
                            constraintSatisfied = lhs >= b[i] - _tol;
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
            }

            sb.AppendLine(string.Format("Nodes explored: {0}", explored));
            result.OutputLog = sb.ToString();
            return result;
        }

        // ---------------- LP Relaxation with bounds via variable shifting ----------------
        private LPResult SolveRelaxation(double[] c, double[,] A, double[] b, double[] L, double[] U, double tol)
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
                if (bShift[i] < -1e-10) // lower bounds impossible
                    return new LPResult { Status = LPStatus.Infeasible };
            }

            // Build A' and b' adding upper bounds y_j <= U_j - L_j where U finite
            // Count extra rows
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
                if (rhs < -1e-12) // infeasible bounds
                    return new LPResult { Status = LPStatus.Infeasible };
                b2[m + r] = rhs;
            }

            // Solve max c^T x = c^T (y + L) = c^T y + c^T L
            // So objective on y is c, and we add constant shift c^T L afterwards.
            var lp = RevisedSimplexNumericMax(A2, b2, c, tol);
            if (lp.Status != LPStatus.Optimal) return lp;

            // Map back: x = y + L
            var x = new double[n];
            for (int j = 0; j < n; j++) x[j] = lp.X[j] + L[j];

            return new LPResult { Status = LPStatus.Optimal, Objective = lp.Objective + Dot(c, L), X = x };
        }

        // ---------------- Lightweight Revised Simplex that returns numeric solution ----------------
        private LPResult RevisedSimplexNumericMax(double[,] A, double[] b, double[] c, double tol)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            // Feasibility pre-check
            for (int i = 0; i < m; i++) if (b[i] < -1e-12) return new LPResult { Status = LPStatus.Infeasible };

            // Build [A|I] and costs [c, 0]
            int N = n + m;
            double[,] Afull = new double[m, N];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) Afull[i, j] = A[i, j];
                Afull[i, n + i] = 1.0; // slack
            }
            double[] cFull = new double[N];
            for (int j = 0; j < n; j++) cFull[j] = c[j];
            // slacks 0

            int[] basis = Enumerable.Range(n, m).ToArray();
            int iters = 0, maxIters = 2000;

            while (true)
            {
                iters++;
                // Build B and Binv
                double[,] B = ExtractColumns(Afull, basis);
                double[,] Binv = Inverse(B);
                // y^T = c_B^T Binv
                double[] cB = basis.Select(idx => cFull[idx]).ToArray();
                double[] yT = MultiplyVectorTranspose(cB, Binv);

                // Reduced costs r_j = c_j - y^T a_j
                int total = N;
                double[] reduced = new double[total];
                for (int j = 0; j < total; j++)
                {
                    if (basis.Contains(j)) { reduced[j] = 0; continue; }
                    double[] aj = GetColumn(Afull, j);
                    double yAj = 0.0; for (int i = 0; i < m; i++) yAj += yT[i] * aj[i];
                    reduced[j] = cFull[j] - yAj;
                }

                int entering = -1; double best = 0.0;
                for (int j = 0; j < total; j++) if (!basis.Contains(j) && reduced[j] > best + tol) { best = reduced[j]; entering = j; }
                if (entering == -1)
                {
                    // Optimal: x_B = Binv b
                    double[] xB = MultiplyMatrixVector(Binv, b);
                    // check feasibility
                    for (int i = 0; i < m; i++) if (xB[i] < -1e-9) return new LPResult { Status = LPStatus.Infeasible };
                    double[] x = new double[n];
                    for (int i = 0; i < m; i++) if (basis[i] < n) x[basis[i]] = xB[i];
                    double obj = 0.0; for (int j = 0; j < n; j++) obj += c[j] * x[j];
                    return new LPResult { Status = LPStatus.Optimal, Objective = obj, X = x };
                }

                // Direction p = Binv * a_enter
                double[] aent = GetColumn(Afull, entering);
                double[] p = MultiplyMatrixVector(Binv, aent);

                // Ratio test on x_B / p_i
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

                basis[leavePos] = entering;
                if (iters > maxIters) return new LPResult { Status = LPStatus.Unbounded };
            }
        }

        // ---------------- Linear algebra helpers (same as in Revised) ----------------
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
        private static string Round(double v) => v.ToString("0.######");
    }
}