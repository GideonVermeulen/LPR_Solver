namespace WinFormsApp1.Solver
{
    public class SimplexResult
    {
        public double[] OptimalSolution { get; set; }
        public double OptimalObjectiveValue { get; set; }
        public double[,] FinalTableau { get; set; }
        public int[] FinalBasis { get; set; }
        public string OutputLog { get; set; }
        public bool IsOptimal { get; set; }
        public bool IsUnbounded { get; set; }
        public bool IsFeasible { get; set; }
        public double[] ReducedCosts { get; set; }
    }
}