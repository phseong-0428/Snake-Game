module Logic

open System
open Domain

let rnd = Random()

// Changing Direction Logic - Not allowed opposite direction
let changeDirection (currentDir: Direction) (newDir: Direction) =
    match currentDir, newDir with
    | Up, Down | Down, Up -> currentDir
    | Left, Right | Right, Left -> currentDir
    | _ -> newDir

// find Empty position and spawn new apple
let rec spawnApple (snake: Position list) (existingApples: Apple list) (aType: AppleType) : Apple =
    let x = rnd.Next(1, boardWidth)
    let y = rnd.Next(1, boardHeight)
    let pos = { X = x; Y = y }
    
    // Is there already any apple or snake?
    let inSnake = snake |> List.exists (fun p -> p = pos)
    let inApples = existingApples |> List.exists (fun a -> a.Pos = pos)
    
    // If there is already apple or snake, recursion spawnApple
    if inSnake || inApples then
        spawnApple snake existingApples aType
    else // Otherwise, return new Apple
        { Pos = pos; AppleType = aType; SpawnTime = DateTime.Now }

// init State
let initialState () =
    let initialSnake = [ { X = 20; Y = 10 }; { X = 19; Y = 10 } ] // Snake start at middle point
    let firstApple = spawnApple initialSnake [] Normal // First Normal Apple

    // Set Initial GameState
    {
        Snake = initialSnake
        CurrentDirection = Right
        Apples = [ firstApple ]
        Score = 0
        NormalApplesEaten = 0
        Status = Playing
    }

// Calculate Next Position
// Input : Current Position, Direction
// Output : New Position
let getNextHead (head: Position) (dir: Direction) =
    match dir with
    | Up -> { head with Y = head.Y - 1 }
    | Down -> { head with Y = head.Y + 1 }
    | Left -> { head with X = head.X - 1 }
    | Right -> { head with X = head.X + 1 }

// Update Game State
let update (state: GameState) =
    if state.Status = GameOver then state // GameOver -> Return Current State
    else
        // ============================= Colision Check ================================
        let head = List.head state.Snake // Current head position
        let nextHead = getNextHead head state.CurrentDirection // Get Next Head Position

        // Collision Check
        let isWallHit = nextHead.X <= 0 || nextHead.X >= boardWidth || nextHead.Y <= 0 || nextHead.Y >= boardHeight
        
        // Collision Check with its body
        let bodyWithoutTail = if state.Snake.Length > 0 then state.Snake |> List.take (state.Snake.Length - 1) else []
        let isBodyHit = bodyWithoutTail |> List.exists (fun p -> p = nextHead)

        // If collision, GameOver
        if isWallHit || isBodyHit then
            { state with Status = GameOver }
        // ============================= Colision Check ================================
        else
        // ============================= Apple / Eaten Apple ===========================
            // Filtering Apple spawned within 10 seconds
            let now = DateTime.Now
            let aliveApples = state.Apples |> List.filter (fun a -> (now - a.SpawnTime).TotalSeconds <= 10.0)

            // Eat Apple
            let eatenApple = aliveApples |> List.tryFind (fun a -> a.Pos = nextHead)
            let remainingApples = aliveApples |> List.filter (fun a -> a.Pos <> nextHead)

            // Whether Eating apple or not
            match eatenApple with
            | Some apple ->
                // When EatenApple exists, does not cut tail
                let newSnake = nextHead :: state.Snake // Add new head
                let scoreInc = match apple.AppleType with | Normal -> 1 | Special -> 3 // Increasement Score
                let newScore = state.Score + scoreInc // Calculate Score
                
                // EatenApple +1
                let newNormalEaten = 
                    if apple.AppleType = Normal then state.NormalApplesEaten + 1 
                    else state.NormalApplesEaten

                // Spawn new Apple
                let newApples = 
                    if apple.AppleType = Normal then // Spawn new Apple only if type of EatenApple is Normal
                        let withNormal = (spawnApple newSnake remainingApples Normal) :: remainingApples
                        // Each 5 EatenApple, spawn new special apple
                        if newNormalEaten > 0 && newNormalEaten % 5 = 0 then
                            (spawnApple newSnake withNormal Special) :: withNormal
                        else withNormal
                    else
                        remainingApples // No spawn when type of eaten apple is Special

                { state with 
                    Snake = newSnake
                    Apples = newApples
                    Score = newScore
                    NormalApplesEaten = newNormalEaten }
                    
            | None ->
                // No EatenApple -> Add new head, and "Cut Tail"
                let newSnake = nextHead :: bodyWithoutTail
                
                // When there is no Normal Apple (due to timer rule), Add normal apple
                let hasNormalApple = remainingApples |> List.exists (fun a -> a.AppleType = Normal)
                let finalApples = 
                    if not hasNormalApple then (spawnApple newSnake remainingApples Normal) :: remainingApples
                    else remainingApples

                { state with Snake = newSnake; Apples = finalApples }
        // ============================= Apple / Eaten Apple ===========================