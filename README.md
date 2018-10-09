# rubiks-cube

This is an implementation of the Rubiks Cube using Unity3d.  I was bored one weekend and decided I wanted to create a Rubiks cube in Unity.

# WebGL Demo

To see the my Rubiks Cube implementation in action, try the Web GL demo link below.  

<https://worthingtonjg.github.io/rubiks-cube/>

- You can Shuffle the cube.
- Reset the cube (explodes the cube and re-assembles it solved).
- You swiping a center slice of the cube will rotate the cube in the direction swiped.
- Swiping an edge slice will rotate that particular slice.
- You can undo your moves with the Undo button.

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

Each cubie has a public CubieType field that can be set to as: core, center, edge, corner

**Methods**

There are 3 public methods on cubies:

1.  GetColors => Returns the colors of the cubie.
2.  GetCenterDirection => Called on center cubies only, determines which side the face is facing (
3.  InPosition => returns true if the cubie is in the correct (solved) position

# CubeController Script

The CubeController maintains the cubie matrix, tracks the state of the cube, and is the top level controller for the cube.

**Fields**

EnumCubeAnimState state => 
EnumAnimType animationType => 
float rotationSpeed = 300f => 
EnumAxis rotationAxis => 
EnumDirection rotationDirection => 
int slice => 
EnumGameState gameState => 

**Public Methods**

StartRotation => 
GetCubies => 
Undo => 
ResetHistory => 
DoRotate => 
SetupCubeAnimation => 





