/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using LanguageExt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCoreBB.Interfaces;
using ServiceStack;
using static LanguageExt.Prelude;

namespace NetCoreBB.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int HashLength = 256 / 8;
        private const int SaltLength = 128 / 8;
        private const int Iterations = 100_000;
        private const KeyDerivationPrf Algorithm = KeyDerivationPrf.HMACSHA256;


        public Option<string> Hash(string password)
        {
            var trimmedPwd = password?.Trim();
            if (trimmedPwd.IsNullOrEmpty()) {
                return None;
            }
            var salt = GenerateSalt();
            var hash = KeyDerivation.Pbkdf2(trimmedPwd, salt, Algorithm, Iterations, HashLength);
            return Convert.ToBase64String(salt) + '.' + Convert.ToBase64String(hash);
        }


        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }


        public Option<(bool, TimeSpan)> Verify(string password, string hash)
        {
            var trimmedPwd = password?.Trim();
            if (trimmedPwd.IsNullOrEmpty() || !Decode(hash, out var salt, out var pwdBytes)) {
                return None;
            }
            var watch = Stopwatch.StartNew();
            var newHash = KeyDerivation.Pbkdf2(trimmedPwd, salt, Algorithm, Iterations, HashLength);
            return (newHash == pwdBytes, watch.Elapsed);
        }


        public bool Decode(string hash, out byte[] salt, out byte[] password)
        {
            if (!IsValid(hash)) {
                salt = null;
                password = null;
                return false;
            }
            try {
                var arr = hash.Split('.');
                salt = Convert.FromBase64String(arr[0]);
                password = Convert.FromBase64String(arr[1]);
            }
            catch (FormatException) {
                salt = null;
                password = null;
                return false;
            }
            return true;
        }


        public bool IsValid(string hash)
        {
            if (hash.IsNull()) {
                return false;
            }
            try {
                var regex = new Regex("^[0-9a-z+/]{" + SaltLength + "}\\.[0-9a-z+/]{" + HashLength + "}$", RegexOptions.IgnoreCase);
                return regex.IsMatch(hash);
            }
            catch (RegexMatchTimeoutException) {
                return false;
            }
        }
    }
}
