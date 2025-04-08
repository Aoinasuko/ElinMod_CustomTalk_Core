using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

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

	/// <summary>
    /// カスタム口調で特定の口調を持っているかチェック
    /// </summary>
    [HarmonyPatch(typeof(Card), "HasTalk")]
    [HarmonyPatch(new Type[]
    {
        typeof(string)
    })]
    internal class Fix_HasTalk
    {
        [HarmonyPostfix]
        public static void Postfix(ref Card __instance, ref bool __result, string idTopic)
        {
			if (__instance is Chara) {
				Chara chara = (Chara)__instance;
				if (CustomTalk_Util.HasCustomTalk(chara))
				{
					if (CustomTalkCore.CustomGame.rows.Any(x => x.customid == chara.GetObj<string>(745001) && x.id == idTopic)) {
						__result = true;
					} else {
						__result = false;
					}
				}
			}
        }
    }

	/// <summary>
    /// カスタム口調でrumorを持っている場合、その内容で固定化される
    /// </summary>
    [HarmonyPatch(typeof(DramaCustomSequence), "GetRumor")]
    [HarmonyPatch(new Type[]
    {
        typeof(Chara)
    })]
    internal class Fix_GetRumor
    {
        [HarmonyPostfix]
        public static void Postfix(ref DramaCustomSequence __instance, ref string __result, Chara c)
        {
			if (CustomTalk_Util.HasCustomTalk(c))
			{
				string customid = CustomTalkCore.TalkChara.GetObj<string>(745001);
				LangCustomGame.Row row = CustomTalkCore.TryGet(customid, "rumor");
				if (row != null)
				{
					if (c.interest <= 0)
					{
						__result = __instance.GetText(c, "rumor", "bored");
					} else {
						__result = c.GetTalkText("rumor");
					}
				}
			}
        }
    }
}