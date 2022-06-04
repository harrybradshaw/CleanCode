namespace CleanCode
{
    public enum MultiAmpMode
    {
        Single,
        Feedback,
    }
    
    public enum RefValue
    {
        Reference,
        Value,
    }

    public enum ParameterMode
    {
        Position,
        Intermediate,
        RelativeMode,
    }
    public enum IntCodeStates
    {
        Running,
        Halted,
        Initialised,
        Paused,
    }
    
    public enum FailureReason
    {
        CouldNotAccessMemoryAddress,
    }
}