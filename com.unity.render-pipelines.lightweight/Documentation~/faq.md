# Frequently asked questions
In this section, we’ve tried to answer some frequently asked questions about the Lightweight Render Pipeline (LWRP). These questions come from the [General Graphics](https://forum.unity.com/forums/general-graphics.76/) section on our forums, from the [Unity Discord](https://discord.gg/unity) channel, and from our support teams.
We’ll do our best to update this with new questions as they arise.

For information about the High Definition Render Pipeline (HDRP), please see the [HDRP documentation](https://github.com/Unity-Technologies/ScriptableRenderPipeline/wiki/High-Definition-Render-Pipeline-overview).

## Can I use LWRP and HDRP at the same time?
No. They're both built with the Scriptable Render Pipeline (SRP), but their render paths and light models are different.
## Can I convert from one pipeline to the other?
Yes! To do so, you'll have to re-write your Assets and redo the lighting in your game or app. You _cannot_ swap pipeline Assets from one pipeline to another at run time.
## How do I update the Lightweight Render Pipeline package?
You should update via the Package Manager. In the Unity Editor, go to __Unity__ > __Window__ > __Package Manager__, and find the __Lightweight RP__ package.

If you’ve added SRP code or Shader Graph manually via Github, make sure to upgrade them to the same package version as LWRP.


## Where has Dynamic Batching gone?

The Dynamic Batching checkbox has moved from the __Player Settings__ to the __Lightweight Render Pipeline Asset__.

## How do I enable Double Sided Global Illumination in the Editor?

In the Inspector for your Shader, find __Render Face__, and select __Both__. This means that both sides of your geometry contribute to global illumination, because LWRP doesn’t cull either side.
## Is this render pipeline usable for desktop apps and games?

Yes. The graphics quality and performance is scalable across platforms, so you can create apps for PCs and consoles as well as mobile devices.


## A certain feature from the Built-in render pipeline is not supported in LWRP. Will it be?

To see which features from the Built-in render pipeline will be supported in LWRP, you can check this [feature table](https://docs.google.com/spreadsheets/d/1nlS8m1OXStUK4A6D7LTOyHr6aAxIaA2r3uaNf9FZRTI/edit). LWRP will not support features marked as `Not Supported` or `Deprecated`. We intend to support features that have an open check box. Next to those, you can see information about which version of Unity we’re aiming to support them in.

If a feature is marked as `In Research`, we plan to support it but don’t have a target release yet.


## Does LWRP support a Deferred Renderer?
Not yet. Our goal is to add support for a mobile-optimized deferred renderer in 2019.3.
## Does LWRP have a public roadmap?
Yes. You can [check it here](https://portal.productboard.com/8ufdwj59ehtmsvxenjumxo82/tabs/3-lightweight-render-pipeline). You can add suggestions as well. To do so, you’ll have to enter your email address, but you won’t have to make an account.

## What’s LWRP state in the 2018 Long Term Service versions?

LWRP is still in the preview state for Long Term Service (LTS) versions of 2018 releases. We are not planning to take LWRP out of preview for 2018 LTS versions. Preview packages are not stable or ready for production, so we don’t support them in LTS versions . Use 2018 versions at your own risk.
## When is LWRP coming out of preview?
LWRP will be released with 2019.1. This means we consider it stable, given the amount of features it supports, butt’s not yet feature complete. Before upgrading your project to LWRP, check our roadmap and [feature comparison table](https://docs.google.com/spreadsheets/d/1nlS8m1OXStUK4A6D7LTOyHr6aAxIaA2r3uaNf9FZRTI/edit) between LWRP and the Built-in render pipeline. 

## I’ve found a bug. How do I report it?
You can open bugs by using the [bug reporter system](https://unity3d.com/unity/qa/bug-reporting). LWRP bugs go through the same process as all other Unity bugs. You can also check the active list of bugs for LWRP in the [issue tracker](https://issuetracker.unity3d.com/product/unity/issues?utf8=%E2%9C%93&package=2&unity_version=&status=1&category=&view=hottest). 

## I have an existing Project. How do I convert it from the Built-in render pipeline to LWRP?
You can check this upgrade guide on [installing an SRP into an existing Project](installing-lwrp-into-an-existing-project.md). 

## I’ve upgraded my Project from the Built-in render pipeline to LWRP, but it’s not running faster. Why?

LWRP and the Built-in render pipeline (RP) have different quality settings. While the Built-in RP configures many settings in different places like the Quality Settings, Graphics Settings, and Player Settings, all LWRP settings are stored in the LWRP Asset. The first thing to do is to check whether your LWRP Asset settings match the settings your Built-in render pipeline Project. For example, if you disabled MSAA or HDR in your Built-in render pipeline Project, make sure they are disabled in the LWRP Asset in your LWRP Project. For advice on configuring LWRP Assets, see documentation on the [LWRP Asset](lwrp-asset.md).

Also, make sure you are doing a fair comparison in terms of renderers. For this release, LWRP only supports a forward renderer, so make sure your Built-in render pipeline Project is using the forward renderer as well. 

If, after comparing the settings, you still experience worse performance with LWRP, please [open a bug report](https://unity3d.com/unity/qa/bug-reporting) and attach your Project. 
## LWRP doesn’t run on device X or platform Y. Is this expected?

No. Please [open a bug report](https://unity3d.com/unity/qa/bug-reporting). 

## My Project takes a long time to build. Is this expected?
We are looking into how to strip Shader keywords more aggressively. You can help the Shader stripper by disabling features you don’t require for your game in the LWRP Asset. For more information on settings that affect shader variants and build time, see the [shader stripping documentation](shader-stripping.md). 

## Is post-processing supported in LWRP?
Some post-processing version 2 (PPv2) features are supported in LWRP. To see which features are supported, see [this spreadsheet](https://docs.google.com/spreadsheets/d/1nlS8m1OXStUK4A6D7LTOyHr6aAxIaA2r3uaNf9FZRTI/edit#gid=0). PPv2 doesn’t support mobile VR.

We are working on many optimizations for post-processing and mobile. We’re aiming to have these in 2019.3. 

## I can’t set camera clear flags with LWRP. Why?

We’ve deprecated camera clear flags in LWRP. Instead, you can set the Background Type in the Camera Inspector. 
We’ve done this because the clear flags `Depth Only` and `Don’t Care` from the Built-in render pipeline has inherent performance pitfalls. The clear flags were used for camera stacking, where one camera depends on the results of a previous camera. This is bad for performance, as it executes culling multiple times and increases bandwidth. Bandwidth cost is especially important for mobile games.

For these reasons, we don’t allow camera stacking in LWRP. We are working on a solution where you can add a render pass with custom camera matrices and FOV. This way, we can provide an optimized workflow instead of creating a Camera object. We plan to expose this custom render pass in a future LWRP package.

## LWRP doesn’t render all passes in my Shader. Is this supported?

This is not supported at the moment. We are looking into an approach to support it. 

## 