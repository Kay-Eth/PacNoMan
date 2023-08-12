namespace PacNoManFSharp

open Godot
open System.Collections.Generic

type Directions = Top = 0 | Bottom = 2 | Left = 4 | Right = 6

[<AllowNullLiteral>]
type MajorNode() =
    inherit Node2D()
    interface IGraphNode with
        member this.GetPosition: Vector2 = 
            this.Position
        member this.GetName: string =
            this.Name

    [<DefaultValue>] val mutable top : MajorNode
    [<DefaultValue>] val mutable bottom : MajorNode
    [<DefaultValue>] val mutable left : MajorNode
    [<DefaultValue>] val mutable right : MajorNode

    [<DefaultValue>] val mutable pathTop : MinorNode[]
    [<DefaultValue>] val mutable pathBottom : MinorNode[]
    [<DefaultValue>] val mutable pathLeft : MinorNode[]
    [<DefaultValue>] val mutable pathRight : MinorNode[]

    override this._Ready() =
        GD.Print("I'm a Major node!")

    member this.GetNeighbours() : MajorNode list = 
        let mutable xs : MajorNode list = []
        if this.top <> null then xs <- (xs @ [this.top])
        if this.bottom <> null then xs <- (xs @ [this.bottom])
        if this.left <> null then xs <- (xs @ [this.left])
        if this.right <> null then xs <- (xs @ [this.right])
        xs

    member this.GetPathToNeighbour(neighbour: MajorNode) : MinorNode[] =
        if (neighbour = this.top) then this.pathTop
        else if (neighbour = this.bottom) then this.pathBottom
        else if (neighbour = this.left) then this.pathLeft
        else if (neighbour = this.right) then this.pathRight
        else failwith "NOT A NEIGHBOUR"
