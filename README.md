# Snake-Game

## How to Run

* This project is built on **.NET 10**, as used in our class.
* If .NET 10 is not installed, please install it first.
* No external libraries or additional packages are used.

Navigate to the root directory (Snake-Game) of this project and execute the following command:

```bash
dotnet run
```

## How to Play

### Board Layout
The game is played on a 40x20 grid surrounded by a `#` border.
* **Snake Head:** `O`
* **Snake Body:** `o`
* **Normal Apple:** `*`
* **Special Apple:** `$`

### Controls
Steer the snake using the **W (Up), A (Left), S (Down), D (Right)** keys or the Arrow keys. 
* The snake moves continuously at a fixed speed.
* You cannot reverse direction directly (e.g., if moving Right, pressing Left has no effect).

### Rules & Scoring
* **Normal Apples (`*`):** Eating one increases your length by 1 and your score by 1. A new normal apple spawns immediately.
* **Special Apples (`$`):** Every time you eat 5 normal apples, a special apple spawns. Eating it increases your length by 1 and your score by 3.
* **Despawn Timer:** Any apple (normal or special) that is not eaten within **10 seconds** will disappear. If a normal apple disappears, a new one spawns immediately.
* **Game Over:** The game ends immediately if the snake's head collides with the `#` border or any part of its own body.

## Project Structure

```text
SnakeGame/
├── Snake-Game.fsproj    # .NET 10 F# project file
├── README.md
├── Domain.fs           # Core types (Position, Direction, Apple, GameState)
├── Logic.fs            # Game rules, state updates, collision, and spawning
├── Render.fs           # Console drawing and UI formatting
└── Program.fs          # Entry point, non-blocking input, and game loop
```

### Module Overview
| Module | Responsibility |
| :--- | :--- |
| **Domain** | Immutable types representing the game state. |
| **Logic** | Functions for updating the snake's position, checking wall/body collisions, and managing 10-second apple timers. |
| **Render** | Clears the console and draws the board, entities, and score. |
| **Program** | The main recursive game loop. |

## LLM Usage Experience

According to the project specification, here is the record of my LLM usage during development:

* **What I used the LLM for:** Since I had no prior experience with console manipulation commands in F#, I heavily relied on the LLM to understand and implement the terminal-based UI. I asked numerous questions about how to use the `System.Console` class to move the cursor (`SetCursorPosition`), hide the blinking cursor (`CursorVisible`), and process keystrokes.
* **Manual changes and reprompting:** When I asked how to get user input for the snake's direction, the LLM initially suggested blocking methods that completely paused the game loop until a key was pressed. I had to reprompt it specifically to ask for a "non-blocking" way to read keyboard inputs so the snake could move continuously, which led to the use of `Console.KeyAvailable`.
* **What the LLM failed to do correctly:** The LLM failed to realize that using standard input functions would print the pressed keys (W, A, S, D) directly onto the terminal, ruining the rendered game board. I had to manually find out and change the code to `Console.ReadKey(true)` to intercept and hide the keystrokes.