# Roller
The roller basically rolls along the ground from its start point to a waypoint.
The roller cannot go up/down, only left/right.

## Gizmos
- red line is the vision line. It looks for the player in that line.
- Freeze On Hit Time is basically when the roller stops moving for a brief moment after getting hit
- Waypoint is the position the roller will move to. Once it reaches it, it will go back to start.
- blue dots are the Start Position & Waypoint. So the roller will move between the dots.

## Combat
- Moves left/right along from start point to waypoint
- Raycasts forward to check for player
- If player is detected, it will speed up towards the player and ram them
- If player is hit
    - It temporarily turns off collision so that it can "go through" the player
    - A configurable damage timer will start, that prevents damaging the player again
- If player is not hit, it will return to normal speed

## Visuals
- We have Idle, Hit, and Die animations.

## Notes
- How to handle situations where dev put waypiont in position roller cannot get to?
    - For now, the roller will simply just spin forever in the direction its trying to go.
- I think it would be fun to add "run away speed". Something where if the player hits the roller, they try to speed away.