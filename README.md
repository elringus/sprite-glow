## Download unitypackage
[SpriteGlow.unitypackage](https://github.com/Elringus/SpriteGlow/releases/download/v2.2a-release/SpriteGlow.unitypackage)

## Description
The glow effect is achieved using sprite outline in HDR colors (applied via shader) and bloom post-processing.

![Glow Intensity](https://i.gyazo.com/698f7d444d334b41657f056fb1ac94c7.gif) 
![Glow Color](https://i.gyazo.com/c8f8ec8a276aa4781b52732c521691db.gif)

For the bloom effect [KinoBloom](https://github.com/keijiro/KinoBloom) is used. You can replace it with similar 3rd-party solutions.

Master branch aims to be on par with the latest Unity version. See [releases](https://github.com/Elringus/SpriteGlow/releases) for previous versions support.

Character sprite by [Mikhail Pigichka](https://www.facebook.com/hundewache).

## FAQ

### Why the glow doesn't appear?
Make sure:

* Camera 'Allow HDR' is enabled:

![](https://i.gyazo.com/e5f67d94e6ed1e5e3652d6ee52668b85.png)

* You've set 'Glow Brightness' high enough:

![](https://i.gyazo.com/94fe6e143e310a526b3428c6c62b45bf.png)

### Why the glow appears on the whole image?
Make sure 'Threshold' value of the bloom image effect is set high enough. It's usually 1 to 1.5, depending on the bloom solution.

![](https://i.gyazo.com/bd3961f0efbceddca9c4a077d7b9a6d4.png)

### It's still not working
Please check the active [issues](https://github.com/Elringus/SpriteGlow/issues).
