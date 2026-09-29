using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EBMOModCoreAPI
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
    public class NotificationUI : MonoBehaviour
    {
        private static NotificationUI _instance;
        private Canvas _canvas;
        private readonly Queue<NotificationData> _queue = new Queue<NotificationData>();
        private bool _isShowing = false;
        private static Font _cachedFont;
        private struct NotificationData
        {
            public string Message;
            public NotificationType Type;
        }
        public static void Initialize()
        {
            if (_instance != null) return;
            GameObject root = new GameObject("[EMBO_NotificationUI]");
            DontDestroyOnLoad(root);
            _instance = root.AddComponent<NotificationUI>();
            _instance.BuildCanvas();
        }
        public static void Show(string message, NotificationType type)
        {
            if (!NotificationConfig.Enabled.Value || _instance == null) return;
            _instance.Enqueue(message, type);
        }
        private void Enqueue(string message, NotificationType type)
        {
            _queue.Enqueue(new NotificationData { Message = message, Type = type });
            if (!_isShowing)
            {
                StartCoroutine(ProcessQueue());
            }
        }
        private void BuildCanvas()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 32767;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            gameObject.AddComponent<GraphicRaycaster>();
        }
        private IEnumerator ProcessQueue()
        {
            _isShowing = true;
            while (_queue.Count > 0)
            {
                var data = _queue.Dequeue();
                yield return StartCoroutine(AnimateToast(data));
            }
            _isShowing = false;
        }
        private IEnumerator AnimateToast(NotificationData data)
        {
            GameObject toastObj = new GameObject("ToastPanel");
            toastObj.transform.SetParent(_canvas.transform, false);
            RectTransform rect = toastObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(500, 70);
            Vector2 hiddenPos = new Vector2(0, 80);
            Vector2 visiblePos = new Vector2(0, -20);
            rect.anchoredPosition = hiddenPos;
            Image bg = toastObj.AddComponent<Image>();
            bg.sprite = GetWhiteSprite();
            switch (data.Type)
            {
                case NotificationType.Info:
                    bg.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
                    break;
                case NotificationType.Warning:
                    bg.color = new Color(0.85f, 0.65f, 0.1f, 0.95f);
                    break;
                case NotificationType.Error:
                    bg.color = new Color(0.85f, 0.15f, 0.15f, 0.95f);
                    break;
            }
            GameObject textObj = new GameObject("ToastText");
            textObj.transform.SetParent(toastObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(15, 8);
            textRect.offsetMax = new Vector2(-15, -5);
            Text text = textObj.AddComponent<Text>();
            text.font = GetGameFont();
            text.fontSize = 18;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = data.Message;
            GameObject barObj = new GameObject("ProgressBar");
            barObj.transform.SetParent(toastObj.transform, false);
            RectTransform barRect = barObj.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0.5f, 0f);
            barRect.anchorMax = new Vector2(0.5f, 0f);
            barRect.pivot = new Vector2(0.5f, 0f);
            barRect.sizeDelta = new Vector2(500, 5);
            barRect.anchoredPosition = Vector2.zero;
            Image barImg = barObj.AddComponent<Image>();
            barImg.sprite = GetWhiteSprite();
            barImg.color = Color.white;
            float animTime = 0.25f;
            float elapsed = 0f;
            while (elapsed < animTime)
            {
                elapsed += Time.unscaledDeltaTime;
                rect.anchoredPosition = Vector2.Lerp(hiddenPos, visiblePos, elapsed / animTime);
                yield return null;
            }
            rect.anchoredPosition = visiblePos;
            float totalDuration = Mathf.Max(0.5f, NotificationConfig.Duration.Value);
            float timer = totalDuration;
            while (timer > 0f)
            {
                timer -= Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(timer / totalDuration);
                barRect.sizeDelta = new Vector2(500f * progress, 5f);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.unscaledDeltaTime;
                rect.anchoredPosition = Vector2.Lerp(visiblePos, hiddenPos, elapsed / 0.2f);
                yield return null;
            }
            Destroy(toastObj);
        }
        private static Sprite _whiteSprite;
        private static Sprite GetWhiteSprite()
        {
            if (_whiteSprite == null)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                _whiteSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            }
            return _whiteSprite;
        }
        private static Font GetGameFont()
        {
            if (_cachedFont != null) return _cachedFont;
            Font[] fonts = Resources.FindObjectsOfTypeAll<Font>();
            foreach (var f in fonts)
            {
                if (f.name == "LiberationSans")
                {
                    _cachedFont = f;
                    return _cachedFont;
                }
            }
            foreach (var f in fonts)
            {
                if (f.name == "GOTHIC" || f.name == "GOTHICB")
                {
                    _cachedFont = f;
                    return _cachedFont;
                }
            }
            if (fonts.Length > 0)
            {
                _cachedFont = fonts[0];
                return _cachedFont;
            }
            _cachedFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
            return _cachedFont;
        }
    }
}