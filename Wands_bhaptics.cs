using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

namespace Wands_bhaptics
{
    public class Wands_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        
        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerTeleportHandler), "Teleport", new Type[] { })]
        public class bhaptics_PlayerTeleport
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("TeleportThrough");
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "OnMatchEnded", new Type[] { typeof(CortopiaEvents.Events.MatchEndedEvent) })]
        public class bhaptics_MatchEnded
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "OnMatchReset", new Type[] { typeof(CortopiaEvents.Events.MatchResetEvent) })]
        public class bhaptics_MatchReset
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "OnPlayerDisconnected", new Type[] { typeof(int) })]
        public class bhaptics_PlayerDisconnected
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "TakeDamage", new Type[] { typeof(Assets.Scripts.Enums.DamageType), typeof(float), typeof(UnityEngine.Vector2) })]
        public class bhaptics_TakeDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Assets.Scripts.Enums.DamageType damageType, float damage, UnityEngine.Vector2 hitDirection)
            {
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Wand.WandManager), "SpellPress", new Type[] { typeof(Assets.Scripts.Enums.WandHand), typeof(int) })]
        public class bhaptics_SpellPress
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Wand.WandManager __instance, Assets.Scripts.Enums.WandHand wandHand, int spellSlotIndex)
            {
                bool isRightHand = false;
                if (wandHand == Assets.Scripts.Enums.WandHand.Right) isRightHand = true;
                tactsuitVr.CastSpell("Fire", isRightHand);
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "UpdateHealth", new Type[] { typeof(float) })]
        public class bhaptics_UpdateHealth
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Player.PlayerControl __instance, float changeValue)
            {
                if (changeValue > 0f) tactsuitVr.PlaybackHaptics("Healing");
                tactsuitVr.LOG("UpdateHealth: " + __instance.Health.ToString());
                
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "ChangeHealth", new Type[] { typeof(float) })]
        public class bhaptics_ChangeHealth
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Player.PlayerControl __instance, float changeValue)
            {
                if (changeValue > 0f) tactsuitVr.PlaybackHaptics("Healing");
                tactsuitVr.LOG("ChangeHealth: " + __instance.Health.ToString());

            }
        }

    }
}
