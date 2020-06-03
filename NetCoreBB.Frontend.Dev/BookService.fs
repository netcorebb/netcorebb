namespace NetCoreBB.Frontend.Dev

open Bolero
open Bolero.Remoting
open Bolero.Remoting.Server
open Microsoft.AspNetCore.Hosting
open NetCoreBB.Frontend
open System
open System.IO


(*
type BookService(ctx: IRemoteContext, env: IWebHostEnvironment) =
    inherit RemoteHandler<MainX.BookService>()

    let books =
        Path.Combine(env.ContentRootPath, "data/books.json")
        |> File.ReadAllText
        |> Json.Deserialize<MainX.Book []>
        |> ResizeArray

    override this.Handler =
        { getBooks = ctx.Authorize <| fun () -> async { return books.ToArray() }

          addBook = ctx.Authorize <| fun book -> async { books.Add(book) }

          removeBookByIsbn = ctx.Authorize <| fun isbn -> async { books.RemoveAll(fun b -> b.isbn = isbn) |> ignore }

          signIn =
              fun (username, password) ->
                  async {
                      if password = "password" then
                          do! ctx.HttpContext.AsyncSignIn(username, TimeSpan.FromDays(365.))
                          return Some username
                      else
                          return None
                  }

          signOut = fun () -> async { return! ctx.HttpContext.AsyncSignOut() }

          getUsername = ctx.Authorize <| fun () -> async { return ctx.HttpContext.User.Identity.Name } }
*)
