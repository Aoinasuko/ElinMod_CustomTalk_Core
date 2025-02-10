using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BEP.CustomTalkCore
{
    [BepInPlugin("bep.customtalkcore", "bep.customtalkcore", "1.0.0.0")]
    public class CustomTalkCore : BaseUnityPlugin
    {
        // カスタム口調一覧
        public static List<string> CustomTalkList = new List<string>();

        // システムメッセージ用
        public static LangCustomGame CustomGame = new LangCustomGame();

        // カスタム口調強制設定用
        public static CustomTalkCharaSetting CustomTalkCharaSetting = new CustomTalkCharaSetting();

        // 誰が発言したかを取得する変数
        public static Chara TalkChara;

        // 吹き出しを表示させるフラグ
        public static bool ChatFlag = false;

        /// <summary>
        /// Harmonyパッチを掛ける
        /// </summary>
        private void Awake()
        {
            var harmony = new Harmony("BEP.CustomTalkCore");
            harmony.PatchAll();
        }

        /// <summary>
        /// MOD起動時にカスタム口調を全MODから読み込む
        /// </summary>
        public void OnStartCore()
        {
            var sources = Core.Instance.sources;
            foreach (BaseModPackage mod in Core.Instance.mods.packages.Where(x => x.activated))
            {
                // Modにカスタム口調があるかの確認を行い、あれば読み込みを行う
                string source = mod.dirInfo.FullName + "/CustomTalk.xlsx";
                if (File.Exists(source))
                {
                    List<SourceCharaText.Row> row_base = new List<SourceCharaText.Row>(sources.charaText.rows);
                    // バニラ実装分はバニラのExcelで読み込む
                    ModUtil.ImportExcel(source, "CustomTalk_Vanilla", sources.charaText);
                    // カスタム口調一覧リストに出力するため追加したテキストの差分を出す
                    List<SourceCharaText.Row> row_sabun = sources.charaText.rows.Where(x => !row_base.Contains(x)).ToList();
                    if (!row_sabun.IsNull())
                    {
                        foreach (SourceCharaText.Row row in row_sabun)
                        {
                            CustomTalkList.Add(row.id);
                        }
                    }
                    // システムメッセージ実装分の読み込み
                    ModUtil.ImportExcel(source, "CustomTalk_System", CustomGame);
                    // カスタム口調強制化設定の読み込み
                    ModUtil.ImportExcel(source, "CustomTalk_CharaSetting", CustomTalkCharaSetting);
                }
            }
        }

		// 特定のカスタム口調取得処理
        public static LangCustomGame.Row TryGet(string customid, string id)
        {
            if (customid == null)
            {
                return null;
            }
            return CustomGame.rows.Where(x => x.customid == customid && x.id == id).FirstOrDefault();
        }
    }
}
