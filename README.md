# rubiks-cube

This is an implementation of the Rubiks Cube using Unity3d.  I was bored one weekend and decided I wanted to create a Rubiks cube in Unity.

# WebGL Demo

To see the my Rubiks Cube implementation in action, try the Web GL demo link below.  

<https://worthingtonjg.github.io/rubiks-cube/>

Interactions:

- You can *shuffle* the cube.
- You can *reset* the cube (explodes the cube and re-assembles it solved).
- You can swipe any *center* slice of the cube to rotate the cube in the direction swiped.
- Swiping an *edge* slice will rotate that particular slice in the direction swiped.
- You can *undo* your moves with the Undo button.

# Scenes

Demo - There is a single Scene in the project, with the following setup...

**Cameras**

There are 4 cameras in the scene:

- *Main Camera*: This is the main camera used to display the game view
- *Camera X, Camera Y, Camera Z*: Three faces of the cube are not visible from the game view, these cameras are positioned on the three hidden sides and used by the mirrors to show the sides. Each camera targets a seperate render texture (3 total), that projects onto a plane (creating a mirror).
- There are no scripts associated with the cameras.

**Lights**

There are 4 lights in scene, basically one for each camera.  There are no scripts associated with the lights.

**Mirrors**

The mirrors in the scene are simply planes that have a render texture applied to them for each of the hidden sides, the render texture on each of these planes is attached to the X, Y and Z cameras so we can see the hidden sides projected onto these planes.

**TouchController**

The *TouchController* controls user input for the cube as follows:

- *TouchController.cs* - script that controls the user input for the cube.
- *Contains 27 Box Colliders* - one collider for each visible face on the cube, used by the *TouchController.cs* to detect swiping direction.

> Note: There are actually 54 faces on the cube, but only 27 are visible at any one time, the box colliders that represent these 27 visible faces do not rotate with the cube.  Rather they stay in place and are used to detect touch on the visible faces of the cube.
> See *TouchController* Script description below for more details.

**CubeController**

The *CubeController* defines the visual layout of the cube, and controls the cube as follows:

- *CubeController.cs* - script that maintains state, tracks cubies, and controls rotation of the cube and the slices in the cube.
- *Contains 27 Cubies (3x3x3)* - each cubie represents one piece of the cube (1 core, 8 corners, 12 edges, 6 centers).  The center cubie is at 0,0,0 in world space, with each cubie arranged around it exactly 1 unit away.
- Cubie model - created in blender.  Model has 7 materials: one for each side of the cube, and one for the cubie border.

> See *Cubie* and *CubeController* script descriptions below for more details.

**SideColliders**

There are 6 side colliders, one for each side of the cube (foward, back, up, down, left and right). These colliders are used to determine orientation and position of cubies, and help the code know if a cubie is in the correct (solved) position.

**Canvas**

Three buttons:

- *Shuffle* - Shuffles the cube by rotating random slices in random directions a random number of times (calls: CubeShuffler.StartShuffle).
- *Undo* - Walks backward through the history of moves to undo moves one at a time (calls: CubeController.Undo).
- *Reset* - Explodes the cube, and pulls it back to a solved cube (calls: Exploder.Explode).

# Cubie Script

Each cubie has a cubie.cs script associated with it.  This script allows us to set the type of cubie and find out information about the cubie like, the colors visible on it, which way the cubie is facing, and if the cubie is in the correct (solved position).

**Public Fields**

- *CubieType* - represents the type of cube - core, center, edge, corner

**Public Methods**

There are 3 public methods on cubies:

*GetColors* 

- Returns the colors of the cubie.  
- Used by the WinController to get the color for each of the center cubies.

*GetCenterDirection* 

- Called on center cubies only, determines which side the cubie is facing.
- Used by the WinController to determine which side each center cubie is facing.

*InPosition* 

- Returns true if the cubie is in the correct (solved) position.
- Called by the WinController to determine if each cubie is in the correct position.

**Private Methods**

*IsFaceCorrect*

- Returns true of a cubie's face is facing the correct side of the cube.

*GetSide*

- Raycasts out in the specified direction and returns the name of the SideCollider it hits.
- Used to determine which side a cubie face is facing.

# CubeController Script

- The CubeController maintains the cubie matrix, tracks the state of the cube, and is the top level controller for the cube.
- Parented to the CubeController GameObject

**Fields**

- EnumCubeAnimState state => 
- EnumAnimType animationType => 
- float rotationSpeed = 300f => 
- EnumAxis rotationAxis => 
- EnumDirection rotationDirection => 
- int slice => 
- EnumGameState gameState => 

**Public Methods**

- StartRotation => 
- GetCubies => 
- Undo => 
- ResetHistory => 
- DoRotate => 
- SetupCubeAnimation => 

# CubeShuffler

- This script that is responsible for shuffling the cube.
- It is parented to the CubeController game object.

**Public Methods**

*StartShuffle*

- Called to start the shuffling routine.
- Determines how many shuffles will happen (ranging from 25 to 50).
- Sets the CubeControllers *gameState* to *shuffling*.
- Calls the *Shuffle* method, to start the shuffle.

**Private Methods**

*OnRotationComplete*

- Called by the *RotationComplete* delegate of the *CubeController* when the rotation animation is completed, so next shuffle step can be executed..
- Only executes if the current *gameState* is *shuffling*.
- Calls *Shuffle* if haven't reached the *timesToRotate* max
- If *timesToRotate* has been reached then sets the *gameState* to *playing* and resets the undo history.

*Shuffle*

- Called by StartShuffle to shuffle the cube by rotating a random slice in a random direction.
- Called again each time a RotationComplete Event is fired on teh CubeController.

# TouchController

- This script controls user input by detecting swipes on the cube.
- It is parented to the TouchController GameObject.
- The TouchController has 27 box colliders as children which are all on the "faces" layer. 
- These box colliders do not move when the cube and slices of the cube are rotated.  
- They stay in place and represent the locations where the cube can be touched.
- It maintains a *touchList* representing the box colliders touched in the last gesture.

**Update**

- *DetectTouch* is called each frame if the mouse button is down, or screen being touched.
- *ProcessTouch* is called when the mouse button is released, or touch is complete, to figure out which of the 27 face colliders were touched as part of the gesture.

**Detect Touch**

- Called each frame by the Update method to detect which faces are being touched.
- Uses a raycast to determine which collider was touched.
- The 27 box colliders that are children of the Touch Controller all belong to a layer called "faces".
- The raycast uses a layermask to detect which of these 27 box colliders was touched. 
- The collider touched is added to the *touchList* (but only if it isn't already in the list).

**Process Touch**

- Called when the mouse button is no longer being pressed, or screen is not being touched.
- The 27 box colliders that are children of the Touch Controller are named using a pattern: FaceX01, FaceY01, FaceZ01.
- These box colliders do not move position as the cube is rotated so always represent the same axis and position on the cube.
- The X, Y, Z represent the axis the face represents.
- The 1st number represents row, and the 2nd number represents the column.
- The code looks to see if 2 or more faces on the same axis and on either the same row or column have been touched.
- From this data the code can determine how to animate the cube using: axis, slice, direction, and animationType
- The code then calls the StartRotation method on the CubeController to start the animation using this data.

# WinController

- This script listens for *OnRotationComplete* action to fire on the CubeController.
- If the current *gameState* is *playing* then it calls the *CheckForWin* method to see if the cube is solved.
- It is parented to the CubeController GameObject.

**CheckForWin**

- returns true if all cubies are in the correct position.
- using the center pieces and the direction each center piece is facing as a reference it checks to see if each cubie is in the correct position.

> Note: This method could be used to set the *gameState* to a *win* state to play a win sequence (animate the cube to spin really fast, shoot fireworks, show a "Cube Solved" message, etc.)  Currently when you solve the cube, a "cube solved" message is printed on the console.

**GetColorDirectionDictionary**

- Returns a *Dictionary<color, direction>* that holds the color and direction of each *center* cubie.
- Used by *CheckForWin* to determine if each cubie is in the correct position.

# Exploder

- This script animates the cube to explode and then return to its original solved position.
- Parented to the CubeController GameObject

**Public Fields**

*power, returnSpeed, rotationalSpeed, exlposionLength*

These can be set in the inspector to customize the speed of the animation.

**Start**

- gets reference to *mirrors, cubies and core cubie*.
- calls *SaveOriginals*

**Update**

- if *reassembling* = true then moves cubies back together and rotates them back toward original positions.
- reassembling is complete when the cubes and rotations are close to their original positions,
- once reassembling is complete mirrors and core are set back to active.

**Public Methods**

*Explode*

- Called from the Reset button to start the Explode animation
- Adds a rigid body to each cubie
- Then applies a force to each cubie to cause an explosion.
- Hides the mirrors
- Hides the core cubie
- Startes the *Reassemble* coroutine

**Private Methods**

*Reassemble Coroutine*

- Waits until the *exlposionLength* has elapsed.
- Removes the rigid body from each cubie.
- Sets the *reassembling* state to true.

*SaveOriginals*

- Saves the original transforms of all the cubies (so they can be restored after the re-assemble is complete)






