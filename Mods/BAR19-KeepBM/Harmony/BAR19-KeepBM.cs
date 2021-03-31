using DMT;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class KeepBM
{
  public class KeepBM_Init : IHarmony
  {
    public void Start()
    {
        Debug.Log(" Loading Patch: " + GetType().ToString());
        var harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
  }
  
  [HarmonyPatch(typeof(EntityPlayer))]
  [HarmonyPatch("SetDead")]
  public class Patch_EntityPlayer_SetDead
  {
    static void Postfix(ref bool ___IsBloodMoonDead)
    {
      ___IsBloodMoonDead = false;
    }
  }
}
