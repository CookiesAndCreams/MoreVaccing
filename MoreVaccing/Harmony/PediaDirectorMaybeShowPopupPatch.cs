using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(PediaDirector), nameof(PediaDirector.MaybeShowPopup), typeof(Identifiable.Id))]
    internal static class PediaDirectorMaybeShowPopupPatch
    {
        public static bool Prefix(PediaDirector __instance, Identifiable.Id identId)
        {
            if (Identifiable.IsLargo(identId))
            {
                __instance.MaybeShowPopup(PediaDirector.Id.LARGO_SLIME);

                var slimeDefinition = identId.GetSlimeDefinition();
                if (slimeDefinition?.BaseSlimes?.Length > 0)
                    foreach (var definition in slimeDefinition.BaseSlimes)
                        __instance.MaybeShowPopup(__instance.GetPediaId(definition.IdentifiableId).Value);

                return false;
            }
            return true;
        }
    }
}
