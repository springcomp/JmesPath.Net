using Newtonsoft.Json.Linq;
using DevLab.JmesPath.Interop;
using DevLab.JmesPath.Utils;
using System.Collections.Generic;

namespace DevLab.JmesPath
{
    public sealed class ReduceAccumulator : IReduceAccumulator
    {
        private readonly Stack<JToken> scopes_
            = new Stack<JToken>()
            ;

        public void PopSeed()
            => scopes_.Pop();

        public void PushSeed(JToken seed)
            => scopes_.Push(seed);

        public JToken Accumulator
        {
            get => scopes_.Peek() ?? JTokens.Null;
            set
            {
                if (scopes_.Count != 0)
                {
                    scopes_.Pop();
                    scopes_.Push(value);
                }
            }
        }
    }
}
