/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using NetCoreBB.Infrastructure;
using NetCoreBB.Interfaces;
using Xunit.Abstractions;

namespace NetCoreBB.UnitTests.Infrastructure
{
    public class SystemConfigTests
    {
        private ISystemConfig Config { get; }
        private ITestOutputHelper Output { get; }

        public SystemConfigTests(ITestOutputHelper helper)
        {
            Config = new SystemConfig("");
            Output = helper;
        }
    }
}
