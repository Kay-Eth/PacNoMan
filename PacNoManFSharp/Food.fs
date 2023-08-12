namespace PacNoManFSharp

open Godot
open System.Collections.Generic;

[<AbstractClass>]
type Food() = class
    inherit StaticBody2D()

    static let mutable foodCount = 240

    abstract member EndGame : unit -> unit

    member this.EatFood(): unit =
        foodCount <- foodCount - 1
        if foodCount = 0 then
            this.EndGame()
        this.QueueFree()

end