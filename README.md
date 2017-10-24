## Download unitypackage
[SpriteGlow.unitypackage](https://github.com/Elringus/SpriteGlow/releases/download/v2.1-release/SpriteGlow.unitypackage)

## Description
The effect is achieved using sprite outline in HDR colors (applied via shader) and bloom post-processing.

![Glow Intensity](https://i.gyazo.com/698f7d444d334b41657f056fb1ac94c7.gif) 
![Glow Color](https://i.gyazo.com/c8f8ec8a276aa4781b52732c521691db.gif)

For the bloom effect [KinoBloom](https://github.com/keijiro/KinoBloom) is used. You can replace it with similar 3rd-party solutions.

Master branch aims to be on par with the latest Unity version. See [releases](https://github.com/Elringus/SpriteGlow/releases) for previous versions support.

Character sprite by [Mikhail Pigichka](https://www.facebook.com/hundewache).

## FAQ

### Why the glow doesn't appear?
Make sure you've set 'Current Brightness' high enough:

![](https://i.gyazo.com/101ac74ac1a6cf1af0814b6b02186174.png)

### How to change glow brightness at runtime?
You can change the brightness in scripts by multiplying the ‘GlowColor’ value.

### Why the glow appears on the whole image?
Make sure 'Threshold' value of the bloom image effect is set high enough. It's usually 1 to 1.5, depending on the bloom solution.

### It's still not working
Please check the active [issues](https://github.com/Elringus/SpriteGlow/issues).
