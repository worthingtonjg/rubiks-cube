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

The CubeController maintains the cubie matrix, tracks the state of the cube, and is the top level controller for the cube.

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

This script controls user input by detecting swipes on the cube.

**Update**

- Calls *DetectTouch* while mouse button is down, or screen being touched.
- When mouse button is released, or touch is complete, it calls *ProcessTouch* to figure out which of the 27 face colliders where touched as part of the gesture.

**Detect Touch**

- Called each frame by the Update method to detect which faces are being touched.
- Uses raycasting to raycast out from the touch location.
- The 27 box colliders that are children of the Touch Controller all belong to a layer called "faces"
- The raycast uses a layermask to detect which of these 27 box colliders are touched and adds them the *touchList*

**Process Touch**

- Called when the mouse button is no longer being pressed, or screen is not being touched.
- The 27 box colliders that are children of the Touch Controller are named using a pattern: FaceX01, FaceY01, FaceZ01.
- These box colliders do not move position as the cube is rotated so always represent the same axis and position on the cube.
- The X, Y, Z represent the axis the face represents.
- The 1st number represents row, and the 2nd number represents the column.
- I look to see if 2 or more multiple faces on the same axis and on either the same row or column have been touched.
- From this data I can determine how to animate the cube using: axis, slice, direction, and animationType
- I then call the StartRotation method on the CubeController to start the animation using this data.

# WinController

- This script listens for *OnRotationComplete* action to fire on the CubeController
- If the current *gameState* is *playing* then it calls the *CheckForWin* method to see if the cube is solved.
- It is parented to the CubeController GameObject

**CheckForWin**

- returns true if all cubies are in the correct position.
- using the center pieces and the direction each center piece is facing as a reference it checks to see if each cubie is in the correct position.

> Note: This method could be used to set the *gameState* to a *win* state to play win sequence.  Currently when you solve the cube, nothing special happens.

**GetColorDirectionDictionary**

- Returns a *Dictionary<color, direction>* that holds the color and direction of each *center* cubie.
- Used by *CheckForWin* to determine if each cubie is in the correct position.

