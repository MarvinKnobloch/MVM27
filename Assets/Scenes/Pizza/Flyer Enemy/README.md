# Design
A flying enemy that shoots projectiles at the target.

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