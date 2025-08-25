using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsApp1.Solver
{
    /// <summary>
    /// Specialized 0/1 Knapsack Branch & Bound Solver
    /// </summary>
    public class KnapsackSolver : ILPSolver
    {
        private class Item
        {
            public int Index;
            public double Weight;
            public double Value;
            public double Ratio => Value / Weight;
        }

        private class Node
        {
            public int Level;
            public double Value;
            public double Weight;
            public double Bound;
            public List<int> Taken = new List<int>();
        }

        public SimplexResult Solve(LPProblem problem)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Branch & Bound Knapsack Solver ===");

            // Initialize SimplexResult
            SimplexResult result = new SimplexResult();
            result.IsOptimal = false;
            result.IsUnbounded = false;
            result.IsFeasible = false; // Assume infeasible until solution found

            // Expecting one knapsack constraint: sum(weights * x) <= capacity
            if (problem.Constraints.GetLength(0) != 1)
                throw new Exception("Knapsack solver expects exactly one capacity constraint.");

            int n = problem.ObjectiveCoefficients.Length;
            double[] c = problem.ObjectiveCoefficients;
            double[] w = new double[n];
            for (int j = 0; j < n; j++)
                w[j] = problem.Constraints[0, j];
            double capacity = problem.RHS[0];

            // Build items
            var items = new List<Item>();
            for (int j = 0; j < n; j++)
                items.Add(new Item { Index = j + 1, Weight = w[j], Value = c[j] });

            // Sort by ratio
            items = items.OrderByDescending(it => it.Ratio).ToList();

            double bestValue = 0;
            List<int> bestItems = new List<int>();

            var stack = new Stack<Node>();
            var root = new Node { Level = -1, Value = 0, Weight = 0, Bound = GetBound(-1, 0, 0, items, capacity), Taken = new List<int>() };
            stack.Push(root);

            int explored = 0;
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                explored++;
                sb.AppendLine($"-- Node {explored} at level {node.Level}, Value={node.Value}, Weight={node.Weight}, Bound={node.Bound}");

                if (node.Bound <= bestValue || node.Level == items.Count - 1)
                    continue;

                int next = node.Level + 1;
                var item = items[next];

                // Branch: include item
                var with = new Node
                {
                    Level = next,
                    Value = node.Value + item.Value,
                    Weight = node.Weight + item.Weight,
                    Taken = new List<int>(node.Taken)
                };
                with.Taken.Add(item.Index);

                if (with.Weight <= capacity)
                {
                    if (with.Value > bestValue)
                    {
                        bestValue = with.Value;
                        bestItems = new List<int>(with.Taken);
                    }
                    with.Bound = GetBound(with.Level, with.Value, with.Weight, items, capacity);
                    if (with.Bound > bestValue) stack.Push(with);
                }

                // Branch: skip item
                var without = new Node
                {
                    Level = next,
                    Value = node.Value,
                    Weight = node.Weight,
                    Taken = new List<int>(node.Taken)
                };
                without.Bound = GetBound(without.Level, without.Value, without.Weight, items, capacity);
                if (without.Bound > bestValue) stack.Push(without);
            }

            sb.AppendLine();
            sb.AppendLine("*** Best Solution Found ***");
            sb.AppendLine("Objective = " + bestValue);
            sb.AppendLine("Items taken: " + string.Join(", ", bestItems));
            sb.AppendLine($"Nodes explored: {explored}");

            result.OptimalSolution = new double[n];
            foreach (int itemIndex in bestItems)
            {
                result.OptimalSolution[itemIndex - 1] = 1.0; // Assuming 0/1 knapsack, 1 if taken
            }
            result.OptimalObjectiveValue = bestValue;
            result.IsOptimal = true;
            result.IsFeasible = true; // If a solution is found, it's feasible

            result.OutputLog = sb.ToString();
            return result;
        }

        private double GetBound(int level, double value, double weight, List<Item> items, double capacity)
        {
            if (weight >= capacity) return 0;

            double bound = value;
            double totalWeight = weight;

            for (int i = level + 1; i < items.Count; i++)
            {
                if (totalWeight + items[i].Weight <= capacity)
                {
                    totalWeight += items[i].Weight;
                    bound += items[i].Value;
                }
                else
                {
                    double remain = capacity - totalWeight;
                    bound += remain * items[i].Ratio;
                    break;
                }
            }
            return bound;
        }
    }
}
