using jmespath.lexer.StateMachines;
using Xunit;

namespace jmespath.lexer.tests;

public class StateMachineTest
{
    [Fact]
    public void StateMachine_Run_Whitespace()
    {
        var machine = new Whitespace();

        AssertMatch(machine.Match(" "), " ");
        AssertMatch(machine.Match("\b"), "\b");
        AssertMatch(machine.Match("\f"), "\f");
        AssertMatch(machine.Match("\n"), "\n");
        AssertMatch(machine.Match("\r"), "\r");
        AssertMatch(machine.Match("\t"), "\t");
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