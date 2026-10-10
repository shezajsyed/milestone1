# Character Controller Milestone

A third-person character controller in a grey-boxed test gym, built in
Unity 6.3 LTS (6000.3.10f1). Focus: tactile responsiveness, animation
blending, and game feel.

## Showcase
- Video: https://drive.google.com/file/d/1lejf5UWYw5rFNi1aHRzwxmjI6V3nKUcD/view?usp=sharing
- Download build: [LINK TO RELEASE OR ZIP]

## Controls

| Action | Keyboard & Mouse | Gamepad |
|---|---|---|
| Move | WASD | Left stick |
| Look | Mouse | Right stick |
| Sprint | Left Shift | Left Stick Press |
| Jump | Space | Button South |
| Crouch (hold) | C | Button East |

## Features
- **Input**: Unity's new Input System, with keyboard/mouse and gamepad support
- **Animation**: custom Animator Controller with a 2D blend tree for
  locomotion (idle, walk, run, strafe), plus jump state and crouch 2D blend tree
- **Camera**: Cinemachine State-Driven Camera blending between standing and
  crouch cameras, breathing noise, and a landing impulse
- **Game feel**: coyote time, jump buffering, landing thud synced with
  camera shake, and a crouch collider that resizes with a headroom check
- **Environment**: grey-box gym with stations for jumping, crouching,
  stairs, and sprinting

## Gym stations
- Stepping stones (jump)
- Pipes (crouch)
- Stairs (climb)
- Floors (sprint)

## How to run
**Build:** unzip the download and run the executable.
**Editor:** open the project in Unity 6.3 LTS (6000.3.10f1), open
`Assets/Scenes/SampleScene`, and press Play.

## Credits and References

**Character**
Erika Archer. Mixamo (Adobe), https://www.mixamo.com. Accessed 9 October 2026.

**Animations** (all from Mixamo, applied to Erika Archer)
Unarmed Idle, Walking, Running, Running Backward, Right Strafe, Right Strafe Walk,
Left Strafe, Left Strafe Walk, Jump, Crouch Walk Right, Crouch Walk Left,
Crouch Walk Forward, Crouch Walk Back, Crouch Idle.

**Audio**
"Classic Footstep SFX" by Matthew Anett. Unity Asset Store, version 1.0
(released 3 July 2020).
https://assetstore.unity.com/packages/audio/sound-fx/classic-footstep-sfx-173668
Licensed under the Standard Unity Asset Store EULA. Accessed 10 October 2026.
Landing sound uses the clip "Floor_step16.wav" from this pack.

**Software and packages**
Unity 6.3 LTS (6000.3.10f1); Cinemachine; Input System; ProBuilder; Recorder.

**Original work**
Animator Controller and blend trees, PlayerMovement script, Cinemachine camera
setup, and grey-box gym geometry were created by the author.

**License note**
Mixamo characters and animations are used under Adobe's Mixamo terms.

## Reflection
See `Reflection.pdf` in this repository.
