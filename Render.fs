module Render

open System
open Domain

// board 크기 정의
let boardWidth = 40
let boardHeight = 20

// 좌표 & 문자 받아서 해당 좌표에 문자를 출력해주는 helper 함수
let drawAt x y (text: string) =
    // Safety code (whether x and y in board)
    if x >= 0 && x < Console.WindowWidth && y >= 0 && y < Console.WindowHeight then
        Console.SetCursorPosition(x, y) // SetCursorPosition
        Console.Write(text) // Write

// Drawing Boarderline
let drawBoard () =
    // Upper/Lower borderline
    [0 .. boardWidth] |> List.iter (fun x ->
        drawAt x 0 "#"
        drawAt x boardHeight "#"
    )
    
    // Side borderline
    [0 .. boardHeight] |> List.iter (fun y ->
        drawAt 0 y "#"
        drawAt boardWidth y "#"
    )

// Snake
let drawSnake (snake: Position list) =
    match snake with
    | head :: tail ->
        // first element, head
        // Draw with "O"
        drawAt head.X head.Y "O"
        // Otherwise, Body
        // Draw with "o"
        tail |> List.iter (fun pos -> drawAt pos.X pos.Y "o")
    | [] -> ()

// Apple
let drawApples (apples: Apple list) =
    apples |> List.iter (fun apple ->
        let symbol = 
            match apple.AppleType with
            | Normal -> "*" // Normal Apple
            | Special -> "$" // Special Apple
        drawAt apple.Pos.X apple.Pos.Y symbol
    )

// Score
let drawScore (score: int) =
    // Location : Below the board (boardHeight + 1)
    drawAt 0 (boardHeight + 1) (sprintf "Score: %d" score)

// Game Over
let drawGameOver (score: int) =
    let msg = " Game Over "
    // Location : Middle of the board
    drawAt (boardWidth / 2 - 5) (boardHeight / 2) msg // Message (Game Over)
    drawAt (boardWidth / 2 - 5) (boardHeight / 2 + 1) (sprintf "Final Score: %d" score) // Final Score

// Render
let render (state: GameState) =
    Console.CursorVisible <- false // Make Cursor Invisible
    Console.Clear() // Clear and Render each Frame
    
    drawBoard ()
    drawApples state.Apples
    drawSnake state.Snake
    drawScore state.Score
    
    // If Game Over, Add drawGameOver
    if state.Status = GameOver then
        drawGameOver state.Score