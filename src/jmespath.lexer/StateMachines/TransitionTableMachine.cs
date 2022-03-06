namespace jmespath.lexer.StateMachines;

public class TransitionTableMachine : StateMachine
{
    private readonly int[] classes_;
    private readonly int[,] state_transition_table_;

    public TransitionTableMachine(
        int initialState,
        int acceptingState,
        int[] classes,
        int[,] transitionTable
    ) : this (
        initialState,
        new int[] { acceptingState },
        classes,
        transitionTable
    )
    {
    }
    public TransitionTableMachine(
        int initialState,
        int[] acceptingStates,
        int[] classes,
        int[,] transitionTable
    ) : base (
        initialState,
        acceptingStates,
        null!
    )
    {
        classes_ = classes;
        state_transition_table_ = transitionTable;

        nextState_ = GetNextState;
    }
    private int GetNextState(int state, char input)
    {
        var ascii = 32 + (input - ' ');
        if (ascii >= classes_.Length)
            return __;

        var characterClass = classes_[ascii];
        if (characterClass == __)
            return __;

        return state_transition_table_[state, characterClass];
    }
}

