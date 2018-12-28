## Download unitypackage
For Unity 2017.3 and later: [SpriteGlow.unitypackage](https://github.com/Elringus/SpriteGlow/releases/download/v1.6-release/SpriteGlow.unitypackage). Check [releases](https://github.com/Elringus/SpriteGlow/releases) for previous versions support.

Please be aware that you don't need to clone the whole repository in order to use the extension in your project. Either download package from the link above or extract `Assets/SpriteGlow` folder from the repository project – it contains all the required assets; other stuff is just for testing purposes.

## Description
The glow effect is achieved using sprite outline in HDR colors (applied via shader) and bloom post-processing.

![Glow Intensity](https://i.gyazo.com/698f7d444d334b41657f056fb1ac94c7.gif) 
![Glow Color](https://i.gyazo.com/c8f8ec8a276aa4781b52732c521691db.gif)

For the bloom effect [Unity Post Processing Stack](https://github.com/Unity-Technologies/PostProcessing) is used. You can replace it with similar 3rd-party solutions.

The effect is compatible with the [Lightweight Render Pipeline](https://github.com/Unity-Technologies/ScriptableRenderPipeline/wiki/Lightweight-Render-Pipeline) (HDR should be enabled in the pipeline asset settings).

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
Please check the active [issues](https://github.com/Elringus/SpriteGlow/issues).
