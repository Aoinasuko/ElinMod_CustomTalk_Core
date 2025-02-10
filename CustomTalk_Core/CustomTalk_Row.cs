using System;

namespace BEP.CustomTalkCore
{
	/// <summary>
	/// システムメッセージ置き換え用の設定
	/// </summary>
    public class LangCustomGame : SourceDataString<LangCustomGame.Row>
    {
        [Serializable]
        public class Row : BaseRow
        {
            // カスタム口調のID
            public string customid;
            // ID
            public string id;
            // 日本語
            public string text_JP;
            // 英語
            public string text;
        }

        public override Row CreateRow()
        {
            return new Row
            {
                customid = SourceData.GetString(0),
                id = SourceData.GetString(1),
                text_JP = SourceData.GetString(2),
                text = SourceData.GetString(3)
            };
        }
    }

	/// <summary>
	/// NPC生成時に強制的にカスタム口調を適応する設定
	/// </summary>
    public class CustomTalkCharaSetting : SourceDataString<CustomTalkCharaSetting.Row>
    {
        [Serializable]
        public class Row : BaseRow
        {
            // カスタム口調のID
            public string customid;
            // ID
            public string charaid;
        }

        public override Row CreateRow()
        {
            return new Row
            {
                customid = SourceData.GetString(0),
                charaid = SourceData.GetString(1)
            };
        }

        public override void SetRow(Row r)
        {
            map[r.charaid] = r;
        }
    }
}
