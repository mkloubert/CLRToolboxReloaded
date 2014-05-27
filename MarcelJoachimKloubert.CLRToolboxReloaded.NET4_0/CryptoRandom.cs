﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// An extension of <see cref="Random" /> that uses a <see cref="RNGCryptoServiceProvider" />
    /// and optional binary seed data to generate random data.
    /// </summary>
    public sealed class CryptoRandom : Random
    {
        #region Fields (2)

        private readonly SeedProvider _PROVIDER;
        private readonly RNGCryptoServiceProvider _RNG;

        #endregion Fields

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="rng">The random data provider.</param>
        /// <param name="provider">The provider for the additional seed data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rng" /> and/or <paramref name="provider" /> are <see langword="null" />.
        /// </exception>
        public CryptoRandom(RNGCryptoServiceProvider rng, SeedProvider provider)
            : base()
        {
            if (rng == null)
            {
                throw new ArgumentNullException("rng");
            }

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._RNG = rng;
            this._PROVIDER = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="provider">The provider for the additional seed data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public CryptoRandom(SeedProvider provider)
            : this(new RNGCryptoServiceProvider(), provider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="rng">The random data provider.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rng" /> is <see langword="null" />.
        /// </exception>
        public CryptoRandom(RNGCryptoServiceProvider rng)
            : this(rng, GetNullSeed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        public CryptoRandom()
            : this(new RNGCryptoServiceProvider())
        {
        }

        #endregion Constructors

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function or method that provides the optional and additional seed
        /// data for generating random data.
        /// </summary>
        /// <param name="rand">The underlying instance.</param>
        /// <returns>The optional seed data.</returns>
        public delegate IEnumerable<byte> SeedProvider(CryptoRandom rand);

        #endregion Events and delegates

        #region Methods (14)

        // Public Methods (11)

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="seed">The seed to use.</param>
        public static CryptoRandom Create(IEnumerable<byte> seed)
        {
            return Create(new RNGCryptoServiceProvider(),
                          seed);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="seed">The seed to use.</param>
        public static CryptoRandom Create(double seed)
        {
            return Create(ToBinary(seed));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="seed">The seed to use.</param>
        public static CryptoRandom Create(Guid seed)
        {
            return Create(seed.ToByteArray());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="rng">The random data provider.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rng" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// The method uses <see cref="Environment.TickCount" /> as seed value.
        /// </remarks>
        public static CryptoRandom Create(RNGCryptoServiceProvider rng)
        {
            return Create(rng,
                          Environment.TickCount);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="rng">The random data provider.</param>
        /// <param name="seed">The seed to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rng" /> is <see langword="null" />.
        /// </exception>
        public static CryptoRandom Create(RNGCryptoServiceProvider rng, double seed)
        {
            return Create(rng,
                          ToBinary(seed));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="rng">The random data provider.</param>
        /// <param name="seed">The seed to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rng" /> is <see langword="null" />.
        /// </exception>
        public static CryptoRandom Create(RNGCryptoServiceProvider rng, Guid seed)
        {
            return Create(rng,
                          seed.ToByteArray());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoRandom" /> class.
        /// </summary>
        /// <param name="rng">The random data provider.</param>
        /// <param name="seed">The seed to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rng" /> is <see langword="null" />.
        /// </exception>
        public static CryptoRandom Create(RNGCryptoServiceProvider rng, IEnumerable<byte> seed)
        {
            return new CryptoRandom(rng,
                                    r => seed);
        }

        /// <inheriteddoc />
        public override int Next()
        {
            return this.Next(int.MaxValue);
        }

        /// <inheriteddoc />
        public override int Next(int maxValue)
        {
            return this.Next(0, maxValue);
        }

        /// <inheriteddoc />
        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }

            if (minValue == maxValue)
            {
                return minValue;
            }

            var moveBy = -(long)minValue;
            var movedMaxValue = (long)maxValue + moveBy;

            int result;
            do
            {
                long randVal;
                do
                {
                    var rng = new byte[8];
                    this.GetRandomBytes(rng);

                    try
                    {
                        unchecked
                        {
                            randVal = Math.Abs(BitConverter.ToInt64(rng, 0));
                        }
                    }
                    catch
                    {
                        // try again
                        randVal = -1;
                    }
                }
                while (randVal < 0);

                try
                {
                    unchecked
                    {
                        result = (int)((randVal % movedMaxValue) - moveBy);
                    }
                }
                catch
                {
                    // try again
                    result = maxValue;
                }
            }
            while ((result < minValue) ||
                   (result >= maxValue));

            return result;
        }

        /// <inheriteddoc />
        public override void NextBytes(byte[] buffer)
        {
            this.GetRandomBytes(buffer);
        }

        /// <inheriteddoc />
        public override double NextDouble()
        {
            var data = new byte[8];
            this.GetRandomBytes(data);

            return BitConverter.ToDouble(data, 0);
        }

        // Private Methods (4)

        private static IEnumerable<byte> GetNullSeed(CryptoRandom rand)
        {
            return null;
        }

        private void GetRandomBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Length < 1)
            {
                return;
            }

            this._RNG.GetBytes(data);

            var seed = this._PROVIDER(this).AsArray();
            if ((seed == null) ||
                (seed.Length < 1))
            {
                return;
            }

            for (var i = 0; i < data.Length; i++)
            {
                try
                {
                    unchecked
                    {
                        data[i] = (byte)(data[i] ^ seed[i % seed.Length]);
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        private static byte[] ToBinary(double? input)
        {
            return input.HasValue ? BitConverter.GetBytes(input.Value) : null;
        }

        #endregion Methods
    }
}