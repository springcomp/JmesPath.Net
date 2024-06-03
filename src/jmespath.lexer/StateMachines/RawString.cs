namespace jmespath.lexer.StateMachines;

public sealed class RawString : TransitionTableMachine
{ 
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CQ  CN  CE  
        /* 0  */ {  1, __, __, },
        /* 1  */ {  3,  2,  4, },
        /* 2  */ {  3,  2,  4, },
        /* 3+ */ { __, __, __, },
        /* 4  */ {  2,  2,  2, },
    };

    private const int CQ = 0; // '
    private const int CN = 1; // [^']
    private const int CE = 2; // \\ 

    public RawString()
        : base(0, 3, null!, state_transition_table_)
    {
    }

    protected override int GetCharacterClass(char input)
    {
        if (input == '\'') return CQ;
        if (input == '\\') return CE;

        return CN;
    }
}

