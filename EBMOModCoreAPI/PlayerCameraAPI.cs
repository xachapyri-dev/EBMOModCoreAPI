using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EBMOModCoreAPI
{
    public static class PlayerCameraAPI
    {
        private static Camera _cachedCamera;
        private static CamFollowing _cachedFollowing;
        private static CamMoving _cachedMoving;
        private static readonly FieldInfo OffsetField = typeof(CamFollowing).GetField("offset", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo HeightField = typeof(CamFollowing).GetField("height", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo SmoothSpeedField = typeof(CamFollowing).GetField("smoothSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        static PlayerCameraAPI()
        {
            SceneManager.sceneLoaded += (scene, mode) => ClearCache();
        }
        /// <summary>
        /// Сбрасывает кэш компонентов камеры (вызывается автоматически при смене сцены).
        /// </summary>
        public static void ClearCache()
        {
            _cachedCamera = null;
            _cachedFollowing = null;
            _cachedMoving = null;
        }
        /// <summary>
        /// Основная камера сцены (Camera.main или объект со скриптом CamFollowing).
        /// </summary>
        public static Camera Camera
        {
            get
            {
                if (_cachedCamera == null)
                {
                    _cachedCamera = Camera.main;
                    if (_cachedCamera == null && Following != null)
                    {
                        _cachedCamera = Following.GetComponent<Camera>();
                    }
                }
                return _cachedCamera;
            }
        }
        /// <summary>
        /// Transform камеры сцены.
        /// </summary>
        public static Transform Transform => Camera != null ? Camera.transform : null;
        /// <summary>
        /// Контроллер плавного следования за игроком (CamFollowing).
        /// </summary>
        public static CamFollowing Following
        {
            get
            {
                if (_cachedFollowing == null)
                {
                    _cachedFollowing = UnityEngine.Object.FindAnyObjectByType<CamFollowing>(FindObjectsInactive.Include);
                }
                return _cachedFollowing;
            }
        }
        /// <summary>
        /// Контроллер кинематографичного перемещения камеры (CamMoving).
        /// </summary>
        public static CamMoving Moving
        {
            get
            {
                if (_cachedMoving == null)
                {
                    _cachedMoving = UnityEngine.Object.FindAnyObjectByType<CamMoving>(FindObjectsInactive.Include);
                }
                return _cachedMoving;
            }
        }
        /// <summary>
        /// Доступна ли камера и её контроллеры на текущей сцене.
        /// </summary>
        public static bool IsAvailable => Camera != null && Following != null;
        /// <summary>
        /// Активен ли скрипт автоматического следования за игроком.
        /// Отключите для ручного контроля камеры или кинематографичных катсцен.
        /// </summary>
        public static bool IsFollowEnabled
        {
            get => Following != null && Following.enabled;
            set
            {
                if (Following != null)
                {
                    Following.enabled = value;
                }
            }
        }
        /// <summary>
        /// Цель следования камеры (Transform персонажа).
        /// </summary>
        public static Transform Target
        {
            get => Following != null ? Following.player : null;
            set
            {
                if (Following != null)
                {
                    Following.player = value;
                }
            }
        }
        /// <summary>
        /// Смещение камеры относительно персонажа (offset).
        /// </summary>
        public static Vector3 Offset
        {
            get => (Following != null && OffsetField != null) ? (Vector3)OffsetField.GetValue(Following) : Vector3.zero;
            set
            {
                if (Following != null && OffsetField != null)
                {
                    OffsetField.SetValue(Following, value);
                }
            }
        }
        /// <summary>
        /// Высота камеры над персонажем (height).
        /// </summary>
        public static float Height
        {
            get => (Following != null && HeightField != null) ? (float)HeightField.GetValue(Following) : 0f;
            set
            {
                if (Following != null && HeightField != null)
                {
                    HeightField.SetValue(Following, value);
                }
            }
        }
        /// <summary>
        /// Скорость сглаживания движения камеры (smoothSpeed).
        /// </summary>
        public static float SmoothSpeed
        {
            get => (Following != null && SmoothSpeedField != null) ? (float)SmoothSpeedField.GetValue(Following) : 0f;
            set
            {
                if (Following != null && SmoothSpeedField != null)
                {
                    SmoothSpeedField.SetValue(Following, value);
                }
            }
        }
        /// <summary>
        /// Минимальные границы перемещения камеры (X, Y).
        /// </summary>
        public static Vector2 MinBounds
        {
            get => Following != null ? Following.minBounds : Vector2.zero;
            set
            {
                if (Following != null) Following.minBounds = value;
            }
        }
        /// <summary>
        /// Максимальные границы перемещения камеры (X, Y).
        /// </summary>
        public static Vector2 MaxBounds
        {
            get => Following != null ? Following.maxBounds : Vector2.zero;
            set
            {
                if (Following != null) Following.maxBounds = value;
            }
        }
        /// <summary>
        /// Принудительно направляет цель следования камеры на текущего активного персонажа (Мэй или Иню).
        /// </summary>
        public static void FollowActiveCharacter()
        {
            if (PlayerAPI.ActiveTransform != null)
            {
                Target = PlayerAPI.ActiveTransform;
            }
        }
        /// <summary>
        /// Плавно перемещает камеру в указанную точку с поворотом за 0.5 секунды (метод CamFollowing.goToPoint).
        /// </summary>
        /// <param name="point">Целевая позиция</param>
        /// <param name="angle">Целевые углы Эйлера (поворот)</param>
        public static void GoToPoint(Vector3 point, Vector3 angle)
        {
            Following?.goToPoint(point, angle);
        }
        /// <summary>
        /// Запускает траекторию движения камеры по ноде Pose через скрипт CamMoving.
        /// </summary>
        public static void Move(global::Pose pose)
        {
            Moving?.Move(pose);
        }
        /// <summary>
        /// Мгновенная телепортация камеры в указанную точку.
        /// </summary>
        public static void SetPosition(Vector3 position, Quaternion? rotation = null)
        {
            if (Transform == null) return;
            Transform.position = position;
            if (rotation.HasValue)
            {
                Transform.rotation = rotation.Value;
            }
        }
    }
}