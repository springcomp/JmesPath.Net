using System.Linq;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Functions
{
    public class MapByFunction : ByFunction
    {
        public MapByFunction()
            : base("map_by")
        {
        }

        public override JToken Execute(params JmesPathFunctionArgument[] args)
        {
            System.Diagnostics.Debug.Assert(args.Length == 2);
            System.Diagnostics.Debug.Assert(args[0].IsToken);
            System.Diagnostics.Debug.Assert(args[1].IsExpressionType);

            var elements = (JArray) (args[0].Token);
            var expression = args[1].Expression;

            var items = elements.Select(e =>
                expression.Transform(e).AsJToken()
                ).ToArray();          

            return new JArray().AddRange(items);
        }     
    }
}