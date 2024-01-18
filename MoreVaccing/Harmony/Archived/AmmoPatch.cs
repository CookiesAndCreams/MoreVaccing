using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(Ammo))]
    internal class AmmoPatch
    {
        [HarmonyPatch(nameof(Ammo.Clear), new Type[] { }), HarmonyPrefix]
        public static void Clear(Ammo __instance)
        {
            if (__instance == null)
                return;
            var ammoSlotUI = AmmoSlotUIPatch.ammoSlotUIs.Find(x => x.lastSelectedAmmoIndex == __instance.GetSelectedAmmoIdx());
            var ammoSlot = __instance.GetSelectedStored().transform.Find("Ammo Slot " + ammoSlotUI.lastSelectedAmmoIndex + 1).gameObject;
            ammoSlotUI.transform.Find("Icon").gameObject.SetActive(true);
            ammoSlot.transform.Find("FirstSlime").gameObject.SetActive(false);
            ammoSlot.transform.Find("SecondSlime").gameObject.SetActive(false);
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Ammo.DecrementSelectedAmmo))]
        public static void DecrementSelectedAmmoPrefix(Ammo __instance, int amount)
        {
            var ammoSlotUI = AmmoSlotUIPatch.ammoSlotUIs.FirstOrDefault(x => x.lastSelectedAmmoIndex == __instance.GetSelectedAmmoIdx());
            if (ammoSlotUI == null || Identifiable.IsLargo(__instance.GetSelectedId()))
                return;

            /*var dataCount = ammoSlotUI.data.Count - amount;
            if (dataCount > 1)
                return;*/

            var ammoSlot = ammoSlotUI.transform.Find("Ammo Slot " + ammoSlotUI.lastSelectedAmmoIndex + 1).gameObject;
            ammoSlotUI.transform.Find("Icon").gameObject.SetActive(true);
            ammoSlot.transform.Find("Base Slime (1)").gameObject.SetActive(false);
            ammoSlot.transform.Find("Base Slime (2)").gameObject.SetActive(false);
        }

        [HarmonyPrefix, HarmonyPatch(nameof(Ammo.MaybeAddToSlot))]
        public static void MaybeAddToSlotPrefix(Ammo __instance, Identifiable.Id id, ref bool __result)
        {
            if (!__result || !Identifiable.IsLargo(id))
                return;

            var ammoSlotUI = AmmoSlotUIPatch.ammoSlotUIs.Find(x => x.lastSelectedAmmoIndex == __instance.GetSelectedAmmoIdx());
            var ammoSlot = ammoSlotUI.transform.Find("Ammo Slot " + ammoSlotUI.lastSelectedAmmoIndex + 1).gameObject;
            var firstSlime = ammoSlot.transform.Find("Base Slime (1)").gameObject;
            var secondSlime = ammoSlot.transform.Find("Base Slime (2)").gameObject;
            var slimeDefinition = id.GetSlimeDefinition();

            if (slimeDefinition == null)
                return;
            ammoSlotUI.transform.Find("Icon").gameObject.SetActive(false);

            SlimeDefinition firstSlimeDefinition = slimeDefinition.BaseSlimes[0];
            SlimeDefinition secondSlimeDefinition = slimeDefinition.BaseSlimes[1];

            firstSlime.GetComponent<Image>().sprite = firstSlimeDefinition.IdentifiableId.GetIcon();
            secondSlime.GetComponent<Image>().sprite = secondSlimeDefinition.IdentifiableId.GetIcon();

            firstSlime.gameObject.SetActive(true);
            secondSlime.gameObject.SetActive(true);
        }
    }
}
