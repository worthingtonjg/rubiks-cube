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
- *Contains 27 Cubies* - each cubie represents one piece of the cube (1 core, 8 corners, 12 edges, 6 centers)

> See *Cubie* and *CubeController* script descriptions below for more details.

**SideColliders**

There are 6 side colliders, one for each side of the cube (foward, back, up, down, left and right). These colliders are used to determine orientation and position of cubies, and help the code know if a cubie is in the correct (solved) position.

**Canvas**

Three buttons:

- *Shuffle* - Shuffles the cube by rotating random slices in random directions a random number of times (calls: CubeShuffler.StartShuffle).
- *Undo* - Walks backward through the history of moves to undo moves one at a time (calls: CubeController.Undo).
- *Reset* - Explodes the cube, and pulls it back to a solved cube (calls: Exploder.Explode).

# 3d Model - Cubie

I created a simple 3d model in blender using a cube as a base.  The model has 7 materials: one for each side of the cube, and one for the cubes border.  

# The Cube

The cube consists of a matrix of 27 cubies (3x3x3).  The center cubie is at 0,0,0 in world space, with each cubie arranged around it exactly 1 unit away.

# Cubie Script

Each cubie has a cubie.cs script added to it.

**EnumCubieType**

There are 4 types of cubies defined using the enum: 

```c#
public enum EnumCubieType { core, center, edge, corner }
```

**Public Fields**

- CubieType: represents the type of cube - core, center, edge, corner

**Methods**

There are 3 public methods on cubies:

1.  GetColors => Returns the colors of the cubie.
2.  GetCenterDirection => Called on center cubies only, determines which side the face is facing (
3.  InPosition => returns true if the cubie is in the correct (solved) position

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

Script that is responsible for shuffling the cube.

**Public Methods**

- StartShuffle => Called to start the shuffling routine.

**Private Methods**

- OnRotationComplete => Called by the RotationComplete delegate when the rotation animation is completed, so next shuffle can be done.
- Shuffle => Called by StartShuffle to shuffle the cube by rotating a random slice in a random direction.

# TouchController

This script controls user input by detecting swipes on the cube

# WinController



