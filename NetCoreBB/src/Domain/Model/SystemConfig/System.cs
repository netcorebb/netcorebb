/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

namespace NetCoreBB.Domain.Model.SystemConfig
{
    public class System
    {
        public bool Installed { get; set; } = false;
        public bool Maintenance { get; set; } = false;
        public bool Development { get; set; } = false;
        public bool UnderAttack { get; set; } = false;
    }
}
