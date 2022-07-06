namespace jmespath.lexer.StateMachines;
public sealed class UnquotedString : TransitionTableMachine
{
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CL  CD  CU
        /* 0  */ {  1, __,  1 },
        /* 1+ */ {  1,  1,  1 },
    };

    private const int CL = 0; // [A-Za-z]
    private const int CD = 1; // [0-9]
    private const int CU = 2; // _

    private static readonly int[] classes = new int[128] {
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,

           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           CD, CD, CD, CD, CD, CD, CD, CD,
           CD, CD, __, __, __, __, __, __,

           __, CL, CL, CL, CL, CL, CL, CL,
           CL, CL, CL, CL, CL, CL, CL, CL,
           CL, CL, CL, CL, CL, CL, CL, CL,
           CL, CL, CL, __, __, __, __, CU,

           __, CL, CL, CL, CL, CL, CL, CL,
           CL, CL, CL, CL, CL, CL, CL, CL,
           CL, CL, CL, CL, CL, CL, CL, CL,
           CL, CL, CL, __, __, __, __, __,
    };

    /// <summary>
    /// Initialize a new instance of the <see cref="UnquotedString"/> class
    /// Matches:
    ///     T_USTRING [A-Za-z_][A-Za-z0-9_]+
    /// </summary>
    public UnquotedString()
        : base(0, 1, classes, state_transition_table_)
    {
    }
}

