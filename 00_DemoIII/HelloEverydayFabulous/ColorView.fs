// Copyright Fabulous contributors. See LICENSE.md for license.
namespace HelloEverydayFabulous

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module ColorView = 
    type Model = 
      { Color: Color }

    //type Msg = 
    //    | GoBack 

    let initModel = { Color=Color.White }

    let init () = initModel, Cmd.none

    let executeGoBack =
        async { do! Async.Sleep 200 }
        |> Cmd.ofAsyncMsg

    //let update msg model =
    //    match msg with
    //    | GoBack -> model, executeGoBack
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
        View.ContentPage(
          backgroundColor = model.Color,
          content = View.StackLayout(padding = Thickness 20.0, verticalOptions = LayoutOptions.Center,
            children = [ 
                View.Button(text = "Go Back", command = (fun () -> dispatch GoBack), horizontalOptions = LayoutOptions.Center)
            ]))

