using jmespath.lexer;
using StarodubOleg.GPPG.Runtime;
using System;
using System.IO;
using System.Text;

namespace DevLab.JmesPath
{
    using LexLocation = StarodubOleg.GPPG.Runtime.LexLocation;
    using Token = jmespath.lexer.Token;

    internal class JmesPathScanner : AbstractScanner<ValueType, LexLocation>
    {
        private PushbackQueue<ScanObj> pushback_;

        private readonly Scanner scanner_;
        private readonly string input_;

        private global::jmespath.lexer.Token nextToken_ = Scanner.T_EOF;

        public JmesPathScanner(Stream input, string codePage)
        {
            using (var reader = new StreamReader(input, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();

                input_ = content;
                scanner_ = new Scanner(content);
            }
        }

        public override LexLocation yylloc { get; set; }

        /// <summary>
        /// This method enables extended lookahead of more that one
        /// token to resolve ambiguities in the grammar.
        /// see: gppg manual, page 51, 10 AppendixC:PushingBackInputSymbols.
        /// see: http://softwareautomata.blogspot.fr/2011/12/doing-ad-hoc-lookahead-in-gppg-parsers_25.html
        /// 
        /// </summary>
        public void InitializeLookaheadQueue()
        {
            pushback_ = PushbackQueue<ScanObj>.NewPushbackQueue(
                () => new ScanObj(yylex(), yylval, yylloc),
                tk => new ScanObj(tk, yylval, yylloc)
                );
        }

        public int EnqueueAndReturnInitialToken(int token)
        {
            return pushback_.EnqueueAndReturnInitialSymbol(token)
                .token
                ;
        }

        public int GetAndEnqueue()
        {
            return pushback_.GetAndEnqueue()
                .token
                ;
        }

        public void AddPushbackBufferToQueue()
        {
            pushback_.AddPushbackBufferToQueue();
        }

        public override void yyerror(string format, params object[] args)
        {
            var line = nextToken_?.Location?.StartLine ?? 0;
            var column = nextToken_?.Location?.StartColumn ?? 0;
            var text = nextToken_?.RawText;

            if (String.IsNullOrEmpty(text))
                text = input_;

            throw new Exception($"Error({line}, {column}): syntax, near '{text}'.");
        }

        public override int yylex()
        {
            // Code to take tokens from non-empty queue.
            if (pushback_.QueueLength > 0)
            {
                ScanObj obj = pushback_.DequeueCurrentToken();
                yylloc = obj.yylloc;
                yylval = obj.yylval;
                return obj.token;
            }

            nextToken_ = scanner_.GetNextToken();

            var location = nextToken_.Location;
            var tokenType = (TokenType)(int)(nextToken_.Type);
            yylval = new ValueType { Token = JmesPath.Token.Create(tokenType, nextToken_.RawText), };
            yylloc = location == null ? null : new LexLocation(location.StartLine, location.StartColumn, location.EndLine, location.EndColumn);

            return (int)nextToken_.Type;
        }
    }
}