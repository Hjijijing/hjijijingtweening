# 1.0.3 - 05-09-2021
### Added:
- You can set a default easing to be used for all tweening actions in an animation. Do this through the tweening animation constructor or using the useEasing() function.
- Added then(float waitForSeconds) function. It is the same as doing then().Wait(waitForSeconds).then();
- Added documentation for TweeningAnimation
# 1.0.2 - 05-09-2021
### Added:
It is now possible to specify a start delay and delay to tweens. The tween will wait for the start delay before starting the tween and will wait for the end delay before marking the tween as ended
# 1.0.1 - 05-09-2021
### Added:
Ability to choose which gameobject to move/rotate/scale on a per tween basis instead of specifying it in the constructor.
# 1.0.0 - 05-09-2021
### Added:
Basic functionality