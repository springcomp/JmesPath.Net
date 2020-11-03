using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using DevLab.JmesPath.Expressions;
using DevLab.JmesPath.Functions;
using DevLab.JmesPath.Interop;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace DevLab.JmesPath
{
    public sealed class JmesPath
    {
        private readonly Encoding _encoding;
        private readonly JmesPathFunctionFactory repository_;

        private readonly IDictionary<string, JmesPathArgument> context_
            = new Dictionary<string, JmesPathArgument>()
            ;

        public JmesPath() : this(Encoding.UTF8)
        {
        }

        public JmesPath(Encoding encoding)
        {
            _encoding = encoding;
            repository_ = new JmesPathFunctionFactory();
            foreach (var name in JmesPathFunctionFactory.Default.Names)
                repository_.Register(name, JmesPathFunctionFactory.Default[name]);

            context_ = new Dictionary<string, JmesPathArgument>();
            var evaluateFunc = new EvaluateExpressionFunction(context_);
            repository_.Register(evaluateFunc.Name, evaluateFunc);
        }

        public IRegisterFunctions FunctionRepository => repository_;

        [Obsolete("Please, use the Transform(string, string) overload instead.")]
        public JToken Transform(JToken token, string expression)
        {
            // this method is deprecated because we do not correctly
            // handle JToken.Date tokens.

            var jmesPath = Parse(expression);
            var result = jmesPath.Transform(token);
            return result.AsJToken();
        }

        public String Transform(string json, string expression)
        {
            var token = ParseJson(json);
            var result = Transform(token, expression);
            return result.AsString();
        }

        public sealed class Expression : JmesPathExpression
        {
            private readonly JmesPathExpression expression_;

            internal Expression(JmesPathExpression expression)
            {
                expression_ = expression;
            }

            public string Transform(string document)
            {
                var token = ParseJson(document);
                var result = Transform(token);
                return result.AsJToken()?.AsString();
            }

            protected override JmesPathArgument Transform(JToken json)
            {
                return expression_.Transform(json);
            }
        }

        public Expression Parse(string expression)
        {
            return Parse(new MemoryStream(_encoding.GetBytes(expression)));
        }

        public Expression Parse(Stream stream)
        {
            var analyzer = new JmesPathGenerator(repository_, new ContextInjectionVisitor(context_));
            Parser.Parse(stream, _encoding, analyzer);

            // perform post-parsing syntax validation

            var syntax = new SyntaxVisitor(context_);
            analyzer.Expression.Accept(syntax);

            return new Expression(analyzer.Expression);
        }

        public static JToken ParseJson(string input)
        {
            using (var textReader = new StringReader(input))
            using (var reader = new JsonTextReader(textReader))
            {
                reader.DateParseHandling = DateParseHandling.None;

                if (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartArray)
                        return JArray.Load(reader);
                    else if (reader.TokenType == JsonToken.StartObject)
                        return JObject.Load(reader);
                    else
                        return JToken.Load(reader);
                }

                throw new JsonReaderException("Unable to read the JSON string.");
            }
        }

        private sealed class SyntaxVisitor : ContextInjectionVisitor
        {
            public SyntaxVisitor(IDictionary<string, JmesPathArgument> context)
                : base(context)
            {
            }

            public void Visit(JmesPathExpression expression)
            {
                base.Visit(expression);

                // parse slice projection

                var projection = expression as JmesPathSliceProjection;
                if (projection?.Step != null && projection.Step.Value == 0)
                    throw new Exception("Error: invalid-value, a slice projection step cannot be 0.");
            }
        }

        private class ContextInjectionVisitor : IVisitor
        {
            private readonly IDictionary<string, JmesPathArgument> context_;

            public ContextInjectionVisitor(IDictionary<string, JmesPathArgument> context)
            {
                context_ = context;
            }

            protected IDictionary<string, JmesPathArgument> Context
                => context_;

            public virtual void Visit(JmesPathExpression expression)
            {
                expression.Context = context_;
            }
        }
    }
}
