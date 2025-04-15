**************************************
*          SHINY - URP SSR           *
*        Created by Kronnect         * 
*            README FILE             *
**************************************


Notice about Universal Rendering Pipeline
-----------------------------------------
This package is designed for URP.
It requires Unity 2019.4 and URP 7.2 or later
To install the plugin correctly:

1) Make sure you have Universal Rendering Pipeline asset installed (from Package Manager).
2) Go to Project Settings / Graphics.
3) Double click the Universal Rendering Pipeline asset.
4) Make sure the "Depth Texture" option is enabled.
5) Double click the Forward Renderer asset.
6) Click "+" to add the Shiny SSR Renderer Feature to the list of the Forward Renderer Features.


Help & Support Forum
--------------------

Check the Documentation folder for detailed instructions:

Have any question or issue?
* Support Forum: https://kronnect.com
* Twitter: @Kronnect
* Email (non support): contact@kronnect.com

If you like Shiny, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of the asset will be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

Version 21.0
- Added "Reflections Style" option. Choose between "Shiny" or "Dark" reflections. Dark reflections ignore source color and are faster to generate.
- Added "Metallic Boost" option under Reflection Intensity section (requires deferred).

Version 20.2
- Added "Start Distance" option for reflections

Version 20.1
- Improved "Refine Thickness" option algorithm

Version 20.0
- Added support for Render Graph (Unity 2023.3)
- Added ability to exclude reflections on certain objects by adding the "ExcludeReflections" script

Version 11.0
- Minimum Unity version required is now Unity 2021.3.16
- API: added Reflections.UpdateMaterialProperties() as alternative to Refresh() which quickly updates material changes

Version 9.1.1
- [Fix] Removed a CommandBuffer allocation

Version 9.1
- Added "Custom Bounds" option (only in deferred) which limits reflections to a specific world space volume
- Skybox contribution improvements. System can now use snapshots of realtime skybox and use it as fallback.
- Option to provide custom skybox cubemap

Vesion 9.0
- Added "Skybox Intensity". Use only if you wish the sky or camera background to be reflected on the surfaces.
- [Fix] Fixed a WebGL compilation issue on Unity 2020

Version 8.1
- Shiny Render Feature: added "Custom Smoothness Metallic" option which will execute a custom pass to gather smoothness/metallic data into a texture and use that information in forward rendering path instead of using Reflections scripts
- Shiny Render Feature: added "Enable Screen Space Normals" option which allows Reflections script to use URP depth normals texture instead of fetching normal map from the material
- Shiny Render Feature: added "Camera Layer Mask"
- Reflections scripts: added option (new default) to read normals from the depth/normals pass in forward rendering path (normal source = screen space normals) instead of reading the material bumpmap property

Version 8.0
- Added "Compute Back Faces". Option to compute true thickness of geometry instead of using a constant which improves accuracy of raytracer. This option can be expensive as it does an additional depth prepass on opaque geometry.
- Added "Puddle" example to demo scene 1. Uses a Reflections scripts with custom uv distortion speed setting (new).

Version 7.2.2
- Added "Skip Deferred Pass" option which ignores the full screen reflections pass in deferred and only computes reflections on desired surfaces with the Reflections script
- [Fix] Fixed a compatibility issue with some Android devices using OpenGL
- [Fix] Fixed some reflections not appearing due to a depth buffer issue in forward rendering mode

Version 7.1
- Performance optimizations
- Added new Output Mode: "Debug Depth"
- Added new Output Mode: "Debug Normals"

Version 7.0
- New workflow option: metallic and smoothness. By enabling this option at the Volume level, Shiny will now use the metallic property for the reflection intensity and the smoothness property for the roughness. Reflections values need to be reauthored when changing workflow as the formulas used are different.
- Reflections script: smoothness source has been simplified to either Material or Custom.

Version 6.1
- Change: reflections scripts can now be used in deferred rendering mode allowing a mixed mode (deferred pass + custom reflections driven by each Reflections script). To use them in deferred, activate the "Use Reflections Script" on the Shiny SSRR inspector when using deferred rendering path.

Version 6.0.3
- [Fix] Fixed reflections material update issue when using with deferred path   

Version 6.0.2
- [Fix] Fixed difference between deferred and forward due to jitter setting issue

Version 6.0
- Improved specular reflections
- Added "Temporal Filter" option

Version 5.2
- Global Settings moved to the Volume component from the render feature. Now, different global settings can be applied to different volumes in the scene.
- Added "Stencil Check" option. Can be used to mask reflections on screen based on stencil buffer.
- Added "Vignette Power" option. More control over the vignette effect.

Version 5.1
- Added support for "NormalMap Scale"
- Added support for Unity 2022
- [Fix] Fixed material refresh issue while in Editor Mode and forward rendering path

Version 5.0.1
- Internal improvements

Version 4.1
- Added "Smoothness Threshold" parameter to global settings

Version 4.0.1
- Added compatibility with third-party shaders (Ciconia)

Version 4.0
- Added raytracing quality presets
- Added "Refine Thickness" option
- Replaced "Step Size" with "Max Ray Length"
- Added "Stop NaN" option which can prevents artifacts when blending reflections due to pixel values out of range
- Added "Blur Strength" option
- Fixes and minor improvements

Version 3.0
- Redesigned reflection blur with simpler settings and contact hardening option
- New pyramid-blur for contact hardening/distance blur
- Rearranged inspector options
- Material smoothness now reduces fuzzyness

Version 2.6
- Ability to customize the material property names for smoothness map & normal map in Reflections script

Version 2.5
- Added support for smoothness map when using metallic workflow in forward rendering path

Version 2.4
- Max Reflections Multiplier increased to 2
- Added Reflections Min and Max clamping parameters

Version 2.3.1
- Improved Reflections component UI interface

Version 2.3
- Smoothness can now be set per submesh

Version 2.2
- In forward rendering mode, material alpha color now influence reflection intensity. This allows further customization when using objects with different materials, as you can customize reflections per material now by using different alpha values.

Version 2.1 16-FEB-2021
- Added "Animated Jitter" option
- Reflections are now listed in the inspector of the Shiny SSRR render feature

Version 2.0 15-FEB-2021
- First release to the Asset Store
- Added support for deferred rendering path in URP 12
