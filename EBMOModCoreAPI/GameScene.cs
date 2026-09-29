namespace EBMOModCoreAPI
{
    /// <summary>
    /// Нужен для выбора сцены (в скобках указано название сцены в unity). Нужен будет для запуска через SceneManager.LoadScene((int)GameScene.название)
    /// </summary>
    public enum GameScene
    {
        /// <summary>
        /// Главное меню (A_StartMenu)
        /// </summary>
        Menu = 0,
        /// <summary>
        /// Локация университета (Катсцена) (Intro_scene)
        /// </summary>
        UniversityСutScene = 1,
        /// <summary>
        /// Локация университета (University)
        /// </summary>
        University = 2,
        /// <summary>
        /// Во дворе унивесетера (WayHome)
        /// </summary>
        OutUniversity = 3,
        /// <summary>
        /// Дом (Home)
        /// </summary>
        Home = 4,
        /// <summary>
        /// Ночь (конец демки) (Night)
        /// </summary>
        Night = 5
    }
}
