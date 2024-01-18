using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreVaccing.Components;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(WeaponVacuum), nameof(WeaponVacuum.ConsumeVacItem))]
    internal static class WeaponVacuumConsumeVacItemPatch
    {
        public static void Prefix(ref GameObject gameObj)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(gameObj)) && Config.ALLOW_LARGO_VACCING)
            {
                var slimeFeral = gameObj.GetComponent<SlimeFeral>();
                if (!slimeFeral || !slimeFeral.IsFeral())
                    return;
                if (!gameObj.GetComponent<FeralizeOnStart>())
                    gameObj.AddComponent<FeralizeOnStart>();
            }
        }
    }
}
