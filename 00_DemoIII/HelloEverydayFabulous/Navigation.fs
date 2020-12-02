namespace HelloEverydayFabulous

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

type NavigationMessage = 
    | NavigateToColorView of Color
    | GoBack 
