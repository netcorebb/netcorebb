/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

namespace NetCoreBB.Domain.Model.SystemConfig
{
    public class System
    {
        public bool Installed { get; set; }
        public bool Maintenance { get; set; }
        public bool Development { get; set; }
        public bool UnderAttack { get; set; }
    }
}
