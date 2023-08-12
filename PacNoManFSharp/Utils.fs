namespace PacNoManFSharp

open Godot
open System.Collections.Generic;

type Quarters = TopLeft = 0 | TopRight = 1 | BottomLeft = 2 | BottomRight = 3

type Utils() =
    static let mutable lastVisitedMajorNode : MajorNode = null
    static let mutable lastVisitedMinorNode : MinorNode = null
    static let mutable playerPos : Vector2 = Vector2.Zero

    static member LastPlayerMajorNode
        with get() = lastVisitedMajorNode
        and set(value) = lastVisitedMajorNode <- value
    static member LastPlayerMinorNode
        with get() = lastVisitedMinorNode
        and set(value) = lastVisitedMinorNode <- value

    static member PlayerPosition
        with get() = playerPos
        and set(value) = playerPos <- value

    static member GetQuarter(position: Vector2) : Quarters =
        if position.x < 112.0f then
            if position.y < 140.0f then
                Quarters.TopLeft
            else
                Quarters.BottomLeft
        else
            if position.y < 140.0f then
                Quarters.TopRight
            else
                Quarters.BottomRight

    static member GetQuarterCross(quater: Quarters) : Quarters =
        if quater = Quarters.TopLeft then Quarters.BottomRight
        else if quater = Quarters.TopRight then Quarters.BottomLeft
        else if quater = Quarters.BottomLeft then Quarters.TopRight
        else Quarters.TopLeft