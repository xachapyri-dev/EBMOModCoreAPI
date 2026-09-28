<p>
    <a href="https://unity.com/"><img src="https://img.shields.io/badge/Unity-6000.0%2B-000000?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 6"></a>
    <a href="https://github.com/BepInEx/BepInEx"><img src="https://img.shields.io/badge/BepInEx-5.4%2B-2563eb?style=for-the-badge" alt="BepInEx 5.4+"></a>
    <a href="https://github.com/pardeike/Harmony"><img src="https://img.shields.io/badge/Harmony-2.x-d97706?style=for-the-badge" alt="Harmony"></a>
    <img src="https://img.shields.io/badge/C%23-.NET_Standard_2.1-512bd4?style=for-the-badge&logo=csharp&logoColor=white" alt="C#">
    <a href="http://k90052gj.beget.tech/projectPages/EBMOModAPI_projectPage/index.html"><img src="https://img.shields.io/badge/Документация-Онлайн-16a34a?style=for-the-badge" alt="Docs"></a>
    <img src="https://img.shields.io/badge/Лицензия-MIT-4b5563?style=for-the-badge" alt="License">
  </p>
                
<div align="center">
  <img src="https://github.com/user-attachments/assets/6706593d-9016-4fe6-a5e3-0c9200f7b9d5" width="72" height="72" alt="EBMO Mod Core API">
  <h1>ЭБМО Mod Core API</h1>
  <p>Базовый фреймворк и модульное API для создания модификаций к игре «Это была моя ошибка»</p>
  <p>
    <a href="http://k90052gj.beget.tech/projectPages/EBMOModAPI_projectPage/index.html"><strong>Официальный сайт документации</strong></a>

  </p>
</div>

---

## Описание

**EBMO Mod Core API** — это инфраструктурный плагин на базе BepInEx и Harmony, разработанный для создания модификаций, кастомных комнат, сюжетных дополнений (DLC) и геймплейных механик для игры «Это была моя ошибка» (Unity 6).

API инкапсулирует внутреннюю архитектуру движка и предоставляет унифицированные классы для работы с персонажами, камерой, ассетами и диалогами.

---

## Основные модули

* **PlayerAPI** — управление персонажами (Мэй и Иня): переключение активного героя, получение координат, безопасная телепортация со сбросом коллизий `CharacterController`, модификаторы скорости бега, блокировка управления и скриптовое движение к точке.
* **PlayerCameraAPI** — интерфейс к системным скриптам камеры (`CamFollowing` и `CamMoving`): управление кинематографичными пролётами, границами обзора (`minBounds`, `maxBounds`), отключение следования для катсцен и привязка фокуса.
* **BundleAPI** — загрузка внешних C#-сборок (`.dll`), монтирование AssetBundle, открытие уровней, а также рантайм-восстановление розовых материалов (перепривязка к оригинальным URP-шейдерам) и починка EventSystem под Unity 6.
* **DialogueAPI** — Harmony-перехват диалоговой системы: глобальная и сценарная подмена реплик NPC, изменение вариантов выбора игрока, режим генерации текстовой шпаргалки диалогов в консоль.
* **NotificationUI** — независимый интерфейс всплывающих уведомлений (тостов) с очередью отображения, таймером на `unscaledDeltaTime` (работает во время паузы) и поддержкой нативных шрифтов игры.

---

## Требования

* **Игра:** «Это была моя ошибка» (Unity 6)
* **Загрузчик модов:** BepInEx 5.4.x (x64)
* **Целевая платформа плагинов:** .NET Standard 2.1

---
> [!TIP]
> Как установить и использование есть в документации

<div align="center">
  
  <p>Это была моя ошибка by ZIWI, ЭБМО Mod Core API by Хачапури dev</p>

</div>
