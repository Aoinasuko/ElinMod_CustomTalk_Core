using HarmonyLib;
using System;

namespace BEP.CustomTalkCore
{
	// キャラが喋る時、カスタム口調のデータがあればそれで喋る
    [HarmonyPatch(typeof(Chara), nameof(Chara.GetTopicText), typeof(string))]
    internal class Fix_Topic
    {
        [HarmonyPostfix]
        public static void Postfix(ref Chara __instance, ref string __result, string topic)
        {
            if (CustomTalk_Util.HasCustomTalk(__instance))
            {
                var key = __instance.GetObj<string>(745001);
                SourceCharaText.Row row = EClass.sources.charaText.map.TryGetValue(key);
                if (row == null)
                {
                    __result = null;
                }
                // 通常の待機会話を取得
				CustomTalkCore.TalkChara = __instance;
                string text = CustomTalk_Util.FilterStringRow(row.GetText(topic, returnNull: true));
				CustomTalkCore.TalkChara = null;
                if (text.IsEmpty())
                {
                    __result = null;
                }
                if (text.StartsWith("@"))
                {
                    row = EClass.sources.charaText.map.TryGetValue(text.Replace("@", ""));
                    if (row == null)
                    {
                        __result = null;
                    }
                    text = row.GetText(topic, returnNull: true);
                    if (text.IsEmpty())
                    {
                        __result = null;
                    }
                }
                __result = text.Split(Environment.NewLine.ToCharArray()).RandomItem();
            }
        }
    }

    /// <summary>
    /// PCの戦闘時の喋りを有効化する
    /// </summary>
    [HarmonyPatch(typeof(Chara), "GoHostile")]
    [HarmonyPatch(new Type[]
    {
        typeof(Card)
    })]
    internal class Fix_GoHostile
    {
        [HarmonyPostfix]
        public static void Postfix(ref Chara __instance, Card _tg)
        {
            if (__instance.IsPC)
            {
                if (CustomTalk_Util.HasCustomTalk(__instance))
                {
                    __instance.TalkTopic("aggro");
                }
            }
            return;
        }
    }

    /// <summary>
    /// 移動時の喋りを有効化する
    /// </summary>
    [HarmonyPatch(typeof(Chara), "_Move")]
    [HarmonyPatch(new Type[]
    {
        typeof(Point),
        typeof(Card.MoveType)
    })]
    internal class Fix_Move
    {
        [HarmonyPostfix]
        public static void Postfix(ref Chara __instance, Point newPoint, Card.MoveType type)
        {
            if (CustomTalk_Util.HasCustomTalk(__instance))
            {
                if (EClass.rnd(50) == 0)
                {
                    __instance.TalkTopic();
                }
            }
        }
    }
}