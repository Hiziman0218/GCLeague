using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : NetworkBehaviour
{
    public Dropdown difficultyDropdown;
    public Dropdown modeDropdown;

    [SyncVar(hook = nameof(OnDifficultyChanged))]
    public int difficulty;

    [SyncVar(hook = nameof(OnModeChanged))]
    public int mode;

    // ホストだけが呼べる
    [Server]
    public void SetDifficulty(int value)
    {
        difficulty = value;
    }

    [Server]
    public void SetMode(int value)
    {
        mode = value;
    }

    void Start()
    {
        // ホスト以外は操作できないようにする
        if (!isServer)
        {
            difficultyDropdown.interactable = false;
            modeDropdown.interactable = false;
        }
    }

    private void OnDifficultyChanged(int oldValue, int newValue)
    {
        difficultyDropdown.value = newValue;
        Debug.Log($"難易度が {newValue} に変更されました");
        // UI更新処理を呼ぶ
    }

    private void OnModeChanged(int oldValue, int newValue)
    {
        modeDropdown.value = newValue;
        Debug.Log($"モードが {newValue} に変更されました");
        // UI更新処理を呼ぶ
    }

    //変更処理
    public void OnDifficultyDropdownChanged(int value)
    {
        if (isServer) // ホストだけ
        {
            SetDifficulty(value); // サーバーに直接反映
        }
        else
        {
            Debug.Log("あなたはホストではないので変更できません");
        }
    }

}
