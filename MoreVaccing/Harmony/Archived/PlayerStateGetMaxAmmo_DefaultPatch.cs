using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVaccing.Harmony
{
    /*[HarmonyPatch(typeof(PlayerState), nameof(PlayerState.GetMaxAmmo_Default))]
    internal static class PlayerStateGetMaxAmmo_DefaultPatch
    {
        public static void Postfix(ref int __result, ref Identifiable.Id id)
        {
            if (Identifiable.IsLargo(id) || id == Identifiable.Id.TARR_SLIME)
                __result /= 2;
        }
    }*/
}
