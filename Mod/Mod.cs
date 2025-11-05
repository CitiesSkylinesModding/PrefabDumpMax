using Colossal.Logging;
using Game;
using Game.Modding;
using Unity.Entities;

namespace PrefabDumpMax
{
    public class Mod : IMod
    {
        public static Setting m_Setting;
        public static ILog log = LogManager
            .GetLogger($"{nameof(PrefabDumpMax)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabDump>();
        }

        public void OnDispose() { }
    }
}
