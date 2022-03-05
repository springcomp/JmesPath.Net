using jmespath.lexer.StateMachines;
using Xunit;

namespace jmespath.lexer.tests;

public class StateMachineTest
{
    [Fact]
    public void StateMachine_Run_T_USTRING()
    {
        var machine = new UnquotedString();

        Assert_T_USTRING(machine.Match("foo"), "foo");
        Assert_T_USTRING(machine.Match("_foo"), "_foo");
        Assert_T_USTRING(machine.Match("_foo_bar"), "_foo_bar");
        Assert_T_USTRING(machine.Match("_foo_bar42"), "_foo_bar42");

        Assert_T_USTRING(machine.Match("_foo_bar42.other"), "_foo_bar42");

        Assert_T_USTRING(machine.Match("42_foo"), null);
    }

    private static void Assert_T_USTRING(Match match, string? expected = null)
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