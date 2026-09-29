using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EBMOModCoreAPI
{
    public static class BundleAPI
    {
        private static readonly Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();
        private static readonly List<Assembly> LoadedAssemblies = new List<Assembly>();
        /// <summary>
        /// Загружает сборку (.dll) со скриптами дополнения в текущий домен игры.
        /// </summary>
        /// <param name="dllPath">Абсолютный путь к файлу .dll со скриптами.</param>
        /// <returns>Загруженная сборка Assembly или null в случае ошибки.</returns>
        public static Assembly LoadScriptAssembly(string dllPath)
        {
            if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath))
            {
                Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Файл скриптов не найден: {dllPath}");
                return null;
            }
            try
            {
                byte[] assemblyBytes = File.ReadAllBytes(dllPath);
                Assembly loadedAssembly = Assembly.Load(assemblyBytes);

                if (!LoadedAssemblies.Contains(loadedAssembly))
                {
                    LoadedAssemblies.Add(loadedAssembly);
                }
                Debug.Log($"[EBMOModCoreAPI] [BundleAPI] Сборка скриптов успешно загружена: {loadedAssembly.GetName().Name}");
                return loadedAssembly;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Ошибка при загрузке сборки {dllPath}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Загружает AssetBundle из указанного файла либо берет его из кэша.
        /// </summary>
        /// <param name="bundlePath">Абсолютный путь к файлу AssetBundle.</param>
        /// <returns>Экземпляр AssetBundle или null при ошибке.</returns>
        public static AssetBundle LoadBundle(string bundlePath)
        {
            if (string.IsNullOrEmpty(bundlePath) || !File.Exists(bundlePath))
            {
                Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Файл бандла не найден: {bundlePath}");
                return null;
            }
            if (LoadedBundles.TryGetValue(bundlePath, out var cachedBundle) && cachedBundle != null)
            {
                return cachedBundle;
            }
            try
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle == null)
                {
                    Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Unity не смогла разобрать AssetBundle: {bundlePath}");
                    return null;
                }
                LoadedBundles[bundlePath] = bundle;
                Debug.Log($"[EBMOModCoreAPI] [BundleAPI] AssetBundle успешно загружен: {bundle.name}");
                return bundle;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Ошибка при загрузке бандла {bundlePath}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Загружает библиотеку скриптов (если задана) и AssetBundle одной операцией.
        /// </summary>
        /// <param name="bundlePath">Абсолютный путь к AssetBundle.</param>
        /// <param name="dllPath">Абсолютный путь к .dll со скриптами дополнения (необязательно).</param>
        /// <returns>Загруженный AssetBundle или null.</returns>
        public static AssetBundle LoadAddonContent(string bundlePath, string dllPath = null)
        {
            if (!string.IsNullOrEmpty(dllPath))
            {
                LoadScriptAssembly(dllPath);
            }
            return LoadBundle(bundlePath);
        }
        /// <summary>
        /// Возвращает пути и имена всех сцен, упакованных внутри переданного AssetBundle.
        /// </summary>
        /// <param name="bundle">Загруженный AssetBundle со сценами.</param>
        /// <returns>Массив внутренних путей к сценам (например, "Assets/Scenes/CustomLevel.unity").</returns>
        public static string[] GetAllScenePaths(AssetBundle bundle)
        {
            if (bundle == null)
            {
                Debug.LogError("[EBMOModCoreAPI] [BundleAPI] Попытка получить список сцен из null-бандла.");
                return Array.Empty<string>();
            }
            return bundle.GetAllScenePaths();
        }
        /// <summary>
        /// Синхронно загружает сцену из AssetBundle по её названию или внутреннему пути.
        /// </summary>
        /// <param name="sceneNameOrPath">Имя сцены (без расширения) или полный путь внутри бандла.</param>
        /// <param name="mode">Режим загрузки: Additive (параллельно текущей сцене) или Single (с полной выгрузкой текущей сцены).</param>
        public static void LoadScene(string sceneNameOrPath, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            try
            {
                string sceneName = Path.GetFileNameWithoutExtension(sceneNameOrPath);
                SceneManager.LoadScene(sceneName, mode);
                Debug.Log($"[EBMOModCoreAPI] [BundleAPI] Сцена '{sceneName}' запущена в режиме {mode}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Не удалось загрузить сцену '{sceneNameOrPath}': {ex.Message}");
            }
        }
        /// <summary>
        /// Асинхронно загружает сцену из AssetBundle в фоновом режиме.
        /// </summary>
        /// <param name="sceneNameOrPath">Имя сцены (без расширения) или полный путь внутри бандла.</param>
        /// <param name="mode">Режим загрузки: Additive или Single.</param>
        /// <returns>Объект AsyncOperation для контроля прогресса загрузки.</returns>
        public static AsyncOperation LoadSceneAsync(string sceneNameOrPath, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            try
            {
                string sceneName = Path.GetFileNameWithoutExtension(sceneNameOrPath);
                AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
                Debug.Log($"[EBMOModCoreAPI] [BundleAPI] Запущена асинхронная загрузка сцены '{sceneName}' в режиме {mode}.");
                return op;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EBMOModCoreAPI] [BundleAPI] Ошибка асинхронной загрузки сцены '{sceneNameOrPath}': {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Загружает префаб или ассет (модели, звуки, текстуры) из AssetBundle.
        /// </summary>
        /// <typeparam name="T">Тип ассета (GameObject, Material, AudioClip и др.).</typeparam>
        /// <param name="bundle">Загруженный AssetBundle.</param>
        /// <param name="assetName">Имя объекта внутри бандла.</param>
        /// <returns>Найденный объект или null.</returns>
        public static T GetAsset<T>(AssetBundle bundle, string assetName) where T : UnityEngine.Object
        {
            if (bundle == null)
            {
                Debug.LogError("[EBMOModCoreAPI] [BundleAPI] Попытка загрузить ассет из null-бандла.");
                return null;
            }
            return bundle.LoadAsset<T>(assetName);
        }
        /// <summary>
        /// Выгружает все бандлы из памяти и очищает кэш.
        /// </summary>
        /// <param name="unloadAllLoadedObjects">Выгружать ли связанные объекты, уже находящиеся на сцене.</param>
        public static void UnloadAll(bool unloadAllLoadedObjects = false)
        {
            foreach (var kvp in LoadedBundles)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Unload(unloadAllLoadedObjects);
                }
            }
            LoadedBundles.Clear();
            Debug.Log("[EBMOModCoreAPI] [BundleAPI] Все бандлы выгружены.");
        }
        /// <summary>
        /// Переназначает шейдеры материалов переданного объекта на активные шейдеры URP рантайма игры, устраняя розовые текстуры.
        /// </summary>
        /// <param name="target">Корневой GameObject модели или префаба.</param>
        public static void FixURPShaders(GameObject target)
        {
            if (target == null) return;
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
            foreach (var rend in renderers)
            {
                if (rend == null) continue;
                Material[] materials = rend.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material mat = materials[i];
                    if (mat == null) continue;
                    string shaderName = mat.shader != null ? mat.shader.name : "Universal Render Pipeline/Lit";
                    Shader runtimeShader = Shader.Find(shaderName);
                    if (runtimeShader != null)
                    {
                        mat.shader = runtimeShader;
                    }
                    else
                    {
                        mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                    }
                }
                rend.materials = materials;
            }
        }
        /// <summary>
        /// Переназначает шейдеры всех материалов на всех объектах заданной сцены на рантайм-шейдеры URP.
        /// </summary>
        /// <param name="scene">Загруженная аддитивная сцена дополнения.</param>
        public static void FixURPSceneShaders(Scene scene)
        {
            if (!scene.IsValid()) return;
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                FixURPShaders(root);
            }
        }
        /// <summary>
        /// Исправление проблемы с UI
        /// </summary>
        public static void FixSceneUI()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            var es = UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (es == null)
            {
                var esObj = new GameObject("EventSystem");
                es = esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            }
            var oldModules = es.GetComponents<UnityEngine.EventSystems.BaseInputModule>();
            foreach (var m in oldModules)
            {
                if (!(m is UnityEngine.EventSystems.StandaloneInputModule))
                {
                    UnityEngine.Object.DestroyImmediate(m);
                }
            }
            var standalone = es.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (standalone == null)
            {
                es.gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
    }
}