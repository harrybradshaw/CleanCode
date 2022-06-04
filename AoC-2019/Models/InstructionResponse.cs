namespace CleanCode
{
    public class InstructionResponse
    {
        public bool WasInstructionSuccess { get; set; }
        public FailureReason? FailureReason { get; set; }
        public long Value { get; set; }
    }
}