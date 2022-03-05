namespace jmespath.lexer.StateMachines;
public sealed class UnquotedString : StateMachine
{
    private static readonly int[,] state_translation_table_ = new int[,]
    {
        /*         L   D   U
        /* 0 */ {  1, __,  1 },
        /* 1 */ {  1,  1,  1 },
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

    public UnquotedString()
        : base(0, 1, GetNextState)
    {
    }

    private static int GetNextState(int state, char input)
    {
        var ascii = 32 + (input - ' ');
        var characterClass = classes[ascii];
        if (characterClass == __)
            return __;

        return state_translation_table_[state, characterClass];
    }
}

