/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;
using NetCoreBB.Infrastructure;
using NetCoreBB.Interfaces;
using ServiceStack;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NetCoreBB.UnitTests.Infrastructure
{
    public class PasswordHasherTests
    {
        private IPasswordHasher Hasher { get; }
        private ITestOutputHelper Output { get; }

        public PasswordHasherTests(ITestOutputHelper helper)
        {
            Hasher = new PasswordHasher();
            Output = helper;
        }


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


        [Fact]
        public void Verify_works_with_positive()
        {
            const string password = "password";

            var res1 = Hasher.Hash(password);
            res1.IsSome.ShouldBeTrue();

            res1.IfSome(hash => {
                var res2 = Hasher.Verify(password, hash);
                res2.IsSome.ShouldBeTrue();

                res2.IfSome(pair => {
                    var (verify, time) = pair;
                    verify.ShouldBeTrue();
                    time.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(10));
                    Output.WriteLine($"Time elapsed: {time.Milliseconds} ms");
                });
            });
        }


        [Fact]
        public void Verify_works_with_negative()
        {
            const string password1 = "password1";
            const string password2 = "password2";

            var res1 = Hasher.Hash(password1);
            res1.IsSome.ShouldBeTrue();

            res1.IfSome(hash => {
                var res2 = Hasher.Verify(password2, hash);
                res2.IsSome.ShouldBeTrue();

                res2.IfSome(pair => {
                    var (verify, time) = pair;
                    verify.ShouldBeFalse();
                    time.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(10));
                    Output.WriteLine($"Time elapsed: {time.Milliseconds} ms");
                });
            });
        }


        [Fact]
        public void Verify_refuses_empty_values()
        {
            var res1 = Hasher.Verify(null, null);
            var res2 = Hasher.Verify(null, "abc");
            var res3 = Hasher.Verify("abc", null);

            var res4 = Hasher.Verify(string.Empty, string.Empty);
            var res5 = Hasher.Verify(string.Empty, "abc");
            var res6 = Hasher.Verify("abc", string.Empty);

            new[] {res1, res2, res3, res4, res5, res6}.ShouldAllBe(x => x.IsNone);

            Hasher.Hash("abc").Match(hash => {
                    var res7 = Hasher.Verify(string.Empty, hash);
                    var res8 = Hasher.Verify(null, hash);
                    var res9 = Hasher.Verify("   ", hash);
                    res7.IsNone.ShouldBeTrue();
                    res8.IsNone.ShouldBeTrue();
                    res9.IsNone.ShouldBeTrue();
                },
                () => true.ShouldBeFalse("Hashing didn't work."));
        }


        [Fact]
        public void Decode_works()
        {
            Hasher.Hash("abc").Match(hash => {
                    var res = Hasher.Decode(hash, out var salt, out var pwd);
                    res.ShouldBeTrue();
                    salt.ShouldNotBeNull();
                    salt.Length.ShouldBeGreaterThan(0);
                    pwd.ShouldNotBeNull();
                    pwd.Length.ShouldBeGreaterThan(0);
                },
                () => true.ShouldBeFalse("Hashing didn't work."));
        }


        [Fact]
        public void Decode_refuses_empty_values()
        {
            new[] {"not a hash", string.Empty, null}.ForEach(hash => {
                var res = Hasher.Decode(hash, out var salt, out var pwd);
                res.ShouldBeFalse();
                salt.ShouldBeNull();
                pwd.ShouldBeNull();
            });
        }


        [Fact]
        public void IsValid_works_with_positive()
        {
            Hasher.Hash("abc").Match(
                hash => Hasher.IsValid(hash).ShouldBeTrue(),
                () => true.ShouldBeFalse("Hashing didn't work."));
        }


        [Fact]
        public void IsValid_works_with_negative()
        {
            Hasher.Hash("abc").Match(hash => {
                    var wrongHash1 = hash.Substring(1);
                    var wrongHash2 = hash.ReplaceAll(".", ",");
                    var wrongHash3 = hash.Reverse().ToString();

                    new[] {"not a hash", string.Empty, null, wrongHash1, wrongHash2, wrongHash3}
                        .ShouldAllBe(wrongHash => !Hasher.IsValid(wrongHash));
                },
                () => true.ShouldBeFalse("Hashing didn't work."));
        }
    }
}
