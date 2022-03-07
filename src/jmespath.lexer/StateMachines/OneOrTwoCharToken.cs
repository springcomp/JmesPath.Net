namespace jmespath.lexer.StateMachines;

public class OneOrTwoCharToken : StateMachine
{
    private readonly char[] toMatch_;
    private int index_ = 0;

    /// <summary>
    /// Initialize a new instance of the <see cref="OneOrTwoCharToken"/> class
    /// that matches ambiguous one- or two-char tokens.
    /// Matches:
    ///   T_ETYPE '&' or T_AND '&&'
    ///   T_PIPE '|' or T_OR '||'
    ///   T_LT '<' or T_LE '<='
    ///   T_GT '>' or T_GE '>='
    ///   T_NOT '!' or T_NE '!='
    /// </summary>
    /// <param name="toMatch"></param>
    public OneOrTwoCharToken(params char[] toMatch)
        : this(new[] { 1, 2, }, toMatch)
    { }

    protected OneOrTwoCharToken(int[] acceptingStates, params char[] toMatch)
        : base(0, acceptingStates, null!)
    {
        nextState_ = GetNextState;
        toMatch_ = toMatch;
    }

    public OneOrTwoCharToken(char c)
        : this(new char[] { c, c, })
    {
    }

    /// <summary>
    /// Helper methods that returns alternative token types
    /// for any given current input character.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static TokenType[] GetCandidateTokenTypes(char c)
    {
        if (c == '&') return new[] { TokenType.T_ETYPE, TokenType.T_AND, };
        if (c == '|') return new[] { TokenType.T_PIPE, TokenType.T_OR, };
        if (c == '<') return new[] { TokenType.T_LT, TokenType.T_LE, };
        if (c == '>') return new[] { TokenType.T_GT, TokenType.T_GE, };
        if (c == '!') return new[] { TokenType.T_NOT, TokenType.T_NE, };

        return new TokenType[] { };
    }

    private int GetNextState(int state, char input)
    {
        switch (state)
        {
            case 0:
            case 1:
                return (input == toMatch_[index_++])
                    ? ++state
                        : __
                        ;
            default:
                return __;
        }
    }
}

