using HarmonyLib;
using System;
using System.Linq;

namespace BEP.CustomTalkCore
{
	/// <summary>
    /// キャラ生成時にカスタム口調の設定がされているならその口調に設定する処理
    /// </summary>
    [HarmonyPatch(typeof(CharaGen), nameof(CharaGen._Create))]
    [HarmonyPatch(new Type[]
    {
        typeof(string),
        typeof(int),
        typeof(int)
    })]
    internal class Fix_CharaGen
    {
        [HarmonyPostfix]
        public static void Postfix(ref Chara __result, string id, int idMat = -1, int lv = -1)
        {
            if (__result == null)
            {
                return;
            }
            // キャラidがカスタム口調を強制している場合、設定を行う
            CustomTalkCharaSetting.Row row = CustomTalkCore.CustomTalkCharaSetting.rows.Where(x => x.charaid == id).FirstOrDefault();
            if (row != null)
            {
                __result.SetObj<string>(745001, row.customid);
            }
        }
    }
}