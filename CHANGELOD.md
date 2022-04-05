# 1.0.13 - 05-04-2022

### Added:

- anchorMove() for animating a rect transforms anchored position. This will make it easier to move ui elements.

# 1.0.12 - 02-04-2022

### Added:

- Sprite color tweener. You can now tween the color of sprites.
- More functions for combining Easings.
- Events for TweeningActions. These can be accesed by using CallOnActionStart, CallOnActionEnd, CallOnActionReverted, CallOnActionForceFinished and CallOnActionStopped.
- Overloads for move, colorMesh, colorSprite, rotate and scale that allow start values and targets to be determined by callbacks.
- to() function can be used to set end value of action. Both to() and from() have overloads where you can pass a callback as a parameter to make the values be determin as the action starts by calling the functions and using the return values.
- Overloads for move, colorMesh, colorSprite, rotate and scale where you only pass duration. Start and end can then be passed by from() and to().
- Repeat(x) can be called and will start the animation and repeat it x times.
- Static Oscillate() function on TweeningAnimation that will return an animation that calls a callback with angle values from 0 to 2\*PI over a given period.
- More minor features

### Changed:

Wait() no longer starts a new sequence, since this can be done with one of the then() overloads.

# 1.0.11 - 1-04-2022

### Added:

- Events for starting, reverting and stopping a TweeningAnimation

# 1.0.10 - 26-03-2022

### Added:

- Branching. You can now create a branch action by calling Branch(). This function takes a condition function and two other functions as input. If the condition returns true then the first function will be called and the second function will be called otherwise.
- Easing combining and Easing reversing. You can now called Easing.combineEasings() to combine to easings in to one. You can also call Easing.reverseEasing() to get an easing that is one minus the easing you put in.
- TweeningAction now has a ReturnBack() function that will make the latest action go to the target point and then back instead of just stopping at the target point.
- TweeningAction now has a function from() that specifies where the latest tweening action should start from. Furthermore rotate(), move(), colorMesh() and scale() all have a new overload where you can also pass a starting point.

### Fixed:

- Reversing seems to be working

# 1.0.9 - 07-10-2021

### Fixed:

- The new Sequence system now works.
- Fixed dates in previous changelog entry

### Error:

- Reversing no longer works :I

### Changed:

- Completely changed the way tweens are stored in TweeningAnimation, by making a TweeningSequence class

# 1.0.8b - 05-09-2021

### Fixed:

- Fixed yet another reversing issue. On the initial reversal it skipped the first action due to it having same queuenumber as the reverse action.

# 1.0.8 - 05-09-2021

### Fixed:

- Reversing now properly reverses. Before it only reversed the targets but not the order of the queues.
- Reverse() now returns the tweening animation, like all other actions.

# 1.0.7c - 05-09-2021

### Fixed:

- Fixed another problem where start and end delays where not set.

# 1.0.7b - 05-09-2021

### Fixed:

- Fixed it for real this time. Maybe?

# 1.0.7 - 05-09-2021

### Fixed:

- Actually fixed the issue now.
- Removed some unused meta files.

# 1.0.6 - 05-09-2021

### Fixed:

- Issue in previous update that caused reverse not to work.

# 1.0.5 - 05-09-2021

### Added:

- Loops: You can now call Loop() instead of Start() to loop an animation infinitely.
- Markers: You can add markers to a queue. This will be used in the future for jumping between queues and stuff. Markers can be created with Marker(string marker) or with then(string marker).
- Reversing: You can now reverse queues by calling Reverse(). This will add reversed versions of all of the queues that were added before you called Reverse(). So if you add Move to 1,0,0 from 0,0,0 and then call Reverse(), it will first move to 1,0,0 and then move back to 0,0,0.
- Added extension methods for the MonoBehavior class so you can now call Tween() on a monobehaviour and it will return a new tweening animation.
- Updated readme with plans for future features.

# 1.0.4b - 05-09-2021

Forgot to change package.json version in update 1.0.4 from "1.0.3" to "1.0.4" so that has been done now.

# 1.0.4 - 05-09-2021

### Added:

- Easing.randomEasing(min,max) will return an easing function that returns random values between min and max. The function will always return 0 when the input is 0 and 1 when the input is 1.
- Updated readme

### Changed:

- Made changes in changelog for 1.0.0, 1.0.1 and 1.0.2 into lists

### In progress:

- Began support for looping, repeating and reversing animations.

# 1.0.3 - 05-09-2021

### Added:

- You can set a default easing to be used for all tweening actions in an animation. Do this through the tweening animation constructor or using the useEasing() function.
- Added then(float waitForSeconds) function. It is the same as doing then().Wait(waitForSeconds).then();
- Added documentation for TweeningAnimation

# 1.0.2 - 05-09-2021

### Added:

- It is now possible to specify a start delay and delay to tweens. The tween will wait for the start delay before starting the tween and will wait for the end delay before marking the tween as ended

# 1.0.1 - 05-09-2021

### Added:

- Ability to choose which gameobject to move/rotate/scale on a per tween basis instead of specifying it in the constructor.

# 1.0.0 - 05-09-2021

### Added:

- Basic functionality
