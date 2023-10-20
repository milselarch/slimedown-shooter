[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-24ddc0f5d75046c5622901739e7c5dd533143b0c8e959d652212380cedb1ea36.svg)](https://classroom.github.com/a/Xmv1pZ8x)
# README.md

**LIFE AND DEATH BY SLIMES**

| Name       | ***REMOVED*** |
| ---------- | --- |
| Student ID | ***REMOVED*** |

## Basic Game Description

Genre, main objective, provide screenshot of your scene if you are proud of it

This game is a top down action game (mainly a shooter)
The main objective is to kill waves of slimes that spawn in and collect the slime particles they leave behind to regain health
The catch is that you also consume health when shooting them, getting attacked by slimes, or when charging them (health has becoem a resource)
![Screenshot from 2023-10-21 06-40-03](https://github.com/50033-game-design-and-development/50033-midterm-partb-milselarch/assets/11241733/a65542ee-675f-40b1-9dfd-fdc86971c445)

### Game Executable

**State system requirements: Windows, macOS, etc**

Provide a **link** to download your game executable
[linux x86_64 executable](https://github.com/50033-game-design-and-development/50033-midterm-partb-milselarch/releases/download/v1.0.0/linux-executable.x86_64)

### How to Play

Simple procedure on how to navigate in your game

Use WASD to move.
click in the direction you want to shoot slimes at
when slimes die they leave behind a slime particle. walk up to the slime particle to collect it and recharge your health
press space to launch a charge attack in the direction the player is facing (does 2 damage to slimes, they have 4 health each)
shooting and charging slimes cost health

After clearing 1 wave of slimes, a new wave will spawn. Each wave has 1 more slime than the previous
theres a time limit for each level (above the wave number) - when you run out of time you will start taking damage every 3 seconds.
You lose when you run out of health

### Gameplay Video

A ~60s recorded run of your game from start to finish (you may record from Unity editor, show your Game window clearly). You may provide a **working link, or a gif embedded directly here.**

## Features Implementation

Fill up the table below based on the **features** that you want us to grade you with. You may implement more features than what you can afford as your feature points, so you can select the features that we can grade. Feature prerequisite rule should apply.

You are free to transform the table below into short paragraphs if you’d like. The goal is to ensure that we **can find** and **confirm** each node implementation.
(in the event I am only allowed to choose up to 90 points worth of nodes, I would elect to forfeit the 1st 3 (nodes 27, 18, 37))

| Node ID | Color | Short Description of Implementation | Feature Point Cost | Marks to earn |
| ------- | ----- | ----------------------------------- | ------------------ | ------------- |
|     27   |  white     |       Made sure text menuUI and game UI are readable and accessible                  |            1        |        3       |
|     18   |  white     |       Added PlayerController.cs to manage player logic and resources                   |            1        |        3       |
|     37   |  white     |       PascalCase for methods and camelCase for vars, args are observed                      |            1        |        3       |
|     5   |  white     |           Attached camera controller to player extended it to cover 2 axes                          |            1        |        3       |
|     7    |  white     |     arranged tilemap, game island collider, a house that has collision                                |           1         |      3         |
|     19    |  white     |    designed, created enemy prefab                                 |              1      |           3    |
|    22     | white      |    enemy slime animator with idle, bouncing, charging clips                                 |              1      |        3       |
|    21     |  white     |  slime enemy has internal health system (4 hp) and can take and deal damage from player                                   |   1                 |      3         |
|   17      |   white    |  added idle, moving, charge animation to player                                   |           1         |       3        |
|   23      |  white     |    created 2D canvas with health and score and wave number and timer                                 |           1         |      3        |
|    33     |   white    |  created health scriptable object that is shared betweem UI, player, and enemy scripts                                  |         1           |        3       |
|  36       |  white     |        use conventional commit convention                             |        1            |       3        |
|      24   | white      |    UI shows character health and score, updated in real time                                 |             1       |    3          |
|  40       |  orange      |   slimes spawn in waves and each wave starts after you clear the last one                                  |           2         |        10       |
|    44     |  orange      |    basic combat system, player can shoot and charge attack, slimes can charge attack, combat costs health (as in launching the attack drains your health also)         |     2               |  10             |
|    45     |     orange  |  movement, charge attack, aim and shoot all use InputSystem                                   |            2        |   10            |
|     74    |  pink     |     used scriptable object game architecture for communicating health, score changes, and when enemies are killed                                |          3          |            15   |
|    59     |   pink    |     create a timer where player health wil be drained after the timer runs out (resets at the start of each new wave)                                |                3    |         15      |

**Total Feature Point spent: 25**

**Maximum Marks to earn: 99**

### Feature Tree Visual Representation

Download the feature tree image and indicate the nodes that you have implemented. Display an image of your completed feature tree here, highlight or circle the **nodes** that you have implemented as a visual aid for us when we grade your submission

![midterm-tree](https://github.com/50033-game-design-and-development/50033-midterm-partb-milselarch/assets/11241733/3c57de1e-69fd-4854-b805-bba88b7f8793)

### Feature Analysis

For **each** of your **orange**, **pink** and **purple** nodes, explain clearly your game design justification on how this feature upgrades the **overall quality** of the game. In short, you’re providing a short **analysis**.

- If the feature stated that it has to support a core drive, explain which core drive.
- If the feature does not state anything concrete, it becomes an **open ended feature. Please** use proper terminologies whenever possible.
  - You can argue that this feature forms an **elegant rule**, or
  - It improves the UX of the game, or
  - **It improves code maintenance** overall
- Consult our lecture slides for inspiration and samples on how to concisely **analyse** a game.

## Notes

Any other notes you would like to add here

## Asset Used & Credits

It’s nice to give **credits** to the creator of the assets (if info is available).
