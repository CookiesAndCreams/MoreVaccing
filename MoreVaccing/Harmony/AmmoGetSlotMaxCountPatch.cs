using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(Ammo), nameof(Ammo.GetSlotMaxCount), new Type[] { typeof(Identifiable.Id), typeof(int) })]
    internal static class AmmoGetSlotMaxCountPatch
    {
        public static void Postfix(ref int __result, ref Identifiable.Id id)
        {
            if (Identifiable.IsLargo(id) || id == Identifiable.Id.TARR_SLIME)
                __result /= 2;
        }
    }
}
