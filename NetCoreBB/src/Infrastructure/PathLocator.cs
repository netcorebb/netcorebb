/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System.IO;
using LanguageExt;
using NetCoreBB.Interfaces;
using ServiceStack;
using static LanguageExt.Prelude;

namespace NetCoreBB.Infrastructure
{
    public class PathLocator : IPathLocator
    {
        public Option<string> Config
        {
            get
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "etc");
                return path.DirectoryExists() ? Some(path) : None;
            }
        }
    }
}
