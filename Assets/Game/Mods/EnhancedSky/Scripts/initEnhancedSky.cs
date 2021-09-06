//Enhanced Sky for Daggerfall Tools for Unity by Lypyl, contact at lypyl@dfworkshop.net
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: LypyL
//Contact: Lypyl@dfworkshop.net
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//this file was added by Michael Rauter (Nystul)

/*  moon textures by Doubloonz @ nexus mods, w/ permission to use for DFTFU.
 *http://www.nexusmods.com/skyrim/mods/40785/
 * 
 */

// only use this define when not building the mod but using it as sub-project to debug (will trigger different prefab injection for debugging)
// IMPORTANT: if this line is used mod build will fail - so uncomment before building mod
//#define DEBUG

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;



using EnhancedSky;

namespace EnhancedSky
{
    public class _startupMod : MonoBehaviour
    {
       // public static Mod mod;
        private static GameObject gameobjectEnhancedSky = null;
        private static SkyManager componentSkyManager = null;
        private static PresetContainer componentPresetContainer = null;
        private static CloudGenerator componentCloudGenerator = null;

        // Settings
        private static bool enableSunFlare = false;
        private static int cloudQuality = 400;

        private static Shader shaderDepthMask = null;
        private static Shader shaderUnlitAlphaWithFade = null;

        //private static Object prefabEnhancedSkyController = null;

        public static Object containerPrefab;

        private static Material skyObjMat = null;
        private static Material skyMat = null;
        //private static Material cloudMat = null;
        //private static Material masserMat = null;
        //private static Material secundaMat = null;
        private static Material starsMat = null;
        private static Material starMaskMat = null;

        private static Flare sunFlare = null;

        //private static Preset presetPresetContainer = null;
        public static void InitStart()
        {
            // check if debug gameobject is present, if so do not initalize mod
          //  if (GameObject.Find("debug_EnhancedSky"))
       //     {
        //        return;
        //    }

            // Get this mod
          //  mod = initParams.Mod;

            // Load settings. Pass this mod as paramater
         //   ModSettings settings = mod.GetSettings();

            // settings
            //enableSunFlare = settings.GetBool("GeneralSettings", "UseSunFlare");
            //cloudQuality = settings.GetInt("GeneralSettings", "CloudQuality");

          //  enableSunFlare = settings.GetValue<bool>("GeneralSettings", "UseSunFlare");
          //  cloudQuality = settings.GetValue<int>("GeneralSettings", "CloudQuality");

        //    shaderDepthMask = mod.GetAsset<Shader>("Materials/Resources/DepthMask.shader") as Shader;        
         //   shaderUnlitAlphaWithFade = mod.GetAsset<Shader>("Materials/Resources/UnlitAlphaWithFade.shader") as Shader;


           // starsMat = mod.GetAsset<Material>("Materials/Resources/Stars") as Material;        
         //   skyMat = mod.GetAsset<Material>("Materials/Resources/Sky") as Material;
            starsMat = Resources.Load("EnhancedSkyRes/Materials/Resources/Stars", typeof(Material)) as Material;
            skyMat = Resources.Load("EnhancedSkyRes/Materials/Resources/Sky", typeof(Material)) as Material;
            //   sunFlare = mod.GetAsset<Flare>("LightFlares/Sun.flare") as Flare;
            sunFlare = Resources.Load("EnhancedSkyRes/LightFlares/Sun", typeof(Flare)) as Flare;
            //cloudMesh = mod.GetAsset<Mesh>("Model Imports/Hemisphere_01_smooth.fbx") as Mesh;

            //containerPrefab = mod.GetAsset<Object>("Prefabs/Resources/NewEnhancedSkyContainer.prefab") as Object;
            containerPrefab = Resources.Load<GameObject>("EnhancedSkyRes/Prefabs/Resources/NewEnhancedSkyContainer");
            //

            shaderDepthMask = Shader.Find("Transparent/DepthMask");
            shaderUnlitAlphaWithFade = Shader.Find("Unlit/UnlitAlphaWithFade");

            initMod();

            //after finishing, set the mod's IsReady flag to true.
          //  ModManager.Instance.GetMod(initParams.ModTitle).IsReady = true;
        }

        /* 
        *   used for debugging
        *   howto debug:
        *       -) add a dummy GameObject to DaggerfallUnityGame scene
        *       -) attach this script (_startupMod) as component
        *       -) deactivate mod in mod list (since dummy gameobject will start up mod)
        *       -) attach debugger and set breakpoint to one of the mod's cs files and debug
        */
      /*  void Awake()
        {
           
/*
#if DEBUG
            if (GameObject.Find("debug_EnhancedSky"))
            {
                containerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Game/Addons/EnhancedSky_standalone/Prefabs/Resources/NewEnhancedSkyContainer.prefab", typeof(Object));

                starsMat = AssetDatabase.LoadAssetAtPath("Assets/Game/Addons/EnhancedSky_standalone/Materials/Resources/Stars.mat", typeof(Material)) as Material;
                skyMat = AssetDatabase.LoadAssetAtPath("Assets/Game/Addons/EnhancedSky_standalone/Materials/Resources/Sky.mat", typeof(Material)) as Material;

                sunFlare = AssetDatabase.LoadAssetAtPath("Assets/Game/Addons/EnhancedSky_standalone/LightFlares/Sun.flare", typeof(Flare)) as Flare;
            }
#endif*/

        /*   initMod();
        }*/

        public static void initMod()
        {
            Debug.Log("init of EnhancedSky standalone");

            gameobjectEnhancedSky = new GameObject("EnhancedSkyController");

            componentPresetContainer = gameobjectEnhancedSky.AddComponent<PresetContainer>() as PresetContainer;
            SetPresetContainerValues(componentPresetContainer);
            PresetContainer.Instance = componentPresetContainer;

            componentSkyManager = gameobjectEnhancedSky.AddComponent<SkyManager>() as SkyManager;
            SkyManager.instance = componentSkyManager;                      

            componentCloudGenerator = gameobjectEnhancedSky.AddComponent<CloudGenerator>() as CloudGenerator;

            //componentSkyManager.ModSelf = mod;

            starMaskMat = new Material(shaderDepthMask);
            skyObjMat = new Material(shaderUnlitAlphaWithFade);            

            componentSkyManager.StarMaskMat = starMaskMat;
            componentSkyManager.SkyObjMat = skyObjMat;
            componentSkyManager.StarsMat = starsMat;
            componentSkyManager.SkyMat = skyMat;

            if (containerPrefab)
            {
                GameObject container = Instantiate(containerPrefab) as GameObject;
                container.transform.SetParent(GameManager.Instance.ExteriorParent.transform, true);

                componentSkyManager.Container = container;

                //container.AddComponent<MoonController>();
                //container.AddComponent<AmbientFogLightController>();

                //container.transform.Find("SkyCam").gameObject.AddComponent<SkyCam>();
                //container.transform.Find("Stars").Find("StarParticles").gameObject.AddComponent<StarController>();
                //container.transform.Find("Rotator").gameObject.AddComponent<RotationScript>();
                //container.transform.Find("cloudPrefab").gameObject.AddComponent<Cloud>();

                GameObject goParticles = container.transform.Find("Stars").Find("StarParticles").gameObject;
                goParticles.GetComponent<ParticleSystemRenderer>().sharedMaterial = new Material(shaderUnlitAlphaWithFade);

                GameObject goSun = container.transform.Find("Rotator").Find("Sun").gameObject;
                goSun.GetComponent<LensFlare>().flare = sunFlare;
            }
            else
                throw new System.NullReferenceException();            

            componentSkyManager.ToggleEnhancedSky(true);
            componentSkyManager.UseSunFlare = enableSunFlare;
            componentSkyManager.cloudQuality = cloudQuality;
            componentSkyManager.SkyObjectSizeChange(0); // set normal size

            
        }

        private static void SetPresetContainerValues(PresetContainer presetContainer)
        {            
            // set color base
            Gradient gradient = new Gradient();
            GradientAlphaKey[] gak = {
                new GradientAlphaKey(55.0f/255.0f, 0.0f),
                new GradientAlphaKey(75.0f/255.0f, 0.21f),
                new GradientAlphaKey(255.0f/255.0f, 0.31f),
                new GradientAlphaKey(255.0f/255.0f, 0.69f),
                new GradientAlphaKey(75.0f/255.0f, 0.79f),
                new GradientAlphaKey(75.0f/255.0f, 1.0f)
            };
            string[] colorsAsHex = { "#3C3C3C", "#727272", "#A8553E", "#DAD6D6", "#D6D6D6", "#C5BFBF", "#A8553E", "#3C3C3C" };
            Color[] colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            GradientColorKey[] gck = {
                new GradientColorKey(colors[0], 0.0f),
                new GradientColorKey(colors[1], 0.159f),
                new GradientColorKey(colors[2], 0.244f),
                new GradientColorKey(colors[3], 0.318f),
                new GradientColorKey(colors[4], 0.5f),
                new GradientColorKey(colors[5], 0.694f),
                new GradientColorKey(colors[6], 0.762f),
                new GradientColorKey(colors[7], 0.835f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.colorBase = gradient;

            // set color over
            gradient = new Gradient();
            gak = new GradientAlphaKey[] {
                new GradientAlphaKey(255.0f/255.0f, 0.0f),
                new GradientAlphaKey(255.0f/255.0f, 1.0f)
            };
            colorsAsHex = new string[] { "#131313", "#656565", "#C5C1C1", "#C2C2C2", "#656565", "#131313" };
            colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            gck = new GradientColorKey[] {
                new GradientColorKey(colors[0], 0.0f),
                new GradientColorKey(colors[1], 0.203f),
                new GradientColorKey(colors[2], 0.303f),
                new GradientColorKey(colors[3], 0.674f),
                new GradientColorKey(colors[4], 0.752f),
                new GradientColorKey(colors[5], 1.0f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.colorOver = gradient;

            // set fog base
            gradient = new Gradient();
            gak = new GradientAlphaKey[] {
                new GradientAlphaKey(255.0f/255.0f, 0.0f),
                new GradientAlphaKey(255.0f/255.0f, 1.0f)
            };
            colorsAsHex = new string[] { "#131313", "#353027", "#5E6E75", "#7C8382", "#6D6D6D", "#765338", "#171717", "#131313" };
            colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            gck = new GradientColorKey[] {
                new GradientColorKey(colors[0], 0.0f),
                new GradientColorKey(colors[1], 0.25f),
                new GradientColorKey(colors[2], 0.309f),
                new GradientColorKey(colors[3], 0.5f),
                new GradientColorKey(colors[4], 0.7f),
                new GradientColorKey(colors[5], 0.744f),
                new GradientColorKey(colors[6], 0.776f),
                new GradientColorKey(colors[7], 1.0f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.fogBase = gradient;

            // set fog over
            gradient = new Gradient();
            gak = new GradientAlphaKey[] {
                new GradientAlphaKey(255.0f/255.0f, 0.0f),
                new GradientAlphaKey(255.0f/255.0f, 1.0f)
            };
            colorsAsHex = new string[] { "#090808", "#7C7777", "#8E9396", "#7D706C", "#151515", "#131313" };
            colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            gck = new GradientColorKey[] {
                new GradientColorKey(colors[0], 0.2f),
                new GradientColorKey(colors[1], 0.309f),
                new GradientColorKey(colors[2], 0.5f),
                new GradientColorKey(colors[3], 0.7f),
                new GradientColorKey(colors[4], 0.81f),
                new GradientColorKey(colors[5], 1.0f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.fogOver = gradient;

            // set star gradient
            gradient = new Gradient();
            gak = new GradientAlphaKey[] {
                new GradientAlphaKey(255.0f/255.0f, 0.0f),
                new GradientAlphaKey(255.0f/255.0f, 0.19f),
                new GradientAlphaKey(0.0f/255.0f, 0.245f),
                new GradientAlphaKey(0.0f/255.0f, 0.75f),
                new GradientAlphaKey(255.0f/255.0f, 0.8f),
                new GradientAlphaKey(255.0f/255.0f, 1.0f)
            };
            colorsAsHex = new string[] { "#FFFFFF", "#FFFFFF" };
            colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            gck = new GradientColorKey[] {
                new GradientColorKey(colors[0], 0.0f),
                new GradientColorKey(colors[1], 1.0f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.starGradient = gradient;

            // set cloud noise base
            gradient = new Gradient();
            gak = new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f/255.0f, 0.45f),
                new GradientAlphaKey(217.0f/255.0f, 0.75f),
                new GradientAlphaKey(255.0f/255.0f, 1.0f)
            };
            colorsAsHex = new string[] { "#000000", "#989898", "#FFFFFF" };
            colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            gck = new GradientColorKey[] {
                new GradientColorKey(colors[0], 0.0f),
                new GradientColorKey(colors[1], 0.45f),
                new GradientColorKey(colors[2], 1.0f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.cloudNoiseBase = gradient;

            // set cloud noise over
            gradient = new Gradient();
            gak = new GradientAlphaKey[] {
                new GradientAlphaKey(207.0f/255.0f, 0.0f),
                new GradientAlphaKey(255.0f/255.0f, 1.0f)
            };
            colorsAsHex = new string[] { "#7C7C7C", "#FFFFFF" };
            colors = new Color[colorsAsHex.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                UnityEngine.ColorUtility.TryParseHtmlString(colorsAsHex[i], out colors[i]);
            }
            gck = new GradientColorKey[] {
                new GradientColorKey(colors[0], 0.0f),
                new GradientColorKey(colors[1], 1.0f)
            };
            gradient.alphaKeys = gak;
            gradient.colorKeys = gck;
            presetContainer.cloudNoiseOver = gradient;

            // set sky tint
            string colorAsHex = "#80808000";
            Color color;
            UnityEngine.ColorUtility.TryParseHtmlString(colorAsHex, out color);
            presetContainer.skyTint = color;

            // set masser color
            colorAsHex = "#AF4B4BFF";
            UnityEngine.ColorUtility.TryParseHtmlString(colorAsHex, out color);
            presetContainer.MasserColor = color;

            // set secunda color
            colorAsHex = "#C3C3C3FF";
            UnityEngine.ColorUtility.TryParseHtmlString(colorAsHex, out color);
            presetContainer.SecundaColor = color;

            // set atmosphere offset
            presetContainer.atmsphrOffset = 1;

            // set atmosphere base curve
            presetContainer.atmosphereBase = new AnimationCurve();
            presetContainer.atmosphereBase.preWrapMode = WrapMode.Clamp;
            presetContainer.atmosphereBase.postWrapMode = WrapMode.Clamp;
            presetContainer.atmosphereBase.keys = new Keyframe[] { new Keyframe(0.0f, 0.9f), new Keyframe(0.3f, 0.1f), new Keyframe(0.65f, 0.1f), new Keyframe(1.0f, 1.0f) };

            // set atmosphere over curve
            presetContainer.atmosphereOver = new AnimationCurve();
            presetContainer.atmosphereOver.preWrapMode = WrapMode.Loop;
            presetContainer.atmosphereOver.postWrapMode = WrapMode.Loop;
            presetContainer.atmosphereOver.keys = new Keyframe[] { new Keyframe(0.0f, -0.33f), new Keyframe(1.0f, -0.33f) };

            // set moon alpha base curve
            presetContainer.moonAlphaBase = new AnimationCurve();
            presetContainer.moonAlphaBase.preWrapMode = WrapMode.Clamp;
            presetContainer.moonAlphaBase.postWrapMode = WrapMode.Clamp;
            presetContainer.moonAlphaBase.keys = new Keyframe[] { new Keyframe(0.0f, 0.3f), new Keyframe(0.28f, 0.0f), new Keyframe(0.8f, 0.1f), new Keyframe(1.0f, 0.3f) };
            presetContainer.moonAlphaBase.SmoothTangents(1, 1.0f);
            presetContainer.moonAlphaBase.SmoothTangents(2, 1.0f);

            // set moon alpha over curve
            presetContainer.moonAlphaOver = new AnimationCurve();
            presetContainer.moonAlphaOver.preWrapMode = WrapMode.Loop;
            presetContainer.moonAlphaOver.postWrapMode = WrapMode.Loop;
            presetContainer.moonAlphaOver.keys = new Keyframe[] { new Keyframe(0.0f, 0.045f), new Keyframe(0.29f, 0.0f), new Keyframe(1.0f, 0.045f) };
            presetContainer.moonAlphaOver.SmoothTangents(1, 1.0f);
        }
    }
}
