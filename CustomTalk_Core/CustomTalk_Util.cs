using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BEP.CustomTalkCore
{
	public static class CustomTalk_Util
    {
		/// <summary>
		/// 喋る候補から、条件を満たしていない候補を除外する処理
		/// </summary>
		public static string FilterStringRow(string list) {
			// キャラを取得して取得できなかった場合はそのままリストを戻す
			if (CustomTalkCore.TalkChara == null || list.IsEmpty()) {
				return list;
			}
			// 文字列処理用の配列
			List<string> buflist = list.Split(Environment.NewLine.ToCharArray()).ToList();
			List<string> outputlist = new List<string>();
			// 出力用の文字列
			string output = "";

			// フィルター用の判定を用意する
			// HPが50%以下
			bool LowHP = CustomTalkCore.TalkChara.hp <= (CustomTalkCore.TalkChara.MaxHP / 2);
			// MPが50%以下
			bool LowMP = CustomTalkCore.TalkChara.mana.value <= (CustomTalkCore.TalkChara.mana.max / 2);
			// 空腹度が空腹より空いている
			bool IsHungry = CustomTalkCore.TalkChara.hunger.GetPhase() >= 3;

			// 拒食症状態
			bool IsAnorexia = CustomTalkCore.TalkChara.HasCondition<ConAnorexia>();
			// 出血状態
			bool IsBleed = CustomTalkCore.TalkChara.HasCondition<ConBleed>();
			// 盲目状態
			bool IsBlind = CustomTalkCore.TalkChara.HasCondition<ConBlind>();
			// ブースト状態
			bool IsBoost = CustomTalkCore.TalkChara.HasCondition<ConBoost>();
			// 炎上状態
			bool IsBurning = CustomTalkCore.TalkChara.HasCondition<ConBurning>();
			// 混乱状態
			bool IsConfuse = CustomTalkCore.TalkChara.HasCondition<ConConfuse>();
			// 病気状態
			bool IsDisease = CustomTalkCore.TalkChara.HasCondition<ConDisease>();
			// 恐怖状態
			bool IsFear = CustomTalkCore.TalkChara.HasCondition<ConFear>();
			// 凍結状態
			bool IsFreeze = CustomTalkCore.TalkChara.HasCondition<ConFreeze>();
			// 毒状態
			bool IsPoison = CustomTalkCore.TalkChara.HasCondition<ConPoison>();

			// どの状態でもない
			bool IsNormal = true;

			// フィルタリングを行う
			foreach (string row in buflist) {
				// 値を編集するため別変数に保管する
				string row_output = row;
				/* 通常条件 */
				// [LowHP] : HPが50%以下
				if (row.Contains("[LowHP]")) {
					if (LowHP) {
						row_output = row_output.Replace("[LowHP]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [LowMP] : MPが50%以下
				if (row.Contains("[LowMP]")) {
					if (LowMP) {
						row_output = row_output.Replace("[LowMP]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsHungry] : 空腹状態以下
				if (row.Contains("[IsHungry]")) {
					if (IsHungry) {
						row_output = row_output.Replace("[IsHungry]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsAnorexia] : 拒食症状態
				if (row.Contains("[IsAnorexia]")) {
					if (IsAnorexia) {
						row_output = row_output.Replace("[IsAnorexia]","");
						IsNormal = false;
					} else {
						continue;
					}
				}

				/* 状態異常条件 */
				// [IsBleed] : 出血状態
				if (row.Contains("[IsBleed]")) {
					if (IsBleed) {
						row_output = row_output.Replace("[IsBleed]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsBlind] : 盲目状態
				if (row.Contains("[IsBlind]")) {
					if (IsBlind) {
						row_output = row_output.Replace("[IsBlind]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsBoost] : ブースト状態
				if (row.Contains("[IsBoost]")) {
					if (IsBoost) {
						row_output = row_output.Replace("[IsBoost]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsBurning] : 炎上状態
				if (row.Contains("[IsBurning]")) {
					if (IsBurning) {
						row_output = row_output.Replace("[IsBurning]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsConfuse] : 混乱状態
				if (row.Contains("[IsConfuse]")) {
					if (IsConfuse) {
						row_output = row_output.Replace("[IsConfuse]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsDisease] : 病気状態
				if (row.Contains("[IsDisease]")) {
					if (IsDisease) {
						row_output = row_output.Replace("[IsDisease]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsFear] : 恐怖状態
				if (row.Contains("[IsFear]")) {
					if (IsFear) {
						row_output = row_output.Replace("[IsFear]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsFreeze] : 凍結状態
				if (row.Contains("[IsFreeze]")) {
					if (IsFreeze) {
						row_output = row_output.Replace("[IsFreeze]","");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsPoison] : 毒状態
				if (row.Contains("[IsPoison]")) {
					if (IsPoison) {
						row_output = row_output.Replace("[IsPoison]","");
						IsNormal = false;
					} else {
						continue;
					}
				}

				/* 置き換え文字列 */
				// [IsRep1:***] : 置き換え文字列内に***が含まれている
				if (row.Contains("[IsRep1:")) {
					string ptn = @"(\[IsRep1:.*\])";
					Match matche = Regex.Match(row_output, ptn);
					var result = matche.Value;
					string rep_1 = result.Replace("[IsRep1:","");
					rep_1 = rep_1.Replace("]","");
					if (CustomTalkCore.RepCheck_1 != null && CustomTalkCore.RepCheck_1.Contains(rep_1)) {
						row_output = Regex.Replace(row_output,ptn,"");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsRep2:***]
				if (row.Contains("[IsRep2:")) {
					string ptn = @"(\[IsRep2:.*\])";
					Match matche = Regex.Match(row_output, ptn);
					var result = matche.Value;
					string rep_2 = result.Replace("[IsRep2:","");
					rep_2 = rep_2.Replace("]","");
					if (CustomTalkCore.RepCheck_2 != null && CustomTalkCore.RepCheck_2.Contains(rep_2)) {
						row_output = Regex.Replace(row_output,ptn,"");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsRep3:***]
				if (row.Contains("[IsRep3:")) {
					string ptn = @"(\[IsRep3:.*\])";
					Match matche = Regex.Match(row_output, ptn);
					var result = matche.Value;
					string rep_3 = result.Replace("[IsRep3:","");
					rep_3 = rep_3.Replace("]","");
					if (CustomTalkCore.RepCheck_3 != null && CustomTalkCore.RepCheck_3.Contains(rep_3)) {
						row_output = Regex.Replace(row_output,ptn,"");
						IsNormal = false;
					} else {
						continue;
					}
				}
				// [IsRep4:***]
				if (row.Contains("[IsRep4:")) {
					string ptn = @"(\[IsRep4:.*\])";
					Match matche = Regex.Match(row_output, ptn);
					var result = matche.Value;
					string rep_4 = result.Replace("[IsRep4:","");
					rep_4 = rep_4.Replace("]","");
					if (CustomTalkCore.RepCheck_4 != null && CustomTalkCore.RepCheck_4.Contains(rep_4)) {
						row_output = Regex.Replace(row_output,ptn,"");
						IsNormal = false;
					} else {
						continue;
					}
				}

				/* 特殊条件 */
				// [IsNormal] : 通常状態
				if (row.Contains("[IsNormal]")) {
					if (IsNormal) {
						row_output = row_output.Replace("[IsNormal]","");
					} else {
						continue;
					}
				}
				outputlist.Add(row_output);
			}
			// 候補を順に文字列に入れていく
			if (outputlist.Count > 0) {
				for (int i = 0; i < outputlist.Count; i++) {
					// 最後の要素の場合は改行を入れない
					if (i == outputlist.Count - 1) {
						output += outputlist[i].Replace(Environment.NewLine, "");
					} else {
						output += outputlist[i].Replace(Environment.NewLine, "") + Environment.NewLine;
					}
				}
			} else {
				output = "";
			}
			return output;
		}

		/// <summary>
		/// 指定したキャラがカスタム口調を適応されているか
		/// </summary>
		public static bool HasCustomTalk(Chara chara) {
			return chara.GetObj<string>(745001) != null && chara.GetObj<string>(745001) != "";
		}

	}
}