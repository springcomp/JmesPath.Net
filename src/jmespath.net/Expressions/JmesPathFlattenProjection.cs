using System.Collections.Generic;
using System.Linq;
using DevLab.JmesPath.Utils;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathFlattenProjection : JmesPathProjection
    {
        public override JmesPathArgument Project(JmesPathArgument argument)
        {
            //if (argument.IsProjection)
            //    argument = argument.AsJToken();

            var items = new List<JmesPathArgument>();

            if(!argument.IsProjection)
            {
                var array = argument.Token as JArray;
                if (array == null)
                    return null;

                foreach (var item in array)
                {
                    if (JTokens.IsNull(item))
                        continue;

                    var nested = item as JArray;
                    if (nested == null)
                        items.Add(new JmesPathArgument(item) { Context = argument.Context});

                    else
                        items.AddRange(
                            nested
                                .Where(i => !JTokens.IsNull(i))
                                .Select(i => { var r = (JmesPathArgument)i; r.Context = argument.Context; return r; })
                        );
                }
            }
            else
            {
                //if (argument.AsJToken() as JArray == null)
                //    return null;

                foreach (var item in argument.Projection)
                {
                    if (JTokens.IsNull(item.Token))
                        continue;

                    var nested = item.Token as JArray;
                    if (nested == null)
                        items.Add(new JmesPathArgument(item.Token) { Context = item.Context });

                    else
                        items.AddRange(
                            nested
                                .Where(i => !JTokens.IsNull(i))
                                .Select(i => { var r = (JmesPathArgument)i; r.Context = item.Context; return r; })
                        );
                }
            }

         

            return new JmesPathArgument(items);
        }

        public override JmesPathArgument Transform(JmesPathArgument argument)
        {
            return argument.IsProjection 
                ? Project(argument) 
                : base.Transform(argument)
                ;
        }
    }
}