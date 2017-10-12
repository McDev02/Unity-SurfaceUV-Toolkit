# Unity-SurfaceUV-Toolkit
An unfinished and not continued research project on "Surface Aware" UV painting.
The main purpose was to include functionality to blend Lightmap seams which now has been implemented by Unity itself.

See:
https://unity3d.com/de/unity/whats-new/unity-2017.2.0:
GI: Added per-object lightmap seam stitching for Progressive Lightmapper.


***Caution***
This project is only for enthusiasts that want to read or continue the algorithm used. 
It has visual feedback but not yet any useful functionality like storing the files.
Reading and storing lightmaps was an issue which is unsolved yet as you have to read the source exr file and not the compressed file which the game uses.

How you test the file:
1. import the project and open the example scene "Examples->Scene".
2. Open the editor window by pressing "Windows->SurfaceAware Toolkit"
3. Select an Object (MeshRenderer) which you want to analyze and edit.
4. You should see statistics on the window and a few buttons and Toggles. Here is what they do:

Edit Texture: enables you to edit the texture. Currently it only wokrs with the base texture but also has functionality for lightmaps.
Blur Seams: Will blur the texture only along the UV seam edges.
PaintOnTexture: Hold your mouse over the object and it will blur the texture at it's seams.
Save Texture: May not work, see its code.

Draw UV: Will draw the selected UV in the window (tabs below).
Draw SplitUV 1 or 2: Will draw the UV Seams on the object, does not seem to always work currently.

Notice that there is functionality to draw the current texture below it's uv in the window, but due to some changes this doesn't work anymore.
