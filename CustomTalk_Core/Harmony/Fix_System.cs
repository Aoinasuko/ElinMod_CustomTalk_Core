using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BEP.CustomTalkCore
{
	/// <summary>
    /// システムメッセージのテキスト差し替え
    /// </summary>
    [HarmonyPatch(typeof(Msg), "GetGameText")]
    [HarmonyPatch(new Type[]
    {
        typeof(string)
    })]
    internal class Fix_GetGameText
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, string idLang)
        {
            if (CustomTalkCore.TalkChara == null)
            {
                return;
            }
            string customid = CustomTalkCore.TalkChara.GetObj<string>(745001);
            if (customid != null && customid != "")
            {
                LangCustomGame.Row row = CustomTalkCore.TryGet(customid, idLang);
                if (row != null)
                {
					string text_base = row.GetText("text");
					// 行をリスト化
					List<string> rowList = CustomTalk_Util.FilterStringRow(text_base).Split(Environment.NewLine.ToCharArray()).ToList();
					string text = "";
					if (rowList.Count > 0) {
						text = rowList.RandomItem();
					} else {
						__result = text;
						return;
					}
					// 文字色保持書式
					if (!text.Contains("[ColorHold]")) {
						bool flag = text.StartsWith("*");
						Msg.SetColor(text.StartsWith("(") ? Msg.colors.Thinking : (flag ? Msg.colors.Ono : Msg.colors.Talk));
					} else {
						text = text.Replace("[ColorHold]","");
					}
					// 吹き出し表示無効化書式
					if (!text.Contains("[NoBalloon]"))
					{
						CustomTalkCore.ChatFlag = true;
					} else
					{
						text = text.Replace("[NoBalloon]","");
					}
                    __result = text;
                }
            }
        }
    }

    /// <summary>
    /// システムメッセージに吹き出しを表示させる
    /// </summary>
    [HarmonyPatch(typeof(Msg), "SayRaw")]
    [HarmonyPatch(new Type[]
    {
        typeof(string)
    })]
    internal class Fix_SayRaw
    {
        [HarmonyPostfix]
        public static void Postfix(string text)
        {
            if (CustomTalkCore.ChatFlag && CustomTalkCore.TalkChara != null && text != "")
            {
                CustomTalkCore.TalkChara.renderer.Say(CustomTalkCore.TalkChara.ApplyNewLine(text.StripBrackets()), default(Color), CustomTalkCore.TalkChara.IsPCParty ? 0.6f : 0f);
            }
			CustomTalkCore.ChatFlag = false;
            CustomTalkCore.TalkChara = null;
        }
    }
}