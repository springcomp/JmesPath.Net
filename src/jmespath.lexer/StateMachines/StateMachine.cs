namespace jmespath.lexer.StateMachines;

public class StateMachine
{
    public const int __ = -1; // universal error code

    private readonly int initialState_;
    private readonly int[] acceptingStates_;
    protected Func<int, char, int>? nextState_ = null;

    public StateMachine(
        int initialState,
        int acceptingState,
        Func<int, char, int> nextState
    ) : this(
        initialState,
        new[] { acceptingState },
        nextState
    )
    { }

    public StateMachine(
        int initialState,
        int[] acceptingStates,
        Func<int, char, int> nextState
    )
    {
        initialState_ = initialState;
        acceptingStates_ = acceptingStates;
        nextState_ = nextState;
    }

    public Match Match(ReadOnlySpan<Char> input)
    {
        var currentState = initialState_;

        int i = 0;

        for (var length = input.Length; i < length; ++i)
        {
            var character = input[i];
            var nextState = nextState_(currentState, character);

            if (nextState == __)
                break;

            currentState = nextState;
        }

        var succeeded = acceptingStates_.Contains(currentState);

        return new Match(
            succeeded,
            succeeded ? input[..i].ToString() : null
            );
    }

    public Match Match(string input)
        => Match((ReadOnlySpan<Char>)input);

    public bool Run(string input)
        => Match(input).Success;

}

public sealed class Match
{
    public Match(bool success, string? text)
    {
        Success = success;
        Text = text;
    }
    public bool Success { get; private set; }
    public string? Text { get; private set; }

    public string MatchedText
    {
        get
        {
            System.Diagnostics.Debug.Assert(Text != null);
            return Text!;
        }
    }
}
