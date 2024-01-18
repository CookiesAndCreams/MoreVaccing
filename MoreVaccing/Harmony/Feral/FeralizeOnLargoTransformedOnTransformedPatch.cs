using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVaccing.Harmony.Feral
{
    [HarmonyPatch(typeof(FeralizeOnLargoTransformed), nameof(FeralizeOnLargoTransformed.OnTransformed))]
    internal static class FeralizeOnLargoTransformedOnTransformedPatch
    {
        public static void Prefix(FeralizeOnLargoTransformed __instance)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(__instance.gameObject)) && Config.ALLOW_LARGO_VACCING)
                __instance.GetComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
        }

        public static void Postfix(FeralizeOnLargoTransformed __instance)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(__instance.gameObject)) && Config.ALLOW_LARGO_VACCING)
                __instance.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
        }
    }
}
