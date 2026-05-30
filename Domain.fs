module Domain
// Module.....

open System
// For DateTime(timer)

// Board Size
let boardWidth = 40
let boardHeight = 20

// Coordination, Size is determined -> Can eval with int
type Position = { X: int; Y: int }

// Direction
type Direction = Up | Down | Left | Right

// Apple, Type(Normal vs Special) and Creation Info.
type AppleType = Normal | Special

type Apple = {
    Pos: Position
    AppleType: AppleType
    SpawnTime: DateTime
}

// Game Status
type GameStatus = Playing | GameOver

// Entire Game State
type GameState = {
    Snake: Position list       // Use First element with Head
    CurrentDirection: Direction // Direction
    Apples: Apple list         // List of all apples
    Score: int                 // Current Score
    NormalApplesEaten: int     // Used when make Special Apple
    Status: GameStatus         // Game Over?
}