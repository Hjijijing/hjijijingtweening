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