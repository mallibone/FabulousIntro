namespace HelloEverydayFabulous

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

//type NavigationMessage = 
//    | NavigateToColorView of Color
//    | GoBack 

module MainView =
    type Model = 
      { Count: int
        Step: int
        TimerOn: bool }


    type Msg = 
        | Increment 
        | Decrement 
        | Reset
        | SetStep of int
        | TimerToggled of bool
        | TimedTick

    let initModel = { Count = 0; Step = 1; TimerOn=false }

    let view (model: Model) dispatch =
        View.ContentPage(content = 
            View.StackLayout(padding = Thickness 20.0, verticalOptions = LayoutOptions.Center,
                children = [
                    View.Button(text = "Blue Page", command = (fun () -> dispatch (NavigateToColorView Color.Blue)), horizontalOptions = LayoutOptions.Center)
                ])
                )

    //let init () = initModel, Cmd.none

module App = 
    type Views =
        | MainView
        | ColorView

    type Model = 
      { NavigationStack: list<Views>
        MainViewState: MainView.Model
        ColorViewState: ColorView.Model }


    let initModel = { NavigationStack = [ Views.MainView ]; MainViewState = MainView.initModel; ColorViewState = ColorView.initModel }

    let init () = initModel, Cmd.none


    //let timerCmd =
    //    async { do! Async.Sleep 200
    //            return TimedTick }
    //    |> Cmd.ofAsyncMsg

    let update msg model =
        match msg with
        | NavigateToColorView color -> { model with ColorViewState = {model.ColorViewState with Color = color}; NavigationStack = Views.ColorView::model.NavigationStack}, Cmd.none
        | GoBack -> {model with NavigationStack = model.NavigationStack |> List.tail}, Cmd.none
        //| Increment -> { model with Count = model.Count + model.Step }, Cmd.none
        //| Decrement -> { model with Count = model.Count - model.Step }, Cmd.none
        //| Reset -> init ()
        //| SetStep n -> { model with Step = n }, Cmd.none
        //| TimerToggled on -> { model with TimerOn = on }, (if on then timerCmd else Cmd.none)
        //| TimedTick -> 
        //    if model.TimerOn then 
        //        { model with Count = model.Count + model.Step }, timerCmd
        //    else 
        //        model, Cmd.none

    let view (model: Model) dispatch =
        View.NavigationPage(
            pages = [
                for page in model.NavigationStack do
                    match page with
                    | MainView ->
                        yield MainView.view model.MainViewState dispatch
                    | ColorView ->
                        yield ColorView.view model.ColorViewState dispatch
                ])
        //View.ContentPage(
        //  content = View.StackLayout(padding = Thickness 20.0, verticalOptions = LayoutOptions.Center,
        //    children = [ 
        //        View.Label(text = sprintf "%d" model.Count, horizontalOptions = LayoutOptions.Center, width=200.0, horizontalTextAlignment=TextAlignment.Center)
        //        View.Button(text = "Increment", command = (fun () -> dispatch Increment), horizontalOptions = LayoutOptions.Center)
        //        View.Button(text = "Decrement", command = (fun () -> dispatch Decrement), horizontalOptions = LayoutOptions.Center)
        //        View.Label(text = "Timer", horizontalOptions = LayoutOptions.Center)
        //        View.Switch(isToggled = model.TimerOn, toggled = (fun on -> dispatch (TimerToggled on.Value)), horizontalOptions = LayoutOptions.Center)
        //        View.Slider(minimumMaximum = (0.0, 10.0), value = double model.Step, valueChanged = (fun args -> dispatch (SetStep (int (args.NewValue + 0.5)))), horizontalOptions = LayoutOptions.FillAndExpand)
        //        View.Label(text = sprintf "Step size: %d" model.Step, horizontalOptions = LayoutOptions.Center) 
        //        View.Button(text = "Reset", horizontalOptions = LayoutOptions.Center, command = (fun () -> dispatch Reset), commandCanExecute = (model <> initModel))
        //    ]))

    // Note, this declaration is needed if you enable LiveUpdate
    let program =
        XamarinFormsProgram.mkProgram init update view
#if DEBUG
        |> Program.withConsoleTrace
#endif


type App () as app = 
    inherit Application ()

    let runner = 
        App.program
        |> XamarinFormsProgram.run app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
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


