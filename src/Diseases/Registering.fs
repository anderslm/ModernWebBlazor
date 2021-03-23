module ChainOfInfection.Registering

open Diseases

type Model =
    { Diseases : Disease list }

type Message =
    | Load
    | Loaded of Disease list
    | Create of Disease

type Effect =
    | LoadEffect of (Disease list -> Message)
    | SaveEffect of Disease * Message
    | None

let init () =
    { Diseases = [] }
    
let update model =
    function
    | Load ->
        model, LoadEffect Loaded
    | Loaded ds ->
        { model with Diseases = ds }
        , None
    | Create d ->
        { model with Diseases = d :: model.Diseases }
        , SaveEffect (d, Load)