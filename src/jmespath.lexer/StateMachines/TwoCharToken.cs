namespace jmespath.lexer.StateMachines;

internal sealed class TwoCharToken : OneOrTwoCharToken
{
    /// <summary>
    /// Initialize a new instance of the <see cref="TwoCharToken"/> class
    /// that matches non ambiguous two-char tokens.
    /// Matches:
    ///   T_EQ '=='
    /// </summary>
    /// <param name="toMatch"></param>
    public TwoCharToken(params char[] toMatch)
        : base(new[] {2}, toMatch)
    {
        System.Diagnostics.Debug.Assert(toMatch.Length == 2);
    }
}