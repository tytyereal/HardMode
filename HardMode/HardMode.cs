using HarmonyLib;
using MelonLoader;

namespace HardMode
{
    public class HardModeMod : MelonMod
    {
        //Redefines Backpack tile gain to be flatter and more difficult early game
        [HarmonyPatch(typeof(Player), nameof(Player.AddExperience))]
        class TileLevelUpCountPatch
        {
            static bool Prefix(ref Player __instance)
            {
                MelonLogger.Msg("Player.Start");
                __instance.gridsToGain = new int[]
                {
                    0, 0, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3
                };

                return true;
            }
        }

        //Limits the the amount of items you can take to 2
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ChangeItemsAllowedToTake))]
        class ItemsAllowedToTakePatch
        {
            static bool Prefix(ref GameManager __instance)
            {
                if (__instance.numOfItemsAllowedToTake == 3)
                {
                    __instance.numOfItemsAllowedToTake = 2;
                }
                return true;

            }
        }

        //Changes default AP per turn
        [HarmonyPatch(typeof(Player), nameof(Player.NextTurn))]
        class ApChangePatch
        {
            static bool Prefix(ref Player __instance)
            {
                __instance.SetAP(2);
                return false;
            }
        }

        //Checks if you are starting a new game
        public static bool startNewOrMatt = false;
        [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.LoadGame))]
        class StartNewGameCheckPatch
        {
            static void Postfix()
            {
                startNewOrMatt = true;
            }
        }
        //Checks if you are starting a matthew game
        [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.LoadMatt))]
        class StartMattGameCheckPatch
        {
            static void Postfix()
            {
                startNewOrMatt = true;
            }
        }
        //Sets Max health to 20 if starting a new run
        [HarmonyPatch(typeof(Status), "Start")]
        class MaxHpPatch
        {
            static void Postfix(ref Status __instance)
            {
                if (!startNewOrMatt) return;
                UnityEngine.Object.FindObjectOfType<Player>().stats.maxHealth = 20;
                startNewOrMatt = false;
                __instance.ClampHealth();
            }
        }
    }
}