using UnityEngine;
using UnityEngine.SceneManagement;

namespace EBMOModCoreAPI
{
    public static class PlayerAPI
    {
        private static PlayerManager _cachedManager;
        static PlayerAPI()
        {
            SceneManager.sceneLoaded += (scene, mode) => ClearCache();
        }
        public static void ClearCache()
        {
            _cachedManager = null;
        }
        /// <summary>
        /// Главный менеджер управления игроками (PlayerManager).
        /// </summary>
        public static PlayerManager Manager
        {
            get
            {
                if (_cachedManager == null)
                {
                    _cachedManager = Object.FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
                }
                return _cachedManager;
            }
        }
        /// <summary>
        /// Заспавнен ли менеджер игроков на сцене.
        /// </summary>
        public static bool IsSpawned => Manager != null;
        /// <summary>
        /// Активна ли Май (true) или Иня (false).
        /// </summary>
        public static bool IsMay => Manager != null && Manager.isMay;
        /// <summary>
        /// Контроллер движения Май (CharMoving).
        /// </summary>
        public static CharMoving May => Manager != null ? Manager.may : null;
        /// <summary>
        /// Контроллер движения Ини (CharMoving).
        /// </summary>
        public static CharMoving Inya => Manager != null ? Manager.inya : null;
        /// <summary>
        /// Контроллер движения активного в данный момент персонажа.
        /// </summary>
        public static CharMoving ActiveMovement => IsMay ? May : Inya;
        /// <summary>
        /// GameObject активного персонажа.
        /// </summary>
        public static GameObject ActiveGameObject => ActiveMovement != null ? ActiveMovement.gameObject : null;
        /// <summary>
        /// Transform активного персонажа.
        /// </summary>
        public static Transform ActiveTransform => ActiveMovement != null ? ActiveMovement.transform : null;
        /// <summary>
        /// Координаты активного персонажа в мире.
        /// </summary>
        public static Vector3 Position
        {
            get => ActiveTransform != null ? ActiveTransform.position : Vector3.zero;
            set
            {
                if (ActiveTransform != null)
                {
                    var cc = ActiveMovement.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;
                    ActiveTransform.position = value;
                    if (cc != null) cc.enabled = true;
                }
            }
        }
        /// <summary>
        /// Скорость перемещения активного персонажа.
        /// </summary>
        public static float MoveSpeed
        {
            get => ActiveMovement != null ? ActiveMovement.moveSpeed : 0f;
            set
            {
                if (ActiveMovement != null)
                {
                    ActiveMovement.moveSpeed = value;
                }
            }
        }
        /// <summary>
        /// Переключить активного персонажа (true -> May, false -> Inya).
        /// </summary>
        /// <param name="toMay">true -> May, false -> Inya</param>
        public static void SwitchCharacter(bool toMay)
        {
            if (Manager == null) return;
            Manager.CharSwitch(!toMay);
        }
        /// <summary>
        /// Заблокировать управление обоим персонажам.
        /// </summary>
        public static void BlockControl() => Manager?.blockControl();
        /// <summary>
        /// Разблокировать управление текущему персонажу.
        /// </summary>
        public static void UnblockControl() => Manager?.unblockControl();
        /// <summary>
        /// Отправить персонажа в заданную точку.
        /// </summary>
        /// <param name="point">Идти к точке</param>
        public static void MoveTo(Vector3 point) => ActiveMovement?.goToPoint(point);
    }
}