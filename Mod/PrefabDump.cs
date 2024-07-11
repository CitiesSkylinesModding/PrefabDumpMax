using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Prefabs;
using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace PrefabDumpMax
{
    public partial class PrefabDump : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem;
        private EntityQuery m_PrefabQuery;
        private ILog m_Log;

        public PrefabDump()
        {
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Log = Mod.log;
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            m_PrefabQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>());

            RequireForUpdate(m_PrefabQuery);
        }

        protected override void OnUpdate()
        {
            var startTime = DateTime.Now;
            m_Log.Info($"{nameof(PrefabDump)} created!");
            NativeArray<Entity> prefabEntities = m_PrefabQuery.ToEntityArray(Allocator.Temp);
            int count_e = prefabEntities.Count();
            int i = 1;
            m_Log.Info($"{count_e} items found");
            string version = Game.Version.current.shortVersion;
            foreach (Entity entity in prefabEntities)
            {
                PrefabData component;
                PrefabBase prefabBase;
                if (!EntityManager.TryGetComponent<PrefabData>(entity, out component) || !m_PrefabSystem.TryGetPrefab<PrefabBase>(component, out prefabBase))
                    return;
                if (prefabBase != null)
                {
                    try
                    {
                        string prefabtype = (prefabBase.prefab.ToString() ?? "Prefabless").Replace(prefabBase.name, "").Replace(" (", "").Replace(")", "");
                        //(prefabBase.asset ?? AssetDatabase.user.AddAsset(AssetDataPath.Create($".Dump_{version}/{prefabtype}/{prefabBase.name}", prefabBase.name ?? ""), prefabBase)).Save();
                        m_Log.Info($"{i} of {count_e} : {prefabtype}/{prefabBase.name ?? "Nameless"} ");
                        //m_Log.Info($"{i} of {count_e} - {prefabBase.GetInstanceID()};{prefabBase.GetPrefabID()}");
                    }
                    catch (Exception e)
                    {
                        m_Log.Info($"ERROR: Item {i} - {e.ToString()}");
                    }
                }
                i++;
            }
            prefabEntities.Dispose();
            Enabled = false;
            var loadTime = DateTime.Now - startTime;
            m_Log.Info("Dump Time: " + loadTime.TotalSeconds + "s");
        }
    }
}
