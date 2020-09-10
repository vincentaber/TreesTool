using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

[CustomEditor(typeof(Trees_Tool))]
public class Trees_Tool_Inspector : Editor
{
    bool ShowTrees = false;
    bool ShowDetail = false;

    
    public override void OnInspectorGUI()
    {
        GUIStyle UndoStyle = new GUIStyle(GUI.skin.button);
        UndoStyle.normal.textColor = Color.red;
        
        Trees_Tool TreesTool = (Trees_Tool)target;

        if (GUILayout.Button("Initialize"))
        {
            TreesTool.InitTrees();

        }

        TreesTool.useExperimental = EditorGUILayout.Toggle("Use Experimental Features", TreesTool.useExperimental);


        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Slope", GUILayout.Width(80));
        TreesTool.minSteepness = EditorGUILayout.FloatField(TreesTool.minSteepness, GUILayout.Width(21));
        EditorGUILayout.MinMaxSlider(ref TreesTool.minSteepness, ref TreesTool.maxSteepness, 0f, 90f);
        TreesTool.maxSteepness = EditorGUILayout.FloatField(TreesTool.maxSteepness, GUILayout.Width(21));
        EditorGUILayout.EndHorizontal();
        if (TreesTool.useExperimental)
             EditorGUILayout.MinMaxSlider("Height", ref TreesTool.minHeight, ref TreesTool.maxHeight, 0f, 1f);


        TreesTool.TextureArrayIndex = EditorGUILayout.IntSlider("Texture Index", TreesTool.TextureArrayIndex, 0, TreesTool.currentTerrainData.terrainLayers.Length - 1);


        Texture TerrainTexture = TreesTool.currentTerrainData.terrainLayers[TreesTool.TextureArrayIndex].diffuseTexture;
        GUILayout.Box(TerrainTexture, GUILayout.Width(120), GUILayout.Height(120));

        GUILayout.Space(10);

        


        ShowTrees = EditorGUILayout.BeginFoldoutHeaderGroup(ShowTrees, "Trees");


        if (ShowTrees)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use Texture");
            GUILayout.Label("Use Slope");
            if (TreesTool.useExperimental)
                GUILayout.Label("Use Height");
            

            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            TreesTool.UseTexture = EditorGUILayout.Toggle(TreesTool.UseTexture);
            TreesTool.UseSlope = EditorGUILayout.Toggle(TreesTool.UseSlope);
            if(TreesTool.useExperimental)
                TreesTool.UseHeight = EditorGUILayout.Toggle(TreesTool.UseHeight);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (TreesTool.useExperimental)
            {
                EditorGUILayout.HelpBox("The Height feature can be slow", MessageType.Warning, true);
                GUILayout.Space(10);
            }
                

            TreesTool.TreeUsePerlin = EditorGUILayout.BeginToggleGroup("Use Perlin Noise", TreesTool.TreeUsePerlin);
            if (TreesTool.TreeUsePerlin)
            {
                TreesTool.TreePerlinScale = EditorGUILayout.Slider("Perlin Scale", TreesTool.TreePerlinScale, 0, 5);

                TreesTool.TreeInvertPerlin = EditorGUILayout.Toggle("Invert Perlin", TreesTool.TreeInvertPerlin);
                TreesTool.TreePerlinSeed = EditorGUILayout.IntField("Perlin Seed", TreesTool.TreePerlinSeed);

                TreesTool.TreePerlinBias = EditorGUILayout.Slider("PerlinBias", TreesTool.TreePerlinBias, -0.5f, 0.5f);
            }
            
            EditorGUILayout.EndToggleGroup();

            GUILayout.Space(15);
            GUILayout.Label("Spawn", EditorStyles.boldLabel);


        TreesTool.TreePrototypeIndex = EditorGUILayout.IntSlider("Tree Index", TreesTool.TreePrototypeIndex, 0, TreesTool.currentTerrainData.treePrototypes.Length - 1);

        TreesTool.TreeSpawnDensity = EditorGUILayout.IntSlider("Spawn Density", TreesTool.TreeSpawnDensity, 1, 100);

       EditorGUILayout.Space(10);
       TreesTool.transitionSmoothness = EditorGUILayout.Slider("Transition Smoothness", TreesTool.transitionSmoothness, 0, 1);
        TreesTool.BorderSize = EditorGUILayout.Slider("Border Size", TreesTool.BorderSize, 0, 1);

      EditorGUILayout.Space(10);

        TreesTool.PositionRandomness = EditorGUILayout.Slider("Position Randomness", TreesTool.PositionRandomness, 0, 1);
        TreesTool.SizeRandomness = EditorGUILayout.Slider("Size Randomness", TreesTool.SizeRandomness, 0, 1);

            if (GUILayout.Button("Spawn"))
            {
                
                    TreesTool.AddTree(TreesTool.UseTexture,TreesTool.UseSlope, TreesTool.UseHeight);
     
            }

            GUILayout.Space(15);


            if(TreesTool.useExperimental)
            {
                GUILayout.Label("Remove", EditorStyles.boldLabel);

                if (GUILayout.Button("Remove"))
                {
                    TreesTool.RemoveTrees(TreesTool.UseTexture, TreesTool.UseSlope, TreesTool.UseHeight);

                }
                EditorGUILayout.HelpBox("The Remove feature can be slow when used on a lot of trees.", MessageType.Warning);

                TreesTool.DecimatePercentage = EditorGUILayout.IntSlider("Decimate Percentage", TreesTool.DecimatePercentage, 1, 100);
                if (GUILayout.Button("Decimate"))
                {
                    TreesTool.DecimateTrees(TreesTool.DecimatePercentage);
                }
                EditorGUILayout.HelpBox("The Decimate feature can be slow when used on a lot of trees.", MessageType.Warning);
            }
            
            GUILayout.BeginHorizontal();
        if (GUILayout.Button("Randomize Size"))
        {
            TreesTool.RandomizeSize();
        }

        if (GUILayout.Button("Randomize Position"))
        {
            TreesTool.RandomizePosition();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Unify Size"))
        {
            TreesTool.UnifySize();
        }


        

        if (GUILayout.Button("Undo", UndoStyle))
        {
            TreesTool.UndoTrees();
        }

        


            GUILayout.Space(10);

        EditorGUILayout.LabelField("Utilities", EditorStyles.centeredGreyMiniLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Total Tree Count"))
        {
            TreesTool.TreeCount();
        }
        if (GUILayout.Button("Selected Tree Count"))
        {
            TreesTool.SelectedTreeCount();
        }
        GUILayout.EndHorizontal();

        }

        EditorGUILayout.EndFoldoutHeaderGroup();


        ShowDetail = EditorGUILayout.BeginFoldoutHeaderGroup(ShowDetail, "Details");


        if (ShowDetail)
        {

            TreesTool.DetailPrototypeIndex = EditorGUILayout.IntSlider("Detail Index", TreesTool.DetailPrototypeIndex, 0, TreesTool.currentTerrainData.detailPrototypes.Length - 1);
            Texture DetailTexture = TreesTool.currentTerrainData.detailPrototypes[TreesTool.DetailPrototypeIndex].prototypeTexture;
            GUILayout.Box(DetailTexture, GUILayout.Width(120), GUILayout.Height(120));
           


            TreesTool.DetailSpawnDensity = EditorGUILayout.IntSlider("Spawn Density", TreesTool.DetailSpawnDensity, 1, 10);
            TreesTool.detailExpand = EditorGUILayout.Slider("Expand Mask", TreesTool.detailExpand, 0, 1);
            TreesTool.LacunarityProbability = EditorGUILayout.Slider("LacunarityProbability", TreesTool.LacunarityProbability, 0, 1);

            TreesTool.DetailUsePerlin = EditorGUILayout.BeginToggleGroup("Use Perlin Noise", TreesTool.DetailUsePerlin);
            if(TreesTool.DetailUsePerlin)
            {
                TreesTool.DetaliPerlinScale = EditorGUILayout.Slider("Perlin Scale", TreesTool.DetaliPerlinScale, 0, 4);

                TreesTool.DetailInvertPerlin = EditorGUILayout.Toggle("Invert Perlin", TreesTool.DetailInvertPerlin);
                TreesTool.DetailPerlinSeed = EditorGUILayout.IntField("Perlin Seed", TreesTool.DetailPerlinSeed);

                TreesTool.DetailPerlinBias = EditorGUILayout.Slider("PerlinBias", TreesTool.DetailPerlinBias, -0.5f, 0.5f);
            }

            EditorGUILayout.EndToggleGroup();


            if (GUILayout.Button("Spawn on Texture"))
            {
                TreesTool.AddDetailsTexture();
            }

            if (GUILayout.Button("Set Density"))
            {
                TreesTool.SetDetailDensity();
            }

            if (GUILayout.Button("Add Lacunarity"))
            {
                TreesTool.AddDetailLacunarity(TreesTool.DetailPrototypeIndex);
            }

            if (GUILayout.Button("Clean Detail"))
            {
                TreesTool.CleanDetails(TreesTool.DetailPrototypeIndex);
            }

            if (GUILayout.Button("Remove from Texture"))
            {
                TreesTool.RemoveDetailsTexture();
            }

            if (GUILayout.Button("Undo", UndoStyle))
            {
                TreesTool.UndoDetails();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

    }


}
