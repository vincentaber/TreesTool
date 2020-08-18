using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[ExecuteInEditMode]
public class Trees_Tool : MonoBehaviour
{

     Terrain currentTerrain;
     public TerrainData currentTerrainData;
     Vector3 terrainPosition;


     public int TextureArrayIndex = 0;
     public int TreePrototypeIndex = 0;
     public int DetailPrototypeIndex = 0;


    public int TreeSpawnDensity;
    public int DetailSpawnDensity;

    public float PositionRandomness = 0.2f;
    public float SizeRandomness = 0.1f;
    public float BorderSize = 0.2f;
    public float LacunarityProbability = 0.5f;

    public float minHeight = 0;
    public float maxHeight = 1f;

    public float minSteepness = 0f;
    public float maxSteepness = 90f;
    public float transitionSmoothness = 0.9f;
    public float detailExpand = 0.5f;

    public int DecimatePercentage;

    public float DetaliPerlinScale = 1;
    public bool DetailUsePerlin;
    public float DetailPerlinBias = 0f;
    public int DetailPerlinSeed = 1;

    public bool DetailInvertPerlin;

    public float TreePerlinScale = 1;
    public bool TreeUsePerlin;
    public float TreePerlinBias = 0f;
    public int TreePerlinSeed = 1;

    public bool TreeInvertPerlin;

    public bool UseTexture;
    public bool UseSlope;
    public bool UseHeight;

    public bool useExperimental;

    private List<TreeInstance> TreesBackup;
    private int[,] DetailBackup;
    

    public void Awake()
    {
        currentTerrain = GetComponent<Terrain>();
        currentTerrainData = currentTerrain.terrainData;
        terrainPosition = transform.position;
        TreesBackup = new List<TreeInstance>();
        Debug.Log("Trees_Tool Initialized");
    }

    public void InitTrees()
    {
        currentTerrain = GetComponent<Terrain>();
        currentTerrainData = currentTerrain.terrainData;
        terrainPosition = transform.position;
        TreesBackup = new List<TreeInstance>();
        Debug.Log("Trees_Tool Initialized");
    }



    public void AddTree(bool useText, bool useSlope, bool useHeight)
    {
        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();
        List<TreeInstance> TreesList = NewTrees.ToList<TreeInstance>();

        float time = (float)EditorApplication.timeSinceStartup;

        int AddedTrees = 0;

        //CreatingTree
        TreeInstance tree = new TreeInstance();

        tree.prototypeIndex = TreePrototypeIndex;
        tree.heightScale = 1;
        tree.widthScale = 1;
        tree.color = Color.white;
        tree.lightmapColor = Color.white;
        tree.rotation = 0;
        tree.position = Vector3.zero;


        float tempheight = tree.heightScale;
        float tempwidth = tree.widthScale;
        int raycastAmount = TreeSpawnDensity * 10;
        Vector3 SpawnPos;
        for (int i = 0; i < raycastAmount; i++)
        {
            for (int x = 0; x < raycastAmount; x++)
            {

                Vector3 treePos = new Vector3((float)i / (float)raycastAmount, 0, (float)x / (float)raycastAmount);
                treePos.x += Random.Range(-0.05f, 0.05f) * PositionRandomness;
                treePos.x = Mathf.Clamp(treePos.x, 0, 1);
                treePos.z += Random.Range(-0.05f, 0.05f) * PositionRandomness;
                treePos.z = Mathf.Clamp(treePos.z, 0, 1);

                float RandomSize = Random.Range(SizeRandomness * -1, SizeRandomness);

                SpawnPos = TreePosToWorldPos(treePos);
                if (useText)
                {

                    float TextureMix = GetTextureThreshold(SpawnPos, TextureArrayIndex);
                    if (TextureMix > Mathf.Abs(1 - transitionSmoothness))
                    {
                        float rando = Random.Range(0, 1);
                        if (rando < TextureMix)
                        {

                            tree.heightScale = (tempheight + RandomSize) * Mathf.Clamp(TextureMix + BorderSize, 0, 1);
                            tree.widthScale = (tempwidth + RandomSize) * Mathf.Clamp(TextureMix + BorderSize, 0, 1);
                            tree.position = treePos;
                            tree.rotation = Random.Range(-180, 180);

                            if (!TreeUsePerlin)
                            {
                                if (useSlope)
                                {
                                    if (currentTerrainData.GetSteepness(treePos.x, treePos.z) >= minSteepness && currentTerrainData.GetSteepness(treePos.x, treePos.z) <= maxSteepness)
                                    {
                                        TreesList.Add(tree);
                                        AddedTrees += 1;
                                    }
                                }

                                else
                                {
                                    TreesList.Add(tree);
                                    AddedTrees += 1;
                                }

                            }

                            else
                            {
                                if (Mathf.Clamp(Mathf.PerlinNoise((treePos.x + TreePerlinSeed) * TreePerlinScale * 100, (treePos.z + TreePerlinSeed) * TreePerlinScale * 100) + TreePerlinBias, 0, 1) > 0.5f && !TreeInvertPerlin)
                                {
                                    if (useSlope)
                                    {
                                        if (currentTerrainData.GetSteepness(treePos.x, treePos.z) >= minSteepness && currentTerrainData.GetSteepness(treePos.x, treePos.z) <= maxSteepness)
                                        {
                                            TreesList.Add(tree);
                                            AddedTrees += 1;
                                        }
                                    }

                                    else
                                    {
                                        TreesList.Add(tree);
                                        AddedTrees += 1;
                                    }

                                }
                                else if (Mathf.Clamp(Mathf.PerlinNoise((treePos.x + TreePerlinSeed) * TreePerlinScale * 100, (treePos.z + TreePerlinSeed) * TreePerlinScale * 100) + TreePerlinBias, 0, 1) < 0.5f && TreeInvertPerlin)
                                {
                                    if (useSlope)
                                    {
                                        if (currentTerrainData.GetSteepness(treePos.x, treePos.z) >= minSteepness && currentTerrainData.GetSteepness(treePos.x, treePos.z) <= maxSteepness)
                                        {
                                            TreesList.Add(tree);
                                            AddedTrees += 1;
                                        }
                                    }

                                    else
                                    {
                                        TreesList.Add(tree);
                                        AddedTrees += 1;
                                    }
                                }
                            }
                        }

                    }
                }

                else
                {


                    tree.heightScale = (tempheight + RandomSize);
                    tree.widthScale = (tempwidth + RandomSize);
                    tree.position = treePos;
                    tree.rotation = Random.Range(-180, 180);

                    if (!TreeUsePerlin)
                    {
                        if (useSlope)
                        {
                            if (currentTerrainData.GetSteepness(treePos.x, treePos.z) >= minSteepness && currentTerrainData.GetSteepness(treePos.x, treePos.z) <= maxSteepness)
                            {
                                TreesList.Add(tree);
                                AddedTrees += 1;
                            }
                        }

                        else
                        {
                            TreesList.Add(tree);
                            AddedTrees += 1;
                        }
                    }

                    else
                    {
                        if (Mathf.Clamp(Mathf.PerlinNoise((treePos.x + TreePerlinSeed) * TreePerlinScale * 100, (treePos.z + TreePerlinSeed) * TreePerlinScale * 100) + TreePerlinBias, 0, 1) > 0.5f && !TreeInvertPerlin)
                        {
                            if (useSlope)
                            {
                                if (currentTerrainData.GetSteepness(treePos.x, treePos.z) >= minSteepness && currentTerrainData.GetSteepness(treePos.x, treePos.z) <= maxSteepness)
                                {
                                    TreesList.Add(tree);
                                    AddedTrees += 1;
                                }
                            }

                            else
                            {
                                TreesList.Add(tree);
                                AddedTrees += 1;
                            }
                        }
                        else if (Mathf.Clamp(Mathf.PerlinNoise((treePos.x + TreePerlinSeed) * TreePerlinScale * 100, (treePos.z + TreePerlinSeed) * TreePerlinScale * 100) + TreePerlinBias, 0, 1) < 0.5f && TreeInvertPerlin)
                        {
                            if (useSlope)
                            {
                                if (currentTerrainData.GetSteepness(treePos.x, treePos.z) >= minSteepness && currentTerrainData.GetSteepness(treePos.x, treePos.z) <= maxSteepness)
                                {
                                    TreesList.Add(tree);
                                    AddedTrees += 1;
                                }
                            }

                            else
                            {
                                TreesList.Add(tree);
                                AddedTrees += 1;
                            }
                        }
                    }

                }


            }
        }

        NewTrees = TreesList.ToArray();
        currentTerrainData.SetTreeInstances(NewTrees, true);
        NewTrees = currentTerrainData.treeInstances;
        TreesList = NewTrees.ToList();

        if (useHeight)
        {
            for (int i = 0; i < currentTerrainData.treeInstanceCount; i++)
            {
                TreeInstance currentTree = NewTrees[i];

                if (currentTree.prototypeIndex == TreePrototypeIndex)
                {
                    if (currentTree.position.y > maxHeight || currentTree.position.y < minHeight)
                    {
                        TreesList.Remove(currentTree);
                        AddedTrees -= 1;
                    }
                }

            }
        }

        NewTrees = TreesList.ToArray();
        currentTerrainData.SetTreeInstances(NewTrees, true);
        time = (float)EditorApplication.timeSinceStartup - time;
        Debug.Log(AddedTrees + " trees added in " + Mathf.RoundToInt(time));

    }

    public void RemoveTrees(bool useTexture, bool useSlope, bool useHeight)
    {
        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();
        List<TreeInstance> TreesList = NewTrees.ToList<TreeInstance>();
        TreesBackup = NewTrees.ToList<TreeInstance>();

        float time = (float)EditorApplication.timeSinceStartup;

        bool treedone;

        int DestroyedTrees = 0;

        for (int i = 0; i < currentTerrainData.treeInstanceCount; i++)
        {
            TreeInstance currentTree = currentTerrainData.GetTreeInstance(i);
            treedone = false;
            if (currentTree.prototypeIndex == TreePrototypeIndex)
            {
                if (useTexture)
                {
                    float TextureMix = GetTextureThreshold(TreePosToWorldPos(currentTree.position), TextureArrayIndex);
                    if (TextureMix > transitionSmoothness)
                    {
                        TreesList.Remove(currentTree);
                        DestroyedTrees += 1;
                        treedone = true;
                    }
                }

                if (useSlope && !treedone)
                {
                    if (currentTerrainData.GetSteepness(currentTree.position.x, currentTree.position.z) > minSteepness && currentTerrainData.GetSteepness(currentTree.position.x, currentTree.position.z) < maxSteepness)
                    {
                        TreesList.Remove(currentTree);
                        DestroyedTrees += 1;
                        treedone = true;
                    }
                }

                if (useHeight && !treedone)
                {
                    if (currentTree.position.y < maxHeight && currentTree.position.y > minHeight)
                    {
                        TreesList.Remove(currentTree);
                        DestroyedTrees += 1;
                    }
                }

                if (TreeUsePerlin && !treedone)
                {
                    if (Mathf.Clamp(Mathf.PerlinNoise((currentTree.position.x + TreePerlinSeed) * TreePerlinScale * 100, (currentTree.position.z + TreePerlinSeed) * TreePerlinScale * 100) + TreePerlinBias, 0, 1) > 0.5f && !TreeInvertPerlin)
                    {
                        TreesList.Remove(currentTree);
                        DestroyedTrees += 1;

                    }
                    else if (Mathf.Clamp(Mathf.PerlinNoise((currentTree.position.x + TreePerlinSeed) * TreePerlinScale * 100, (currentTree.position.z + TreePerlinSeed) * TreePerlinScale * 100) + TreePerlinBias, 0, 1) < 0.5f && TreeInvertPerlin)
                    {
                        TreesList.Remove(currentTree);
                        DestroyedTrees += 1;
                    }
                }


            }

        }


        NewTrees = TreesList.ToArray();


        if (useTexture)
        {

            for (int i = 0; i < NewTrees.Length; i++)
            {
                TreeInstance currentTree = NewTrees[i];


                float TextureMix = Mathf.Abs(1 - GetTextureThreshold(TreePosToWorldPos(currentTree.position), TextureArrayIndex));

                if (currentTree.prototypeIndex == TreePrototypeIndex)
                {
                    NewTrees[i].heightScale = currentTree.heightScale * Mathf.Clamp(TextureMix + BorderSize, 0, 1);
                    NewTrees[i].widthScale = currentTree.widthScale * Mathf.Clamp(TextureMix + BorderSize, 0, 1);
                }
            }
        }


        currentTerrainData.SetTreeInstances(NewTrees, true);
        time = (float)EditorApplication.timeSinceStartup - time;
        Debug.Log(DestroyedTrees + " trees removed in " + Mathf.RoundToInt(time));
    

    }

    //Not implemented, but might be usefull ?
    public void KeepTreesTexture()
    {
        
        TreesBackup = currentTerrainData.treeInstances.ToList<TreeInstance>();
        List<TreeInstance> TreesList = currentTerrainData.treeInstances.ToList<TreeInstance>();

        float time = (float)EditorApplication.timeSinceStartup;

        int DestroyedTrees = 0;

        for (int i = 0; i < currentTerrainData.treeInstanceCount; i++)
        {
            TreeInstance currentTree = currentTerrainData.GetTreeInstance(i);
            if (currentTree.prototypeIndex == TreePrototypeIndex)
            {
                if (!(GetTextureThreshold(TreePosToWorldPos(currentTree.position), TextureArrayIndex) > Mathf.Abs(1 - transitionSmoothness)))
                {
                    TreesList.Remove(currentTree);
                    DestroyedTrees += 1;
                }
            }
        }

        TreeInstance[] NewTrees = TreesList.ToArray();

        for (int i = 0; i < NewTrees.Length; i++)
        {
            TreeInstance currentTree = NewTrees[i];


            float TextureMix =  GetTextureThreshold(TreePosToWorldPos(currentTree.position), TextureArrayIndex);

            if (currentTree.prototypeIndex == TreePrototypeIndex)
            {
                NewTrees[i].heightScale = currentTree.heightScale * Mathf.Clamp(TextureMix + BorderSize, 0, 1);
                NewTrees[i].widthScale = currentTree.widthScale * Mathf.Clamp(TextureMix + BorderSize, 0, 1);
            }
        }
        currentTerrainData.SetTreeInstances(NewTrees, true);
        time = (float)EditorApplication.timeSinceStartup - time;
        Debug.Log(DestroyedTrees + " trees removed in " + Mathf.RoundToInt(time));
    }

    public void DecimateTrees(int PercentageToKill)
    {

        float time = (float)EditorApplication.timeSinceStartup;

        if (PercentageToKill == 0)
            return;
        else if (PercentageToKill == 100)
        {
            RemoveTreesIndex();
            return;
        }

        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();
        List<TreeInstance> TreesList = NewTrees.ToList<TreeInstance>();

        int DestroyedTrees = 0;
        int rando;
        for (int i = 0; i < currentTerrainData.treeInstanceCount; i ++)
        {
            TreeInstance currentTree = currentTerrainData.GetTreeInstance(i);
            if (currentTree.prototypeIndex == TreePrototypeIndex)
            {
                rando = Random.Range(0, 100);
                if (rando < PercentageToKill)
                {
                    TreesList.Remove(currentTree);
                    DestroyedTrees += 1;
                }
                
            }
                
        }
        currentTerrainData.SetTreeInstances(TreesList.ToArray(), true);
        time = (float)EditorApplication.timeSinceStartup - time;
        Debug.Log(DestroyedTrees + " trees removed in " + Mathf.RoundToInt(time));
    }

  

   

    public void RandomizeSize()
    {
        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();
        for (int i = 0; i < NewTrees.Length; i++)
        {
            if (NewTrees[i].prototypeIndex == TreePrototypeIndex)
            {
                float RandomSize = Random.Range(SizeRandomness * -1, SizeRandomness);

                NewTrees[i].heightScale = (NewTrees[i].heightScale + RandomSize);
                NewTrees[i].widthScale = (NewTrees[i].widthScale + RandomSize);
            }
            

        }
        currentTerrainData.SetTreeInstances(NewTrees, false);

    }

    public void UnifySize()
    {
        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();


        for (int i = 0; i < NewTrees.Length; i++)
        {
            if (NewTrees[i].prototypeIndex == TreePrototypeIndex)
            {
                NewTrees[i].heightScale = 1;
                NewTrees[i].widthScale = 1;
            }
        }

        currentTerrainData.SetTreeInstances(NewTrees, false);
    }

    public void RandomizePosition()
    {
        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();
        for (int i = 0; i < NewTrees.Length; i++)
        {
            if (NewTrees[i].prototypeIndex == TreePrototypeIndex)
            {
                NewTrees[i].position.x += Random.Range(-0.05f, 0.05f) * PositionRandomness;
            NewTrees[i].position.x = Mathf.Clamp(NewTrees[i].position.x, 0, 1);
            NewTrees[i].position.z += Random.Range(-0.05f, 0.05f) * PositionRandomness;
            NewTrees[i].position.z = Mathf.Clamp(NewTrees[i].position.z, 0, 1);
            }
        }
        currentTerrainData.SetTreeInstances(NewTrees, true);
    }


    public void UndoTrees()
    {
        TreeInstance[] CurrentTrees = currentTerrainData.treeInstances;
        TreeInstance[] RestoredTrees = TreesBackup.ToArray();
        currentTerrainData.SetTreeInstances(RestoredTrees, false);
        TreesBackup = CurrentTrees.ToList();
    }

    private float[] GetTextureMix(Vector3 WorldPos)
    {


        // calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int)(((WorldPos.x - terrainPosition.x) / currentTerrainData.size.x) * currentTerrainData.alphamapWidth);
        int mapZ = (int)(((WorldPos.z - terrainPosition.z) / currentTerrainData.size.z) * currentTerrainData.alphamapHeight);
        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = currentTerrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }



    private float GetTextureThreshold(Vector3 WorldPos, int textureIndex)
    {

        float[] mix = GetTextureMix(WorldPos);
        return mix[textureIndex];
      
    }

    Vector3 TreePosToWorldPos(Vector3 TreePos)
    {
        Vector3 WorldPos = Vector3.zero;
        WorldPos.x = TreePos.x * currentTerrainData.size.x + transform.position.x;
        WorldPos.z = TreePos.z * currentTerrainData.size.z + transform.position.z;

        return WorldPos;
    }

    Vector3 WorldPosToTreePos(Vector3 WorldPos)
    {
        Vector3 TreePos = Vector3.zero;
        TreePos.x = (WorldPos.x - transform.position.x) / currentTerrainData.size.x;
        TreePos.z = (WorldPos.z - transform.position.z) / currentTerrainData.size.z;
        return TreePos;
    }

    public void TreeCount()
    {
        Debug.Log(currentTerrainData.treeInstanceCount + "trees on the terrain");
    }
    public void SelectedTreeCount()
    {
        int SelectedTrees = 0;
        foreach(TreeInstance tree in currentTerrainData.treeInstances)
        {
            if (tree.prototypeIndex == TreePrototypeIndex)
            {
                SelectedTrees += 1;
            }
        }
        Debug.Log(SelectedTrees + " trees of index " + TreePrototypeIndex + " on the terrain");
    }

    public void RemoveTreesIndex()
    {
        TreeInstance[] NewTrees = currentTerrainData.treeInstances;
        TreesBackup = NewTrees.ToList<TreeInstance>();
        List<TreeInstance> TreesList = NewTrees.ToList<TreeInstance>();

        for (int i = 0; i < currentTerrainData.treeInstanceCount; i++)
        {
            TreeInstance currentTree = currentTerrainData.GetTreeInstance(i);
            if (currentTree.prototypeIndex == TreePrototypeIndex)
            {

                TreesList.Remove(currentTree);
            }

        }
        currentTerrainData.SetTreeInstances(TreesList.ToArray(), true);
    }

    

    TreeInstance CreateTree(TreeInstance[] TreeSamples)
    {
        int prototypeIndex = TreeSamples[0].prototypeIndex;
        float heightScale = (TreeSamples[0].heightScale + TreeSamples[1].heightScale + TreeSamples[2].heightScale + TreeSamples[3].heightScale + TreeSamples[4].heightScale) / 5;
        float widthScale = (TreeSamples[0].widthScale + TreeSamples[1].widthScale + TreeSamples[2].widthScale + TreeSamples[3].widthScale + TreeSamples[4].widthScale) / 5;
        Color color = Color.white;
        Color lightmapColor = Color.white;
        float rotation = 0;
        Vector3 position = Vector3.zero;

        TreeInstance CreatedTree = new TreeInstance();

        CreatedTree.prototypeIndex = prototypeIndex;
        CreatedTree.heightScale = heightScale;
        CreatedTree.widthScale = widthScale;
        CreatedTree.color = color;
        CreatedTree.lightmapColor = lightmapColor;
        CreatedTree.rotation = rotation;
        CreatedTree.position = position;

        return CreatedTree;
   
    }


    public void AddDetailsTexture()
    {
        DetailBackup = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);


        float[,,] alphaMapData = currentTerrainData.GetAlphamaps(0, 0, currentTerrainData.alphamapWidth, currentTerrainData.alphamapHeight);//The terrain texture maps
        int[,] olddetailsMap = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);//The terrain detail maps(Where everything is placed)


        Texture2D temp = new Texture2D(currentTerrainData.alphamapWidth, currentTerrainData.alphamapHeight);
        for (int x = 0; x < currentTerrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < currentTerrainData.alphamapHeight; y++)
            {
                temp.SetPixel(x, y, new Color(0, 0, 0, alphaMapData[x, y, TextureArrayIndex]));
            }
        }
        temp.Apply();
        int targetLength = olddetailsMap.GetLength(0);
        TextureScale.Bilinear(temp, targetLength, targetLength);
        temp.Apply();

        int [,] detailsMap = new int[targetLength, targetLength];
        for (int x = 0; x < targetLength; x += 1)
        {
            for (int y = 0; y < targetLength; y += 1)
            {

               if (!DetailUsePerlin)
                {
                    detailsMap[x, y] = Mathf.RoundToInt(Mathf.Clamp(olddetailsMap[x, y], 0, 100) + (temp.GetPixel(x, y).a > (Mathf.Abs(1 - detailExpand)) ? DetailSpawnDensity : 0));
                }

               else
                {
                    if (!DetailInvertPerlin)
                    {
                        detailsMap[x, y] = Mathf.RoundToInt(Mathf.Clamp(olddetailsMap[x, y], 0, 100) + (temp.GetPixel(x, y).a > (Mathf.Abs(1 - detailExpand)) ? DetailSpawnDensity : 0) * (Mathf.Clamp(Mathf.PerlinNoise((x + DetailPerlinSeed) * DetaliPerlinScale / 10, (y + DetailPerlinSeed) * DetaliPerlinScale / 10) + DetailPerlinBias, 0 ,1) > 0.5f ? 0 : 1));
                    }
                    else
                    {
                        detailsMap[x, y] = Mathf.RoundToInt(Mathf.Clamp(olddetailsMap[x, y], 0, 100) + (temp.GetPixel(x, y).a > (Mathf.Abs(1 - detailExpand)) ? DetailSpawnDensity : 0) * (Mathf.Clamp(Mathf.PerlinNoise((x + DetailPerlinSeed) * DetaliPerlinScale / 10, (y + DetailPerlinSeed) * DetaliPerlinScale / 10) + DetailPerlinBias, 0, 1) > 0.5f ? 1 : 0));
                    }
                }
               
               

            }
        }

        currentTerrainData.SetDetailLayer(0, 0, DetailPrototypeIndex, detailsMap);
    }

    public void RemoveDetailsTexture()
    {
        DetailBackup = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);
        #region Get Terrain Data
        float[,,] alphaMapData = currentTerrainData.GetAlphamaps(0, 0, currentTerrainData.alphamapWidth, currentTerrainData.alphamapHeight);//The terrain texture maps
        int[,] olddetailsMap = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);//The terrain detail maps(Where everything is placed)
        #endregion
        #region Convert texture map data length to details map length
        Texture2D temp = new Texture2D(currentTerrainData.alphamapWidth, currentTerrainData.alphamapHeight);
        for (int x = 0; x < currentTerrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < currentTerrainData.alphamapHeight; y++)
            {
                temp.SetPixel(x, y, new Color(0, 0, 0, alphaMapData[x, y, TextureArrayIndex]));
            }
        }
        temp.Apply();
        int targetLength = olddetailsMap.GetLength(0);
        TextureScale.Bilinear(temp, targetLength, targetLength);
        temp.Apply();
        #endregion
        #region Apply detail data by user presents and selected texture
        int[,] detailsMap = new int[targetLength, targetLength];
        for (int x = 0; x < targetLength; x += 1)
        {
            for (int y = 0; y < targetLength; y += 1)
            {

                if (olddetailsMap[x, y] > 0)
                    detailsMap[x, y] = (temp.GetPixel(x, y).a > (Mathf.Abs(1 - detailExpand)) ? 0 : olddetailsMap[x, y]);

            }
        }
        #endregion
        currentTerrainData.SetDetailLayer(0, 0, DetailPrototypeIndex, detailsMap);
    }
    public void SetDetailDensity()
    {
        DetailBackup = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);
        int[,] detailsMap = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);
        int targetLength = detailsMap.GetLength(0);
        for (int x = 0; x<targetLength; x += 1)
        {
            for (int y = 0; y<targetLength; y += 1)
            {
                if (detailsMap[x, y] > 0)
                detailsMap[x, y] = DetailSpawnDensity;

            }
        }
        
        currentTerrainData.SetDetailLayer(0, 0, DetailPrototypeIndex, detailsMap);
    }

    public void AddDetailLacunarity(int PrototypeIndex)
    {
        DetailBackup = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, PrototypeIndex);
        bool LastwasHole = false;
        int[,] detailsMap = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, PrototypeIndex);
        
        int targetLength = detailsMap.GetLength(0);

        float rando;
 

        for (int x = 0; x < targetLength; x += 1)
        {
            for (int y = 0; y < targetLength; y += 1)
            {

                rando = Random.Range(0f, 1f); ;

                if (detailsMap[x, y] > 0)
                {
                    if (!LastwasHole)
                    {
                        if (rando < LacunarityProbability)
                        {

                            detailsMap[x, y] = 0;
                            LastwasHole = true;
                        }
                    }
                    else
                    {
                        if (rando < LacunarityProbability * 0.3f)
                        {

                            detailsMap[x, y] = 0;
                            LastwasHole = false;

                        }
                    }
                }


            }
        }

        currentTerrainData.SetDetailLayer(0, 0, PrototypeIndex, detailsMap);
    }

  
    public void CleanDetails(int layer)
    {
        DetailBackup = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);
        for (int i = 0; i < currentTerrainData.detailPrototypes.Length; i++)
        {
            int[,] map = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, layer == -1 ? i : layer);

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = 0;
                }
            }
            currentTerrainData.SetDetailLayer(0, 0, layer == -1 ? i : layer, map);
            if (layer != -1)
            {
                return;
            }
        }
    }

    public void UndoDetails()
    {
        int[,] temp = currentTerrainData.GetDetailLayer(0, 0, currentTerrainData.detailWidth, currentTerrainData.detailHeight, DetailPrototypeIndex);
        currentTerrainData.SetDetailLayer(0, 0, DetailPrototypeIndex, DetailBackup);
        DetailBackup = temp;
    }



}


