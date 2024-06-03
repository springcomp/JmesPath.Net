using jmespath.lexer.StateMachines;
using jmespath.lexer.Utils;

namespace jmespath.lexer;
public sealed class Scanner
{
    private static readonly Token T_EOF = Token.Create(TokenType.EOF, String.Empty);

    private readonly string input_;

    private int position_ = 0;
    private int line_ = 0;
    private int column_ = 0;

    public Scanner(string text)
    {
        machines_ = GetStateMachines();
        input_ = text;
    }

    public Token GetNextToken()
    {
        if (position_ >= input_.Length)
            return T_EOF;

        if (IsWhitespace(input_[position_]))
            Trim();

        var c = input_[position_];

        // recognize one-char unambiguous tokens.
        // matches on one of ":*,.@{}]()"

        if (TryRecognizeSingleCharacterToken(c, out var t_singleChar))
            return t_singleChar;

        // recognize one- or two-char tokens
        // this uses a state machine that recognizes either one of two possible tokens
        // i.e one of  &&, ||, <=, >=, !=

        if (TryRecognizeOneOrTwoCharacterToken(c, out var t_oneOrTwoChar))
            return t_oneOrTwoChar;

        // recognizes two-char tokens
        // this uses a state machine that recognizes ambiguous one- or two-char tokens
        // i.e one of [, [], [?

        if (TryRecognizeTwoCharacterToken(c, out var t_twoChar))
            return t_twoChar;

        // recognize complex patterns

        if (TryRecognizeToken(c, out var token))
            return token;

        return T_EOF;
    }

    private bool TryRecognizeSingleCharacterToken(char c, out Token token)
    {
        token = T_EOF;

        var tokenType = OneCharToken.GetTokenType(c);
        if (tokenType != TokenType.EOF)
        {
            token = Consume(tokenType, new string(new[] { c }));
            return true;
        }

        return false;
    }

    private bool TryRecognizeOneOrTwoCharacterToken(char c, out Token token)
    {
        token = T_EOF;

        var tokenTypes = OneOrTwoCharToken.GetCandidateTokenTypes(c);
        if (tokenTypes.Length != 0)
        {
            token = GetNextToken(tokenTypes[0], (_, text) =>
            {
                System.Diagnostics.Debug.Assert(text.Length == 1 || text.Length == 2);
                return Consume(tokenTypes[text.Length - 1], text);
            });
            return true;
        }

        return false;
    }
    private bool TryRecognizeTwoCharacterToken(char c, out Token token)
    {
        token = T_EOF;

        if (c == '[')
        {
            token = GetNextToken(TokenType.T_LBRACKET);

            if (token.RawText == "[]")
                token = Token.Create(TokenType.T_FLATTEN, token.RawText);
            else if (token.RawText == "[?")
                token = Token.Create(TokenType.T_FILTER, token.RawText);

            return true;
        }

        return false;
    }

    private void Trim()
    {
        int i = 0;

        var span = input_.AsSpan(position_);
        for (var length = span.Length; i < length; ++i)
        {
            var character = span[i];

            if (!IsWhitespace(character))
                break;

            position_++;

            if (character == '\n')
            {
                column_ = 0;
                line_++;
            }
            else
            {
                column_++;
            }
        }
    }

    private bool TryRecognizeToken(char c, out Token token)
    {
        (Func<char, bool> Condition, TryRecognizeTokenDelegate Recognizer)[]
            dispatchers = new (Func<char, bool>, TryRecognizeTokenDelegate)[] {
                (cc => cc == '=', TryRecognizeEqualSign),
                (cc => cc == '`', TryRecognizeLiteralString),
                (cc => cc == '\'', TryRecognizeRawString),
                (cc => cc == '"', TryRecognizeQuotedString),
                (cc => char.IsAscii(cc) && (cc == '-' || char.IsDigit(cc)), TryRecognizeNumber),
                (cc => char.IsAscii(cc) && char.IsLetter(cc), TryRecognizeUnquotedString),
            };

        token = T_EOF;

        foreach (var dispatcher in dispatchers)
        {
            if (dispatcher.Condition(c) && dispatcher.Recognizer(out token))
                return true;
        }

        return false;
    }
    private bool TryRecognizeEqualSign(out Token token)
        => TryRecognizeTokenType(TokenType.T_EQ, out token);
    private bool TryRecognizeNumber(out Token token)
        => TryRecognizeTokenType(TokenType.T_NUMBER, out token);
    private bool TryRecognizeLiteralString(out Token token)
        => TryRecognizeTokenType(TokenType.T_LSTRING, out token);
    private bool TryRecognizeQuotedString(out Token token)
        => TryRecognizeTokenType(TokenType.T_QSTRING, out token);
    private bool TryRecognizeRawString(out Token token)
        => TryRecognizeTokenType(TokenType.T_RSTRING, out token);
    private bool TryRecognizeUnquotedString(out Token token)
        => TryRecognizeTokenType(TokenType.T_USTRING, out token);

    private delegate bool TryRecognizeTokenDelegate(out Token token);
    private bool TryRecognizeTokenType(TokenType tokenType, out Token token)
    {
        token = GetNextToken(tokenType);
        return token.Type == tokenType;
    }
    private Token GetNextToken(TokenType tokenType, OnMatchedDelegate? onMatched = null)
    {
        System.Diagnostics.Debug.Assert(machines_[(int)tokenType] != null);
        StateMachine machine = machines_[(int)tokenType]!;

        onMatched ??= Consume;

        var span = input_.AsSpan(position_);
        var match = machine.Match(span);
        if (match.Success)
            return onMatched(tokenType, match.MatchedText);

        return T_EOF;
    }

    private delegate Token OnMatchedDelegate(TokenType tokenType, string text);

    #region Utils

    private Token Consume(TokenType type, string text)
    {
        var token = MakeToken(type, text, line_, column_);

        position_ += text.Length;
        column_ += text.Length;

        return token;
    }

    private static Token MakeToken(TokenType type, string text, int bLine, int bColumn, int? eLine = null, int? eColumn = null)
    {
        var token = Token.Create(type, text);
        token.Location = new LexLocation(
            bLine,
            bColumn,
            eLine ?? bLine,
            eColumn ?? bColumn + text.Length
            );

        return token;
    }

    internal static bool IsWhitespace(char input)
    {
        var ascii = 32 + (input - ' ');
        return (
              (ascii == 32) /*  ' ' */ ||
              (ascii == 8) /* '\b' */ ||
              (ascii == 12) /* '\f' */ ||
              (ascii == 13) /* '\r' */ ||
              (ascii == 10) /* '\n' */ ||
              (ascii == 9) /* '\t' */
            );
    }

    #endregion

    // these match either of two tokens (i.e '&' or '&&', '<' or '<=', etc.)

    private readonly StateMachine etype_ = new OneOrTwoCharToken('&', '&');
    private readonly StateMachine pipe_ = new OneOrTwoCharToken('|', '|');
    private readonly StateMachine lt_ = new OneOrTwoCharToken('<', '=');
    private readonly StateMachine gt_ = new OneOrTwoCharToken('>', '=');
    private readonly StateMachine not_ = new OneOrTwoCharToken('!', '=');

    // these match unambiguous two-character tokens (i.e '==')

    private readonly StateMachine eq_ = new TwoCharToken('=', '=');

    // these match ambiguous one- or two-char tokens (i.e '[', '[]', or '[?').

    private readonly StateMachine bracket_ = new BracketToken();

    // matches T_USTRING token

    private readonly StateMachine number_ = new Number();
    private readonly StateMachine literalString_ = new LiteralString();
    private readonly StateMachine quotedString_ = new QuotedString();
    private readonly StateMachine rawString_ = new RawString();
    private readonly StateMachine unquotedString_ = new UnquotedString();

    // state machine, if any, associated with any token type

    private readonly StateMachine?[] machines_ = new StateMachine?[(int)TokenType.T_MAX + 1];

    private StateMachine?[] GetStateMachines()
        => new StateMachine?[] {
            /* E_UNRECOGNIZED */ null,
            /* T_WHITESPACE   */ null,
            /* error          */ null,
            /* EOF            */ null,
            /* T_AND       && */ null,
            /* T_OR        || */ null,
            /* T_NOT        ! */ not_,
            /* T_COLON      : */ null,
            /* T_COMMA      , */ null,
            /* T_DOT        . */ null,
            /* T_PIPE        | */ pipe_,
            /* T_EQ        == */ eq_,
            /* T_GT         > */ gt_,
            /* T_GE        >= */ null,
            /* T_LT         < */ lt_,
            /* T_LE        <= */ null,
            /* T_NE        != */ null,
            /* T_FILTER    [? */ bracket_,
            /* T_FLATTEN   [] */ bracket_,
            /* T_STAR       * */ null,
            /* T_CURRENT    @ */ null,
            /* T_ETYPE      & */ etype_,
            /* T_NUMBER       */ number_,
            /* T_LSTRING      */ literalString_,
            /* T_QSTRING      */ quotedString_,
            /* T_RSTRING      */ rawString_,
            /* T_USTRING      */ unquotedString_,
            /* T_LBRACE     { */ null,
            /* T_RBRACE     } */ null,
            /* T_LBRACKET   [ */ bracket_,
            /* T_RBRACKET   ] */ null,
            /* T_LPAREN     ( */ null,
            /* T_RPAREN     ) */ null,
            /* T_MAX          */ null,
        };
}
