module ModernWebBlazor.Browser.Main

open Bolero
open Bolero.Html
open Bolero.Remoting
open ChainOfInfection
open ChainOfInfection.Main
open Diseases
open Elmish

type DiseasesService =
    { GetDiseases: unit -> Async<Disease list>
      AddDisease: Disease -> Async<unit> }

    interface IRemoteService with
        member this.BasePath = "/diseases"
        
let effectAdapter diseasesService =
    function
    | RegisteringEffect effect ->
        match effect with
        | Registering.LoadEffect msg ->
            Cmd.OfAsync.perform diseasesService.GetDiseases () (RegisteringMsg << msg)
        | Registering.SaveEffect (disease, msg) ->
            Cmd.OfAsync.perform diseasesService.AddDisease disease (fun () -> RegisteringMsg msg)
        | Registering.None -> Cmd.none
    | None -> Cmd.none

type Main = Template<"wwwroot/main.html">

let view model dispatch =
    Main()
        .Diseases(forEach model.Registering.Diseases (fun d ->
            Main.DiseaseTemplate()
                .Name(d.Name)
                .InfectiousAgent(d.ChainOfInfection.InfectiousAgent |> string)
                .Reservoir(d.ChainOfInfection.Reservoir |> string)
                .PortalOfExit(d.ChainOfInfection.PortalOfExit |> string)
                .ModeOfTransmission(d.ChainOfInfection.ModeOfTransmission |> string)
                .PortalOfEntry(d.ChainOfInfection.PortalOfEntry |> string)
                .SusceptibleHost(d.ChainOfInfection.SusceptibleHost |> string)
                .Elt()))
        .Create(fun _ -> dispatch (RegisteringMsg <|
                            Registering.Create { Name = "Covid 19 - English variation"
                                                 ChainOfInfection = {
                                                      InfectiousAgent = Viruses
                                                      Reservoir = [People]
                                                      PortalOfExit = [PortalOfExit.Mouth]
                                                      ModeOfTransmission = [Contact;Droplets]
                                                      PortalOfEntry = [Mouth;Eyes]
                                                      SusceptibleHost = [Anyone]
                                                 } } ))
        .Elt()
