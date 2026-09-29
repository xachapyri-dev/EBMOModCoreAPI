using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EBMOModCoreAPI
{
    public static class DialogueAPI
    {
        public static bool EnableLogging = false;
        private static bool isSessionActive = false;
        private static readonly Dictionary<string, string> NpcReplacements = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> ChoiceReplacements = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> SceneScopedNpcReplacements = new Dictionary<string, string>();
        public static event Action<DialogueChar, OWDialogueNode> OnDialogueDisplay;
        /// <summary>
        /// Регистрирует замену реплики персонажа во всех сценах игры при обнаружении совпадения с искомой подстрокой.
        /// </summary>
        /// <param name="originalSnippet">Фрагмент или полный текст оригинальной реплики для поиска.</param>
        /// <param name="newText">Новый текст реплики, на который будет произведена замена.</param>
        public static void ReplaceNpcText(string originalSnippet, string newText)
        {
            if (!string.IsNullOrEmpty(originalSnippet))
                NpcReplacements[originalSnippet] = newText;
        }
        /// <summary>
        /// Регистрирует замену реплики персонажа исключительно в пределах заданной сцены игры.
        /// </summary>
        /// <param name="sceneName">Имя целевой сцены Unity (например, "Home").</param>
        /// <param name="originalSnippet">Фрагмент или полный текст оригинальной реплики для поиска.</param>
        /// <param name="newText">Новый текст реплики, на который будет произведена замена.</param>
        public static void ReplaceNpcTextInScene(string sceneName, string originalSnippet, string newText)
        {
            if (!string.IsNullOrEmpty(sceneName) && !string.IsNullOrEmpty(originalSnippet))
            {
                string key = $"{sceneName}|{originalSnippet}";
                SceneScopedNpcReplacements[key] = newText;
            }
        }
        /// <summary>
        /// Регистрирует замену варианта ответа игрока (кнопки выбора) при обнаружении совпадения с искомой подстрокой.
        /// </summary>
        /// <param name="originalChoiceSnippet">Фрагмент или полный текст оригинального варианта ответа для поиска.</param>
        /// <param name="newChoiceText">Новый текст варианта ответа, на который будет произведена замена.</param>
        public static void ReplaceChoiceText(string originalChoiceSnippet, string newChoiceText)
        {
            if (!string.IsNullOrEmpty(originalChoiceSnippet))
                ChoiceReplacements[originalChoiceSnippet] = newChoiceText;
        }
        /// <summary>
        /// Завершает текущую активную сессию диалога и отправляет финальную строку шпаргалки в лог консоли.
        /// </summary>
        public static void EndDialogueSession()
        {
            if (isSessionActive && EnableLogging)
            {
                Debug.Log("[EBMOModCoreAPI] Диалог завершён\n");
            }
            isSessionActive = false;
        }
        private static void PrintCheatSheet(DialogueChar charac, OWDialogueNode node)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string speaker = !string.IsNullOrWhiteSpace(charac.name) ? charac.name.Trim() : "Без имени";
            string text = node != null && !string.IsNullOrEmpty(node.text)
                ? node.text.Replace("\r\n", " ").Replace("\n", " ").Trim()
                : string.Empty;
            var sb = new StringBuilder();
            if (!isSessionActive)
            {
                isSessionActive = true;
                sb.AppendLine();
                sb.AppendLine($"[EBMOModCoreAPI] Начат диалог с {speaker} [Сцена: {currentScene}]");
            }
            List<string> parts = new List<string>
            {
                speaker,
                text
            };
            if (node?.choices != null && node.choices.Count > 0)
            {
                for (int i = 0; i < node.choices.Count; i++)
                {
                    string choiceText = node.choices[i].text?.Replace("\r\n", " ").Replace("\n", " ").Trim();
                    parts.Add($"Выбор({i}): {choiceText}");
                }
            }
            sb.Append(string.Join("|", parts));
            Debug.Log(sb.ToString());
        }
        internal static void HandleDialogue(DialogueChar charac, OWDialogueNode node)
        {
            if (node == null) return;
            if (EnableLogging)
            {
                PrintCheatSheet(charac, node);
            }
            string currentScene = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(node.text))
            {
                bool replaced = false;
                foreach (var kvp in SceneScopedNpcReplacements)
                {
                    string[] split = kvp.Key.Split(new[] { '|' }, 2);
                    if (currentScene == split[0] && node.text.Contains(split[1]))
                    {
                        node.text = kvp.Value;
                        replaced = true;
                        break;
                    }
                }
                if (!replaced)
                {
                    foreach (var kvp in NpcReplacements)
                    {
                        if (node.text.Contains(kvp.Key))
                        {
                            node.text = kvp.Value;
                            break;
                        }
                    }
                }
            }
            if (node.choices != null)
            {
                for (int i = 0; i < node.choices.Count; i++)
                {
                    var choice = node.choices[i];
                    if (string.IsNullOrEmpty(choice.text)) continue;

                    foreach (var kvp in ChoiceReplacements)
                    {
                        if (choice.text.Contains(kvp.Key))
                        {
                            choice.text = kvp.Value;
                            node.choices[i] = choice;
                            break;
                        }
                    }
                }
            }
            OnDialogueDisplay?.Invoke(charac, node);
        }
    }
    [HarmonyPatch(typeof(OWDialogueUI), nameof(OWDialogueUI.ShowDialogue))]
    internal static class Patch_OWDialogueUI
    {
        [HarmonyPrefix]
        private static void Prefix(DialogueChar charac, OWDialogueNode node)
        {
            DialogueAPI.HandleDialogue(charac, node);
        }
    }
    [HarmonyPatch(typeof(OWDialogueTrigger), nameof(OWDialogueTrigger.stopChating))]
    internal static class Patch_OWDialogueTrigger_Stop
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            DialogueAPI.EndDialogueSession();
        }
    }
}