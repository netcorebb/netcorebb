(*
 * This file is part of the NetCoreBB forum software package.
 * License: GNU General Public License, version 3 (GNU GPLv3)
 * Copyright © 2019–2020 Roman Volkov
 *)

namespace NetCoreBB.Spec

type User = {
    handle: string
    display: string
    email: string

    isAdmin: bool
    isMod: bool
    isRestricted: bool
    isBanned: bool
}
