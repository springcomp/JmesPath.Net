using System;
using System.Collections.Generic;
using System.Linq;
using DevLab.JmesPath;
using JmesPath.Net.Expressions;

namespace JmesPath
{
    sealed class AstBuilder : IJmesPathGenerator
    {
        /// <summary>
        /// holds the functions available to the parser
        /// </summary>
        //readonly IFunctionRepository repository_;

        readonly Stack<IDictionary<string, Expression>> selectHashes_
            = new Stack<IDictionary<string, Expression>>()
            ;

        readonly Stack<IList<Expression>> selectLists_
            = new Stack<IList<Expression>>()
            ;

        readonly Stack<IList<Expression>> functions_
            = new Stack<IList<Expression>>();

        readonly Stack<Expression> expressions_
            = new Stack<Expression>()
            ;

        Expression expression_;

        //public JmesPathGenerator(IFunctionRepository repository)
        //{
        //    repository_ = repository;
        //}

        public Expression Expression => expression_;

        public void OnExpression()
        {
            if (expression_ == null)
                expression_ = expressions_.Pop();
        }

        bool Prolog()
        {
            if (expression_ != null)
            {
                expressions_.Push(expression_);
                expression_ = null;
                return true;
            }

            return false;
        }

        public bool IsProjection()
        {
            //var pop = Prolog();

            //try
            //{
            //    if (expressions_.Count == 0)
            //        return false;

            //    return expressions_.Peek() is JmesPathProjection;
            //}
            //finally
            //{
            //    if (pop)
            //        OnExpression();
            //}

            return false;
        }

        #region Expressions

        public void OnSubExpression()
        {
            //Prolog();

            //System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            //var right = expressions_.Pop();
            //var left = expressions_.Pop();

            //var expression = new JmesPathSubExpression(left, right);

            //expressions_.Push(expression);
        }

        #region index_expression

        public void OnIndex(int index)
        {
            //Prolog();

            //var expression = new JmesPathIndex(index);

            //Expressions_.Push(expression);
        }

        public void OnFilterProjection()
        {
            //Prolog();

            //System.Diagnostics.Debug.Assert(expressions_.Count >= 1);

            //var comparison = expressions_.Pop();
            //var expression = new JmesPathFilterProjection(comparison);

            //expressions_.Push(expression);
        }

        public void OnFlattenProjection()
        {
            //Prolog();

            //expressions_.Push(new JmesPathFlattenProjection());
        }

        public void OnListWildcardProjection()
        {
            //Prolog();

            //expressions_.Push(new JmesPathListWildcardProjection());
        }

        public void OnIndexExpression()
        {
            //Prolog();

            //System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            //var right = expressions_.Pop();
            //var left = expressions_.Pop();

            //var expression = new JmesPathIndexExpression(left, right);

            //expressions_.Push(expression);
        }

        public void OnSliceExpression(int? start, int? stop, int? step)
        {
            //Prolog();

            //var sliceExpression = new JmesPathSliceProjection(start, stop, step);

            //expressions_.Push(sliceExpression);
        }

        #endregion

        #region comparator_expression

        public void OnComparisonEqual() =>
            //PopPush((left, right) => new JmesPathEqualOperator(left, right));
            throw new NotImplementedException();

        public void OnComparisonNotEqual() =>
            //PopPush((left, right) => new JmesPathNotEqualOperator(left, right));
            throw new NotImplementedException();

        public void OnComparisonGreaterOrEqual() =>
            //PopPush((left, right) => new JmesPathGreaterThanOrEqualOperator(left, right));
            throw new NotImplementedException();

        public void OnComparisonGreater() =>
            //PopPush((left, right) => new JmesPathGreaterThanOperator(left, right));
            throw new NotImplementedException();

        public void OnComparisonLesserOrEqual() =>
            //PopPush((left, right) => new JmesPathLessThanOrEqualOperator(left, right));
            throw new NotImplementedException();

        public void OnComparisonLesser() =>
            //PopPush((left, right) => new JmesPathLessThanOperator(left, right));
            throw new NotImplementedException();

        #endregion

        public void OnOrExpression() =>
            //PopPush((left, right) => new JmesPathOrExpression(left, right));
            throw new NotImplementedException();

        public void OnAndExpression() =>
            //PopPush((left, right) => new JmesPathAndExpression(left, right));
            throw new NotImplementedException();

        public void OnNotExpression() =>
            //PopPush(e => new JmesPathNotExpression(e));
            throw new NotImplementedException();

        public void OnIdentifier(string name)
        {
            Prolog();

            var expression = new Identifier(name);
            expressions_.Push(expression);
        }

        //static readonly JmesPathHashWildcardProjection JmesPathHashWildcardProjection = new JmesPathHashWildcardProjection();

        public void OnHashWildcardProjection()
        {
            //Prolog();

            //expressions_.Push(JmesPathHashWildcardProjection);
        }

        #region multi_select_hash

        public void PushMultiSelectHash()
        {
            selectHashes_.Push(new Dictionary<string, Expression>());
        }

        public void AddMultiSelectHashExpression()
        {
            Prolog();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);
            System.Diagnostics.Debug.Assert(selectHashes_.Count > 0);

            var expression = expressions_.Pop();

            var identifier = expressions_.Pop() as Identifier;
            System.Diagnostics.Debug.Assert(identifier != null);
            var name = identifier.Name;
            System.Diagnostics.Debug.Assert(name != null);

            var items = selectHashes_.Peek();
            items.Add(name, expression);
        }

        public void PopMultiSelectHash()
        {
            //System.Diagnostics.Debug.Assert(selectHashes_.Count > 0);
            //var items = selectHashes_.Pop();
            //var expression = new JmesPathMultiSelectHash(items);
            //expressions_.Push(expression);
        }

        #endregion

        #region multi_select_list

        public void PushMultiSelectList()
        {
            selectLists_.Push(new List<Expression>());
        }

        public void AddMultiSelectListExpression()
        {
            Prolog();

            System.Diagnostics.Debug.Assert(selectLists_.Count > 0);
            var expression = expressions_.Pop();
            var items = selectLists_.Peek();
            items.Add(expression);
        }

        public void PopMultiSelectList()
        {
            //System.Diagnostics.Debug.Assert(selectLists_.Count > 0);
            //var items = selectLists_.Pop();
            //var expression = new JmesPathMultiSelectList(items);

            //expressions_.Push(expression);
        }

        #endregion

        public void OnLiteralString(string literal)
        {
            //Prolog();

            //var token = JToken.Parse(literal);
            //var expression = new JmesPathLiteral(token);
            //expressions_.Push(expression);
        }

        public void OnPipeExpression()
        {
            //Prolog();

            //System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            //var right = expressions_.Pop();
            //var left = expressions_.Pop();

            //var expression = new JmesPathPipeExpression(left, right);

            //expressions_.Push(expression);
        }

        #region function_expression

        public void PushFunction()
        {
            functions_.Push(new List<Expression>());
        }

        public void PopFunction(string name)
        {
            //System.Diagnostics.Debug.Assert(functions_.Count > 0);

            //var args = functions_.Pop();
            //var expressions = args.ToArray();

            //var expression = new JmesPathFunctionExpression(repository_, name, expressions);

            //expressions_.Push(expression);
        }

        public void AddFunctionArg()
        {
            Prolog();

            System.Diagnostics.Debug.Assert(functions_.Count > 0);

            var expression = expressions_.Pop();
            functions_.Peek().Add(expression);
        }

        public void OnExpressionType()
        {
            //Prolog();

            //var expression = expressions_.Pop();
            //Expression.MakeExpressionType(expression);
            //expressions_.Push(expression);
        }

        #endregion

        public void OnRawString(string value)
        {
            //Prolog();

            //var expression = new JmesPathRawString(value);
            //expressions_.Push(expression);
        }

        public void OnCurrentNode()
        {
            //Prolog();

            //expressions_.Push(new JmesPathCurrentNodeExpression());
        }

        #endregion // Expressions

        void PopPush<T>(Func<Expression, T> factory)
            where T : Expression
        {
            Prolog();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 1);

            var arg = expressions_.Pop();

            expressions_.Push(factory(arg));
        }

        void PopPush<T>(Func<Expression, Expression, T> factory)
            where T : Expression
        {
            Prolog();

            System.Diagnostics.Debug.Assert(expressions_.Count >= 2);

            var right = expressions_.Pop();
            var left = expressions_.Pop();

            expressions_.Push(factory(left, right));
        }
    }
}
