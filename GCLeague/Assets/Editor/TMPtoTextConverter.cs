using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// エディタ専用 TMPをTextに変換する
/// Tools → Convert TMP to Text
/// </summary>
public class TMPtoTextConverter : EditorWindow
{
    [MenuItem("Tools/Convert TMP to Text")]
    static void ConvertTMPtoText()
    {
        var tmps = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (var tmp in tmps)
        {
            GameObject go = tmp.gameObject;

            // 元の情報を保存
            string textValue = tmp.text;
            RectTransform rect = tmp.rectTransform;
            Vector2 anchoredPos = rect.anchoredPosition;
            Vector2 sizeDelta = rect.sizeDelta;
            TextAnchor anchor = tmp.alignment.ToString().Contains("Center") ? TextAnchor.MiddleCenter : TextAnchor.UpperLeft;

            // TMP削除
            DestroyImmediate(tmp);

            // Text追加
            Text text = go.AddComponent<Text>();

            // フォント
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.font = defaultFont;

            text.text = textValue;
            text.color = Color.black;
            text.alignment = anchor;

            // RectTransformを維持
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = sizeDelta;

            Debug.Log($"Converted: {go.name}");
        }

        Debug.Log("変換が完了しました！");
    }
}