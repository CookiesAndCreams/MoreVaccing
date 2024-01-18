using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SECTR_AudioSystem;
using static SlimeAppearance;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(AmmoSlotUI), nameof(AmmoSlotUI.GetCurrentIcon))]
    internal static class AmmoSlotUIGetCurrentIconPatch
    {
        private static AmmoSlotUI _ammoSlotUI;

        internal static readonly Dictionary<Identifiable.Id, Dictionary<SlimeAppearance, Sprite>> _slimeCached = new();
        internal static readonly Dictionary<Identifiable.Id, Sprite> _otherCached = new();

        public static bool Prefix(AmmoSlotUI __instance, ref Sprite __result, ref Identifiable.Id id)
        {
            if (Identifiable.IsSlime(id) && !Identifiable.IsLargo(id))
                return true;

            if (id != Identifiable.Id.NONE)
            {
                if (!Identifiable.IsLargo(id))
                {
                    __result = __instance.lookupDir.GetIcon(id);
                    if (__result)
                        return false;
                }

                SlimeAppearance slimeAppearance = null;
                if (Identifiable.IsSlime(id))
                {
                    slimeAppearance = __instance.slimeAppearanceDirector.GetChosenSlimeAppearance(id.GetSlimeDefinition());
                    if (!slimeAppearance)
                        return true;
                }

                if (Identifiable.IsSlime(id))
                {
                    if (_slimeCached.ContainsKey(id) && _slimeCached[id] != null)
                    {
                        if (_slimeCached[id].ContainsKey(slimeAppearance) && _slimeCached[id][slimeAppearance] != null)
                        {
                            __result = _slimeCached[id][slimeAppearance];
                            return false;
                        }
                    }
                }
                else
                {
                    if (_otherCached.ContainsKey(id) && _otherCached[id] != null)
                    {
                        __result = _otherCached[id];
                        return false;
                    }
                }

                Sprite icon = GenerateDynamicRender(id);
                if (!icon)
                    return true;

                __result = icon;
                if (Identifiable.IsSlime(id))
                {
                    _slimeCached.AddIfDoesNotContain(id, new());
                    _slimeCached[id].AddIfDoesNotContain(slimeAppearance, icon);
                }
                else
                    _otherCached.AddIfDoesNotContain(id, icon);

                return false;
            }

            return true;
        }

        public static Sprite GetCurrentIcon(Identifiable.Id identifiable)
        {
            if (!_ammoSlotUI)
                _ammoSlotUI = UnityEngine.Object.FindObjectOfType<AmmoSlotUI>();
            return _ammoSlotUI.GetCurrentIcon(identifiable);
        }

        public static Sprite GenerateDynamicRender(Identifiable.Id identifiable)
        {
            GameObject prefab = identifiable.GetPrefab();
            Texture2D icon = prefab?.RenderImage(Main.globalRenderConfig, out Exception exception);
            if (!icon)
                return null;
            icon.name = prefab?.name + "Icon";
            return icon.ToSprite();
        }
    }
}
