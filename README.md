# `SLIMEDOWN SHOOTER`

## `Introduction`
### `Game Summary Pitch`
Your life depends on slimes, and so does your death...  

![islands-2](https://github.com/milselarch/slimedown-shooter/assets/11241733/17ed31dd-e6ee-49dc-8534-a29fe806df95)


https://github.com/user-attachments/assets/fcb4c4d7-cc55-44b8-915a-da4a8135ecc8



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

### `Gameplay Overview`

Simple procedure on how to navigate in your game

Use WASD to move.  
click in the direction you want to shoot slimes at
when slimes die they leave behind a slime ball. walk up to the slime ball to
collect it and recharge your health. Shooting at slime balls destroys them. 
Press space to launch a charge attack in the direction of the mouse pointer 
(does 2 damage to slimes, they have 4 health each)
shooting and charging slimes cost health, so you can't just spam the attacks.

After clearing 1 wave of slimes, a new wave will spawn. Each wave has 1 more slime 
than the previous theres a time limit for each level (above the wave number) - 
when you run out of time you will start taking damage every 3 seconds.
You lose when you run out of health. 

### `Game Mechanics`

1. Movement and slime ball collection  
Use WASD to move around in the island, and the player picks up a slime ball when
they come into contact with it.
2. Shooting mechanic  
Click to fire a blaster shot at slimes, at the cost of 2 health. 
If it hits a slime they take damage. If a slime gets shot and its health goes down
to 0 then the the slime degenerates into a slime ball.
If the blaster shot hits a slime ball then it is destroyed.
3. Charging mechanic  
Press space to launch a charge attack in the direction of the mouse pointer.
If you hit a slime moving towards you while charging you deal damage to the slime
You can also charge through caps between points in the island, and won't fall if you're
not on the island for as long as the charge attack is still playing.
4. Slime movement and attack  
Slimes can hop from place to place towards the player with a timed cooldown 
between hops, and also do a longer-range charge attack towards the player when
it gets close enough to the player. If the slime hits the player during regular movement
or during the charge attack, then the slime takes damage instead of the player
5. Falling from the island  
If the player is outside the boundaries of the island, then they can fall and die. 
This can happen when the player charges out of the island, walks past the edges of
the island or gets pushed off the edge by slimes.

## `Art`

### `Design`
Pixel art for everything - UI, game world and sprites. 
Sprites and game world should be bright, colorful and crisp.

## `Audio`

### `Music`
Music should be retro and have action vibes

### `Sound Effects`
Sound effects should be retro and synthesized rather than realistic

## `Game Experience`

### `UI`

An in-game overlay will show the score, health, wave number and time left for 
the current attack wave.

### `Controls`
1. Keyboard: 
   1. `WASD`: movement
   2. `Space`: charge in direction of cursor screen position
   3. `Left Mouse Click`: fire a blaster shot

## `Features Implementation`

### `Features Implemented`
1. create game world tile map
2. create collider tile map with house
3. design, create, enemy prefab and behavior
4. enemy slime animator with idle, bouncing, charging clips
5. slime enemy has internal health system (4 hp) and can take and deal damage from player
6. add idle, moving, charge animation to player. 
UI shows character health and score, updated in real time
7. create 2D canvas with health and score and wave number and timer
8. created health scriptable object that is shared between UI, player, and enemy scripts 
9. slimes spawn in waves and each wave starts after you clear the last one
10. basic combat system, player can shoot and charge attack, slimes can charge attack,
combat costs health (as in launching the attack drains your health also)
11. movement, charge attack, aim and shoot all use InputSystem
12. use scriptable object game architecture for communicating health, 
score changes, and when enemies are killed
13. create a timer where player health wil be drained after the timer runs out 
(resets at the start of each new wave)
14. smooth edge transitions for island tiles
15. enable URP shader rendering pipeline
16. make slime balls shine in and out using URP shader
17. Show when damage is taken by player (sprite turns red)
18. show health bar of player (URP shader turns x% of player green from the top)
19. add in game crosshair, hide default mouse pointer when in-game
20. show charge cooldown on crosshair 
21. refactor in-game HUD overlay with UIDocument
22. add a pause menu w/ main menu button
23. add slime animation to game loading screen, make game loading async
24. debug wave spawning bug
25. implement game restart functionality 
26. make player fall off the map when he's past the island boundaries 
(except when charging)
27. debug charge attack
(only inflict damage to slime if velocities are opposite or displacement 
vector aligns with player velocity vector)
28. enemy death counting refactor (use GameState to track alive slimes)
29. parallax clouds effect tied to time and movement
30. have camera detect from player after they drop far enough
31. use AI navigation for slimes
32. create license file for repo
33. add restart button in pause menu
34. (bugfix) stop player movement being carried over on restart
35. add bomb slime enemy type
36. menu: overhaul start menu to use ui toolkit

### `Features todo`
1. Add URP shader for charge attack
2. menu: Add windowed mode, full screen mode
3. menu: Exit game option
4. menu: master and music audio sliders
5. Add instructions page 
6. Add initial instructions overlay 
7. have slime ball shine timings be relative to time of slime ball degeneration
8. Create a nice starter menu screen

### `Future Potential Features todo`
1. refactor game over menu using UIDocument
2. dynamic tile map rules-based generation
3. day / night cycle
4. more weapon types
5. make slimes be able to fall off the map too

## `Asset Used & Credits`

1. [Cozy farms](https://shubibubi.itch.io/cozy-farm) asset usages:
    1. tile map:
       1. dirt, water, and islands were taken from the cozy farms global sprite sheet
       2. all variants of flowers were from cozy farm tiles folder        
       3. house in middle of map is from cozy farms buildings sprite sheet
    2. slime sprites were taken from cozy farm enemies (green slime folder)
2. Player sprite are from pixel-adventure-1, main characters / virtual guy sprite sheet
[pixel adventure pack](https://assetstore.unity.com/packages/2d/characters/pixel-adventure-1-155360)
3. Sound effect when blaster shot is fired is from 
`shapeforms-audio-effects/Sci Fi Weapons Cyberpunk Arsenal
Preview/AUDIO/EXPLDsgn_Implode_15.wav`
[Shapeforms Audio Effects Page](https://shapeforms.itch.io/shapeforms-audio-free-sfx/download/v9ETHAwN2i4n_Lt51l6pOZda0idBf_xsxjjzGHro)
4. Parallax pixel art clouds background is from 
[Free Sky Backgrounds](https://free-game-assets.itch.io/free-sky-with-clouds-background-pixel-art-set)
5. Background music is [Chiptronical by Patrick De Arteaga](https://patrickdearteaga.com/arcade-music/)



