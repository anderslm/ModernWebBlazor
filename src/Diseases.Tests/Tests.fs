module Tests

open ChainOfInfection
open Diseases
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers 

let covid19 =
    { Name = "Covid-19"
      ChainOfInfection =
          { InfectiousAgent = Viruses
            Reservoir = [People;Animals]
            PortalOfExit = [PortalOfExit.Mouth]
            ModeOfTransmission = [Contact;Droplets]
            PortalOfEntry = [Mouth;Eyes]
            SusceptibleHost = [Anyone]} }

let influenza =
    { Name = "Influenza"
      ChainOfInfection =
          { InfectiousAgent = Viruses
            Reservoir = [People]
            PortalOfExit = [PortalOfExit.Mouth]
            ModeOfTransmission = [Droplets]
            PortalOfEntry = [Mouth]
            SusceptibleHost = [Elderly] }}
        
type ``Risk level tests`` () =
    [<Fact>]
    let ``Covid 19 is a pandemic type disease`` () =
        covid19
        |> createRiskLevel
        |> should be (ofCase<@ Pandemic @>)
        
    [<Fact>]
    let ``Influenza is a sporadic disease`` () =
        influenza
        |> createRiskLevel
        |> should be (ofCase<@ Sporadic @>)
    
type ``Main tests`` () =
    let model, effect = Main.init()
    
    [<Fact>]
    let ``Initial page is the main page`` () =
        model.Page |> should be (ofCase<@Main.Main@>)
    
    [<Fact>]
    let ``Initial effect is to fetch diseases`` () =
        effect
        |> function
           | Main.RegisteringEffect effect ->
               effect |> should be (ofCase<@Registering.LoadEffect@>)
           | _ -> failwith "Expected registering effect"

    [<Fact>]    
    let ``Has a model for registering`` () =
        model.Registering |> should equal (Registering.init())
        
    [<Fact>]
    let ``Can handle registering messages`` () =
        let model, _ = Main.update model (Main.RegisteringMsg <| Registering.Loaded [covid19])
        
        model.Registering.Diseases
        |> should equal [covid19]
        
    [<Fact>]
    let ``Can handle registering effects`` () =
        let _, effect = Main.update model (Main.RegisteringMsg <| Registering.Load)
        
        effect
        |> function
            | Main.RegisteringEffect e ->
                e |> should be (ofCase<@Registering.LoadEffect@>)
            | _ -> failwith "Expected a registering effect"

    [<Fact>]
    let ``Can set page`` () =
        let model, _ = Main.update model (Main.SetPage Main.Main)
        
        model.Page |> should be (ofCase<@Main.Main@>)
    
type ``Registering tests`` () =
    let model = Registering.init()
    
    [<Fact>]
    let ``Can load diseases`` () =
        let _, effect = Registering.update model Registering.Load
        
        effect
        |> function
            | Registering.LoadEffect msg ->
                let model, _ =
                    [covid19;influenza]
                    |> msg
                    |> Registering.update model
                model.Diseases
                |> should equal [covid19;influenza]
            | _ -> failwith "Expected load effect"
    
    [<Fact>]
    let ``Can register new diseases`` () =
        let model, _ = Registering.update model (Registering.Create covid19)
        let model, effect = Registering.update model (Registering.Create influenza)
        
        model.Diseases
        |> should equal [influenza;covid19]
        effect
        |> function
            | Registering.SaveEffect (disease, msg) ->
                disease |> should equal (influenza)
                msg |> should be (ofCase<@Registering.Load@>)
            | _ -> failwith "Expected save effect"