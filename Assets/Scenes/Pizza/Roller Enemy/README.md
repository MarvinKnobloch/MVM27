# Roller
The roller basically rolls along the ground from its start point to a waypoint.
The roller cannot go up/down, only left/right.

## Combat
- Moves left/right along from start point to waypoint
- Raycasts forward to check for player
- If player is detected, it will speed up towards the player and ram them
- If player is hit, it will push the player back and run through them
    - A configurable damage timer will start, that prevents damaging the player again
- If player is not hit, it will return to normal speed

## Visuals
- We have Idle, Hit, and Die animations.