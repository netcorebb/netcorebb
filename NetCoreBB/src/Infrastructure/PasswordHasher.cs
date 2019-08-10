/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCoreBB.Infrastructure.Interfaces;
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


        public Option<string> Hash(Some<string> password)
        {
            var trimmedPwd = password.Value.Trim();
            if (trimmedPwd.IsEmpty()) {
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


        public Option<(bool, TimeSpan)> Verify(Some<string> password, Some<string> hash)
        {
            var trimmedPwd = password.Value.Trim();
            if (trimmedPwd.IsEmpty() || !Decode(hash, out var salt, out var pwdBytes)) {
                return None;
            }
            var watch = Stopwatch.StartNew();
            var newHash = KeyDerivation.Pbkdf2(trimmedPwd, salt.ValueUnsafe(), Algorithm, Iterations, HashLength);
            return (newHash.SequenceEqual(pwdBytes.ValueUnsafe()), watch.Elapsed);
        }


        public bool Decode(Some<string> hash, out Option<byte[]> salt, out Option<byte[]> password)
        {
            if (!IsValid(hash)) {
                salt = None;
                password = None;
                return false;
            }
            try {
                var arr = hash.Value.Split('.');
                salt = Convert.FromBase64String(arr[0]);
                password = Convert.FromBase64String(arr[1]);
            }
            catch (FormatException) {
                salt = None;
                password = None;
                return false;
            }
            return true;
        }


        public bool IsValid(Some<string> hash)
        {
            try {
                var lengthSalt = (int)Math.Ceiling(SaltLength * 4.0 / 3);
                var lengthHash = (int)Math.Ceiling(HashLength * 4.0 / 3);
                var regex = new Regex("^[0-9a-zA-Z+/]{" + lengthSalt + "}==\\.[0-9a-zA-Z+/]{" + lengthHash + "}=$");
                return regex.IsMatch(hash.Value);
            }
            catch (RegexMatchTimeoutException) {
                return false;
            }
        }
    }
}
