using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Common;
using Colossal.PSI.Environment;
using Game;
using Game.Assets;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace PrefabDumpMax
{
    public partial class PrefabDump : GameSystemBase
    {
        private readonly Dictionary<string, int> sourceLib = new();

        protected override void OnUpdate() { }

        public void CreateDump()
        {
            PrefabSystem prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            EntityQuery prefabQuery = SystemAPI.QueryBuilder().WithAll<PrefabData>().Build();
            EntityQuery contentQuery = SystemAPI.QueryBuilder().WithAll<ContentData>().Build();
            var startTime = DateTime.Now;
            string logline = $"{nameof(PrefabDump)} created!";
            NativeArray<Entity> prefabEntities = prefabQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Entity> contentEntities = contentQuery.ToEntityArray(Allocator.Temp);
            int count_e = prefabEntities.Count();
            int i = 1;

            List<string> sorted = new() { "\n", $"{count_e} items found" };
            string version = Game.Version.current.shortVersion;

            Dictionary<ContentPrefab, string> dlcDict = new();

            foreach (Entity entity in contentEntities)
            {
                if (!EntityManager.TryGetComponent(entity, out PrefabData prefabData))
                    return;
                if (!prefabSystem.TryGetPrefab(prefabData, out PrefabBase prefabBase))
                    return;

                if (prefabBase.TryGet(out DlcRequirement dlcRequirement))
                {
                    dlcDict[(ContentPrefab)prefabBase] = PlatformManager.instance.GetDlcName(
                        dlcRequirement.m_Dlc
                    );
                }
            }

            foreach (Entity entity in prefabEntities)
            {
                if (!EntityManager.TryGetComponent(entity, out PrefabData prefabData))
                    return;
                if (!prefabSystem.TryGetPrefab(prefabData, out PrefabBase prefabBase))
                    return;

                try
                {
                    string source = "00_BaseGame";
                    string prefabtype = (prefabBase.prefab.ToString() ?? "Prefabless")
                        .Replace(prefabBase.name, "")
                        .Replace(" (", "")
                        .Replace(")", "");
                    sorted.Add($"{prefabtype}/{prefabBase.name ?? "Nameless"}");
                    string fileName = $"{prefabBase.name}.Prefab";

                    prefabBase.TryGet(out ContentPrerequisite contentPrerequisite);
                    if (prefabBase.asset == null)
                    {
                        if (
                            contentPrerequisite != null
                            && contentPrerequisite.m_ContentPrerequisite
                        )
                            if (dlcDict.ContainsKey(contentPrerequisite.m_ContentPrerequisite))
                                source = dlcDict[contentPrerequisite.m_ContentPrerequisite];
                        source = SourceFormatter(source);
                        AssetDataPath adp = AssetDataPath.Create(
                            $".Dump_{version}/{source}/{prefabtype}/{prefabBase.name}",
                            fileName ?? "",
                            EscapeStrategy.None
                        );
                        (prefabBase.asset ?? AssetDatabase.user.AddAsset(adp, prefabBase)).Save(
                            ContentType.Text,
                            false,
                            true
                        );
                        if (sourceLib.ContainsKey(source))
                        {
                            sourceLib[source] = sourceLib[source] + 1;
                        }
                        else
                        {
                            sourceLib.Add(source, 1);
                        }
                    }
                    else if (
                        prefabBase.asset != null
                        && prefabBase.asset.path != null
                        && prefabBase.asset.path.Contains(EnvPath.kContentPath)
                    )
                    {
                        try
                        {
                            if (
                                contentPrerequisite != null
                                && contentPrerequisite.m_ContentPrerequisite
                            )
                                if (dlcDict.ContainsKey(contentPrerequisite.m_ContentPrerequisite))
                                    source = dlcDict[contentPrerequisite.m_ContentPrerequisite];
                                else
                                    source = prefabBase
                                        .asset.path.Replace(EnvPath.kContentPath + "/", "")
                                        .Split('/')[0];

                            if (source == "00_BaseGame")
                            {
                                var suff = prefabBase.asset.path.Replace(
                                    EnvPath.kContentPath + "/",
                                    ""
                                );
                                source = suff.Split('/')[1]
                                    .Replace("Prefabs_", "")
                                    .Replace(".cok", "");
                                if (source == "Prefabs")
                                    source = suff.Split('/')[0];
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            source = $"{prefabBase.asset.subPath}".Split('/')[1];
                        }
                        catch (Exception) { }

                        //PrefabAsset prefabAsset = prefabBase.asset;
                        PrefabBase newPrefabBase = prefabBase.Clone(prefabBase.name);
                        source = SourceFormatter(source);
                        AssetDataPath adp = AssetDataPath.Create(
                            $".Dump_{version}/{source}/{prefabtype}/{prefabBase.name}",
                            fileName ?? "",
                            EscapeStrategy.None
                        );
                        AssetDatabase
                            .user.AddAsset(adp, newPrefabBase)
                            .Save(ContentType.Text, false, true);
                        if (sourceLib.ContainsKey(source))
                            sourceLib[source] = sourceLib[source] + 1;
                        else
                            sourceLib.Add(source, 1);
                    }
                    else
                    {
                        logline += $"\r\nSKIP: Item {i} - {prefabBase.name}";
                    }
                }
                catch (Exception e)
                {
                    logline += $"\r\nERROR: Item {i} - {e}";
                }

                i++;
            }
            Mod.log.Info(logline);
            var loadTime = DateTime.Now - startTime;
            logline = $"Dump Time: {loadTime.TotalSeconds}s";

            int count = 0;
            foreach (var item in sourceLib)
            {
                logline += $"\r\n{item.Key} => {item.Value} items";
                count += item.Value;
            }
            logline += $"\r\nTotal => {count} items";
            Mod.log.Info(logline);

            sorted.Sort();
            Mod.log.Info(string.Join("\n", sorted));
        }

        private static string SourceFormatter(string source)
        {
            return source switch
            {
                "CS1TreasureHunt" => $"00_BaseGame_{source}",
                "LandmarkBuildings" => $"00_BaseGame_{source}",
                "SanFranciscoSet" => $"00_BaseGame_{source}",
                "ModernArchitecture" => $"01_{source}",
                "UrbanPromenades" => $"02_{source}",
                "FreeUpdate02" => $"03_{source}",
                "LeisureVenues" => $"04_{source}",
                "MediterraneanHeritage" => $"05_{source}",
                "DragonGate" => $"06_{source}",
                "BridgesAndPorts" => $"07_{source}",
                _ => source,
            };
        }
    }
}
