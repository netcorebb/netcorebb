/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

namespace NetCoreBB.Infrastructure.Interfaces
{
    /// <summary>
    /// Execution environment-related information
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// True in ASP.NET Core development mode
        /// </summary>
        bool IsDevelopment { get; }

        /// <summary>
        /// True in ASP.NET Core production mode
        /// </summary>
        bool IsProduction { get; }
    }
}
