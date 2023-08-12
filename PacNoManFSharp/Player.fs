namespace PacNoManFSharp

open Godot
open System.Collections.Generic;

[<AbstractClass>]
type Player() = class
    inherit Area2D()

    [<Literal>]
    let SPEED : int = 36

    [<Literal>]
    let TILE_SIZE : float32 = 8.0f

    let TopRay : Vector2 = Vector2(0.0f, -TILE_SIZE)
    let DownRay : Vector2 = Vector2(0.0f, TILE_SIZE)
    let LeftRay : Vector2 = Vector2(-TILE_SIZE, 0.0f)
    let RightRay : Vector2 = Vector2(TILE_SIZE, 0.0f)

    let mutable lastVisitedMajorNode : MajorNode = null
    let mutable lastVisitedMinorNode : MinorNode = null

    [<DefaultValue>] val mutable PlayerRayCast : RayCast2D

    member this.LastVisitedMajorNode
        with get () = Utils.LastPlayerMajorNode
        and set (value) = Utils.LastPlayerMajorNode <- value
    member this.LastVisitedMinorNode
        with get () = Utils.LastPlayerMinorNode
        and set (value) = Utils.LastPlayerMinorNode <- value

    abstract member SendLastVisitedMajorChangedSignal : unit -> unit
    abstract member SendLastVisitedMinorChangedSignal : unit -> unit
    abstract member SendEndGameSignal : unit -> unit

    abstract member ChangeAnimationFrame : unit -> unit

    [<DefaultValue>] val mutable StartAnimationFrame : int

    let mutable isMoving = false
    let mutable direction = Vector2(0.0f, 0.0f)
    let mutable distanceToWalk = 0.0f

    override this._Process(delta : float32) : unit =
        Utils.PlayerPosition <- this.Position
        if not (isMoving) then
            if Input.IsKeyPressed(int KeyList.Up) then
                if this.StartAnimationFrame <> int Directions.Top && this.StartAnimationFrame <> (int Directions.Top + 1) then
                    this.StartAnimationFrame <- (int Directions.Top)
                    this.ChangeAnimationFrame()

                direction <- Vector2.Up
                this.PlayerRayCast.CastTo <- TopRay
                this.PlayerRayCast.ForceRaycastUpdate()
                if not (this.PlayerRayCast.IsColliding()) then
                    isMoving <- true
            else if Input.IsKeyPressed(int KeyList.Down) then
                if this.StartAnimationFrame <> int Directions.Bottom && this.StartAnimationFrame <> (int Directions.Bottom + 1) then
                    this.StartAnimationFrame <- (int Directions.Bottom)
                    this.ChangeAnimationFrame()

                direction <- Vector2.Down
                this.PlayerRayCast.CastTo <- DownRay
                this.PlayerRayCast.ForceRaycastUpdate()
                if not (this.PlayerRayCast.IsColliding()) then
                    isMoving <- true
            else if Input.IsKeyPressed(int KeyList.Left) then
                if this.StartAnimationFrame <> int Directions.Left && this.StartAnimationFrame <> (int Directions.Left + 1) then
                    this.StartAnimationFrame <- (int Directions.Left)
                    this.ChangeAnimationFrame()

                direction <- Vector2.Left
                this.PlayerRayCast.CastTo <- LeftRay
                this.PlayerRayCast.ForceRaycastUpdate()
                if not (this.PlayerRayCast.IsColliding()) then
                    isMoving <- true
            else if Input.IsKeyPressed(int KeyList.Right) then
                if this.StartAnimationFrame <> int Directions.Right && this.StartAnimationFrame <> (int Directions.Right + 1) then
                    this.StartAnimationFrame <- (int Directions.Right)
                    this.ChangeAnimationFrame()

                direction <- Vector2.Right
                this.PlayerRayCast.CastTo <- RightRay
                this.PlayerRayCast.ForceRaycastUpdate()
                if not (this.PlayerRayCast.IsColliding()) then
                    isMoving <- true
            distanceToWalk <- TILE_SIZE
        else
            let mutable toWalk = delta * (float32 SPEED)
            
            if distanceToWalk < toWalk then
                this.Translate(direction * distanceToWalk)
                isMoving <- false
            else
                this.Translate(direction * toWalk)
                distanceToWalk <- distanceToWalk - toWalk
    
    member this.ResetDistance(): unit =
        isMoving <- false
        distanceToWalk <- TILE_SIZE
end