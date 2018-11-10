## Download unitypackage
For Unity 2017.3 and later: [SpriteGlow.unitypackage](https://github.com/Elringus/SpriteGlow/releases/download/v1.6-release/SpriteGlow.unitypackage). Check [releases](https://github.com/Elringus/SpriteGlow/releases) for previous versions support.

## Description
The glow effect is achieved using sprite outline in HDR colors (applied via shader) and bloom post-processing.

![Glow Intensity](https://i.gyazo.com/698f7d444d334b41657f056fb1ac94c7.gif) 
![Glow Color](https://i.gyazo.com/c8f8ec8a276aa4781b52732c521691db.gif)

For the bloom effect [Unity Post Processing Stack](https://github.com/Unity-Technologies/PostProcessing) is used. You can replace it with similar 3rd-party solutions.

The effect is compatible with the [Lightweight Render Pipeline](https://github.com/Unity-Technologies/ScriptableRenderPipeline/wiki/Lightweight-Render-Pipeline) (HDR should be enabled in the pipeline asset settings).

Character sprite by [Mikhail Pigichka](https://www.facebook.com/hundewache).

## FAQ

### Can I use the effect with components other than SpriteRenderer (UI Image, Tilemap, etc)?

Not directly (after all, this project is called **Sprite**Glow). It is only possible if the component is using a shader similar to sprites, e.g. , “2D” stuff such as UI objects, tilemaps, etc. To accomplish this, a material based on the `Sprites/Outline` shader must be created and manually applied to the component. Material editor can then be used to control all of the glow parameters. In the project, scenes with examples of applying the effect to UI Button or a tilemap can be found. 


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

NullReferenceException error in build is likely due to the outline shader not being included to the build. This often happens when none of the included assets reference SpriteGlow component directly. The shader can be force-included by assigning it to the "Always Included Shaders" in Edit -> Project Settings -> Graphics.

### It's still not working
Please check the active [issues](https://github.com/Elringus/SpriteGlow/issues).
