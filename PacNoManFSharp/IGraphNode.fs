namespace PacNoManFSharp

open Godot

[<AllowNullLiteral>]
type IGraphNode = 
    abstract member GetPosition : Vector2
    abstract member GetName : string
