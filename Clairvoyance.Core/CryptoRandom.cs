using System;
using System.Security.Cryptography;

namespace Clairvoyance.Core
{
    public static class CryptoRandom
    {
        /// <summary>
        /// Gets a random number between <paramref name="min"/> and <paramref name="max"/>
        /// using the <see cref="RNGCryptoServiceProvider"/>.
        /// </summary>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>A random number between <paramref name="min"/> and <paramref name="max"/>.</returns>
        /// <remarks>
        /// http://www.vcskicks.com/code-snippet/rng-int.php
        /// https://stackoverflow.com/questions/4892588/rngcryptoserviceprovider-random-number-review
        /// </remarks>
        public static int NextInt(int min, int max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];

            rng.GetBytes(buffer);
            int result = BitConverter.ToInt32(buffer, 0);

            return new Random(result).Next(min, max);
        }
    }
}