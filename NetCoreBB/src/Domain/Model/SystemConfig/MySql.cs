/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using LanguageExt;

namespace NetCoreBB.Domain.Model.SystemConfig
{
    public class MySql : Record<MySql>
    {
        /// <summary>
        /// Table prefix like `ncbb_`
        /// </summary>
        public string? Prefix { get; set; } = null;

        public string Server { get; set; } = "localhost";
        public string SslMode { get; set; } = "preferred";
        public string Database { get; set; } = "netcorebb";
        public string Username { get; set; } = "root";
        public string Password { get; set; } = "";
        public string Protocol { get; set; } = "socket";
        public uint Port { get; set; } = 3306;

        /// <summary>
        /// Other MySQL parameters excluding the main parameters above
        /// </summary>
        public Map<string, string> Other { get; set; } = Map<string, string>.Empty;
    }
}
