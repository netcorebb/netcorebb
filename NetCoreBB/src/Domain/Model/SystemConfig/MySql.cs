/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using LanguageExt;

namespace NetCoreBB.Domain.Model.SystemConfig
{
    public class MySql
    {
        public string Server { get; set; }
        public string SslMode { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Protocol { get; set; }
        public int Port { get; set; }
        public Map<string, string> Other { get; set; }
    }
}
