using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SECTR_AudioSystem;
using RichPresence;

namespace MoreVaccing.Harmony
{
    [HarmonyPatch(typeof(StorageSlotUI), nameof(StorageSlotUI.Update))]
    internal static class StorageSlotUIUpdatePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (var v in instr)
            {
                if (v.opcode == OpCodes.Callvirt)
                {
                    MethodInfo info = v.operand as MethodInfo;
                    if (info.Name == "GetIcon")
                        yield return new CodeInstruction(OpCodes.Call, typeof(StorageSlotUIUpdatePatch).GetMethod(nameof(GetCurrentSiloIcon)));
                    else if (info.Name == "GetColor")
                        yield return new CodeInstruction(OpCodes.Call, typeof(StorageSlotUIUpdatePatch).GetMethod(nameof(GetCurrentSiloColor)));
                    else
                        yield return v;
                }
                else yield return v;
            }
        }

        public static Sprite GetCurrentSiloIcon(LookupDirector director, Identifiable.Id id) => AmmoSlotUIGetCurrentIconPatch.GetCurrentIcon(id);

        public static Color GetCurrentSiloColor(LookupDirector director, Identifiable.Id id) => AmmoSlotUIGetCurrentColorPatch.GetCurrentColor(id);
    }
}
