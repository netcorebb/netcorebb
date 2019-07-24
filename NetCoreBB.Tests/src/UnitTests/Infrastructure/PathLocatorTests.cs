/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System.IO;
using NetCoreBB.Infrastructure;
using NetCoreBB.Interfaces;
using Shouldly;
using Xunit;

namespace NetCoreBB.UnitTests.Infrastructure
{
    public class PathLocatorTests
    {
        private IPathLocator Locator { get; } = new PathLocator();
        private string EtcPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), "etc");


        [Fact]
        public void Config_returns_path()
        {
            Directory.CreateDirectory(EtcPath);

            Locator.Config.IsSome.ShouldBeTrue();
            Locator.Config.IfSome(path => path.ShouldBe(EtcPath));

            Directory.Exists(EtcPath).ShouldBeTrue();
            Directory.Delete(EtcPath);
            Directory.Exists(EtcPath).ShouldBeFalse();
        }


        [Fact]
        public void Config_is_none_when_path_does_not_exist()
        {
            if (Directory.Exists(EtcPath)) {
                Directory.Delete(EtcPath);
                Directory.Exists(EtcPath).ShouldBeFalse();
            }

            Locator.Config.IsNone.ShouldBeTrue();
        }
    }
}
