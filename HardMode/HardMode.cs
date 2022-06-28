using HarmonyLib;
using MelonLoader;

namespace HardMode
{
    public class ModTest : MelonMod
    {
        //Redefines Backpack tile gain to be flatter and more difficult early game
        [HarmonyPatch(typeof(Player), "Start")]
        class TileLevelUpCountPatch
        {
            static void Postfix(ref Player __instance)
            { //Standard Level Rewards: 4, 4, 3, 3, 3, 3, 3, 2, 2, 2, 2, 1, 1, 1, 1, 1
                int[] newRewards =
                {
                    3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3
                };
                for (int i = 0; i < __instance.chosenCharacter.levelUps.Count; ++i)
                {
                    __instance.chosenCharacter.levelUps[i].rewards[0].rewardValue = newRewards[i];
                }
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

        //Limits your AP to 2
        [HarmonyPatch(typeof(Player), "Start")]
        class ApChangePatch
        {
            static void Postfix(ref Player __instance)
            {
                __instance.APperTurn = 2;
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