using DevLab.JmesPath.Utils;
using System;
using System.Linq;

namespace jmespath.net.Functions.Impl
{
    internal static class TextExtensions
    {
        /// <summary>
        /// Supports the `reverse()` the function.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Text Invert(this Text text)
            => (Text)String.Join( "", text.ToArray().Reverse() );
    }
}
