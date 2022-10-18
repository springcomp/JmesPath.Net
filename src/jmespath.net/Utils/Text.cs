using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DevLab.JmesPath.Utils
{
    /// <summary>
    /// The <see cref="Text" /> class represents a sequence of Unicode codepoints.
    /// If differs from the .NET <see cref="String" /> class in that is correctly
    /// handles codepoints from supplementary planes, including surrogate pairs.
    ///
    /// characters in a .NET string are represented by a 21-bit code value
    /// of a character in the Basic Multilingual Plane (0x0000 - 0x10FFFF)
    /// it can consists of either:
    ///
    /// - a single 16-bit codepoint (U+0000 to U+FFFF excluding the surrogate range U+D800 to U+DFFF).
    /// In that case, a Unicode character, identified by its codepoint, maps to a single 16-bit code unit.
    ///   
    /// or
    ///
    /// - a pair of 16-bit surrogate code units (high U+D800 to U+DBFF and low U+DC00 to U+DFFF).
    /// In that case, a Unicode character, identifier by its codepoint, maps to a sequence of two 16-bit code units.
    ///
    /// Additionally, some Unicode characters can have multiple representations. For instance, the
    /// character 'é' can be encoded using the following two difference sequences of codepoints:
    ///
    /// - 'é' U+00E9 LATIN SMALL LETTER E WITH ACUTE ACCENT.
    /// - 'è' U+0065 LATIN SMALL LETTER E, U+0301 COMBINING ACUTE ACCENT.
    ///
    /// </summary>
    internal sealed partial class Text : IEnumerable<string>, IEquatable<Text>, IComparable<Text>
    {
        private readonly string text_;
        private readonly StringInfo info_;

        private static readonly IComparer<Text> defaultComparer_
            = new TextComparer();

        /// <summary>
        /// Initialize a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="text"></param>
        public Text(string text)
        {
            text_ = text;
            info_ = new StringInfo(text_);
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Text" /> class.
        /// </summary>
        /// <param name="codePoints"></param>
        public Text(params int[] codePoints)
        {
            var sb = new StringBuilder();
            foreach (var codePoint in codePoints)
                sb.Append(Char.ConvertFromUtf32(codePoint));

            text_ = sb.ToString();
            info_ = new StringInfo(text_);
        }

        /// <summary>
        /// Returns an <see cref="IComparer{Text}" /> implementation
        /// that compares Text using the numerical value of its codepoints.
        /// </summary>
        public static IComparer<Text> CodePointComparer
            => defaultComparer_;

        /// <summary>
        /// The number of Unicode codepoints.
        /// </summary>
        public int Length
            => info_.LengthInTextElements;

        /// <summary>
        /// Returns a enumerator over the sequence of Unicode codepoints.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetCodePointsEnumerator()
            => new CodePointEnumerator(this);

        /// <summary>
        /// The sequence of Unicode codepoints.
        /// </summary>
        public IEnumerable<int> CodePoints
            => new CodePointEnumerator(this).AsEnumerable();

        public static implicit operator String(Text text)
            => text.ToString();

        public static explicit operator Text(string text)
            => new Text(text);

        public IEnumerator<string> GetEnumerator()
            => new TextEnumerator(text_);

        /// <summary>
        /// Returns true if the two strings are equal
        /// i.e. if the two sequences of Unicode codepoints
        /// are identical.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Text other)
            => CompareTo(other) == 0;

        /// <summary>
        /// Compares the two sequences of Unicode codepoints.
        /// A string will sort based on the numerical value
        /// of its first differring codepoint.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CompareTo(Text other)
        {
            var length = Math.Min(this.Length, other.Length);
            var codePoints = this.CodePoints.ToArray();
            var otherCodePoints = other.CodePoints.ToArray();

            for (var index = 0; index < length; index++)
            {
                if (codePoints[index] < otherCodePoints[index])
                    return -1;
                else if (codePoints[index] > otherCodePoints[index])
                    return 1;
            }

            if (codePoints.Length < otherCodePoints.Length)
                return -1;
            else if (codePoints.Length > otherCodePoints.Length)
                return 1;

            return 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public override string ToString()
            => text_;
    }
}