using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace jmespath.lexer.tests;

public class MiscellaneousTests
{
    [Fact]
    public void IsWhitespace()
    {
        Assert.True(Scanner.IsWhitespace(' '));
        Assert.True(Scanner.IsWhitespace('\b'));
        Assert.True(Scanner.IsWhitespace('\f'));
        Assert.True(Scanner.IsWhitespace('\n'));
        Assert.True(Scanner.IsWhitespace('\r'));
        Assert.True(Scanner.IsWhitespace('\t'));
    }

    [Fact]
    public void TrackPosition()
    {
        // offset ×10              0             1
        // offset × 1              0 123 456 7 890 123
        //                         a\t.b\n.c\r\n.d\r.eEOF
        // lines                   0 000 011 1  22 222  2
        // columns                 0 123 401 2  01 234  5
        var scanner = new Scanner("a\t.b\n.c\r\n.d\r.e");

        var positions = new LexLocation[] {
            new LexLocation(0, 0, 0, 1), // T_USTRING a
            new LexLocation(0, 2, 0, 3), // T_DOT     .
            new LexLocation(0, 3, 0, 4), // T_USTRING b
            new LexLocation(1, 0, 1, 1), // T_DOT     .
            new LexLocation(1, 1, 1, 2), // T_USTRING c
            new LexLocation(2, 0, 2, 1), // T_DOT     .
            new LexLocation(2, 1, 2, 2), // T_USTRING d
            new LexLocation(2, 3, 2, 4), // T_DOT     .
            new LexLocation(2, 4, 2, 5), // T_USTRING e
            new LexLocation(2, 5, 2, 5), // T_EOF
        };

        IEnumerable<Token> Tokenize()
        {
            Token? eof = null;
            while (true)
            {
                var token = scanner.GetNextToken();
                if (token.Type == TokenType.EOF || token.Type == TokenType.E_UNRECOGNIZED)
                {
                    eof = token;
                    break;
                }
                yield return token;
            }
            yield return eof;
        }

        var locations = Tokenize()
            .Select(t => t.Location)
            .ToArray()
            ;

        Assert.Equal(10, locations.Length);

        for (var index = 0; index < locations.Length; index++)
            AssertEqual(locations[index], positions[index]);
    }

    private void AssertEqual(LexLocation? location, LexLocation lexLocation)
    {
        System.Diagnostics.Debug.Assert(location != null);

        Assert.Equal(location!.StartLine, lexLocation.StartLine);
        Assert.Equal(location!.StartColumn, lexLocation.StartColumn);   
        Assert.Equal(location!.EndLine, lexLocation.EndLine);
        Assert.Equal(location!.EndColumn, lexLocation.EndColumn);
    }
}
