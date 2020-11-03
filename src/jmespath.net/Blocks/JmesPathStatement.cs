using DevLab.JmesPath.Expressions;
using System;
using System.Collections.Generic;

namespace jmespath.net.Blocks
{
    public class JmesPathBlock
    {
        private readonly IList<JmesPathStatement> statements_;
        public JmesPathBlock()
            : this(new JmesPathStatement[] { })
        {
        }

        public JmesPathBlock(IEnumerable<JmesPathStatement> statements)
            => statements_ = new List<JmesPathStatement>(statements);

        public void AddStatement(JmesPathStatement statement)
        {
            statements_.Add(statement);
        }

        public void Execute(JmesPathArgument argument)
        {
            foreach (var statement in statements_)
                statement.Execute(argument);
        }
    }
    public abstract class JmesPathStatement
    {
        public abstract void Execute(JmesPathArgument argument);
    }

    public sealed class JmesPathClosure : JmesPathStatement
    {
        private readonly string identifier_;
        private readonly JmesPathExpression expression_;

        public JmesPathClosure(string identifier, JmesPathExpression expression)
        {
            identifier_ = identifier;
            expression_ = expression;
        }

        public string Identifier => identifier_;
        public JmesPathExpression Expression => expression_;

        public override void Execute(JmesPathArgument argument)
        {
            var token = expression_.Transform(argument);
            // TODO: Contains ?
            Expression.Context.Add(identifier_, token);
        }
    }
}
