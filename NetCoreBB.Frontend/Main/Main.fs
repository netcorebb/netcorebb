module NetCoreBB.Frontend.Main

open Bolero
open Bolero.Html
open Bolero.Json
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client
open Elmish
open System

type Page = | [<EndPoint "/">] Home


type Model =
    { Page: Page }

let model = fun _ -> { Page = Home }


type Message = SetPage of Page

type MainTpl = Template<"Main/main.html">


let update (message: Message) (model: Model) = model


let render (model: Model) (dispatch: Dispatch<Message>) =  MainTpl.Root().Elt()


let router = Router.infer SetPage (fun model -> model.Page)


let program =
    Program.mkSimple model update render |> Program.withRouter router
#if DEBUG
    |> Program.withHotReload
#endif


type RootComponent() =
    inherit ProgramComponent<Model, Message>()
    override this.Program = program
