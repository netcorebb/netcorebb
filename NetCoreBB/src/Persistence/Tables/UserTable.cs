/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using LanguageExt;
using NetCoreBB.Domain.Model.Core;

namespace NetCoreBB.Persistence.Tables
{
    public class UserTable : TableBase<User>
    {
        public override string Name => "Users";

        public UserTable(Domain.Model.SystemConfig.MySql config) : base(config) { }
    }
}
