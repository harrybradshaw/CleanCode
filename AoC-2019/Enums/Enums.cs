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
        AwaitingInput,
    }
    
    public enum FailureReason
    {
        CouldNotAccessMemoryAddress,
        AwaitingInputThatIsNotPresent,
    }
}