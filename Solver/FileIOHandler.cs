using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WinFormsApp1.Solver
{
    public static class FileIOHandler
    {
        public static LPProblem ReadFromFile(string path)
        {
            var lines = File.ReadAllLines(path)
                            .Select(l => l.Trim())
                            .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("#"))
                            .ToList();

            if (lines.Count == 0) throw new Exception("Empty input file");

            // Objective
            var objTokens = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool maximize = objTokens[0].ToLower() == "max";
            if (!maximize && objTokens[0].ToLower() != "min")
                throw new Exception("Objective must start with max or min");
            double[] c = objTokens.Skip(1).Select(double.Parse).ToArray();

            // Detect variable type line
            int lastLineIndex = lines.FindLastIndex(l =>
            {
                string low = l.ToLower();
                return low.Contains("int") || low.Contains("bin") || low.Contains("urs") ||
                       low.StartsWith("+") || low.StartsWith("-");
            });

            var constraintLines = lines.Skip(1).Take(Math.Max(0, lastLineIndex - 1)).ToList();
            int m = constraintLines.Count;
            int n = c.Length;
            double[,] A = new double[m, n];
            double[] b = new double[m];

            for (int i = 0; i < m; i++)
            {
                var tokens = constraintLines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n; j++) A[i, j] = double.Parse(tokens[j]);
                if (tokens[n] != "<=") throw new Exception("Only <= supported for now");
                b[i] = double.Parse(tokens[n + 1]);
            }

            // Default all vars continuous
            var types = Enumerable.Repeat(LPProblem.VarType.Continuous, n).ToArray();

            if (lastLineIndex != -1 && lastLineIndex < lines.Count)
            {
                var typeTokens = lines[lastLineIndex].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n && j < typeTokens.Length; j++)
                {
                    string t = typeTokens[j].ToLower();
                    if (t == "int") types[j] = LPProblem.VarType.Integer;
                    else if (t == "bin") types[j] = LPProblem.VarType.Binary;
                    else types[j] = LPProblem.VarType.Continuous;
                }
            }

            return new LPProblem
            {
                ObjectiveCoefficients = c,
                Constraints = A,
                RHS = b,
                Maximize = maximize,
                VariableTypes = types
            };
        }

        public static void WriteToFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }


}
