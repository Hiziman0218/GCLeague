using UnityEngine;                 // Unityエンジンの基本機能を使用
using UnityEngine.SceneManagement; // シーンを扱うために必要

public class AdditiveSceneLoader : MonoBehaviour
{
    [Header("追加でロードしたいシーン名")]
    [Tooltip("Project Settings > Build Settings に登録されているシーン名を指定します")]
    public string m_SceneName;  // Additiveで読み込む対象シーン名

    // --------------------------
    // シーン追加ロード関数
    // --------------------------
    public void LoadAdditiveScene()
    {
        // シーン名が空なら何もしない
        if (string.IsNullOrEmpty(m_SceneName))
        {
            Debug.LogError("AdditiveSceneLoader: シーン名が設定されていません");
            return;
        }

        // ※ 非同期で追加ロード（LoadSceneMode.Additive が重要）
        SceneManager.LoadSceneAsync(m_SceneName, LoadSceneMode.Additive);
    }

    // --------------------------
    // シーンアンロード関数
    // --------------------------
    public void UnloadAdditiveScene()
    {
        if (string.IsNullOrEmpty(m_SceneName))
        {
            Debug.LogError("AdditiveSceneLoader: シーン名が設定されていません");
            return;
        }

        // 読み込まれているシーンをアンロード
        SceneManager.UnloadSceneAsync(m_SceneName);
    }
}