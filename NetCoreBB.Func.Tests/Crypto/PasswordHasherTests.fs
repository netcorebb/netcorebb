(*
 * This file is part of the NetCoreBB forum software package.
 * License: GNU General Public License, version 3 (GNU GPLv3)
 * Copyright © 2019–2020 Roman Volkov
 *)

namespace NetCoreBB.Func.Crypto

open NetCoreBB.Func.Crypto
open Xunit

module PasswordHasherTests =

    [<Fact>]
    let ``Password is being hashed correctly``() =
        // Arrange
        let pwd = "test"

        // Act
        let res = PasswordHasher.hash pwd

        // Assert
        Assert.NotEmpty res
