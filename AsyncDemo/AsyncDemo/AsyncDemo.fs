// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace AsyncDemo

open System.Diagnostics
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms

module App = 
    type Model = 
      { Message : string
        IsLoading : bool }

    type Msg = 
        | Reset 
        | GetNewMessage 
        | NewMessageReceived of string

    let initModel = { Message = "It's a fabulous day to learn F#"; IsLoading = false }

    let init () = initModel, Cmd.none

    let GetMessage = 
        async { 
                // Fake webrequest - man is this wifi slow ;-)
                do! Async.Sleep 2000
                return (NewMessageReceived "Still a great day to learn F#") }
        |> Cmd.ofAsyncMsg

    let update msg model =
        match msg with
        | Reset -> init ()
        | GetNewMessage -> {model with IsLoading = true}, GetMessage
        | NewMessageReceived s -> { model with Message = s; IsLoading = false }, Cmd.none

    let view (model: Model) dispatch =
        View.ContentPage(
          content = View.Grid(
            children = [
                View.StackLayout(padding = 20.0, verticalOptions = LayoutOptions.Center,
                    children = [ 
                        View.Label(text = model.Message, horizontalOptions = LayoutOptions.CenterAndExpand, fontSize=42, horizontalTextAlignment=TextAlignment.Center)
                        View.Button(text = "Get new quote", command = (fun () -> dispatch GetNewMessage), horizontalOptions = LayoutOptions.Center)
                        View.Button(text = "Reset", command = (fun () -> dispatch Reset), horizontalOptions = LayoutOptions.Center)
                    ])
                View.Grid(
                    isVisible = model.IsLoading,
                    backgroundColor = Color.FromRgba(0., 0., 0., 0.6),
                    children=[
                        View.ActivityIndicator(
                            verticalOptions = LayoutOptions.Center, 
                            horizontalOptions = LayoutOptions.Center, 
                            isEnabled = model.IsLoading, 
                            isRunning = model.IsLoading, 
                            color = Color.White)
                    ])
            ]))

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


