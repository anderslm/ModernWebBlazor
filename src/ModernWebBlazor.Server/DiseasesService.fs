namespace ModernWebBlazor.Server

open System.IO
open Diseases
open FSharp.Json
open Microsoft.AspNetCore.Hosting
open Bolero.Remoting.Server
open ModernWebBlazor

type DiseasesService(ctx : IRemoteContext, env : IWebHostEnvironment) =
    inherit RemoteHandler<Browser.Main.DiseasesService>()

    let diseases =
        Path.Combine(env.ContentRootPath, "data/diseases.json")
        |> File.ReadAllText
        |> Json.deserialize<Disease[]>
        |> ResizeArray

    override this.Handler =
        { GetDiseases = fun () ->
            async {
                return diseases |> List.ofSeq
            }
          AddDisease = fun book ->
            async {
              diseases.Add(book)
            } }
