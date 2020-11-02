using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using DevLab.JmesPath.Expressions;
using DevLab.JmesPath.Interop;
using jmespath.net.Blocks;

namespace DevLab.JmesPath
{

    sealed class JmesPathGenerator : IJmesPathGenerator
    {
        /// <summary>
        /// holds the functions available to the parser
        /// </summary>
        readonly IFunctionRepository repository_;

        readonly Stack<IDictionary<string, JmesPathExpression>> selectHashes_
            = new Stack<IDictionary<string, JmesPathExpression>>()
            ;

        readonly Stack<IList<JmesPathExpression>> selectLists_
            = new Stack<IList<JmesPathExpression>>()
            ;

        readonly Stack<IList<JmesPathExpression>> functions_
            = new Stack<IList<JmesPathExpression>>();

        readonly Stack<JmesPathExpression> expressions_
            = new Stack<JmesPathExpression>()
            ;

        readonly Stack<JmesPathBlock> blocks_
            = new Stack<JmesPathBlock>()
            ;

        JmesPathExpression expression_;

        public JmesPathGenerator(IFunctionRepository repository)
        {
            repository_ = repository;
        }

        public JmesPathExpression Expression => expression_;

        public void OnExpression()
        {
            if (expression_ == null)
                expression_ = expressions_.Pop();
        }

        public bool IsProjection()
        {
            var pop = PushExpression();

            try
            {
                if (expressions_.Count == 0)
                    return false;

                return expressions_.Peek() is JmesPathProjection;
            }
            finally
            {
                if (pop)
                    OnExpression();
            }
        }

        #region Expressions

        public void OnSubExpression()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            var right = expressions_.Pop();
            var left = expressions_.Pop();

            var expression = new JmesPathSubExpression(left, right);

            expressions_.Push(expression);
        }

        #region index_expression

        public void OnIndex(int index)
        {
            PushExpression();

            var expression = new JmesPathIndex(index);

            expressions_.Push(expression);
        }

        public void OnFilterProjection()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 1);

            var comparison = expressions_.Pop();
            var expression = new JmesPathFilterProjection(comparison);

            expressions_.Push(expression);
        }

        public void OnFlattenProjection()
        {
            PushExpression();

            expressions_.Push(new JmesPathFlattenProjection());
        }

        public void OnListWildcardProjection()
        {
            PushExpression();

            expressions_.Push(new JmesPathListWildcardProjection());
        }

        public void OnIndexExpression()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            var right = expressions_.Pop();
            var left = expressions_.Pop();

            var expression = new JmesPathIndexExpression(left, right);

            expressions_.Push(expression);
        }

        public void OnSliceExpression(int? start, int? stop, int? step)
        {
            PushExpression();

            var sliceExpression = new JmesPathSliceProjection(start, stop, step);

            expressions_.Push(sliceExpression);
        }

        #endregion

        #region comparator_expression

        public void OnComparisonEqual() =>
            PopPush((left, right) => new JmesPathEqualOperator(left, right));

        public void OnComparisonNotEqual() =>
            PopPush((left, right) => new JmesPathNotEqualOperator(left, right));

        public void OnComparisonGreaterOrEqual() =>
            PopPush((left, right) => new JmesPathGreaterThanOrEqualOperator(left, right));

        public void OnComparisonGreater() =>
            PopPush((left, right) => new JmesPathGreaterThanOperator(left, right));

        public void OnComparisonLesserOrEqual() =>
            PopPush((left, right) => new JmesPathLessThanOrEqualOperator(left, right));

        public void OnComparisonLesser() =>
            PopPush((left, right) => new JmesPathLessThanOperator(left, right));

        #endregion

        public void OnOrExpression() =>
            PopPush((left, right) => new JmesPathOrExpression(left, right));

        public void OnAndExpression() =>
            PopPush((left, right) => new JmesPathAndExpression(left, right));

        public void OnNotExpression() =>
            PopPush(e => new JmesPathNotExpression(e));

        public void OnIdentifier(string name)
        {
            PushExpression();

            var expression = new JmesPathIdentifier(name);
            expression.Context = ConsumeBlock();

            expressions_.Push(expression);
        }

        private JmesPathBlock ConsumeBlock()
        {
            if (blocks_.Count > 0)
                return blocks_.Pop();
            return null;
        }

        static readonly JmesPathHashWildcardProjection JmesPathHashWildcardProjection = new JmesPathHashWildcardProjection();

        public void OnHashWildcardProjection()
        {
            PushExpression();

            expressions_.Push(JmesPathHashWildcardProjection);
        }

        #region multi_select_hash

        public void PushMultiSelectHash()
        {
            selectHashes_.Push(new Dictionary<string, JmesPathExpression>());
        }

        public void AddMultiSelectHashExpression()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);
            System.Diagnostics.Debug.Assert(selectHashes_.Count > 0);

            var expression = expressions_.Pop();

            var identifier = expressions_.Pop() as JmesPathIdentifier;
            System.Diagnostics.Debug.Assert(identifier != null);
            var name = identifier.Name;
            System.Diagnostics.Debug.Assert(name != null);

            var items = selectHashes_.Peek();
            items.Add(name, expression);
        }

        public void PopMultiSelectHash()
        {
            System.Diagnostics.Debug.Assert(selectHashes_.Count > 0);
            var items = selectHashes_.Pop();
            var expression = new JmesPathMultiSelectHash(items);
            expressions_.Push(expression);
        }

        #endregion

        #region multi_select_list

        public void PushMultiSelectList()
        {
            selectLists_.Push(new List<JmesPathExpression>());
        }

        public void AddMultiSelectListExpression()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(selectLists_.Count > 0);
            var expression = expressions_.Pop();
            var items = selectLists_.Peek();
            items.Add(expression);
        }

        public void PopMultiSelectList()
        {
            System.Diagnostics.Debug.Assert(selectLists_.Count > 0);
            var items = selectLists_.Pop();
            var expression = new JmesPathMultiSelectList(items);

            expressions_.Push(expression);
        }

        #endregion

        public void OnLiteralString(string literal)
        {
            PushExpression();

            var token = JToken.Parse(literal);
            var expression = new JmesPathLiteral(token);
            expressions_.Push(expression);
        }

        public void OnPipeExpression()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            var right = expressions_.Pop();
            var left = expressions_.Pop();

            var expression = new JmesPathPipeExpression(left, right);

            expressions_.Push(expression);
        }

        #region function_expression

        public void PushFunction()
        {
            functions_.Push(new List<JmesPathExpression>());
        }

        public void PopFunction(string name)
        {
            System.Diagnostics.Debug.Assert(functions_.Count > 0);

            var args = functions_.Pop();
            var expressions = args.ToArray();

            var expression = new JmesPathFunctionExpression(repository_, name, expressions);

            expressions_.Push(expression);
        }

        public void AddFunctionArg()
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(functions_.Count > 0);

            var expression = expressions_.Pop();
            functions_.Peek().Add(expression);
        }

        public void OnExpressionType()
        {
            PushExpression();

            var expression = expressions_.Pop();
            JmesPathExpression.MakeExpressionType(expression);
            expressions_.Push(expression);
        }

        #endregion

        public void OnRawString(string value)
        {
            PushExpression();

            var expression = new JmesPathRawString(value);
            expressions_.Push(expression);
        }

        public void OnCurrentNode()
        {
            PushExpression();

            expressions_.Push(new JmesPathCurrentNodeExpression());
        }

        #endregion // Expressions

        #region Blocks

        public void OnClosure(string identifier)
        {
            System.Diagnostics.Debug.Assert(expression_ != null);

            if (blocks_.Count == 0)
                blocks_.Push(new JmesPathBlock());

            blocks_.Peek().AddStatement(
                new JmesPathClosure(identifier, expression_)
            );

            expression_ = null;
        }

        #endregion

        private bool PushExpression()
        {
            if (expression_ != null)
            {
                expressions_.Push(expression_);
                expression_ = null;
                return true;
            }

            return false;
        }

        void PopPush<T>(Func<JmesPathExpression, T> factory)
            where T : JmesPathExpression
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 1);

            var arg = expressions_.Pop();

            expressions_.Push(factory(arg));
        }

        void PopPush<T>(Func<JmesPathExpression, JmesPathExpression, T> factory)
            where T : JmesPathExpression
        {
            PushExpression();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            var right = expressions_.Pop();
            var left = expressions_.Pop();

            expressions_.Push(factory(left, right));
        }
    }
}
