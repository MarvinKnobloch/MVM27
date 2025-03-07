# Flyer
A flying enemy that shoots projectiles at the target.

# HOW TO CONFIG

## Terms
- **Idle**: flyer is not in combat
- **Alert**: flyer is aware of a enemy
- **Attack Stages**: things like PreAttack, Attack, PostAttack, Hit, Death

## In Editor Circles
- **Green**: wander area. This is the area the flyer will wander around while idle.
- **Blue**: vision area. If the player steps in this zone, and has line of sight, they will be detected.
- **Yellow**: combat reposition area. When the flyer is alert, this is the area they will reposition in while fighting.
- **Red** attack range. Player must also be visible before flyer attacks.

# Configuration
- The idle configuration can be set to whatever is desired. Note that patrol points only matter if the flyer is in patrol movement type.
- Overall, any settings here can be adjusted as desired.
- The layer masks are important, and likely to be adjusted as the game is built. They make sure collision and vision are correct.













# Development & Design
Note. The below design is what I origionally did. Stuff has changed since I simplified the flyer and removed some features. See notes below.

## Movement
*This section details movement while not alert*
- **Wander**: Wanders a medium distance from their start location. Will attack on sight and chase a distance.
- **Gaurd**: Stays in one place, facing one direction. Low range of chase. Acts defensive, not offensive.
- **Patrol**: Moves from point to point. Low range of chase. Smaller chase distance than wander, returns to patroling after attack.
### Notes
- configurable max distance to chase player
- array of transforms for patrol
- start position is assumed to be home position

## Detection
### Terms
- **Suspicous**: Thinks a target is near and is investigating
- **Alert**: Knows of target and is in combat mode
- **Lost**: Lost target and is returning to previous position (at faster speed)
### Methods
- **Line of Sight**: If the target is in sight, they will become **Alert**
- **Notify**: If anything triggers near the enemy (like casting an attack, or tripping a sensor). This will make the enemy **Suspicous**.
- **Hit**: If the platargetyer hits the enemy, they will become **Alert*. 
### Notes
- configurable range for line of sight
- configurable range for notify detection.
- line of sight will be raycast to the player
- it is assumed no angle is needed for the line of sight (just distance)
- notify will be event based (if we implement entity system)

## Combat
### Shooting
Combat is to consist of shooting projectiles. There will be 3 options:
- **Standard Shot**: a single shot periodically
- **Double Shot**: shoots 2 projectiles at once
- **Quick Shot**: rapid fire shots that deal half damage
### Shoot & Move
- **Never**: enemy will stay idle if in range to attack the player
- **Sometimes**: enemy will move every 2 shots
- **Frequent**: enemy will move every shot
### Notes
- it is assumed simple projectile style shots will suffice
- once the enemy is alert, it is assumed they will shoot at any angle
- for simplicity, enemy will shoot at target even if they are no longer line of sight
- will chase target if they are now a configurable distance away
- there is a max chase distance before the enemy gives up and double times back
- it assumed the enemy will not heal when returning form Alert -> Normal

## Visuals
### Movement Animations
- **Float**: Basically the sprite moves up and down as if floating up and down. controlled via script so we can turn it off/on based on combat.
- Idle & Walking (not in combat) flying animation with float
- When alert or suspicous flying animation without float
### Sounds
- Buzzing noise when target within range. Will get louder as you get closer
- Alert sound when target becomes alert
- Shoot sound for every shot (same sound for each type)






## Extra Notes
### Multiple Enemies At Once
I am not sure what to do when enemies swarm onto each other. 
Logic to ensure they chose a location that is a not layered ontop of each other might be a pain for just a jam.
For now, I will assume this is something we return to at the end if needed and we have time.
### Visual Indicator
Do we want a exclamation icon or some similar effect that occurs when an enemy sees the player?





## Refactor
So I decided to refactor this flyer to be much simpler and match the feel of the project.
Below are some main points as to why.
### Suspicous
I am removing the suspicous system for now. Can add back if we decide we want it, but I think goign straight to alert is fine.
### Chase / Lost / Out of Bounds
This required the flyer to do lots of navigating the map. Currently, there is no pathfinding system in.
Without that, there was alot of code needed to ensure the flyer didnt get stuck or act odd.
Adding a pathfinding system in (that checks the tiles and doesnt affect performance) is far beyond
the scope of the current task. 
Thinking about how the player controls work, and the theme of the game so far; I decided an older style AI
is a better match. One that simply wanders around a smal zone. It can do some repositioning after attacking, but that is it.
I will keep the old flyer code in a backup incase we ever want to return to it.
## Shots
I removed explosion shot, just to limit scope of the task.
The single shot and scatter shot should be enough. I am going to remvoe the "player following" logic
and let the designers play with it and tweak the settings. If they request something like "shoot ahead"
or stuff like that, we can do that. But based on the style of the game, I doubt that will be needed. At least
not now.










