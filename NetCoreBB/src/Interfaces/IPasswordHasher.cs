/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System;
using LanguageExt;

namespace NetCoreBB.Interfaces
{
    /// <summary>
    /// Password hashing and verifying facility
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Calculates the salt+hash string from a raw password. Used e.g. on password creation/update.
        /// </summary>
        /// <param name="password">Raw non-empty password string</param>
        /// <returns>Base64-encoded salt+hash string</returns>
        Option<string> Hash(Some<string> password);

        /// <summary>
        /// Checks whether a password is correct given a salt+hash string. Used e.g. on logins.
        /// </summary>
        /// <param name="password">Raw non-empty password string</param>
        /// <param name="hash">Base64-encoded salt+hash string</param>
        /// <returns>True + time elapsed if password is correct</returns>
        Option<(bool, TimeSpan)> Verify(Some<string> password, Some<string> hash);

        /// <summary>
        /// Decodes a salt+hash string into its (salt, hash) byte array pair.
        /// </summary>
        /// <param name="hash">Base64-encoded salt+hash string</param>
        /// <param name="saltBytes">Bytes of salt</param>
        /// <param name="passwordBytes">Bytes of password hash</param>
        /// <returns>True upon success</returns>
        bool Decode(Some<string> hash, out Option<byte[]> saltBytes, out Option<byte[]> passwordBytes);

        /// <summary>
        /// Checks whether a given salt+hash string is syntactically correct.
        /// </summary>
        /// <param name="hash">Base64-encoded salt+hash string</param>
        /// <returns>True if valid</returns>
        bool IsValid(Some<string> hash);
    }
}
