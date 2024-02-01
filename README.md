# `SLIMEDOWN SHOOTER`
## `Introduction`

### `Game Summary Pitch`
Your life depends on slimes, and so does your death...  

![277063858-a65542ee-675f-40b1-9dfd-fdc86971c445](https://github.com/milselarch/slimedown-shooter/assets/11241733/60d1f43b-d3b9-4851-8150-c37c25d50a4c)
Slimedown shooter is a top down 2D shooter game where the main objective 
is to survive the endless waves of slimes that spawn in and collect the slime 
particles they leave behind to replenish your health. The catch is that you also consume 
health when shooting them, getting attacked by slimes, or when charging them 
(health has become a resource).

### `Inspiration`
1. Boxhead is a 2.5D shooter game where you need to survive waves of zombies and devils
2. Noita is a 2D rouguelite where you kill yourself easily interacting 
with the world and where every pixel is simulated, leading to a very 
chaotic and dynamic environment

### `Player Experience`
On an island in the sky you gotta survive the endless slime hordes that come at
you for as long as possible. You survive the waves of slimes by shooting and 
charging at them, and when they die they turn into slime balls that you can use 
to replenish your health. Deciding how and when to attack and avoid the slimes 
must be done tactically to ensure survival.

### `Platform`
The game is developed to be released on windows and linux (Ubuntu 20.04) PC

### `Development Software`
- Unity 2022.3.9f1
- Code Rider for writing C# code
- GIMP for editing sprites

### `Genre`
Singleplayer, shooter, casual

### `Target Audience`
Without heavy or complicated ideas, and intuitive-to-grasp mechanics, 
this game is marketed to at least casual game players who are up for shooting
slimes and dodging their attacks to survive as long as possible and get
a high score.

## `Concept`

### Gameplay Overview

Simple procedure on how to navigate in your game

Use WASD to move.
click in the direction you want to shoot slimes at
when slimes die they leave behind a slime particle. walk up to the slime particle to
collect it and recharge your health. Press space to launch a charge attack in the
direction of the mouse pointer (does 2 damage to slimes, they have 4 health each)
shooting and charging slimes cost health, so you can't just spam the attacks.

After clearing 1 wave of slimes, a new wave will spawn. Each wave has 1 more slime 
than the previous theres a time limit for each level (above the wave number) - 
when you run out of time you will start taking damage every 3 seconds.
You lose when you run out of health. 

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

orange rules:        
40 - I added slime wave spawning because I thought its very natural to have. slimes are not very intimidating by themselves, what makes them dangerous in most games is the fact that theres a lot of them, and so this felt like an elegant rule. By having the number of enemies spawned increase each wave it naturally makes the game harder over time as well     
44 - user experience is improved massively by slimes and the player being able to attack one another because that forms the central conflict and mechanic of the game, otherwise there wouldn't even be a game at all     
45 - InputSystem helps with code maintainance as porting the game to a different device will not require any code changes most likely as far as handling player input data is concerned    

pink rules:         
74 - used scriptable game object architecture mostly for game maintainance reasons. It's very nice to use events to communicate between player and enemy prefab instantiations, or player to UI      
59 - I added a timer for each slime attack wave. The players health will start to drain after the time runs out. This disincentivises the player from just running around the slimes and slowly picking them off one by one as he know has to engage in them more aggressibely in order to defeat them without dying himself. Adds a sense of urgancy to the game in my opinion.    

## Notes

Any other notes you would like to add here
Not sure why the font isnt working in the exported game, but its functional regardless
Used a tilemap for the game background, and a collision tilemap to set the boundaries of the world
was originally planning to do node 69 (AI navigation) but the problem I faced was that that module is specific with 3D models only

## Asset Used & Credits

It’s nice to give **credits** to the creator of the assets (if info is available).
tilemap:
dirt, water, and islands were taken from the cozy farms global spritesheet      
all variants of flowers were from cozy farm tiles folder       
slime sprites were taken from cozy farm enemies (green slime folder)            
house in middle of map is from cozy farms buildings sprite sheet    

player sprite are from pixel-adventure-1, main characters / virtual guy sprite sheet     
fireball that player shoots out has its sprite from gothicvania church files/gothicvania church files/SPRITES/fx/fireball
finally sound effect when fireball is shot out is from everyday-stuff-sfx/shapeforms-audio-effects/Sci Fi Weapons Cyberpunk Arsenal Preview/AUDIO/EXPLDsgn_Implode_15.wav




