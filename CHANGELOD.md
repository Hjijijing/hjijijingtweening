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