namespace jmespath.lexer.StateMachines;

public sealed class Whitespace : StateMachine
{
    public Whitespace()
        : base(0, 0, GetNextState)
    {
    }

    private static int GetNextState(int state, char input)
        => IsWhitespace(input) ? state: __;

    internal static bool IsWhitespace(char input)
    { 
        var ascii = 32 + (input - ' ');
        return (
              (ascii == 32) /*  ' ' */ ||
              (ascii ==  8) /* '\b' */ ||
              (ascii == 12) /* '\f' */ ||
              (ascii == 13) /* '\r' */ ||
              (ascii == 10) /* '\n' */ ||
              (ascii ==  9) /* '\t' */
            );
    }
}

