using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace PrefabDumpMax
{
    public partial class PrefabDump : GameSystemBase
    {
        private PrefabSystem prefabSystem;
        private EntityQuery prefabQuery;
        private readonly Dictionary<string, int> sourceLib = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            prefabQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabData>());

            RequireForUpdate(prefabQuery);
        }

        protected override void OnUpdate()
        {
            var startTime = DateTime.Now;
            Mod.log.Info($"{nameof(PrefabDump)} created!");
            NativeArray<Entity> prefabEntities = prefabQuery.ToEntityArray(Allocator.Temp);
            int count_e = prefabEntities.Count();
            int i = 1;
            Mod.log.Info($"{count_e} items found");
            string version = Game.Version.current.shortVersion;
            foreach (Entity entity in prefabEntities)
            {
                if (!EntityManager.TryGetComponent(entity, out PrefabData prefabData))
                    return;
                if (!prefabSystem.TryGetPrefab(prefabData, out PrefabBase prefabBase))
                    return;

                try
                {
                    string source = "00_BaseGame";
                    string prefabtype = (prefabBase.prefab.ToString() ?? "Prefabless").Replace(prefabBase.name, "").Replace(" (", "").Replace(")", "");
                    Mod.log.Info($"{i} of {count_e} : {prefabtype}/{prefabBase.name ?? "Nameless"}");
                    // m_Log.Info($"{i} of {count_e} - {prefabBase.GetInstanceID()};{prefabBase.GetPrefabID()}");
                    string fileName = $"{prefabBase.name}.Prefab";

                    if (prefabBase.asset == null)
                    {
                        AssetDataPath adp = AssetDataPath.Create($".Dump_{version}/{source}/{prefabtype}/{prefabBase.name}", fileName ?? "", EscapeStrategy.None);
                        (prefabBase.asset ?? AssetDatabase.user.AddAsset(adp, prefabBase)).Save(ContentType.Text, false, true);
                        if (sourceLib.ContainsKey(source)) { sourceLib[source] = sourceLib[source] + 1; } else { sourceLib.Add(source, 1); }
                    }
                    else if (prefabBase.asset != null && prefabBase.asset.path != null && prefabBase.asset.path.Contains(EnvPath.kContentPath))
                    {
                        try
                        {
                            source = prefabBase.asset.path.Replace(EnvPath.kContentPath + "/", "").Split('/')[0];
                            if (source == "Game")
                            {
                                source = prefabBase.asset.path.Replace(EnvPath.kContentPath + "/", "").Split('/')[1].Replace("Prefabs_", "").Replace(".cok", "");
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            source = $"{prefabBase.asset.subPath}".Split('/')[1];
                        }
                        catch (Exception)
                        {
                        }

                        PrefabAsset prefabAsset = prefabBase.asset;
                        PrefabBase newPrefabBase = prefabBase.Clone(prefabBase.name);
                        source = SourceFormatter(source);
                        AssetDataPath adp = AssetDataPath.Create($".Dump_{version}/{source}/{prefabtype}/{prefabBase.name}", fileName ?? "", EscapeStrategy.None);
                        AssetDatabase.user.AddAsset(adp, newPrefabBase).Save(ContentType.Text, false, true);
                        if (sourceLib.ContainsKey(source)) { sourceLib[source] = sourceLib[source] + 1; } else { sourceLib.Add(source, 1); }
                    }
                    else
                    {
                        Mod.log.Info($"SKIP: Item {i} - {prefabBase.name}");
                    }
                }
                catch (Exception e)
                {
                    Mod.log.Info($"ERROR: Item {i} - {e}");
                }

                i++;
            }
            prefabEntities.Dispose();
            Enabled = false;
            var loadTime = DateTime.Now - startTime;
            Mod.log.Info("Dump Time: " + loadTime.TotalSeconds + "s");



            int count = 0;
            foreach (var item in sourceLib)
            {
                Mod.log.Info($"{item.Key} => {item.Value} items");
                count += item.Value;
            }
            Mod.log.Info($"Total => {count} items");
            GameManager.QuitGame();
        }

        private string SourceFormatter(string source)
        {
            return source switch
            {
                "ModernArchitecture" => $"01_{source}",
                "UrbanPromenades" => $"02_{source}",
                "FreeUpdate02" => $"03_{source}",
                "DragonGate" => $"04_{source}",
                "LeisureVenues" => $"05_{source}",
                "MediterraneanHeritage" => $"06_{source}",
                _ => source,
            };
        }
    }
}