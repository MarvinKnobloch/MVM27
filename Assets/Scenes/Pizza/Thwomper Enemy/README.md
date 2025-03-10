# Thwomper
The Thwomper patrols along a path. It constantly checks for the player
below them, and will fall if it detects the player. If it smashes the player
it will shove the player out of the way. The Thwomper is only damagable while on the ground.
Thwomper will wait a specified time before going back into the air.

## Combat
- Uses a raycast down to check for the player
- Once the player is detected, a configurable delay timer wills start
- The Thwomper will fall, regardless if the player is still there or not
- After falling, a configurable return timer will start
- The Thwomper is only damageable while on the ground
- Once returning to the air, it will either continue patrolling, or go back to falling on the player
    - There is a slight delay after rising before it continues patroling or falling

## Visuals
- TODO: Ask team what the animations are for Idle, Falling, Hit, Death