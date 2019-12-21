/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using Shouldly;
using Xunit;

namespace NetCoreBB.UnitTests.Persistence
{
    public partial class TableTests
    {
        [Fact]
        public void Single_is_default()
        {
            CreateTable();
            Table.Single(1).IsSome.ShouldBeTrue();
            Table.Single(2).IsSome.ShouldBeTrue();
            Table.Single(3).IsSome.ShouldBeFalse();
        }
    }
}
