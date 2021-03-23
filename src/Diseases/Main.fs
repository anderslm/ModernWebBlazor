module ChainOfInfection.Main

type Page =
    | Main

type Model =
    { Page : Page
      Registering : Registering.Model }

type Message =
    | SetPage of Page
    | RegisteringMsg of Registering.Message

type Effect =
    | RegisteringEffect of Registering.Effect
    | None

let init () =
    { Page = Main
      Registering = Registering.init() }
    , RegisteringEffect <| Registering.LoadEffect Registering.Loaded
    
let update model = function
    | SetPage p ->
        { model with Page = p }
        , None
    | RegisteringMsg msg ->
        let m, e = Registering.update model.Registering msg
        { model with Registering = m }
        , RegisteringEffect e