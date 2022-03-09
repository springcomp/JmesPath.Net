using jmespath.lexer;
using StarodubOleg.GPPG.Runtime;
using System;
using System.IO;
using System.Text;

namespace DevLab.JmesPath
{
    using LexLocation = StarodubOleg.GPPG.Runtime.LexLocation;
    using Token = jmespath.lexer.Token;

    internal partial class JmesPathScanner : AbstractScanner<ValueType, LexLocation>
    {
        private PushbackQueue<ScanObj> pushback_;

        private readonly Scanner scanner_;
        private global::jmespath.lexer.Token nextToken_ = Scanner.T_EOF;

        private int yycol = 0;
        private string yytext = "";
        private string yyline = "";

        public JmesPathScanner(Stream input, string codePage)
        {
            using (var reader = new StreamReader(input, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();
                scanner_ = new Scanner(content);
            }
        }

        public override LexLocation yylloc { get; set; }

        /// <summary>
        /// This method enable extended lookahead of more that one
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
            var line = yyline;
            var column = yycol;
            var text = yytext;

            throw new Exception($"Error({line}, {column}): syntax, near '{text}'.");
        }

        public override int yylex()
        {
            if (this.pushback_.QueueLength > 0)
            {
                var obj = this.pushback_.DequeueCurrentToken();
                this.yylloc = obj.yylloc;
                this.yylval = obj.yylval;
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