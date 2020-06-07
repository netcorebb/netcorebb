module NetCoreBB.Admin.Root

open Bolero
open Bolero.Templating.Client
open Elmish


// Model

type Page =
    | [<EndPoint "/readme">] ReadMe
    | [<EndPoint "/install">] Install
    | [<EndPoint "/">] Dashboard

type Model =
    { Page: Page
      DualPaneOn: bool
      OpenNewTabs: bool }

let initialModel = { Page = Dashboard; DualPaneOn = false; OpenNewTabs = false }


// Update

type Message = NavigateToPage of Page

let update (message: Message) (model: Model) = model


// View

type Html = Template<"Root/root.html">

let render (model: Model) (dispatch: Dispatch<Message>) = Html.root().Elt()


// Component

let router = Router.infer NavigateToPage (fun model -> model.Page)

let program =
    let model = fun _ -> initialModel
    Program.mkSimple model update render |> Program.withRouter router
#if DEBUG
    |> Program.withHotReload
#endif

type Component() =
    inherit ProgramComponent<Model, Message>()
    override _.Program = program
