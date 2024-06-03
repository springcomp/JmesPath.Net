namespace jmespath.lexer.StateMachines;

public sealed class QuotedString : TransitionTableMachine
{
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CQ  CN  CE  CC  CU  CD
        /* 0  */ {  1, __, __, __, __, __ },
        /* 1  */ { __,  2,  4,  2,  2,  2 },
        /* 2  */ {  3,  2,  4,  2,  2,  2 },
        /* 3+ */ { __, __, __, __, __, __ },
        /* 4  */ {  2, __,  2,  2,  5, __ },
        /* 5  */ { __, __, __, __, __,  6 },
        /* 6  */ { __, __, __, __, __,  7 },
        /* 7  */ { __, __, __, __, __,  8 },
        /* 8  */ { __, __, __, __, __,  2 },
    };

    private const int CQ = 0; // "
    private const int CN = 1; // [^"]
    private const int CE = 2; // \\ 
    private const int CC = 3; // [/bfnrt]
    private const int CU = 4; // u
    private const int CD = 5; // [0-9]

    public QuotedString()
        : base(0, 3, null!, state_transition_table_)
    {
    }

    protected override int GetCharacterClass(char input)
    {
        if (input == '\"') return CQ;
        if (input == '\\') return CE;

        if (input == '/') return CC;
        if (input == 'b') return CC;
        if (input == 'f') return CC;
        if (input == 'n') return CC;
        if (input == 'r') return CC;
        if (input == 't') return CC;

        if (input == 'u') return CU;
        if (input >= '0' && input <= '9') return CD;

        return CN;
    }
}

