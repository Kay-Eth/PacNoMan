namespace PacNoManFSharp

open Godot
open System.Collections.Generic;

type GhostState = GoTo | Follow | Scared | Scatter | Dead | Idle

[<AbstractClass>]
type Ghost() = class
    inherit KinematicBody2D()

    [<Literal>]
    let SPEED : int = 32

    let _path : Queue<IGraphNode> = Queue<IGraphNode>()

    let mutable currentState : GhostState = GhostState.Idle
    let mutable nextState : GhostState = GhostState.Scatter

    let mutable lastVisitedMajorNode : MajorNode = null
    let mutable lastVisitedMinorNode : MinorNode = null

    let mutable currentScatter : int = -1
    let mutable endScatter : bool = false

    let mutable dead : bool = false

    static let mutable topLeftCorner : MajorNode = null
    static let mutable topRightCorner : MajorNode = null
    static let mutable bottomLeftCorner : MajorNode = null
    static let mutable bottomRightCorner : MajorNode = null

    static let mutable major15node : MajorNode = null
    static let mutable major18node : MajorNode = null

    member this.IsDead
        with get() = dead
        and set(value) = dead <- value

    member this.CurrentState = currentState
    member this.NextState = nextState

    member this.SetCurrentState(state: GhostState): unit =
        GD.Print(this.Name, " changing state to ", state)
        currentState <- state
        nextState <- state

    abstract member SendReachedSignal : unit -> unit

    abstract member ScatterPath : IGraphNode[]

    abstract member ChangeAnimationFrame : unit -> unit
    abstract member GetStartMajorNode : unit -> MajorNode

    static member TopLeftCornerNode
        with get() = topLeftCorner
        and set(value) = topLeftCorner <- value
    static member TopRightCornerNode
        with get() = topRightCorner
        and set(value) = topRightCorner <- value
    static member BottomLeftCornerNode
        with get() = bottomLeftCorner
        and set(value) = bottomLeftCorner <- value
    static member BottomRightCornerNode
        with get() = bottomRightCorner
        and set(value) = bottomRightCorner <- value

    static member Major15Node
        with get() = major15node
        and set(value) = major15node <- value

    static member Major18Node
        with get() = major18node
        and set(value) = major18node <- value

    static member GetCornerFromQuarter(quarter: Quarters): MajorNode =
        if quarter = Quarters.TopLeft then topLeftCorner
        else if quarter = Quarters.TopRight then topRightCorner
        else if quarter = Quarters.BottomLeft then bottomLeftCorner
        else bottomRightCorner

    [<DefaultValue>] val mutable StartAnimationFrame : int

    member this.LastVisitedNode
        with get () = lastVisitedMajorNode
        and set (value) = lastVisitedMajorNode <- value
    
    member this.Init(startNode: MajorNode): unit =
        lastVisitedMajorNode <- startNode
        lastVisitedMinorNode <- null

        currentState <- GhostState.Scatter

    member this.GoToScatterMode(): unit =
        endScatter <- false
        currentState <- GhostState.Scatter
        nextState <- GhostState.Scatter

    member this.IsNodeInScatterPath(node: IGraphNode): int =
        let mutable result = -1
        for i = 0 to this.ScatterPath.Length - 1 do
            if node = this.ScatterPath.[i] then
                result <- i
        result

    override this._Process(delta : float32) : unit =
        //-------------- GO TO ----------------------
        if currentState = GhostState.GoTo then
            currentScatter <- -1
            if _path.Count > 0 then
                let arrived : bool = this.MoveTo(_path.Peek().GetPosition, delta)
                if arrived then
                    let lN = _path.Dequeue()
                    if lN :? MajorNode then
                        //if nextState = GhostState.Follow then
                        //    _path.Clear()
                        lastVisitedMajorNode <- lN :?> MajorNode
                        lastVisitedMinorNode <- null
                    else if lN :? MinorNode then
                        lastVisitedMinorNode <- lN :?> MinorNode
                    if _path.Count = 0 then
                        currentState <- nextState
                        this.SendReachedSignal()
            else 
                currentState <- nextState
        //-------------- SCATTER ----------------------
        if currentState = GhostState.Scatter then
            if _path.Count > 0 then
                nextState <- GhostState.Scatter
                currentState <- GhostState.GoTo
            else
                let index = this.IsNodeInScatterPath(lastVisitedMajorNode)
                if index = -1 then
                    nextState <- GhostState.Scatter
                    this.GoToNode(this.ScatterPath.[0] :?> MajorNode)
                else
                    if currentScatter = -1 then
                        currentScatter <- index
                        this.DoScatter(delta)
                    else
                        this.DoScatter(delta)
        //-------------- FOLLOW ----------------------
        if currentState = GhostState.Follow then
            if _path.Count > 0 then
                nextState <- GhostState.Follow
                currentState <- GhostState.GoTo
            else if currentScatter <> -1 then
                nextState <- GhostState.Follow
                currentState <- GhostState.Scatter
                endScatter <- true
            else
                endScatter <- false
                if this.LastVisitedNode <> Utils.LastPlayerMajorNode then
                    this.GoToNode(Utils.LastPlayerMajorNode)
                    nextState <- GhostState.Follow
                else
                    if Utils.LastPlayerMinorNode <> null then
                        for major in this.LastVisitedNode.GetNeighbours() do
                            if this.IsMinorNodeInPath(Utils.LastPlayerMinorNode, this.LastVisitedNode.GetPathToNeighbour(major)) then
                                nextState <- GhostState.Follow
                                for minor in this.LastVisitedNode.GetPathToNeighbour(major) do
                                    _path.Enqueue(minor)
                                _path.Enqueue(major)
                                currentState <- GhostState.GoTo
                    else
                        let direction = Utils.PlayerPosition - this.Position

                        GD.Print(Utils.PlayerPosition)
                        GD.Print(this.Position)

                        let mutable found = false

                        //if lastVisitedMajorNode.Name = "Major15" then
                        //    if direction.x < 0.0f then
                        //        this.GoToNode(major18node)
                        //        found <- true
                        //else if lastVisitedMajorNode.Name = "Major18" then
                        //    if direction.x > 0.0f then
                        //        this.GoToNode(major15node)
                        //        found <- true
                        if (not found) then
                            let direction = Utils.PlayerPosition - this.Position
                            GD.Print(direction)
                            if direction.y = 0.0f then
                                if direction.x > 0.0f then
                                    nextState <- Follow
                                    this.TakePath(Directions.Right)
                                else
                                    nextState <- Follow
                                    this.TakePath(Directions.Left)
                            else if direction.x = 0.0f then
                                if direction.y > 0.0f then
                                    nextState <- Follow
                                    this.TakePath(Directions.Bottom)
                                else
                                    nextState <- Follow
                                    this.TakePath(Directions.Top)
                            else
                                if Mathf.Abs(direction.y) > Mathf.Abs(direction.x) then
                                    if direction.y < 0.0f then
                                        if this.LastVisitedNode.top <> null then
                                            nextState <- Follow
                                            this.TakePath(Directions.Top)
                                        else
                                            if direction.x > 0.0f then
                                                nextState <- Follow
                                                this.TakePath(Directions.Right)
                                            else
                                                nextState <- Follow
                                                this.TakePath(Directions.Left)
                                    else
                                        if this.LastVisitedNode.bottom <> null then
                                            nextState <- Follow
                                            this.TakePath(Directions.Bottom)
                                        else
                                            if direction.x > 0.0f then
                                                nextState <- Follow
                                                this.TakePath(Directions.Right)
                                            else
                                                nextState <- Follow
                                                this.TakePath(Directions.Left)
                                else
                                    if direction.x < 0.0f then
                                        if this.LastVisitedNode.left <> null then
                                            nextState <- Follow
                                            this.TakePath(Directions.Left)
                                        else
                                            if direction.y > 0.0f then
                                                nextState <- Follow
                                                this.TakePath(Directions.Bottom)
                                            else
                                                nextState <- Follow
                                                this.TakePath(Directions.Top)
                                    else
                                        if this.LastVisitedNode.right <> null then
                                            nextState <- Follow
                                            this.TakePath(Directions.Right)
                                        else
                                            if direction.y > 0.0f then
                                                nextState <- Follow
                                                this.TakePath(Directions.Bottom)
                                            else
                                                nextState <- Follow
                                                this.TakePath(Directions.Top)
        //-------------- SCARED ----------------------
        if currentState = GhostState.Scared then
            if _path.Count > 0 then
                currentState <- GhostState.GoTo
                nextState <- GhostState.Scared
            else if currentScatter <> -1 then
                nextState <- GhostState.Scared
                endScatter <- true
                currentState <- GhostState.Scatter
            else
                let playerQuarter = Utils.GetQuarter(Utils.PlayerPosition)
                let ghostQuarter = Utils.GetQuarter(this.Position)

                nextState <- GhostState.Scared
                if playerQuarter = ghostQuarter || playerQuarter = Utils.GetQuarterCross(ghostQuarter) then
                    let mutable targetNode = topLeftCorner
                    if ghostQuarter = Quarters.TopLeft then targetNode <- topRightCorner
                    else if ghostQuarter = Quarters.TopRight then targetNode <- bottomRightCorner
                    else if ghostQuarter = Quarters.BottomRight then targetNode <- bottomLeftCorner
                    this.GoToNode(targetNode)
                else
                    if playerQuarter = Quarters.TopLeft then
                        this.GoToNode(bottomRightCorner)
                    else if playerQuarter = Quarters.TopRight then
                        this.GoToNode(bottomLeftCorner)
                    else if playerQuarter = Quarters.BottomLeft then
                        this.GoToNode(topRightCorner)
                    else
                        this.GoToNode(topLeftCorner)
        //-------------- DEAD ----------------------
        if currentState = GhostState.Dead then
            this.DeadMovement(delta)
        //-------------- IDLE ----------------------
        if currentState = GhostState.Idle then
            if (not dead) then
                currentState <- nextState
            

    member this.MoveTo(destination: Vector2, delta: float32) : bool =
        let mutable toWalk = delta * (float32 SPEED)

        if this.Position.y > 132.0f && this.Position.y < 148.0f then
            if this.Position.x < 44.0f || this.Position.x > 180.0f then
                toWalk <- toWalk / 2.0f

        if nextState = GhostState.Scared then
            toWalk <- toWalk * 0.5f

        let direction = (destination - this.Position).Normalized()

        if (direction.x = -1.0f) then this.StartAnimationFrame <- (int Directions.Left)
        else if (direction.x = 1.0f) then this.StartAnimationFrame <- (int Directions.Right)
        else if (direction.y = -1.0f) then this.StartAnimationFrame <- (int Directions.Top)
        else if (direction.y = 1.0f) then this.StartAnimationFrame <- (int Directions.Bottom)

        let distance = this.Position.DistanceTo(destination)
        
        if distance < toWalk then
            toWalk <- distance
            this.Translate(direction * toWalk)
            true
        else
            this.Translate(direction * toWalk)
            false

    member this.GoToNode(targetNode: MajorNode) : unit =
        let mutable (frontier : (MajorNode * int) list) = []
        
        let enqueue value = frontier <- value :: frontier
        let dequeue() =
            let sorted = frontier |> List.sortBy (fun (_, priority) -> priority)
            frontier <- sorted.Tail
            sorted.Head

        enqueue(lastVisitedMajorNode, 0)
        let cameFrom : Dictionary<MajorNode, MajorNode> = Dictionary<MajorNode, MajorNode>()
        let costSoFar : Dictionary<MajorNode, int> = Dictionary<MajorNode, int>()

        cameFrom.[lastVisitedMajorNode] <- null
        costSoFar.[lastVisitedMajorNode] <- 0

        while not (frontier.Length = 0) do
            let current, _ = dequeue()

            if (current <> targetNode) then
                for next in current.GetNeighbours() do
                    if not (costSoFar.ContainsKey(current)) then costSoFar.[current] <- 0
                    let newCost = costSoFar.[current] + current.GetPathToNeighbour(next).Length
                    if (not (costSoFar.ContainsKey(next))) || (newCost < costSoFar.[next]) then
                        if not (costSoFar.ContainsKey(next)) then costSoFar.[next] <- 0
                        costSoFar.[next] <- newCost
                        let mutable priority = newCost + Mathf.FloorToInt(Mathf.Abs(targetNode.GetPosition().x - next.GetPosition().x) + Mathf.Abs(targetNode.GetPosition().y - next.GetPosition().y))
                        if (targetNode = major15node && next = major18node) then priority <- 1
                        else if (targetNode = major18node && next = major15node) then priority <- 1
                        enqueue(next, priority)
                        cameFrom.[next] <- current

        let mutable curCheck = cameFrom.[targetNode]
        let majorPath = List<MajorNode>()

        majorPath.Add(targetNode)
        while curCheck <> lastVisitedMajorNode do
            majorPath.Add(curCheck)
            curCheck <- cameFrom.[curCheck]

        for minor in lastVisitedMajorNode.GetPathToNeighbour(majorPath.[majorPath.Count - 1]) do
            _path.Enqueue(minor)

        for i = (majorPath.Count - 1) downto 1 do
            _path.Enqueue(majorPath.[i])
            for minor in majorPath.[i].GetPathToNeighbour(majorPath.[i - 1]) do
                _path.Enqueue(minor)
        _path.Enqueue(targetNode)

        currentState <- GhostState.GoTo

    member this.ShortenPath(): unit =
        let toWalk = List<IGraphNode>()
        
        GD.Print(_path.Count);
        while not(_path.Peek() :? MajorNode) do
            toWalk.Add(_path.Dequeue())
        
        _path.Clear()
        
        for node in toWalk do
            _path.Enqueue(node)

    member this.DoScatter(delta: float32) : unit =
        let mutable toWalk = delta * (float32 SPEED)
        if nextState = GhostState.Scared then
            toWalk <- toWalk * 0.5f

        let destination = this.ScatterPath.[currentScatter].GetPosition
        let direction = (destination - this.Position).Normalized()

        if (direction.x = -1.0f) then this.StartAnimationFrame <- (int Directions.Left)
        else if (direction.x = 1.0f) then this.StartAnimationFrame <- (int Directions.Right)
        else if (direction.y = -1.0f) then this.StartAnimationFrame <- (int Directions.Top)
        else if (direction.y = 1.0f) then this.StartAnimationFrame <- (int Directions.Bottom)

        let distance = this.Position.DistanceTo(destination)
        
        if distance < toWalk then
            toWalk <- distance
            this.Translate(direction * toWalk)
            if this.ScatterPath.[currentScatter] :? MajorNode then
                lastVisitedMajorNode <- this.ScatterPath.[currentScatter] :?> MajorNode
                lastVisitedMinorNode <- null
            else
                lastVisitedMinorNode <- this.ScatterPath.[currentScatter] :?> MinorNode
            currentScatter <- currentScatter + 1
            if currentScatter = this.ScatterPath.Length then
                currentScatter <- 0
            if currentScatter = 1 && endScatter then
                currentState <- nextState
                currentScatter <- -1
        else
            this.Translate(direction * toWalk)

    member this.IsMinorNodeInPath(minor: MinorNode, path: MinorNode[]): bool =
        let mutable result = false
        for node in path do
            if minor = node then
                result <- true
        result

    member this.GoToScaredMode() : unit =
        currentState <- GhostState.Scared
        nextState <- GhostState.Scared

    member this.Die() : unit = 
        dead <- true
        currentState <- GhostState.Dead
        nextState <- GhostState.Scatter

        endScatter <- false
        currentScatter <- -1

        _path.Clear()

        let direction = (this.GetStartMajorNode().Position - this.Position).Normalized()
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) then
            if direction.x > 0.0f then
                this.StartAnimationFrame <- int Directions.Right
            else
                this.StartAnimationFrame <- int Directions.Left
        else
            if direction.y > 0.0f then
                this.StartAnimationFrame <- int Directions.Bottom
            else
                this.StartAnimationFrame <- int Directions.Top

    member this.DeadMovement(delta: float32) : unit =
        let mutable toWalk = delta * (float32 SPEED)
        let destination = this.GetStartMajorNode().Position
        let direction = (destination - this.Position).Normalized()

        let distance = this.Position.DistanceTo(destination)
        
        if distance < toWalk then
            toWalk <- distance
            this.Translate(direction * toWalk)
            
            this.Init(this.GetStartMajorNode())
            currentState <- GhostState.Idle
        else
            this.Translate(direction * toWalk)

    member this.TeleportHack(): unit = 
        if _path.Count > 0 then
            _path.Dequeue() |> ignore
        
    member this.TakePath(dir: Directions): unit =
        GD.Print("Take Path: ", dir)
        match dir with
        | Directions.Top -> 
            for minor in this.LastVisitedNode.GetPathToNeighbour(this.LastVisitedNode.top) do
                _path.Enqueue(minor)
            _path.Enqueue(this.LastVisitedNode.top)
        | Directions.Bottom -> 
            for minor in this.LastVisitedNode.GetPathToNeighbour(this.LastVisitedNode.bottom) do
                _path.Enqueue(minor)
            _path.Enqueue(this.LastVisitedNode.bottom)
        | Directions.Left -> 
            for minor in this.LastVisitedNode.GetPathToNeighbour(this.LastVisitedNode.left) do
                _path.Enqueue(minor)
            _path.Enqueue(this.LastVisitedNode.left)
        | Directions.Right -> 
            for minor in this.LastVisitedNode.GetPathToNeighbour(this.LastVisitedNode.right) do
                _path.Enqueue(minor)
            _path.Enqueue(this.LastVisitedNode.right)
        | _ -> ()
        currentState <- GhostState.GoTo
        nextState <- GhostState.Follow
end