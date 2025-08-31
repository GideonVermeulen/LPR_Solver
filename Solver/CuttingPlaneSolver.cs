using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsApp1.Solver
{
    public class CuttingPlaneSolver : ILPSolver
    {
        public SimplexResult Solve(LPProblem problem)
        {
            var overallLog = new StringBuilder();
            overallLog.AppendLine("=== Cutting Plane Algorithm Started ===");

            LPProblem currentProblem = problem.Clone();
            var solver = new PrimalSimplexSolver();

            int maxCuts = 100; 
            for (int i = 0; i < maxCuts; i++)
            {
                overallLog.AppendLine($"\n--- Node {i + 1} ---");

                SimplexResult relaxationResult = solver.Solve(currentProblem);
                overallLog.Append(relaxationResult.OutputLog);

                if (!relaxationResult.IsOptimal)
                {
                    overallLog.AppendLine("Relaxed LP is not optimal. Stopping.");
                    return new SimplexResult { IsOptimal = false, OutputLog = overallLog.ToString() };
                }

                int fractionalVarIndex = -1;
                double fractionalVarValue = 0;
                for (int j = 0; j < problem.ObjectiveCoefficients.Length; j++)
                {
                    // Assume variable should be integer if not specified as continuous
                    if (problem.VariableTypes == null || j >= problem.VariableTypes.Length || problem.VariableTypes[j] != LPProblem.VarType.Continuous)
                    {
                        double val = relaxationResult.OptimalSolution[j];
                        if (Math.Abs(val - Math.Round(val)) > 1e-6)
                        {
                            fractionalVarIndex = j;
                            fractionalVarValue = val;
                            break;
                        }
                    }
                }

                if (fractionalVarIndex == -1)
                {
                    overallLog.AppendLine("\n=== Optimal Integer Solution Found ===");
                    relaxationResult.OutputLog = overallLog.ToString();
                    return relaxationResult;
                }

                overallLog.AppendLine($"\n--- Adding Cut on Variable x{fractionalVarIndex + 1} ---");
                
                double[] newConstraintCoefficients = new double[problem.ObjectiveCoefficients.Length];
                newConstraintCoefficients[fractionalVarIndex] = 1.0;
                
                double newRhs = Math.Floor(fractionalVarValue);

                overallLog.AppendLine($"Cut Added: x{fractionalVarIndex + 1} <= {newRhs}");

                currentProblem.AddConstraint(newConstraintCoefficients, newRhs, LPProblem.ConstraintType.LessThanOrEqual);
            }

            overallLog.AppendLine("\n=== Max iterations reached. Returning last solution. ===");
            var finalResult = solver.Solve(currentProblem);
            finalResult.OutputLog = overallLog.ToString() + finalResult.OutputLog;
            return finalResult;
        }
    }

    public static class LPProblemExtensions
    {
        public static LPProblem Clone(this LPProblem problemToClone)
        {
            var newProblem = new LPProblem();
            newProblem.ObjectiveCoefficients = (double[])problemToClone.ObjectiveCoefficients.Clone();
            newProblem.Constraints = (double[,])problemToClone.Constraints.Clone();
            newProblem.RHS = (double[])problemToClone.RHS.Clone();
            newProblem.Maximize = problemToClone.Maximize;
            newProblem.ConstraintTypes = (LPProblem.ConstraintType[])problemToClone.ConstraintTypes.Clone();
            if (problemToClone.VariableTypes != null)
                newProblem.VariableTypes = (LPProblem.VarType[])problemToClone.VariableTypes.Clone();
            if (problemToClone.VariableSigns != null)
                newProblem.VariableSigns = (LPProblem.VarSign[])problemToClone.VariableSigns.Clone();
            newProblem.HasSignRestrictionLine = problemToClone.HasSignRestrictionLine;
            return newProblem;
        }

        public static void AddConstraint(this LPProblem problem, double[] coefficients, double rhs, LPProblem.ConstraintType type)
        {
            int numConstraints = problem.Constraints.GetLength(0);
            int numVars = problem.Constraints.GetLength(1);

            double[,] newConstraints = new double[numConstraints + 1, numVars];
            Array.Copy(problem.Constraints, newConstraints, problem.Constraints.Length);
            for (int i = 0; i < numVars; i++)
            {
                newConstraints[numConstraints, i] = coefficients[i];
            }

            double[] newRhs = new double[numConstraints + 1];
            Array.Copy(problem.RHS, newRhs, numConstraints);
            newRhs[numConstraints] = rhs;

            LPProblem.ConstraintType[] newTypes = new LPProblem.ConstraintType[numConstraints + 1];
            Array.Copy(problem.ConstraintTypes, newTypes, numConstraints);
            newTypes[numConstraints] = type;

            problem.Constraints = newConstraints;
            problem.RHS = newRhs;
            problem.ConstraintTypes = newTypes;
        }
    }
}