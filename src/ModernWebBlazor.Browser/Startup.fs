namespace ModernWebBlazor.Browser

open Bolero
open Bolero.Remoting
open ChainOfInfection.Main
open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Bolero.Remoting.Client
open ChainOfInfection
open Elmish
open Bolero.Templating.Client

module Program =
    let router = Router.infer SetPage (fun model -> model.Page)

    type MyApp() =
        inherit ProgramComponent<Model, Message>()

        override this.Program =
            let diseasesService = this.Remote<Main.DiseasesService>()
            let mainEffectAdapter = Main.effectAdapter diseasesService
            Program.mkProgram
                (fun _ ->
                    let model, effect = init()
                    
                    model, mainEffectAdapter effect
                    )
                (fun msg model ->
                    let model, effect = update model msg
                    
                    model,
                    mainEffectAdapter effect)
                Main.view
            |> Program.withRouter router
    #if DEBUG
            |> Program.withHotReload
    #endif

    [<EntryPoint>]
    let Main args =
        let builder = WebAssemblyHostBuilder.CreateDefault(args)
        builder.RootComponents.Add<MyApp>("#main")
        builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
        builder.Build().RunAsync() |> ignore
        0
