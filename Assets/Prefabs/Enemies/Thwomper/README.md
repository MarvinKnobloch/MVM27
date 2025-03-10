# Thwomper
The Thwomper patrols along a path. It constantly checks for the player
below them, and will fall if it detects the player. If it smashes the player
it will shove the player out of the way. The Thwomper is only damagable while on the ground.
Thwomper will wait a specified time before going back into the air.

## Gizmos
- red line is line of sight
- blue dots are the start position and waypoint (what it walks between)

## Combat
- Uses a raycast down to check for the player
- Once the player is detected, it will begin to fall.
- The Thwomper will fall, regardless if the player is still there or not
- After falling, a configurable return timer will start
- The Thwomper is only damageable while on the ground
- Once returning to the air, it will either continue patrolling, or go back to falling on the player

## Visuals
We did not have many animations for the Thwomper, so here is what I did.
- Closed eyes while idle
- Open angry face when player detected and falling
- Will stay angry until it rises or gets hit by the player
- Hit animation flashes a red tint over the sprite
- Death animation fades to transparent

## Notes
- This enemy does not have an invuneralable to block damage for a period after an attack.
    I might add that so you cannot spam click it to die.
- If the thwomper hits the player, it will turn off colision to avoid pushing them through the map.
    If the issue persists, I will change to using a seperate collider for damage detection.