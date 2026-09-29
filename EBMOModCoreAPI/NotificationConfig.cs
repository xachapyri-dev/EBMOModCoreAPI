using BepInEx.Configuration;

namespace EBMOModCoreAPI
{
    public static class NotificationConfig
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<float> Duration;
        public static void Init(ConfigFile config)
        {
            Enabled = config.Bind("Notifications", "Enabled", true, "Включить внутриигровые уведомления об ошибках и логах");
            Duration = config.Bind("Notifications", "Duration", 3.0f, "Длительность показа уведомления в секундах");
        }
    }
}