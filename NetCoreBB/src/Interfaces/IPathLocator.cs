/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using LanguageExt;

namespace NetCoreBB.Interfaces
{
    /// <summary>
    /// Service for retrieving filesystem paths the system uses
    /// </summary>
    public interface IPathLocator
    {
        /// <summary>
        /// Full path where configuration files are stored, e.g. 'etc'
        /// </summary>
        Option<string> Config { get; }
    }
}
