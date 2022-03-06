using jmespath.lexer.StateMachines;
using Xunit;

namespace jmespath.lexer.tests;

public class StateMachineTest
{
    [Fact]
    public void StateMachine_Run_Bracket()
    {
        var machine = new BracketToken();

        AssertMatch(machine.Match("["), "[");
        AssertMatch(machine.Match("[]"), "[]");
        AssertMatch(machine.Match("[?"), "[?");

        AssertMatch(machine.Match("[ ?"), "[");

        AssertMatch(machine.Match("] ?"), null);
    }

    [Fact]
    public void StateMachine_Run_Number()
    {
        var machine = new Number();

        AssertMatch(machine.Match("-0"), "-0");
        AssertMatch(machine.Match("0"), "0");

        AssertMatch(machine.Match("42"), "42");
        AssertMatch(machine.Match("-42"), "-42");
    }

    [Fact]
    public void StateMachine_Run_LiteralString()
    {
        var machine = new LiteralString();

        AssertMatch(machine.Match("`hello`"), "`hello`");
        AssertMatch(machine.Match("`hello`.world"), "`hello`");
        AssertMatch(machine.Match("`hello\\` world!`"), "`hello\\` world!`");

        AssertMatch(machine.Match("foo"), null);
    }
    [Fact]
    public void StateMachine_Run_UnquotedString()
    {
        var machine = new UnquotedString();

        AssertMatch(machine.Match("foo"), "foo");
        AssertMatch(machine.Match("_foo"), "_foo");
        AssertMatch(machine.Match("_foo_bar"), "_foo_bar");
        AssertMatch(machine.Match("_foo_bar42"), "_foo_bar42");

        AssertMatch(machine.Match("_foo_bar42.other"), "_foo_bar42");

        AssertMatch(machine.Match("42_foo"), null);
    }

    private static void AssertMatch(Match match, string? expected = null)
    {
        if (expected == null)
            Assert.False(match.Success);

        else
        {
            Assert.True(match.Success);
            Assert.Equal(expected, match.Text);
        }
    }
}