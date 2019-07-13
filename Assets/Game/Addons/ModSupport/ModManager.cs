// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Utility.ModSupport
{
    public class ModManager : MonoBehaviour
    {
        #region Fields

        public const string MODEXTENSION        = ".dfmod";
        public const string MODINFOEXTENSION    = ".dfmod.json";
        public const string MODCONFIGFILENAME   = "Mod_Settings.json";
        bool alreadyAtStartMenuState            = false;
        static bool alreadyStartedInit          = false;
        int loadedModCount = 0;
        [SerializeField]
        List<Mod> Mods;
        public static readonly fsSerializer _serializer = new fsSerializer();

        public static string[] textExtensions = new string[]
        {
            ".txt",
            ".html",
            ".html",
            ".xml",
            ".bytes",
            ".json",
            ".csv",
            ".yaml",
            ".fnt",
        };

        #endregion

        #region Properties

        public int LoadedModCount
        {
            get { return loadedModCount; }
            private set { loadedModCount = value; }
        }

        public string ModDirectory { get; set; }

        public static ModManager Instance { get; private set; }

        #endregion

        #region Unity

        void Awake()
        {
            if (string.IsNullOrEmpty(ModDirectory))
                ModDirectory = Path.Combine(Application.streamingAssetsPath, "Mods");
            if (!Directory.Exists(ModDirectory))
            {
                var di = Directory.CreateDirectory(ModDirectory);
                if (!di.Exists)
                {
                    Debug.LogError(string.Format("Mod Directory doesn't exist {0}", ModDirectory));
                }
            }

            SetupSingleton();

            if (Instance == this)
                StateManager.OnStateChange += StateManager_OnStateChange;
        }

        // Use this for initialization
        void Start()
        {
            if (!DaggerfallUnity.Settings.LypyL_ModSystem)
            {
                Debug.Log("Mod System disabled");
                StateManager.OnStateChange -= StateManager_OnStateChange;
                Destroy(this);
            }
            Mods = new List<Mod>();
            FindModsFromDirectory();
            LoadModSettings();
        }

        void Update()
        {
            LoadedModCount = Mods.Count;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get index for mod by title
        /// </summary>
        /// <param name="modTitle">The title of a mod.</param>
        /// <returns>The index of the mod with the given title or -1 if not found.</returns>
        public int GetModIndex(string modTitle)
        {
            if (string.IsNullOrEmpty(modTitle))
                return -1;

            for (int i = 0; i < Mods.Count; i++)
            {
                if (Mods[i].Title == modTitle)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Get mod using index
        /// </summary>
        /// <param name="index">The index of a mod.</param>
        /// <returns>The mod at the given index or null if the index is invalid.</returns>
        public Mod GetMod(int index)
        {
            if (index < 0 || index > Mods.Count)
                return null;
            else
                return Mods[index];
        }

        /// <summary>
        /// Get mod using Mod Title
        /// </summary>
        /// <param name="modTitle">The title of a mod.</param>
        /// <returns>The mod with the given title or null if not found.</returns>
        public Mod GetMod(string modTitle)
        {
            int index = GetModIndex(modTitle);

            if (index >= 0)
                return Mods[index];
            else
                return null;
        }

        /// <summary>
        /// Get mod from GUID
        /// </summary>
        /// <param name="modGUID">The unique identifier of a mod.</param>
        /// <returns>The mod with the given GUID or null if not found.</returns>
        public Mod GetModFromGUID(string modGUID)
        {
            if (string.IsNullOrEmpty(modGUID))
                return null;
            else if (modGUID == "invalid")
                return null;
            else
            {
                foreach (var mod in Mods)
                {
                    if (mod.GUID == modGUID)
                        return mod;
                }

                return null;
            }
        }

        /// <summary>
        /// Get mod title from GUID
        /// </summary>
        /// <param name="modGUID">The unique identifier of a mod.</param>
        /// <returns>The title of the mod with the given GUID or null if not found.</returns>
        public string GetModTitleFromGUID(string modGUID)
        {
            if (string.IsNullOrEmpty(modGUID))
                return null;
            else if (modGUID == "invalid")
                return null;
            else
            {
                foreach (var mod in Mods)
                {
                    if (mod.GUID == modGUID)
                        return mod.Title;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns all loaded mods in array
        /// </summary>
        /// <param name="loadOrder">ordered by load priority if true</param>
        /// <returns>A collection with all the mods.</returns>
        public Mod[] GetAllMods(bool loadOrder = false)
        {
            var selection = from mod in Mods
                            select mod;
            if (loadOrder)
                return selection.OrderBy(x => x.LoadPriority).ToArray();
            else
                return selection.ToArray();
        }

        /// <summary>
        /// Get modtitle string for each loaded mod
        /// </summary>
        /// <returns>A collection with all mod titles.</returns>
        public string[] GetAllModTitles()
        {
            var selection = from modInfo in GetAllModInfo()
                            select modInfo.ModTitle;
            return selection.ToArray();
        }

        /// <summary>
        /// Get mod file name string for each loaded mod
        /// </summary>
        /// <returns>A collection with all mod file names.</returns>
        public string[] GetAllModFileNames()
        {
            var selection = from mod in Mods
                            select mod.FileName;
            return selection.ToArray();
        }

        /// <summary>
        /// Get array of ModInfo objects for each loaded mod
        /// </summary>
        /// <returns>A collection with all mod informations.</returns>
        public ModInfo[] GetAllModInfo()
        {
            var selection = from mod in GetAllMods()
                            where (mod.ModInfo != null)
                            select mod.ModInfo;
            return selection.ToArray();
        }

        /// <summary>
        /// Gets all the mod GUIDs which are defined and valid.
        /// </summary>
        /// <returns>A collection of valid mod GUIDs.</returns>
        public string[] GetAllModGUID()
        {
            var selection = from mod in Mods
                            where (mod.ModInfo != null && mod.GUID != "invalid")
                            select mod.ModInfo.GUID;
            return selection.ToArray();
        }

        /// <summary>
        /// Gets all mods wich provide contributes to save data.
        /// </summary>
        /// <returns>An enumeration of mods with save data.</returns>
        public IEnumerable<Mod> GetAllModsWithSaveData()
        {
            return from mod in Mods
                   where mod.SaveDataInterface != null
                   select mod;
        }

        /// <summary>
        /// Gets all mods, in reverse order, that provide contributes that match the given filter.
        /// </summary>
        /// <param name="filter">A filter that accepts or rejects a mod; can be used to check if a contribute is present.</param>
        /// <returns>An enumeration of mods with contributes.</returns>
        internal IOrderedEnumerable<Mod> GetAllModsWithContributes(Predicate<ModContributes> filter = null)
        {
            return from mod in Mods
                   where mod.ModInfo.Contributes != null && (filter == null || filter(mod.ModInfo.Contributes))
                   orderby mod.LoadPriority descending
                   select mod;
        }

        /// <summary>
        /// Get all asset names from mod
        /// </summary>
        /// <param name="modTitle">The title of a mod.</param>
        /// <returns>A collection with the names of all assets from a mod or null if not found.</returns>
        public string[] GetModAssetNames(string modTitle)
        {
            int index = GetModIndex(modTitle);
            if (index < 0)
                return null;
            else
                return Mods[index].AssetNames;
        }

        /// <summary>
        /// Get type t asset from mod using name of asset
        /// </summary>
        /// <typeparam name="T">Asset Type</typeparam>
        /// <param name="assetName">asset name</param>
        /// <param name="modTitle">title of mod</param>
        /// <param name="clone">return copy of asset</param>
        ///<param name="check">true if loaded sucessfully</param>
        /// <returns>The loaded asset or null if not found.</returns>
        public T GetAssetFromMod<T>(string assetName, string modTitle, bool clone, out bool check) where T : UnityEngine.Object
        {
            check = false;
            T asset = null;

            int index = GetModIndex(modTitle);

            if (index < 0)
                return null;

            asset = Mods[index].GetAsset<T>(assetName, clone);
            check = asset != null;
            return asset;
        }

        /// <summary>
        /// Seek asset in all mods with load order.
        /// </summary>
        /// <param name="name">Name of asset to seek.</param>
        /// <param name="clone">Make a copy of asset?</param>
        /// <param name="asset">Loaded asset or null.</param>
        /// <remarks>
        /// If multiple mods contain an asset with given name, priority is defined by load order.
        /// </remarks>
        /// <returns>True if asset is found and loaded sucessfully.</returns>
        public bool TryGetAsset<T>(string name, bool clone, out T asset) where T : UnityEngine.Object
        {
            var query = from mod in Mods where mod.AssetBundle != null
                        orderby mod.LoadPriority descending
                        where mod.AssetBundle.Contains(name)
                        select mod.GetAsset<T>(name, clone);

            return (asset = query.FirstOrDefault()) != null;
        }

        /// <summary>
        /// Seek asset in all mods with load order.
        /// Check all names for each mod with the given priority.
        /// </summary>
        /// <param name="names">Names of asset to seek ordered by priority.</param>
        /// <param name="clone">Make a copy of asset?</param>
        /// <param name="asset">Loaded asset or null.</param>
        /// <remarks>
        /// If multiple mods contain an asset with any of the given names, priority is defined by load order.
        /// If chosen mod contains multiple assets, priority is defined by order of names list.
        /// </remarks>
        /// <returns>True if asset is found and loaded sucessfully.</returns>
        public bool TryGetAsset<T>(string[] names, bool clone, out T asset) where T : UnityEngine.Object
        {
            var query = from mod in Mods where mod.AssetBundle != null
                        orderby mod.LoadPriority descending
                        from name in names where mod.AssetBundle.Contains(name)
                        select mod.GetAsset<T>(name, clone);

            return (asset = query.FirstOrDefault()) != null;
        }

        /// <summary>
        /// convert full relative path to just the asset name for example:
        /// /Assets/examples/myscript.cs to myscript.cs
        /// </summary>
        /// <param name="assetPath">The full path of an asset.</param>
        /// <returns>The name of the file in the given path with the extension.</returns>
        public static string GetAssetName(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;
            int startIndex = assetPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
            return assetPath.Substring(startIndex).ToLower();
        }

        /// <summary>
        /// Utility method for gettings lines of text from a text asset
        /// </summary>
        public static List<string> GetTextAssetLines(TextAsset asset)
        {
            List<string> lines = new List<string>();
            string line;
            using (StringReader reader = new StringReader(asset.text))
            {
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
            }
            return lines;
        }

        #endregion

        #region Mod Loading & setup

        /// <summary>
        /// Look for changes in mod directory before the compiling / loading process has begun.
        /// </summary>
        public void Refresh()
        {
            if (!alreadyAtStartMenuState)
                FindModsFromDirectory(true);
        }

        /// <summary>
        /// Locates all the .dfmod files in the mod path
        /// </summary>
        /// <param name="refresh">Checks for mods to unload.</param>
        private void FindModsFromDirectory(bool refresh = false)
        {
            if (!Directory.Exists(ModDirectory))
            {
                Debug.Log("invalid mod directory: " + ModDirectory);
                return;
            }

            var modFiles = Directory.GetFiles(ModDirectory, "*" + MODEXTENSION, SearchOption.AllDirectories);
            var modFileNames = new string[modFiles.Length];
            var loadedModNames = GetAllModFileNames();

            for (int i = 0; i < modFiles.Length; i++)
            {
                string modFilePath = modFiles[i];

                string DirPath = modFilePath.Substring(0, modFilePath.LastIndexOf(Path.DirectorySeparatorChar));
                modFileNames[i] = GetModNameFromPath(modFilePath);

                if (string.IsNullOrEmpty(modFileNames[i]))
                {
                    Debug.Log("failed to get name of mod");
                    continue;
                }

                //prevent trying to re-load same asset bundles on refresh
                if (loadedModNames.Length > 0)
                {
                    if (loadedModNames.Contains(modFileNames[i]))
                        continue;
                }

                AssetBundle ab;
                if (!LoadModAssetBundle(modFilePath, out ab))
                    continue;
                Mod mod = new Mod(modFileNames[i], DirPath, ab);

                mod.LoadPriority = i;
                int index = GetModIndex(mod.Title);
                if (index < 0)
                    Mods.Add(mod);
            }

            if (refresh)
            {
                for (int j = 0; j < loadedModNames.Length; j++)
                {
                    if (!modFileNames.Contains(loadedModNames[j]))
                    {
                        Debug.Log(string.Format("mod {0} no longer loaded", loadedModNames[j]));
                        UnloadMod(loadedModNames[j], true);
                    }
                }
            }
        }

        // Loads Asset bundle and adds to ModLookUp dictionary
        private static bool LoadModAssetBundle(string modFilePath, out AssetBundle ab)
        {
            ab = null;
            if (!File.Exists(modFilePath))
            {
                Debug.Log(string.Format("Asset Bundle not found: {0}", modFilePath));
                return false;
            }

            try
            {
                ab = AssetBundle.LoadFromFile(modFilePath);
                return ab != null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        // Unload mod and related asset bundle
        private bool UnloadMod(string modTitle, bool unloadAllAssets)
        {
            try
            {
                int index = GetModIndex(modTitle);
                if (index < 0)
                {
                    Debug.Log("Failed to unload mod as mod title wasn't found: " + modTitle);
                    return false;
                }
                Mods[index].AssetBundle.Unload(unloadAllAssets);
                Mods.RemoveAt(index);
                OnUnloadMod(modTitle);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        //begin setting up mods
        private void Init()
        {
            if (alreadyStartedInit)
                return;

            alreadyStartedInit = true;
            WriteModSettings();

            Mod[] mods = GetAllMods();

            for (int i = 0; i < mods.Length; i++)
            {
                Mod mod = mods[i];

                if (mod == null || !mod.Enabled)
                {
                    Debug.Log("removing mod at index: " + i);
                    UnloadMod(mod.Title, true);
                    continue;
                }
                Debug.Log("ModManager - started loading mod: " + mod.Title);
                mod.CompileSourceToAssemblies();
            }
            Debug.Log("ModManager - init finished.  Mod Count: " + LoadedModCount);
        }

        private void InvokeModLoaders(StateManager.StateTypes state)
        {
#if DEBUG
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
#endif
            if (alreadyAtStartMenuState)
            {
                Mod[] mods = GetAllMods(true);

                for (int i = 0; i < mods.Length; i++)
                {
                    try
                    {
                        List<SetupOptions> setupOptions = mods[i].FindModLoaders(state);

                        if (setupOptions == null)
                        {
                            Debug.Log("No mod loaders found for mod: " + mods[i].Title);
                            continue;
                        }

                        for (int j = 0; j < setupOptions.Count; j++)
                        {
                            SetupOptions options = setupOptions[j];
                            MethodInfo mi = options.mi;
                            if (mi == null)
                                continue;
                            InitParams initParams = new InitParams(options.mod, ModManager.Instance.GetModIndex(options.mod.Title), LoadedModCount);
                            mi.Invoke(null, new object[] { initParams });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
#if DEBUG
                timer.Stop();
                Debug.Log("InvokeModLoaders() finished...time: " + timer.ElapsedMilliseconds);
#endif
            }
        }

        #endregion

        #region Mod Source Loading/Compiling

        /// <summary>
        /// Compile source files in mod bundle to assembly
        /// </summary>
        /// <param name="source">The content of source files.</param>
        /// <returns>The compiled assembly or null.</returns>
        public static Assembly CompileFromSourceAssets(string[] source)
        {
            if (source == null || source.Length < 1)
            {
                Debug.Log("nothing to compile");
                return null;
            }

            Assembly assembly;

            try
            {
                assembly = Compiler.CompileSource(source, true);
                return assembly;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Writes mod settings (title, priority, enabled) to file
        /// </summary>
        /// <returns>True if settings written successfully.</returns>
        public static bool WriteModSettings()
        {
            try
            {
                if (ModManager.Instance.Mods == null || ModManager.Instance.Mods.Count <= 0)
                {
                    return false;
                }

                fsData sdata = null;
                var result = _serializer.TrySerialize<List<Mod>>(ModManager.Instance.Mods, out sdata);

                if (result.Failed)
                {
                    return false;
                }

                File.WriteAllText(Path.Combine(ModManager.Instance.ModDirectory, MODCONFIGFILENAME), fsJsonPrinter.PrettyJson(sdata));
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Failed to write mod settings: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Attempts to load saved mod settings from file and updates loaded mods.
        /// </summary>
        /// <returns>True if settings loaded successfully.</returns>
        public static bool LoadModSettings()
        {
           fsResult result = new fsResult();

            try
            {
                string filepath = Path.Combine(ModManager.Instance.ModDirectory, MODCONFIGFILENAME);
                if (!File.Exists(filepath))
                    return false;

                var serializedData = File.ReadAllText(filepath);
                if (string.IsNullOrEmpty(serializedData))
                    return false;

                List<Mod> temp = new List<Mod>();
                fsData data = fsJsonParser.Parse(serializedData);
                result = _serializer.TryDeserialize<List<Mod>>(data, ref temp);

                if (result.Failed || temp.Count <= 0)
                    return false;

                foreach (Mod _mod in temp)
                {
                    if (ModManager.Instance.GetModIndex(_mod.Title) >= 0)
                    {
                        Mod mod = ModManager.Instance.GetMod(_mod.Title);
                        if (mod == null)
                            continue;
                        mod.Enabled = _mod.Enabled;
                        mod.LoadPriority = _mod.LoadPriority;
                        ModManager.Instance.Mods[ModManager.Instance.GetModIndex(_mod.Title)] = mod;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error trying to load mod settings: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Helper function to assist with serializing and deserializing prefabs.
        /// </summary>
        /// <param name="trans">Parent transform.</param>
        /// <param name="transforms">A list already containing data or null to request a new list.</param>
        /// <returns>The list of transforms.</returns>
        public static List<Transform> GetAllChildren(Transform trans, ref List<Transform> transforms)
        {
            if (transforms == null)
                transforms = new List<Transform>() { trans };
            else
                transforms.Add(trans);

            for (int i = 0; i < trans.childCount; i++)
            {
                GetAllChildren(trans.GetChild(i), ref transforms);
            }

            return transforms;
        }

        /// <summary>
        /// Send data to a mod that has a valid DFModMessageReceiver delegate.
        /// </summary>
        /// <param name="modTitle">The title of the target mod.</param>
        /// <param name="message">A string to be sent to the target mod.</param>
        /// <param name="data">Data to send with the message.</param>
        /// <param name="callback">An optional message callback.</param>
        public void SendModMessage(string modTitle, string message, object data = null, DFModMessageCallback callback = null)
        {
            if (Mods == null || Mods.Count < 1)
                return;
            var mod = GetMod(modTitle);
            if (mod == null || mod.MessageReceiver == null)
                return;
            else
                mod.MessageReceiver(message, data, callback);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Seeks asset contributes for the target mod, reading the folder name of each asset.
        /// </summary>
        /// <param name="modInfo">Manifest data for a mod, which will be filled with retrieved contributes.</param>
        /// <remarks>
        /// Assets are imported from loose files according to folder name,
        /// for example all textures inside `SpellIcons` are considered icon atlases.
        /// This method replicates the same behaviour for mods, doing all the hard work at build time.
        /// Results are stored to json manifest file for performant queries at runtime.
        /// </remarks>
        public static void SeekModContributes(ModInfo modInfo)
        {
            List<string> spellIcons = null;
            List<string> booksMapping = null;

            foreach (string file in modInfo.Files)
            {
                string directory = Path.GetDirectoryName(file);

                if (directory.EndsWith("SpellIcons"))
                    AddNameToList(ref spellIcons, file);
                else if (directory.EndsWith("Books/Mapping"))
                    AddNameToList(ref booksMapping, file);
            }

            if (spellIcons != null || booksMapping != null)
            {
                var contributes = modInfo.Contributes ?? (modInfo.Contributes = new ModContributes());
                contributes.SpellIcons = spellIcons != null ? spellIcons.ToArray() : null;
                contributes.BooksMapping = booksMapping != null ? booksMapping.ToArray() : null;
            }
        }

        private static void AddNameToList(ref List<string> names, string path)
        {
            if (names == null)
                names = new List<string>();

            string name = Path.GetFileNameWithoutExtension(path);
            if (!names.Contains(name))
                names.Add(name);
        }
#endif

        #endregion

        #region Internal methods

        /// <summary>
        /// Gets a localized string for a mod system text.
        /// </summary>
        internal static string GetText(string key)
        {
            return TextManager.Instance.GetText("ModSystem", key);
        }

        #endregion

        #region Private Methods

        private static string GetModNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(MODEXTENSION, "");
        }

        private void SetupSingleton()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple ModManager instances detected in scene!", true);
                    Destroy(this);
                }
            }
        }

        #endregion

        #region Events

        //public delegate void NewObjectCreatedHandler(object obj, SetupOptions options);
        //public static event NewObjectCreatedHandler OnNewObjectCreated;

        //public delegate void NewModRegistered(IModController newModController);
        //public static event NewModRegistered OnNewModControllerRegistered;

        public delegate void AssetUpdate(string ModTitle, string AssetName, Type assetType);
        public static event AssetUpdate OnLoadAssetEvent;

        public delegate void ModUpdate(string ModTitle);
        public static event ModUpdate OnUnloadModEvent;

        private void OnUnloadMod(string ModTitle)
        {
            if (OnUnloadModEvent != null)
                OnUnloadModEvent(ModTitle);
        }

        public static void OnLoadAsset(string ModTitle, string assetName, Type assetType)
        {
            if (OnLoadAssetEvent != null)
                OnLoadAssetEvent(ModTitle, assetName, assetType);
        }

        public void StateManager_OnStateChange(StateManager.StateTypes state)
        {
            if (state == StateManager.StateTypes.Start)
            {
                alreadyAtStartMenuState = true;
                Init();
                InvokeModLoaders(state);
            }
            else if (state == StateManager.StateTypes.Game)
            {
                alreadyAtStartMenuState = true;
                InvokeModLoaders(state);
                StateManager.OnStateChange -= StateManager_OnStateChange;
            }
        }

        #endregion
    }
}
