using HarmonyLib;
using System;
using System.Collections.Generic;

namespace BEP.CustomTalkCore
{
	/// <summary>
    /// 言葉の鞭の使用時にカスタム口調変更選択肢を出す
    /// </summary>
    [HarmonyPatch(typeof(TraitWhipLove), nameof(TraitWhipLove.TrySetHeldAct))]
    [HarmonyPatch(new Type[]
    {
        typeof(ActPlan)
    })]
    internal class Fix_Whip
    {
        [HarmonyPostfix]
        public static void Postfix(ref TraitWhipLove __instance, ActPlan p)
        {
            if (__instance is TraitWhipInterest == false)
            {
                return;
            }
            TraitWhipLove instance = __instance;
            Card owner = __instance.owner;
			// 選択肢を出す
            p.pos.ListCards().ForEach(delegate (Card a)
            {
                Chara c = a.Chara;
                if (c != null)
                {
                    List<Hobby> list = c.ListWorks(useMemberType: false);
                    List<Hobby> list2 = c.ListHobbies(useMemberType: false);
                    if (p.IsSelfOrNeighbor && EClass.pc.CanSee(a) && c.IsPCFaction && c.homeBranch != null && list.Count > 0 && list2.Count > 0)
                    {
                        string whip = "actWhip".lang() + "(CustomTalk)";
                        p.TrySetAct(whip, delegate
                        {
							// 鞭使用処理
                            EClass.pc.Say("use_whip", c, owner);
                            EClass.pc.Say("use_scope2", c);
                            c.Talk("pervert2");
                            EClass.pc.PlaySound("whip");
                            c.PlayAnime(AnimeID.Shiver);
                            c.OnInsulted();
							// カスタム口調一覧表示及び設定
                            EClass.ui.AddLayer<LayerList>().SetStringList(delegate
                            {
                                List<string> list = new List<string>();
                                list.Add("None");
                                list.AddRange(CustomTalkCore.CustomTalkList);
                                return list;
                            }, delegate (int id, string b)
                            {
                                if (id == 0)
                                {
                                    a.SetObj<string>(745001, "");
                                    return;
                                }
                                a.SetObj<string>(745001, b);
                            }).SetSize();
                            ;
                            EClass.pc.Say("use_whip4", c);
							// 残り回数消費
                            owner.ModCharge(-1);
                            if (owner.c_charges <= 0)
                            {
                                EClass.pc.Say("spellbookCrumble", owner);
                                owner.Destroy();
                            }
                            return true;
                        }, c);
                    }
                }
            });
            return;
        }
    }
}