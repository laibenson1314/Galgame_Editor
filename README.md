# Galgame Editor

A visual novel editor built with **Unity 6**, allowing you to create your own Galgame entirely through plain text scripts — no programming required.

---

## Features

- Script-driven visual novel engine
- Character sprites with emotion switching
- Background management
- Branching story paths with choices
- Multi-ending support

---

## Getting Started

### Requirements

- Unity 6+

### Installation

1. Clone or download this repository
2. Open the project in Unity 6
3. Place your story scripts in `Assets/StreamingAssets/Stories/`
4. Press **Play** to run

> **Important:** The first story script **must** be named `StartingStory.txt`.  
> If you prefer a different name, change the `startingStory` field in `GameManager`.

---

## Writing Your Story

Story scripts are plain `.txt` files placed in `Assets/StreamingAssets/Stories/`.

Each line is either a **command** (starting with `#`), a **route label** (starting with `*`), or **dialogue**.

### Dialogue

```
Mio: Hello, welcome to my story.
Alice: Nice to meet you!
```

### Commands

| Command | Description |
|---|---|
| `#bg <background_name>` | Change the background image |
| `#enter <character_name> <emotion>` | Bring a character onto the screen |
| `#exit <character_name>` | Remove a character from the screen |
| `#change <character_name> <emotion>` | Change a character's displayed emotion |
| `#scene <scene_name>` | Transition to a scene |
| `#jump <story_name>` | Jump to another story script |
| `#end <ending_name>` | Display an ending image and finish the story |
| `#choice` ... `#choice` | Present a branching choice to the player |

> Always end a script with either `#jump` or `#end`.

---

## Command Reference

### `#bg <background_name>`

Changes the current background.

- Source path: `Assets/Resources/Backgrounds/<background_name>.png`

```
#bg rooftop_night
```

---

### `#enter <character_name> <emotion>`

Brings a character onto the screen with the specified emotion sprite.

- Character folder: `Assets/Resources/Characters/<character_name>/`
- Sprite file: `Assets/Resources/Characters/<character_name>/<emotion>.png`

```
#enter mio normal
```

---

### `#exit <character_name>`

Removes the specified character from the screen.

```
#exit mio
```

---

### `#change <character_name> <emotion>`

Switches the displayed sprite of a character already on screen.

- Sprite file: `Assets/Resources/Characters/<character_name>/<emotion>.png`

```
#change mio blush
```

---

### `#scene <scene_name>`

**For developers.** Pauses the story and launches a custom Unity scene — useful for minigames, interactive sequences, or any gameplay that goes beyond dialogue. Once the custom scene is done, call the following from your own code to return and continue the story from where it left off:

```csharp
SceneController.Instance.ReturnScene();
```

In the story script:

```
#scene MyMinigame
```

> `<scene_name>` must match the Unity scene name exactly and be added to **Build Settings**.

---

### `#jump <story_name>`

Jumps to another story script file.

- Target file: `Assets/StreamingAssets/Stories/<story_name>.txt`

```
#jump Scene2
```

---

### `#end <ending_name>`

Displays an ending image and concludes the story.

- Image file: `Assets/Resources/Endings/<ending_name>.png`

```
#end demo_end
```

---

### `#choice`

Presents the player with a set of choices, each branching to a route label defined in the same script. Wrap options between two `#choice` lines.

```
#choice
Hold her tight => ending_hug
Kiss her directly => ending_kiss
Should I prepare my will first? => ending_joke
#choice
```

- The text before `=>` is the button label shown to the player
- The text after `=>` is the route label to jump to (defined with `*`)

---

### `*<label>`

Defines a route label that `#choice` options can branch to. Place it anywhere below the `#choice` block.

```
*ending_hug
#change mio blush
Mio: ……You idiot……doing that so suddenly……
#jump DemoEnd
```

---

## Example Script

```
#bg rooftop_night
#enter mio normal

Mio: ……Alone again, as always.
Mio: ——Hey, new kid. Want me to "devour" you?

#choice
Run away => route_run
Stay and talk => route_talk
#choice

*route_run
Mio: Heh……coward.
#jump EndingA

*route_talk
#change mio blush
Mio: ……You actually stayed.
#jump EndingB
```

---

## Project Structure

```
Assets/
├── Resources/
│   ├── Backgrounds/        # Background images (.png)
│   ├── Characters/
│   │   └── <name>/         # One folder per character
│   │       └── <emotion>.png
│   └── Endings/            # Ending images (.png)
└── StreamingAssets/
    └── Stories/            # Story scripts (.txt)
        └── StartingStory.txt
```

---

## License

This project is open source. See [LICENSE](LICENSE) for details.