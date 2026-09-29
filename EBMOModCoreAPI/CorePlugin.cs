using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EBMOModCoreAPI
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class CorePlugin : BaseUnityPlugin
    {
        public const string ModGUID = "com.xachapuridev.ebmomodcoreapi";
        public const string ModName = "EBMO Mod Core API";
        public const string ModVersion = "1.0.0";
        public static ConfigEntry<bool> EnableCheatSheet;
        public static ConfigEntry<KeyCode> ToggleCheatSheetKey;

        private void Awake()
        {
            EnableCheatSheet = Config.Bind("Debug", "EnableDialogueCheatSheet", false, "Включать ли шпаргалку при старте");
            ToggleCheatSheetKey = Config.Bind("Debug", "ToggleKey", KeyCode.F10, "Кнопка вкл/выкл шпаргалки");
            DialogueAPI.EnableLogging = EnableCheatSheet.Value;
            NotificationConfig.Init(Config);
            var harmony = new Harmony(ModGUID);
            harmony.PatchAll();
            Logger.LogInfo("EBMO Mod Core API Успешно загрузился!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(ToggleCheatSheetKey.Value))
            {
                DialogueAPI.EnableLogging = !DialogueAPI.EnableLogging;
                EnableCheatSheet.Value = DialogueAPI.EnableLogging;
                Logger.LogInfo($"Шпаргалка диалогов: {(DialogueAPI.EnableLogging ? "ВКЛЮЧЕНА" : "ВЫКЛЮЧЕНА")}");
            }
        }
    }
}