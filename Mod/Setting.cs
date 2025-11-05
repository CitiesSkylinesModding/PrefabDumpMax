using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Unity.Entities;

namespace PrefabDumpMax
{
    [FileLocation(nameof(PrefabDumpMax))]
    [SettingsUITabOrder(main)]
    public class Setting : ModSetting
    {
        public const string main = "Main";

        public Setting(IMod mod)
            : base(mod) { }

        [SettingsUISection(main, "")]
        public bool CreateDump
        {
            set =>
                World
                    .DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabDump>()
                    .CreateDump();
        }

        public override void SetDefaults() { }
    }
}
