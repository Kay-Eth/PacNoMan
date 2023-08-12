namespace PacNoManFSharp

open Godot

[<AllowNullLiteral>]
type MinorNode() =
    inherit Node2D()
    interface IGraphNode with
        member this.GetPosition: Vector2 = 
            this.Position
        member this.GetName: string =
            this.Name

    [<DefaultValue>] val mutable top : IGraphNode
    [<DefaultValue>] val mutable bottom : IGraphNode
    [<DefaultValue>] val mutable left : IGraphNode
    [<DefaultValue>] val mutable right : IGraphNode

    override this._Ready() =
        GD.Print("I'm a Major node!")