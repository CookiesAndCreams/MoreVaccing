using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVaccing.Harmony.Feral
{
    [HarmonyPatch(typeof(SlimeFeral), nameof(SlimeFeral.Awake))]
    internal static class SlimeFeralAwakePatch
    {
        public static void Prefix(SlimeFeral __instance)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(__instance.gameObject)) && Config.ALLOW_LARGO_VACCING)
                __instance.GetComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
        }

        public static void Postfix(SlimeFeral __instance)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(__instance.gameObject)) && Config.ALLOW_LARGO_VACCING)
                __instance.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
        }
    }
}
