using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MoreVaccing.Main;
using UnityEngine;

namespace MoreVaccing.Harmony.Feral
{
    [HarmonyPatch]
    internal static class DirectedActorSpawnerSpawnPatch
    {
        public static MethodInfo TargetMethod() => AccessTools.Method(AccessTools.Inner(typeof(DirectedActorSpawner), "<Spawn>d__22"), "MoveNext");

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instrs = instructions.ToList();
            object stloc = null;
            for (int i = 0; i < instrs.Count; i++)
            {
                var instr = instrs[i];
                yield return instr;

                if (instr.operand is MethodInfo ia && ia.Name == "InstantiateActor")
                {
                    i++;
                    yield return instrs[i];

                    stloc = instrs[i].operand;
                    yield return new CodeInstruction(OpCodes.Ldloc, stloc);
                    yield return new CodeInstruction(OpCodes.Call, typeof(DirectedActorSpawnerSpawnPatch).GetMethod(nameof(SetVacuumableToLarge)));
                }

                if (instr.operand is MethodInfo sf && sf.Name == "SetFeral" && stloc != null)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc, stloc);
                    yield return new CodeInstruction(OpCodes.Call, typeof(DirectedActorSpawnerSpawnPatch).GetMethod(nameof(SetVacuumableToNormal)));
                }
            }
        }

        public static void SetVacuumableToLarge(GameObject gameObj)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(gameObj)))
                gameObj.GetComponent<Vacuumable>().size = Vacuumable.Size.LARGE;
        }

        public static void SetVacuumableToNormal(GameObject gameObj)
        {
            if (Identifiable.IsLargo(Identifiable.GetId(gameObj)))
                gameObj.GetComponent<Vacuumable>().size = Vacuumable.Size.NORMAL;
        }
    }
}
