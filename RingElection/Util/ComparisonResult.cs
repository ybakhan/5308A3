namespace RingElection.Util
{
    class ComparisonResult
    {
        public int NodeCount { get; set; }
        public int AllTheWay_MessageCount { get; set; }
        public int AsFarAs_MessageCount { get; set; }
        public int AsFarAsBi_MessageCount { get; set; }
        public int ContDist_MessageCount { get; set; }
        public int Stages_MessageCount { get; set; }
        public int AltSteps_MessageCount { get; set; }
        public double AllTheWay_Seconds { get; set; }
        public double AsFarAs_Seconds { get; set; }
        public double AsFarAsBi_Seconds { get; set; }
        public double ContDist_Seconds { get; set; }
        public double Stages_Seconds { get; set; }
        public double AltSteps_Seconds { get; set; }
    }

    class TestResult
    {
        public int NodeCount { get; set; }
        public int MessageCount { get; set; }
        public double Seconds { get; set; }
    }
}
