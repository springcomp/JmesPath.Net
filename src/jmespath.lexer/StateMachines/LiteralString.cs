namespace jmespath.lexer.StateMachines;

public sealed class LiteralString : TransitionTableMachine
{
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CT  CN  CB
        /* 0  */ {  1, __, __ },
        /* 1  */ {  3,  1,  2 },
        /* 2  */ {  1, __, __ },
        /* 3+ */ { __, __, __ },
    };

    private const int CT = 0; // `
    private const int CN = 1; // [^`]
    private const int CB = 2; // \\ 

    public LiteralString()
        : base(0, 3, null!, state_transition_table_)
    {
    }

    protected override int GetCharacterClass(char input)
    {
        if (input == '`') return CT;
        if (input == '\\') return CB;
        return CN;
    }
}

