//Distant Terrain Mod for Daggerfall-Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;


using DistantTerrain;

namespace DistantTerrain
{
    public class _startupMod : MonoBehaviour
    {
       // public static Mod mod;
        private static GameObject gameobjectDistantTerrain = null;
        private static DistantTerrain componentDistantTerrain = null;

        // Settings
        private static bool enableTerrainTransition = true;
        private static bool enableFadeIntoSkybox = true;
        private static bool enableSeaReflections = false;
        private static bool enableImprovedTerrain = true;
        private static bool indicateLocations = true;

        private static Shader shaderDistantTerrainTilemap = null;
        private static Shader shaderBillboardBatchFaded = null;
        private static Shader shaderTransitionRingTilemap = null;
        private static Shader shaderTransitionRingTilemapTextureArray = null;

    
        public static void InitStart()
        {
            //mod = initParams.Mod;

            gameobjectDistantTerrain = new GameObject("DistantTerrain");
            componentDistantTerrain = gameobjectDistantTerrain.AddComponent<DistantTerrain>();

            //ModSettings settings = mod.GetSettings();

            //shaderDistantTerrainTilemap = Resources.Load<Shader>("Assets/Game/Mods/DistantTerrain/Shaders/DistantTerrainTilemap.shader");
            shaderDistantTerrainTilemap = Shader.Find("Daggerfall/DistantTerrain/DistantTerrainTilemap");
            //shaderDistantTerrainTilemap = mod.GetAsset<Shader>("Shaders/DistantTerrainTilemap.shader");
            //  shaderBillboardBatchFaded = Resources.Load<Shader>("Assets/Game/Mods/DistantTerrain/Shaders/DaggerfallBillboardBatchFaded.shader");
            shaderBillboardBatchFaded = Shader.Find("Daggerfall/DistantTerrain/BillboardBatchFaded");
            //shaderTransitionRingTilemap = Resources.Load<Shader>("Assets/Game/Mods/DistantTerrain/Shaders/TransitionRingTilemap.shader");
            shaderTransitionRingTilemap = Shader.Find("Daggerfall/DistantTerrain/TransitionRingTilemap");
            //shaderTransitionRingTilemapTextureArray = Resources.Load<Shader>("Assets/Game/Mods/DistantTerrain/Shaders/TransitionRingTilemapTextureArray.shader");
            shaderTransitionRingTilemapTextureArray = Shader.Find("Daggerfall/DistantTerrain/TransitionRingTilemapTextureArray");
            /*enableTerrainTransition = settings.GetValue<bool>("GeneralSettings", "TerrainTransition");
            enableFadeIntoSkybox = settings.GetValue<bool>("GeneralSettings", "FadeIntoSkybox");
            enableSeaReflections = settings.GetValue<bool>("GeneralSettings", "SeaReflections");
            enableImprovedTerrain = settings.GetValue<bool>("ImprovedTerrainSettings", "EnableImprovedTerrain");
            indicateLocations = settings.GetValue<bool>("ImprovedTerrainSettings", "IndicateLocations");*/

            componentDistantTerrain.EnableTerrainTransition = enableTerrainTransition;
            componentDistantTerrain.EnableFadeIntoSkybox = enableFadeIntoSkybox;
            componentDistantTerrain.EnableSeaReflections = enableSeaReflections;
            componentDistantTerrain.EnableImprovedTerrain = enableImprovedTerrain;
            componentDistantTerrain.IndicateLocations = indicateLocations;
            componentDistantTerrain.ShaderDistantTerrainTilemap = shaderDistantTerrainTilemap;
            componentDistantTerrain.ShaderBillboardBatchFaded = shaderBillboardBatchFaded;
            componentDistantTerrain.ShaderTransitionRingTilemap = shaderTransitionRingTilemap;
            componentDistantTerrain.ShaderTransitionRingTilemapTextureArray = shaderTransitionRingTilemapTextureArray;
        }

    /*    void Awake()
        {
            mod.IsReady = true;
        }*/


        //[Invoke(StateManager.StateTypes.Start)]
        //public static void InitStart(InitParams initParams)
        //{
        //    // check if debug gameobject is present, if so do not initalize mod
        //    if (GameObject.Find("debug_DistantTerrain"))
        //        return;

        //    // Get this mod
        //    mod = initParams.Mod;

        //    // Load settings.
        //    ModSettings settings = mod.GetSettings();

        //    // settings
        //    enableTerrainTransition = settings.GetValue<bool>("GeneralSettings", "TerrainTransition");
        //    enableFadeIntoSkybox = settings.GetValue<bool>("GeneralSettings", "FadeIntoSkybox");
        //    enableSeaReflections = settings.GetValue<bool>("GeneralSettings", "SeaReflections");
        //    enableImprovedTerrain = settings.GetValue<bool>("ImprovedTerrainSettings", "EnableImprovedTerrain");
        //    indicateLocations = settings.GetValue<bool>("ImprovedTerrainSettings", "IndicateLocations");

        //    shaderDistantTerrainTilemap = mod.GetAsset<Shader>("Shaders/DistantTerrainTilemap.shader");
        //    shaderBillboardBatchFaded = mod.GetAsset<Shader>("Shaders/DaggerfallBillboardBatchFaded.shader");
        //    shaderTransitionRingTilemap = mod.GetAsset<Shader>("Shaders/TransitionRingTilemap.shader");
        //    shaderTransitionRingTilemapTextureArray = mod.GetAsset<Shader>("Shaders/TransitionRingTilemapTextureArray.shader");

        //    initMod();

        //    //after finishing, set the mod's IsReady flag to true.
        //    ModManager.Instance.GetMod(initParams.ModTitle).IsReady = true;
        //}

        ///*  
        //*   used for debugging
        //*   howto debug:
        //*       -) add a dummy GameObject to DaggerfallUnityGame scene
        //*       -) attach this script (_startupMod) as component
        //*       -) deactivate mod in mod list (since dummy gameobject will start up mod)
        //*       -) attach debugger and set breakpoint to one of the mod's cs files and debug
        //*/
        //void Awake()
        //{
        //    shaderDistantTerrainTilemap = Shader.Find("Daggerfall/DistantTerrain/DistantTerrainTilemap");
        //    shaderBillboardBatchFaded = Shader.Find("Daggerfall/DistantTerrain/BillboardBatchFaded");
        //    shaderTransitionRingTilemap = Shader.Find("Daggerfall/DistantTerrain/TransitionRingTilemap");
        //    shaderTransitionRingTilemapTextureArray = Shader.Find("Daggerfall/DistantTerrain/TransitionRingTilemapTextureArray");

        //    initMod();
        //}

        //public static void initMod()
        //{
        //    gameobjectDistantTerrain = new GameObject("DistantTerrain");
        //    componentDistantTerrain = gameobjectDistantTerrain.AddComponent<DistantTerrain>();
        //    componentDistantTerrain.EnableTerrainTransition = enableTerrainTransition;
        //    componentDistantTerrain.EnableFadeIntoSkybox = enableFadeIntoSkybox;
        //    componentDistantTerrain.EnableSeaReflections = enableSeaReflections;
        //    componentDistantTerrain.EnableImprovedTerrain = enableImprovedTerrain;
        //    componentDistantTerrain.IndicateLocations = indicateLocations;
        //    componentDistantTerrain.ShaderDistantTerrainTilemap = shaderDistantTerrainTilemap;
        //    componentDistantTerrain.ShaderBillboardBatchFaded = shaderBillboardBatchFaded;
        //    componentDistantTerrain.ShaderTransitionRingTilemap = shaderTransitionRingTilemap;
        //    componentDistantTerrain.ShaderTransitionRingTilemapTextureArray = shaderTransitionRingTilemapTextureArray;
        //}
    }
}
