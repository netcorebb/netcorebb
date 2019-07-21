/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System.Text.RegularExpressions;
using NetCoreBB.Infrastructure;
using NetCoreBB.Interfaces;
using Shouldly;
using Xunit;

namespace NetCoreBB.UnitTests.Infrastructure
{
    public class PasswordHasherTests
    {
        public IPasswordHasher Hasher { get; } = new PasswordHasher();


        [Fact]
        public void Hash_generates_plausible_result()
        {
            var res = Hasher.Hash("abcdef");
            res.IsSome.ShouldBeTrue();
            res.IfSome(hash => {
                var regex = new Regex("^[a-zA-Z0-9+/=\\.]{20,200}$");
                regex.IsMatch(hash).ShouldBeTrue();
                hash.Contains('.').ShouldBeTrue();
            });
        }


        [Fact]
        public void Hash_generates_different_results_for_same_password()
        {
            const string password = "password";

            var res1 = Hasher.Hash(password);
            var res2 = Hasher.Hash(password);
            var res3 = Hasher.Hash(password);
            var res4 = Hasher.Hash(password);
            var res5 = Hasher.Hash(password);

            var arr = new[] {res1, res2, res3, res4, res5};
            arr.ShouldAllBe(x => x.IsSome);
            arr.ShouldBeUnique();
        }


        [Fact]
        public void Hash_refuses_empty_password()
        {
            var res1 = Hasher.Hash(string.Empty);
            var res2 = Hasher.Hash(" ");
            var res3 = Hasher.Hash("   ");
            var res4 = Hasher.Hash(null);

            new[] {res1, res2, res3, res4}.ShouldAllBe(x => x.IsNone);
        }
    }
}
