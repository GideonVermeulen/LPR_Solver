using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Solver
{
    public static class LPParser
    {
        public static LPProblem Parse(List<string> lines)
        {
            if (lines.Count < 1)
            {
                throw new ArgumentException("Input file is empty.");
            }

            // Parse objective function
            var objectiveParts = TokenizeLine(lines[0]);
            if (objectiveParts.Count < 2)
            {
                throw new ArgumentException("Invalid objective function line. Expected 'max' or 'min' followed by coefficients.");
            }

            bool maximize = objectiveParts[0].ToLower() == "max";
            if (!maximize && objectiveParts[0].ToLower() != "min")
            {
                throw new ArgumentException("Objective function must start with 'max' or 'min'.");
            }

            int numVariables = objectiveParts.Count - 1;
            double[] objectiveCoefficients = new double[numVariables];
            for (int i = 0; i < numVariables; i++)
            {
                if (!double.TryParse(objectiveParts[i + 1], out objectiveCoefficients[i]))
                {
                    throw new ArgumentException($"Invalid coefficient in objective function: {objectiveParts[i + 1]}");
                }
            }

            // Detect variable sign/type line
            string typeLine = null;
            int typeLineIndex = -1;

            // Iterate backwards from the end of the lines (excluding objective) to find the type line
            for (int i = lines.Count - 1; i >= 1; i--)
            {
                string currentLine = lines[i];
                var tokens = TokenizeLine(currentLine);

                bool isTypeLine = false;
                foreach (var token in tokens)
                {
                    string lowerToken = token.ToLower();
                    if (lowerToken == "int" || lowerToken == "bin" || lowerToken == "urs" ||
                        lowerToken == "+" || lowerToken == "-")
                    {
                        isTypeLine = true;
                        break;
                    }
                }

                if (isTypeLine)
                {
                    typeLine = currentLine;
                    typeLineIndex = i;
                    break; // Found the last type line, so break
                }
            }

            List<string> constraintLines = new List<string>();
            if (typeLineIndex != -1)
            {
                // Constraints are all lines between objective and typeLine
                for (int i = 1; i < typeLineIndex; i++)
                {
                    constraintLines.Add(lines[i]);
                }
            }
            else
            {
                // No type line found, all lines after objective are constraints
                constraintLines = lines.Skip(1).ToList();
            }

            int numConstraints = constraintLines.Count;
            double[,] constraints = new double[numConstraints, numVariables];
            double[] rhs = new double[numConstraints];
            LPProblem.ConstraintType[] constraintTypes = new LPProblem.ConstraintType[numConstraints];

            for (int i = 0; i < numConstraints; i++)
            {
                var tokens = TokenizeLine(constraintLines[i]);
                if (tokens.Count != numVariables + 2)
                {
                    throw new ArgumentException($"Invalid constraint on line {i + 2}. Expected {numVariables} coefficients, an operator, and a RHS value.");
                }

                for (int j = 0; j < numVariables; j++)
                {
                    if (!double.TryParse(tokens[j], out constraints[i, j]))
                    {
                        throw new ArgumentException($"Invalid coefficient in constraint on line {i + 2}: {tokens[j]}");
                    }
                }

                string op = tokens[numVariables];
                if (op == "<=")
                {
                    constraintTypes[i] = LPProblem.ConstraintType.LessThanOrEqual;
                }
                else if (op == ">=")
                {
                    constraintTypes[i] = LPProblem.ConstraintType.GreaterThanOrEqual;
                }
                else if (op == "=")
                {
                    constraintTypes[i] = LPProblem.ConstraintType.Equal;
                }
                else
                {
                    throw new ArgumentException($"Invalid operator in constraint on line {i + 2}: {op}");
                }

                if (!double.TryParse(tokens[numVariables + 1], out rhs[i]))
                {
                    throw new ArgumentException($"Invalid RHS value in constraint on line {i + 2}: {tokens[numVariables + 1]}");
                }
            }

            // Initialize VariableTypes and VariableSigns arrays
            var variableTypes = Enumerable.Repeat(LPProblem.VarType.Continuous, numVariables).ToArray();
            var variableSigns = Enumerable.Repeat(LPProblem.VarSign.NonNegative, numVariables).ToArray();

            // Parse variable types and signs if typeLine exists
            bool hasSignRestrictionLine = (typeLine != null); // Set the new flag
            if (typeLine != null)
            {
                var typeTokens = TokenizeLine(typeLine);
                if (typeTokens.Count != numVariables)
                {
                    throw new ArgumentException($"Variable type/sign line has {typeTokens.Count} tokens, but expected {numVariables} (one for each variable).");
                }

                for (int j = 0; j < numVariables; j++)
                {
                    string type = typeTokens[j].ToLower();
                    if (type == "int")
                    {
                        variableTypes[j] = LPProblem.VarType.Integer;
                    }
                    else if (type == "bin")
                    {
                        variableTypes[j] = LPProblem.VarType.Binary;
                    }
                    else if (type == "urs")
                    {
                        variableSigns[j] = LPProblem.VarSign.Unrestricted;
                    }
                    else if (type == "-")
                    {
                        variableSigns[j] = LPProblem.VarSign.NonPositive;
                    }
                    else if (type == "+")
                    {
                        variableSigns[j] = LPProblem.VarSign.NonNegative; // Explicitly set, though it's the default
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown variable type/sign '{typeTokens[j]}' on variable type/sign line. Expected 'int', 'bin', 'urs', '+', or '-'.");
                    }
                }
            }

            return new LPProblem
            {
                ObjectiveCoefficients = objectiveCoefficients,
                Constraints = constraints,
                RHS = rhs,
                ConstraintTypes = constraintTypes,
                Maximize = maximize,
                VariableTypes = variableTypes,
                VariableSigns = variableSigns,
                HasSignRestrictionLine = hasSignRestrictionLine // Set the new property
            };
        }

        private static List<string> TokenizeLine(string line) =>
            line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        private static double ParseNumber(string s) => double.Parse(s);
    }
}
