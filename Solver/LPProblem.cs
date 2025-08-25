
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Solver
{
    public class LPProblem
    {
        public double[] ObjectiveCoefficients { get; set; }
        public double[,] Constraints { get; set; }
        public double[] RHS { get; set; }
        public bool Maximize { get; set; }

        public enum ConstraintType { LessThanOrEqual, Equal, GreaterThanOrEqual }
        public ConstraintType[] ConstraintTypes { get; set; }

        public enum VarType { Continuous, Integer, Binary }
        public VarType[] VariableTypes { get; set; }

        public enum VarSign { NonNegative, NonPositive, Unrestricted }
        public VarSign[] VariableSigns { get; set; }

        public bool HasSignRestrictionLine { get; set; }
    }

}
