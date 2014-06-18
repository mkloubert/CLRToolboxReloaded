// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define STRING_IS_CHAR_SEQUENCE
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// Handles a string instance as a char sequence.
    /// </summary>
    public sealed class CharSequence :   IEnumerable<char>
                                       , IEquatable<string>, IEquatable<IEnumerable<char>>, IEquatable<StringBuilder>
                                       , IComparable, IComparable<string>, IComparable<IEnumerable<char>>, IComparable<StringBuilder>
    {
        #region Fields (1)

        private readonly string _STRING;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="CharSequence" /> class.
        /// </summary>
        /// <param name="str">The inner string.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="str" /> is <see langword="null" />.
        /// </exception>
        public CharSequence(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            this._STRING = str;
        }

        #endregion Constructors (1)

        #region Methods (14)

        /// <summary>
        /// Casts a general char sequence object to a <see cref="CharSequence" />.
        /// </summary>
        /// <param name="chars">The input value.</param>
        /// <returns>The output value.</returns>
        public static CharSequence Cast(IEnumerable<char> chars)
        {
            CharSequence result = null;

            if (chars != null)
            {
                result = chars as CharSequence;
                if (result == null)
                {
                    result = new CharSequence(chars.AsString());
                }
            }

            return result;
        }
        
        /// <inheriteddoc />
        public int CompareTo(StringBuilder other)
        {
            return this.CompareTo(other != null ? other.ToString() : null);
        }

        /// <inheriteddoc />
        public int CompareTo(IEnumerable<char> other)
        {
            return this.CompareTo(other.AsString());
        }

        /// <inheriteddoc />
        public int CompareTo(string other)
        {
            return this._STRING
                       .CompareTo(other);
        }

        /// <inheriteddoc />
        public int CompareTo(object other)
        {
            if (other is string)
            {
                return this.CompareTo((string)other);
            }

            if (other is IEnumerable<char>)
            {
                return this.CompareTo((IEnumerable<char>)other);
            }

            if (other is StringBuilder)
            {
                return this.CompareTo((StringBuilder)other);
            }

            return ((IComparable)this._STRING).CompareTo(other);
        }
        
        /// <inheriteddoc />
        public bool Equals(StringBuilder other)
        {
            return other != null ? this.Equals(other.ToString()) : false;
        }

        /// <inheriteddoc />
        public bool Equals(string other)
        {
            return this._STRING == other;
        }

        /// <inheriteddoc />
        public bool Equals(IEnumerable<char> other)
        {
            if (other == null)
            {
                return false;
            }

            IEnumerable<char> chars;
#if STRING_IS_CHAR_SEQUENCE
            chars = this._STRING;
#else
            chars = this._STRING.ToCharArray();
#endif

            return chars.SequenceEqual(other);
        }

        /// <inheriteddoc />
        public override bool Equals(object other)
        {
            if (other is string)
            {
                return this.Equals((string)other);
            }

            if (other is IEnumerable<char>)
            {
                return this.Equals((IEnumerable<char>)other);
            }

            if (other is StringBuilder)
            {
                return this.Equals((StringBuilder)other);
            }

            return base.Equals(other);
        }

        /// <inheriteddoc />
        public IEnumerator<char> GetEnumerator()
        {
#if STRING_IS_CHAR_SEQUENCE
            return this._STRING.GetEnumerator();
#else
            return global::System.Linq.Enumerable.Cast<char>(this._STRING)
                                                 .GetEnumerator();
#endif
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheriteddoc />
        public override int GetHashCode()
        {
            return this._STRING
                       .GetHashCode();
        }

        /// <summary>
        /// Converts the data of that instance to a char array.
        /// </summary>
        /// <returns>That instance as char array.</returns>
        public char[] ToArray()
        {
            return this._STRING.ToCharArray();
        }

        /// <inheriteddoc />
        public override string ToString()
        {
            return this._STRING;
        }

        #endregion Methods (12)

        #region Operators (8)

        /// <summary>
        /// Checks if tqo char sequences are equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal or not.</returns>
        public static bool operator == (CharSequence left, CharSequence right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Checks if tqo char sequences are NOT equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>
        /// Are equal (<see langword="false" />) or not (<see langword="true" />).
        /// </returns>
        public static bool operator !=(CharSequence left, CharSequence right)
        {
            return (left == right) == false;
        }

        /// <summary>
        /// Converts a string to a <see cref="CharSequence" />.
        /// </summary>
        /// <param name="str">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator CharSequence(string str)
        {
            return str != null ? new CharSequence(str)
                               : null;
        }

        /// <summary>
        /// Converts a <see cref="CharSequence" /> to a string.
        /// </summary>
        /// <param name="chars">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator string(CharSequence chars)
        {
            return chars != null ? chars._STRING
                                 : null;
        }

        /// <summary>
        /// Converts a <see cref="CharSequence" /> to an array.
        /// </summary>
        /// <param name="chars">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator char[](CharSequence chars)
        {
            return chars != null ? chars.ToArray()
                                 : null;
        }

        /// <summary>
        /// Converts a char array to a <see cref="CharSequence" />.
        /// </summary>
        /// <param name="array">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator CharSequence(char[] array)
        {
            return array != null ? new CharSequence(new string(array))
                                 : null;
        }

        /// <summary>
        /// Converts a <see cref="CharSequence" /> to a <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="chars">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator StringBuilder(CharSequence chars)
        {
            return chars != null ? new StringBuilder(chars._STRING)
                                 : null;
        }

        /// <summary>
        /// Converts a <see cref="StringBuilder" /> to a <see cref="CharSequence" />.
        /// </summary>
        /// <param name="builder">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator CharSequence(StringBuilder builder)
        {
            return builder != null ? new CharSequence(builder.ToString())
                                   : null;
        }

        #endregion Operators (10)
    }
}