global using SRML;
global using UnityEngine;
global using static Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SRML.SR;
using SRML.SR.SaveSystem;
using SRML.Config.Attributes;
using MoreVaccing.Components;
using static SRML.Console.Console;
using MoreVaccing.Harmony;

namespace MoreVaccing
{
    [ConfigFile("MoreVaccing")]
    internal static class Config
    {
        public static bool ALLOW_LARGO_VACCING = true;
        public static bool ALLOW_CRATE_VACCING = true;
        public static bool ALLOW_TOY_VACCING = true;
        public static bool ALLOW_TARR_VACCING = true;

        public static bool PREVENT_GOLD_FLEEING = true;
        public static bool PREVENT_LUCKY_FLEEING = true;
    }

    public class Main : ModEntryPoint
    {
        private static readonly Dictionary<Identifiable.Id, Color> nonVaccableSlimes = new()
        {
            {
                Identifiable.Id.GOLD_SLIME,
                new Color(0.8088f, 0.5522f, 0)
            },
            {
                Identifiable.Id.LUCKY_SLIME,
                new Color(0.5179f, 0.6358f, 0.6838f)
            },
            {
                Identifiable.Id.QUICKSILVER_SLIME,
                new Color(0.3843f, 0.4196f, 0.4392f)
            },
            {
                Identifiable.Id.TARR_SLIME,
                new Color(0.1882f, 0.1529f, 0.1569f)
            }
        };

        internal static RenderConfig globalRenderConfig;

        internal static ConsoleInstance ModConsole = new ConsoleInstance("MoreVaccing");

        public override void PreLoad()
        {
            HarmonyInstance.PatchAll();
            SaveRegistry.RegisterDataParticipant<FeralizeOnStart>();

            if (globalRenderConfig == null)
            {
                globalRenderConfig = new RenderConfig(512, 512, Quaternion.LookRotation(new Vector3(1, -0.5f, -1)));
                globalRenderConfig.BeforeRender = go =>
                {
                    var rad_aura = go?.transform?.gameObject.FindChildWithPartialName("rad_aura_")?.gameObject;
                    var rad_core = go?.transform?.gameObject.FindChildWithPartialName("rad_core_")?.gameObject;
                    if (rad_aura)
                        rad_aura.SetActive(false);
                    if (rad_core)
                        rad_core.SetActive(false);
                };
            }
        }

        public override void Load()
        {
            SRCallbacks.PreSaveGameLoaded += delegate (SceneContext sceneContext)
            {
                sceneContext.SlimeAppearanceDirector.onSlimeAppearanceChanged += delegate (SlimeDefinition def, SlimeAppearance app)
                {
                    foreach (var storageUI in UnityEngine.Object.FindObjectsOfType<StorageSlotUI>())
                    {
                        if (!storageUI.lookupDir)
                            continue;

                        if (!storageUI.currentlyStoredId.HasValue || storageUI.currentlyStoredId.Value == Identifiable.Id.NONE)
                            continue;

                        storageUI.slotIcon.sprite = AmmoSlotUIGetCurrentIconPatch.GetCurrentIcon(storageUI.currentlyStoredId.Value);
                        storageUI.bar.barColor = AmmoSlotUIGetCurrentColorPatch.GetCurrentColor(storageUI.currentlyStoredId.Value);
                    }
                };
            };

            foreach (var pair in nonVaccableSlimes)
            {
                var identifiable = pair.Key;
                var color = pair.Value;

                if (identifiable == Identifiable.Id.TARR_SLIME && !Config.ALLOW_TARR_VACCING)
                    continue;

                var prefab = identifiable.GetPrefab();
                if (!prefab)
                    continue;

                prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;

                if (Config.PREVENT_GOLD_FLEEING)
                {
                    if (prefab.GetComponent<SlimeFlee>()) 
                        UnityEngine.Object.Destroy(prefab.GetComponent<SlimeFlee>());
                }

                if (Config.PREVENT_LUCKY_FLEEING)
                {
                    if (prefab.GetComponent<LuckySlimeFlee>())
                        UnityEngine.Object.Destroy(prefab.GetComponent<LuckySlimeFlee>());
                }

                identifiable.GetSlimeDefinition().AppearancesDefault[0].ColorPalette.Ammo = color;
                LookupRegistry.RegisterVacEntry(identifiable, color, identifiable.GetIcon());
                AmmoRegistry.RegisterAmmoPrefab(PlayerState.AmmoMode.DEFAULT, prefab);
            }
        }

        public override void PostLoad()
        {
            foreach (var v in Identifiable.SLIME_CLASS.Concat(Identifiable.LARGO_CLASS))
                AmmoRegistry.RegisterSiloAmmo(SiloStorage.StorageType.NON_SLIMES, v);

            if (Config.ALLOW_TOY_VACCING)
            {
                foreach (var prefab in GameContext.Instance.LookupDirector.identifiablePrefabs)
                {
                    if (!prefab)
                        continue;

                    var identifiable = Identifiable.GetId(prefab);
                    if (Identifiable.IsToy(identifiable))
                    {
                        prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                        AmmoRegistry.RegisterPlayerAmmo(PlayerState.AmmoMode.DEFAULT, identifiable);
                        LookupRegistry.RegisterVacEntry(identifiable, Color.clear, null);
                    }
                }
            }

            if (Config.ALLOW_CRATE_VACCING)
            {
                foreach (var prefab in GameContext.Instance.LookupDirector.identifiablePrefabs)
                {
                    if (!prefab)
                        continue;

                    var identifiable = Identifiable.GetId(prefab);
                    if (prefab.name.StartsWith("crate") || Identifiable.STANDARD_CRATE_CLASS.Contains(identifiable))
                    {
                        prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                        AmmoRegistry.RegisterPlayerAmmo(PlayerState.AmmoMode.DEFAULT, identifiable);
                        LookupRegistry.RegisterVacEntry(identifiable, Color.clear, null);
                        TranslationPatcher.AddActorTranslation("l." + identifiable.ToString().ToLower(), identifiable.ToString().ToPascalCase().Replace("_01", "").Replace("_", " "));
                    }
                }
            }

            if (Config.ALLOW_LARGO_VACCING)
            {
                foreach (var prefab in GameContext.Instance.LookupDirector.identifiablePrefabs)
                {
                    if (!prefab)
                        continue;

                    var identifiable = Identifiable.GetId(prefab);
                    if (Identifiable.IsLargo(identifiable))
                    {
                        prefab.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
                        AmmoRegistry.RegisterPlayerAmmo(PlayerState.AmmoMode.DEFAULT, identifiable);
                        LookupRegistry.RegisterVacEntry(identifiable, Color.clear, null);
                    }
                }
            }
        }

        /*static void OnSecretStylesEnabled()
        {
            foreach (var v in GameContext.Instance.SlimeDefinitions.largoDefinitionByBaseDefinitions.Select(x => x.Value))
            {
                foreach (var a in v.Appearances)
                {
                    var set1 = a.DependentAppearances[0].SaveSet;
                    var set2 = a.DependentAppearances[1].SaveSet;
                    if (set1 == set2 && set1 == AppearanceSaveSet.CLASSIC) continue;
                    var spr = largoSprites[v.IdentifiableId][new KeyValuePair<AppearanceSaveSet, AppearanceSaveSet>(set1, set2)]();
                    a.Icon = spr;
                    var color = AverageColorFromTexture(spr.texture);

                    a.ColorPalette = new Palette()
                    {
                        Ammo = color,
                        Bottom = a.ColorPalette.Bottom,
                        Top = a.ColorPalette.Top,
                        Middle = a.ColorPalette.Middle
                    };
                }
            }
        }*/

        /*static Color32 AverageColorFromTexture(Texture2D tex)
        {

            Color32[] texColors = tex.GetPixels32();

            int total = texColors.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            int used = 0;

            for (int i = 0; i < total; i++)
            {
                if (texColors[i].a == 0) continue;
                used++;
                r += texColors[i].r;

                g += texColors[i].g;

                b += texColors[i].b;

            }

            return new Color32((byte)(r / used), (byte)(g / used), (byte)(b / used), 255);

        }*/


        /*public static string[] ParseStringCapitals(string source)
        {
            List<string> strings = new List<string>();
            string toAdd = "";
            foreach (char c in source)
            {
                if (Char.IsUpper(c))
                {
                    if (toAdd != "")
                    {
                        strings.Add(toAdd);
                        toAdd = "";
                    }

                }
                toAdd += c;
            }
            if (toAdd != "") strings.Add(toAdd);
            return strings.ToArray();
        }*/

        /*[HarmonyPatch(typeof(StorageSlotUI))]
        [HarmonyPatch("Update")]
        public static class StorageSlotUpdatePatch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                foreach (var v in instr)
                {
                    if (v.opcode == OpCodes.Callvirt)
                    {
                        MethodInfo info = v.operand as MethodInfo;
                        if (info.Name == "GetIcon")
                        {
                            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StorageSlotUpdatePatch), "AlternateIcon"));
                        }
                        else if (info.Name == "GetColor")
                        {
                            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StorageSlotUpdatePatch), "AlternateColor"));
                        }
                        else yield return v;
                    }
                    else yield return v;
                }
            }

            public static Color AlternateColor(LookupDirector director, Identifiable.Id id)
            {
                if (Identifiable.IsSlime(id))
                {
                    var a = SceneContext.Instance.SlimeAppearanceDirector.GetChosenSlimeAppearance(id);
                    return a.ColorPalette.Ammo;
                }
                if (id != Identifiable.Id.NONE)
                {
                    return director.GetColor(id);
                }
                return Color.clear;
            }

            public static Sprite AlternateIcon(LookupDirector director, Identifiable.Id id)
            {
                if (Identifiable.IsSlime(id))
                {
                    return SceneContext.Instance.SlimeAppearanceDirector.GetCurrentSlimeIcon(id);
                }
                if (id != Identifiable.Id.NONE)
                {
                    return director.GetIcon(id);
                }
                return null;
            }
        }


        [HarmonyPatch(typeof(SavedGame))]
        public static class SavedGamePatch
        {
            public static MethodInfo TargetMethod()
            {
                return AccessTools.Method(typeof(SavedGame), "Pull",
                    new Type[] { typeof(GameModel), typeof(List<ActorDataV09>), typeof(WorldV22) });
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                foreach (var c in instr)
                {
                    if (c.opcode == OpCodes.Ldc_I4 && (int)c.operand == 166)
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    }
                    else
                    {
                        yield return c;
                    }
                }
            }
        }*/
    }
}
