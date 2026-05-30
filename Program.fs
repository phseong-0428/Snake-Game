// For more information see https://aka.ms/fsharp-console-apps
// printfn "Hello from F#"
open System
open Domain
open Logic
open Render

// 1. 키보드 입력 처리 (논블로킹)
let getInput (currentDir: Direction) =
    let rec flushKeys dir =
        if Console.KeyAvailable then // If there is key input in buffer
            let key = Console.ReadKey(true).Key // read the key, and set intercept true (no print)
            let newDir =
                match key with
                | ConsoleKey.W | ConsoleKey.UpArrow -> Up
                | ConsoleKey.S | ConsoleKey.DownArrow -> Down
                | ConsoleKey.A | ConsoleKey.LeftArrow -> Left
                | ConsoleKey.D | ConsoleKey.RightArrow -> Right
                | _ -> dir
            flushKeys newDir // recursion until there is no key input
        else
            dir // if there is no key in buffer, remain current dir

    let nextDir = flushKeys currentDir
    // Cannot move to opposite direction
    Logic.changeDirection currentDir nextDir

// Main Game Loop
let rec gameLoop (state: GameState) =
    // Check key input and update direction
    let newDir = getInput state.CurrentDirection
    let stateWithNewDir = { state with CurrentDirection = newDir }
    
    // Update(move, eat apple) in Logic.fs
    // Should update before rendering
    let nextState = Logic.update stateWithNewDir
    
    // Rendering
    Render.render nextState
    
    // 4) 게임 종료 판정 및 프레임 대기
    // Whether gameover
    if nextState.Status = GameOver then
        () // Stop Recursion
    else
        // Adjust Game Speed(Smaller number, faster the snake)
        System.Threading.Thread.Sleep(150)
        gameLoop nextState // Recursion with next frame

[<EntryPoint>]
let main argv =
    Console.CursorVisible <- false
    
    // Set Initial State
    let startState = Logic.initialState ()
    
    // Render First, and wait a while
    Render.render startState
    System.Threading.Thread.Sleep(1000)
    
    // Start Game Loop
    gameLoop startState
    
    // End Game and Wait
    Console.SetCursorPosition(0, Domain.boardHeight + 3)
    Console.WriteLine("Press Any Key to Terminate")

    // Make Key Buffer Empty
    while Console.KeyAvailable do Console.ReadKey(true) |> ignore 
    Console.ReadKey(true) |> ignore
    
    0 // Terminate
