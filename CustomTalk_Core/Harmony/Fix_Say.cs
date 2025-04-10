using HarmonyLib;
using System;

namespace BEP.CustomTalkCore
{
	/// <summary>
    /// 喋らせる前に誰が喋っているかを取得する処理
    /// </summary>
    [HarmonyPatch(typeof(Card), "Say")]
    [HarmonyPatch(new Type[]
    {
        typeof(string),
        typeof(string),
        typeof(string)
    })]
    internal class Fix_Say_A
    {
        [HarmonyPrefix]
        public static void Prefix(ref Card __instance, string lang, string ref1 = null, string ref2 = null)
        {
            if (__instance is Chara)
            {
                CustomTalkCore.TalkChara = (Chara)__instance;
				CustomTalkCore.RepCheck_1 = ref1;
				CustomTalkCore.RepCheck_2 = ref2;
            }
        }
    }

    [HarmonyPatch(typeof(Card), "Say")]
    [HarmonyPatch(new Type[]
    {
        typeof(string),
        typeof(Card),
        typeof(Card),
        typeof(string),
        typeof(string)
    })]
    internal class Fix_Say_B
    {
        [HarmonyPrefix]
        public static void Prefix(ref Card __instance, string lang, Card c1, Card c2, string ref1 = null, string ref2 = null)
        {
            if (__instance is Chara)
            {
                CustomTalkCore.TalkChara = (Chara)__instance;
				CustomTalkCore.RepCheck_1 = Msg.GetName(c1);
				CustomTalkCore.RepCheck_2 = Msg.GetName(c2);
				CustomTalkCore.RepCheck_3 = ref1;
				CustomTalkCore.RepCheck_4 = ref2;
            }
        }
    }

    [HarmonyPatch(typeof(Card), "Say")]
    [HarmonyPatch(new Type[]
    {
        typeof(string),
        typeof(Card),
        typeof(string),
        typeof(string)
    })]
    internal class Fix_Say_C
    {
        [HarmonyPrefix]
        public static void Prefix(ref Card __instance, string lang, Card c1, string ref1 = null, string ref2 = null)
        {
            if (__instance is Chara)
            {
                CustomTalkCore.TalkChara = (Chara)__instance;
				CustomTalkCore.RepCheck_1 = Msg.GetName(c1);
				CustomTalkCore.RepCheck_2 = ref1;
				CustomTalkCore.RepCheck_3 = ref2;
            }
        }
    }
}