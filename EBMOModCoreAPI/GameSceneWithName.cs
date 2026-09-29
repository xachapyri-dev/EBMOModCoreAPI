namespace EBMOModCoreAPI
{
    /// <summary>
    /// Противоположение enum GameScene. Будет нужен например для напрямового вызова из SceneManager.LoadScene(GameSceneWithName.A_StartMenu.ToString())
    /// </summary>
    public enum GameSceneWithName
    {
        /// <summary>
        /// Главное меню (Menu)
        /// </summary>
        A_StartMenu = 0,
        /// <summary>
        /// Локация университета (Катсцена) (UniversityСutScene)
        /// </summary>
        Intro_scene = 1,
        /// <summary>
        /// Локация университета (University)
        /// </summary>
        University = 2,
        /// <summary>
        /// Во дворе унивесетера (OutUniversity)
        /// </summary>
        WayHome = 3,
        /// <summary>
        /// Дом (Home)
        /// </summary>
        Home = 4,
        /// <summary>
        /// Ночь (конец демки) (Night)
        /// </summary>
        Night = 5,
    }
}
