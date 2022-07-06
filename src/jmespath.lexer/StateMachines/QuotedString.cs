namespace jmespath.lexer.StateMachines;

public sealed class QuotedString : TransitionTableMachine
{
    private static readonly int[,] state_transition_table_ = new int[,]
    {
        /*         CQ  CN  CE  CC  CU  CD  CF
        /* 0  */ {  1, __, __, __, __, __, __ },
        /* 1  */ { __,  2,  4,  2,  2,  2,  2 },
        /* 2  */ {  3,  2,  4,  2,  2,  2,  2 },
        /* 3+ */ { __, __, __, __, __, __, __ },
        /* 4  */ {  2, __,  2,  2,  5, __,  2 },
        /* 5  */ { __, __, __, __, __,  6,  6 },
        /* 6  */ { __, __, __, __, __,  7,  7 },
        /* 7  */ { __, __, __, __, __,  8,  8 },
        /* 8  */ { __, __, __, __, __,  2,  2 },
    };

    private const int CQ = 0; // "
    private const int CN = 1; // [^"]
    private const int CE = 2; // \\ 
    private const int CC = 3; // [/nrt]
    private const int CU = 4; // u
    private const int CD = 5; // [0-9A-Fac-e]
    private const int CF = 6; // [bf]

    private static readonly int[] classes_ = new int[128] {
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,
           __, __, __, __, __, __, __, __,

           CN, CN, CQ, CN, CN, CN, CN, CN, 
           CN, CN, CN, CN, CN, CN, CN, CC,
           CD, CD, CD, CD, CD, CD, CD, CD,
           CD, CD, CN, CN, CN, CN, CN, CN,  

           CN, CD, CD, CD, CD, CD, CD, CN,
           CN, CN, CN, CN, CN, CN, CN, CN,
           CN, CN, CN, CN, CN, CN, CN, CN,
           CN, CN, CN, CN, CE, CN, CN, CN, 

           CN, CD, CF, CD, CD, CD, CF, CN,
           CN, CN, CN, CN, CN, CN, CC, CN,
           CN, CN, CC, CN, CC, CU, CN, CN,
           CN, CN, CN, CN, CN, CN, CN, CN, 
    };

    public QuotedString()
        : base(0, 3, null!, state_transition_table_)
    {
    }

    protected override int GetCharacterClass(char input)
    {
        var ascii = 32 + (input - ' ');
        if (ascii >= classes_.Length)
            return CN;

        return classes_[ascii];
    }
}

