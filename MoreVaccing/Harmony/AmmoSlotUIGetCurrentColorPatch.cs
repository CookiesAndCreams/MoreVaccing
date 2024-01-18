using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(AmmoSlotUI), nameof(AmmoSlotUI.GetCurrentColor))]
    internal static class AmmoSlotUIGetCurrentColorPatch
    {
        private static AmmoSlotUI _ammoSlotUI;

        internal static readonly Dictionary<Identifiable.Id, Dictionary<SlimeAppearance, Color>> _slimeCached = new();
        internal static readonly Dictionary<Identifiable.Id, Color> _otherCached = new();

        public static bool Prefix(AmmoSlotUI __instance, ref Color __result, ref Identifiable.Id id)
        {
            if (Identifiable.IsSlime(id) && !Identifiable.IsLargo(id))
                return true;

            if (id != Identifiable.Id.NONE)
            {
                if (!Identifiable.IsLargo(id))
                {
                    __result = __instance.lookupDir.GetColor(id);
                    if (__result != Color.clear)
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

                Color color = AverageColorFromTexture(__instance.GetCurrentIcon(id).texture);
                if (color == Color.clear)
                    return true;

                __result = color;
                if (Identifiable.IsSlime(id))
                {
                    _slimeCached.AddIfDoesNotContain(id, new());
                    _slimeCached[id].AddIfDoesNotContain(slimeAppearance, color);
                }
                else
                    _otherCached.AddIfDoesNotContain(id, color);

                return false;
            }

            return true;
        }

        public static Color GetCurrentColor(Identifiable.Id identifiable)
        {
            if (!_ammoSlotUI)
                _ammoSlotUI = UnityEngine.Object.FindObjectOfType<AmmoSlotUI>();
            return _ammoSlotUI.GetCurrentColor(identifiable);
        }

        public static Color AverageColorFromTexture(Texture2D texture)
        {
            Color32[] texColors = texture.GetPixels32();
            int total = texColors.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            int used = 0;
            for (int i = 0; i < total; i++)
            {
                if (texColors[i].a == 0)
                    continue;
                used++;

                r += texColors[i].r;

                g += texColors[i].g;

                b += texColors[i].b;
            }

            return new Color32((byte)(r / used), (byte)(g / used), (byte)(b / used), 255);
        }
    }
}
