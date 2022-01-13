using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
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

        #region Teleport and Health

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerTeleportHandler), "Teleport", new Type[] { })]
        public class bhaptics_PlayerTeleport
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("TeleportThrough");
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "UpdateHealth", new Type[] { typeof(float) })]
        public class bhaptics_UpdateHealth
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Player.PlayerControl __instance, float changeValue)
            {
                if (changeValue > 0f)
                {
                    if (!tactsuitVr.IsPlaying("Healing")) tactsuitVr.PlaybackHaptics("Healing");
                }
                if (__instance.Health == 0f) { tactsuitVr.StopHeartBeat(); return; }
                if (__instance.Health <= 25f) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "UpdateMana", new Type[] { typeof(float) })]
        public class bhaptics_UpdateMana
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Player.PlayerControl __instance, float changeValue)
            {
                if (changeValue > 0f)
                {
                    if (!tactsuitVr.IsPlaying("Healing")) tactsuitVr.PlaybackHaptics("Healing");
                }
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

        #endregion

        #region Damage and casting

        [HarmonyPatch(typeof(Cortopia.Scripts.Player.PlayerControl), "TakeDamage", new Type[] { typeof(Assets.Scripts.Enums.DamageType), typeof(float), typeof(Vector2) })]
        public class bhaptics_TakeDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Player.PlayerControl __instance, Assets.Scripts.Enums.DamageType damageType, float damage, Vector2 hitDirection)
            {
                // bhaptics starts in the front, then rotates to the left. 0° is front, 90° is left, 270° is right.
                Vector2 frontFacing = new Vector2(1f, 1f);
                float playerRotation = __instance.CameraTransform.rotation.eulerAngles.y;
                float hitAngle = Vector2.SignedAngle(hitDirection, frontFacing);
                float myAngle = hitAngle - playerRotation;
                myAngle *= -1f;
                float correctedAngle = 360f - myAngle;
                //tactsuitVr.LOG("Hit: " + correctedAngle.ToString());
                if (correctedAngle > 360f) correctedAngle -= 360f;
                if (correctedAngle < 0f) correctedAngle += 360f;
                tactsuitVr.PlayBackHit("Impact", correctedAngle, 0f);
            }
        }

        [HarmonyPatch(typeof(Cortopia.Scripts.Wand.WandManager), "SpellRelease", new Type[] { typeof(Assets.Scripts.Enums.WandHand), typeof(int) })]
        public class bhaptics_SpellRelease
        {
            [HarmonyPostfix]
            public static void Postfix(Cortopia.Scripts.Wand.WandManager __instance, Assets.Scripts.Enums.WandHand wandHand, int spellSlotIndex)
            {
                bool isRightHand = false;
                if (wandHand == Assets.Scripts.Enums.WandHand.Right) isRightHand = true;
                tactsuitVr.CastSpell("Fire", isRightHand);
            }
        }
        #endregion

    }
}
