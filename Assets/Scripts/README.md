How to make Magica Voxel Prefabs

MAke the model in Magica, export as .obj.  This will create a .obj .mtl and .png (in case of colored model).

In Unity inside Asset folder I have "Magica Originals" folder, in here create a separate folder for each wepon or enemy.  In its corresponding folder inser the 3 files exported from Magica.

To create a prefab.
From the MAgicaOriginals folder, drag and drop only the .vox that has a default child object to the scene. 

Now use this gameobject to edit it as needed as a prefab, (sizes, colliders, scripts, tags etc)

Once this is completed, drag this gameobject to the Prefabs folder in its corresponding folder.    This will be the prefab to use in game.  

