using Colossal.Logging;
using Game;
using Game.Modding;

namespace PrefabDumpMax
{
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(PrefabDumpMax)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            updateSystem.UpdateAfter<PrefabDump>(SystemUpdatePhase.PrefabUpdate);
        }

        public void OnDispose() { }
    }
}
