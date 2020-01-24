## Installation
Use [UPM](https://docs.unity3d.com/Manual/upm-ui.html) to install the package via the following git URL: `https://github.com/Elringus/SpriteGlow.git#package`.

![](https://i.gyazo.com/b54e9daa9a483d9bf7f74f0e94b2d38a.gif)

## Description
The glow effect is achieved using sprite outline in HDR colors (applied via shader) and bloom post-processing.

![Glow Intensity](https://i.gyazo.com/698f7d444d334b41657f056fb1ac94c7.gif) 
![Glow Color](https://i.gyazo.com/c8f8ec8a276aa4781b52732c521691db.gif)

For the bloom effect [Unity Post Processing Stack (PPS)](https://github.com/Unity-Technologies/PostProcessing) is used. You can replace it with similar 3rd-party solutions.

The effect is compatible with the [Universal Render Pipeline (URP, aka LWRP)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest). Bloom should be configured via [URP's own post-processing solution](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/integration-with-post-processing.html); don't forget to enable HDR in the pipeline asset settings.

Character sprite by [Mikhail Pigichka](https://www.facebook.com/hundewache).

## FAQ

### Can I use the effect with components other than SpriteRenderer (UI Image, Tilemap, etc)?
Not directly (after all, this is **Sprite**Glow), but it's possible if the component is using a shader similar to sprites (e.g UI objects, tilemaps and other "2D" stuff). For this you will have to create a material based on the `Sprites/Outline` shader and manually apply it to the component. You can then control all the glow parameters using the material editor. In the project you can find scenes with the examples for applying the effect to UI Button and a tilemap.

![](https://i.gyazo.com/6c92f315d8a25600bf4ec930c5b7de3e.png)

### Why the glow doesn't appear?
Make sure:

* A bloom post-processing effect is enabled (you can use any, eg [the one from Unity's PPS](https://docs.unity3d.com/Manual/PostProcessing-Bloom.html))

* HDR is enabled for target platform and tier (Edit > Project Settings > Graphics):

![](https://i.gyazo.com/3523e3174080dce3347874e59539e58c.png)

* Camera 'Allow HDR' is enabled:

![](https://i.gyazo.com/e5f67d94e6ed1e5e3652d6ee52668b85.png)

* You've set 'Glow Brightness' high enough:

![](https://i.gyazo.com/94fe6e143e310a526b3428c6c62b45bf.png)

### Why the glow appears on the whole image?
Make sure 'Threshold' value of the bloom image effect is set high enough. It's usually 1 to 1.5, depending on the bloom solution.

![](https://i.gyazo.com/bd3961f0efbceddca9c4a077d7b9a6d4.png)

### Why I get a NullReferenceException in build?
Most likely the outline shader is not included to the build (it happens when none of the included assets reference SpriteGlow component directly). You can force-include the shader by assigning it to the "Always Included Shaders" in Edit -> Project Settings -> Graphics.

### It's still not working
Make sure you're using a compatible Unity version; supported versions per release are available on the [releases page](https://github.com/Elringus/SpriteGlow/releases).

Download (clone) this repository and examine the example scenes, comparing them with your setup. Chances are, you've misconfigured something.

In case nothing of the above helps, check the active [issues](https://github.com/Elringus/SpriteGlow/issues) or open a new one. Don't forget to include the full [error log](https://docs.unity3d.com/Manual/LogFiles.html), detailed information about your system, Unity version, package version and steps required to reproduce the issue. 
