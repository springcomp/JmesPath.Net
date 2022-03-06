namespace jmespath.lexer.StateMachines;

public sealed class BracketToken : TransitionTableMachine
{ 
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CL  CR  CQ
        /* 0  */ {  1, __, __ },
        /* 1+ */ { __,  2,  2 },
        /* 2+ */ { __, __, __ },
    };

    private const int CL = 0; // [
    private const int CR = 1; // ]
    private const int CQ = 2; // ?

    private static readonly int[] classes = new int[128] {
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,

           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, CQ,

           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, CL, __, CR, __, __,

           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
    };

    /// <summary>
    /// Initialize a new instance of the <see cref="BracketToken"/> class
    /// that matches tokens that start with a '['.
    /// Matches:
    ///     T_LBRACKET '['
    ///     T_FLATTEN '[]'
    ///     T_FILTER '[?'
    /// </summary>
    public BracketToken()
        : base(0, new[] {1, 2}, classes, state_transition_table_)
    {
    }
}

