(*
 * This file is part of the NetCoreBB forum software package.
 * License: GNU General Public License, version 3 (GNU GPLv3)
 * Copyright © 2019–2020 Roman Volkov
 *)

namespace NetCoreBB.Func.Crypto

open Microsoft.AspNetCore.Cryptography.KeyDerivation
open System
open System.Security.Cryptography

module PasswordHasher =
    let hashLength = 512 / 8
    let saltLength = 256 / 8
    let iterations = 100_000

    let algorithm = KeyDerivationPrf.HMACSHA512


    let hash (password: string) =
        let generateSalt() =
            let salt = Array.zeroCreate saltLength
            use rng = RandomNumberGenerator.Create()
            rng.GetBytes salt
            salt

        let password = password.Trim()
        let salt = generateSalt()
        let hash = KeyDerivation.Pbkdf2(password, salt, algorithm, iterations, hashLength)
        let base64 = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash)
        base64


    let verify (password: string) (hash: string) =
        let isValid() = true

        let decode() =
            let arr = hash.Split('.')
            let salt = Convert.FromBase64String arr.[0]
            let pwd = Convert.FromBase64String arr.[1]
            salt, pwd

        let password = password.Trim()
        let salt, bytes = decode()
        let computed = KeyDerivation.Pbkdf2(password, salt, algorithm, iterations, hashLength)
        let eq = seq bytes = seq computed
        eq

    (*var lengthSalt = (int)Math.Ceiling(SaltLength * 4.0 / 3);
                var lengthHash = (int)Math.Ceiling(HashLength * 4.0 / 3);
                var regex = new Regex("^[0-9a-zA-Z+/]{" + lengthSalt + "}==\\.[0-9a-zA-Z+/]{" + lengthHash + "}=$");
                return regex.IsMatch(hash.Value);*)
