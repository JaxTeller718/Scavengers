using DMT;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Reflection.Emit;
using System.Linq;

public class RH_GameOptionWanderingHorde
{
    public class RH_GameOptionWanderingHorde_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log(" Loading Patch : " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(AIDirectorWanderingHordeComponent))]
    [HarmonyPatch("ChooseNextWanderingHordeTime")]
    public class PatchAIDirectorWanderingHordeComponentChooseNextWanderingHordeTime
    {
        static bool Prefix(AIDirectorWanderingHordeComponent __instance, ref ulong ___m_nextWanderingHordeTime)
        {
            ulong frequency = 24000u;

            switch (RH_Options.GameOptions.WanderingHordeFrequency)
            {
                case 0:
                    frequency = (ulong)__instance.Random.RandomRange(12000, 24000);
                    break;
                case 1:
                    frequency = (ulong)__instance.Random.RandomRange(10000, 22000);
                    break;
                case 2:
                    frequency = (ulong)__instance.Random.RandomRange(8000, 20000);
                    break;
                case 3:
                    frequency = (ulong)__instance.Random.RandomRange(6000, 18000);
                    break;
                case 4:
                    frequency = (ulong)__instance.Random.RandomRange(4000, 16000);
                    break;
                case 5:
                    frequency = (ulong)__instance.Random.RandomRange(2000, 14000);
                    break;
            }

            ___m_nextWanderingHordeTime = __instance.Director.World.worldTime + frequency; // CHANGED 12000, 24000
            __instance.DisplayWanderingHordeTime();

            //Log.Out("ChooseNextWanderingHordeTime m_nextWanderingHordeTime : " + __instance.m_nextWanderingHordeTime);

            return false;
        }
    }

    [HarmonyPatch(typeof(AIDirectorHordeComponent))]
    [HarmonyPatch("FindWanderingHordeTargets")]
    public class PatchAIDirectorHordeComponentFindWanderingHordeTargets
    {
        static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 12)
                {
                    codes[i].operand = (sbyte)GetDelay(); // TODO : Change this to an in game call to this method, atm GameDifficulty is always 2 as the actual game hasnt loaded yet
                }
            }

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(AIDirectorGameStagePartySpawner))]
    [HarmonyPatch("SetupGroup")]
    public class Patch_AIDirectorGameStagePartySpawnerSetupGroup
    {
        static void Postfix(AIDirectorGameStagePartySpawner __instance, ref GameStageDefinition.SpawnGroup ___spawnGroup, ref int ___numToSpawn)
        {
            if(___spawnGroup != null)
            {
                ___numToSpawn = EntitySpawner.ModifySpawnCountByGameDifficulty(___spawnGroup.spawnCount) * (RH_Options.GameOptions.WanderingHordeMultiplier);
            }
        }
    }

    private static uint GetDelay()
    {
        switch (GameStats.GetInt(EnumGameStats.GameDifficulty))
        {
            case 0:
                return 12u;
            case 1:
                return 10u;
            case 2:
                return 8u;
            case 3:
                return 6u;
            case 4:
                return 4u;
            case 5:
                return 2u;
        }

        return 12u;
    }
}

