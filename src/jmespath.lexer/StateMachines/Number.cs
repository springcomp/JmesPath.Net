namespace jmespath.lexer.StateMachines;

public sealed class Number : TransitionTableMachine
{ 
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CM  CD  
        /* 0  */ {  1,  1, },
        /* 1+ */ { __,  1, },
    };

    private const int CM = 0; // \-
    private const int CD = 1; // [0-9]

    private static readonly int[] classes = new int[64] {
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,

           __, __, __, __, __, __, __, __,
           __, __, __, __, __, CM, __, __,
           CD, CD, CD, CD, CD, CD, CD, CD,
           CD, CD, __, __, __, __, __, __,
    };

    /// <summary>
    /// Initialize a new instance of the <see cref="Number"/> class
    /// Matches:
    ///     T_NUMBER -?[0-9]+
    /// </summary>
    public Number()
        : base(0, 1, classes, state_transition_table_)
    {
    }
}

