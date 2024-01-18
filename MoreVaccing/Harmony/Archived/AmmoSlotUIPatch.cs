using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(AmmoSlotUI))]
    internal static class AmmoSlotUIPatch
    {
        private static readonly List<AmmoSlotUI> ammoSlotUIs = new List<AmmoSlotUI>();

        [HarmonyPrefix, HarmonyPatch(nameof(AmmoSlotUI.Awake))]
        public static void AwakePrefix(AmmoSlotUI __instance)
        {
            if (ammoSlotUIs.Find(x => x == null))
                ammoSlotUIs.Clear();
            ammoSlotUIs.Add(__instance);

            var ammoSlot = __instance.transform.Find("Ammo Slot " + __instance.lastSelectedAmmoIndex + 1);
            var icon = __instance.transform.Find("Icon");

            RectTransform firstSlime = UnityEngine.Object.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
            firstSlime.name = "Base Slime (1)";
            firstSlime.sizeDelta /= 1.6f;
            firstSlime.anchoredPosition = new Vector2(9, 35);

            RectTransform secondSlime = __instance.Instantiate(icon, ammoSlot.transform).GetComponent<RectTransform>();
            secondSlime.name = "Base Slime (2)";
            secondSlime.sizeDelta /= 1.6f;
            secondSlime.anchoredPosition = new Vector2(-13.5f, -6.3f);

            firstSlime.gameObject.SetActive(false);
            secondSlime.gameObject.SetActive(false);
        }

        [HarmonyPrefix, HarmonyPatch(nameof(AmmoSlotUI.Update))]
        public static void UpdateAmmoDisplayPrefix(AmmoSlotUI __instance)
        {
            var identifiable = SceneContext.Instance.PlayerState.Ammo.GetSelectedId();
            if (!Identifiable.IsLargo(identifiable))
                return;

            var ammoSlot = __instance.transform.Find("Ammo Slot " + __instance.lastSelectedAmmoIndex + 1).gameObject;
            var firstSlime = ammoSlot.transform.Find("Base Slime (1)").gameObject;
            var secondSlime = ammoSlot.transform.Find("Base Slime (2)").gameObject;

            if (firstSlime.activeSelf)
                return;
            __instance.transform.Find("Icon").gameObject.SetActive(false);

            var slimeDefinition = identifiable.GetSlimeDefinition();
            if (slimeDefinition == null)
                return;

            SlimeDefinition firstSlimeDefinition = slimeDefinition.BaseSlimes[0];
            SlimeDefinition secondSlimeDefinition = slimeDefinition.BaseSlimes[1];
            firstSlime.GetComponent<Image>().sprite = firstSlimeDefinition.IdentifiableId.GetIcon();
            secondSlime.GetComponent<Image>().sprite = secondSlimeDefinition.IdentifiableId.GetIcon();

            firstSlime.gameObject.SetActive(true);
            secondSlime.gameObject.SetActive(true);
        }
    }
}
