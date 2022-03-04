using JmesPath.Net.Expressions;

namespace JmesPath.Net.Interop
{
    public interface IVisitor
    {
        void Visit(Expression expression);
    }
}
