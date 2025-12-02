using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : NetworkBehaviour
{
    public Dropdown difficultyDropdown;
    public Dropdown modeDropdown;
    private ClientChatSystemTester chatSystem;

    [SyncVar(hook = nameof(OnDifficultyChanged))]
    public int difficulty;

    [SyncVar(hook = nameof(OnModeChanged))]
    public int mode;

    public override void OnStartLocalPlayer()
    {
        // シーン内の UI 管理オブジェクトを探す
        var ui = FindObjectOfType<GameSettingsUI>();
        if (ui != null)
        {
            //UIの紐づけ
            ui.Bind(this);
            Debug.Log($"UI Bind 完了");

            // PlayerNetWorkSystem からフラグを受け取る
            var playerSystem = GetComponent<MirrorChatSystems.PlayerNetWorkSystem>();

            // ホスト判定はここで直接制御
            ui.difficultyDropdown.interactable = playerSystem.isHostPlayer;
            ui.modeDropdown.interactable = playerSystem.isHostPlayer;
        }

        // 同じプレイヤーにアタッチされているチャットシステムを探す
        chatSystem = GetComponent<ClientChatSystemTester>();

    }

    // クライアントからサーバーへ送る
    [Command]
    public void CmdSetDifficulty(int value)
    {
        Debug.Log($"サーバーで難易度更新: {value}");

        difficulty = value; // サーバーで更新 → 全員に同期
    }

    [Command]
    public void CmdSetMode(int value)
    {
        mode = value;
    }

    private void OnDifficultyChanged(int oldValue, int newValue)
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.SetValueWithoutNotify(newValue);
            // チャットで全員に通知
            if (chatSystem != null)
            {
                chatSystem.CmdSendMessage($"難易度が {newValue} に変更されました", null, null, -1);
            }

        }
        else
        {
            Debug.LogWarning($"difficultyDropdown が未初期化のまま hook が呼ばれました（値: {newValue}）");
        }
    }

    private void OnModeChanged(int oldValue, int newValue)
    {
        if (modeDropdown != null)
        {
            modeDropdown.SetValueWithoutNotify(newValue);
            // チャットで全員に通知
            if (chatSystem != null)
            {
                chatSystem.CmdSendMessage($"モードが {newValue} に変更されました", null, null, -1);
            }
        }
        else
        {
            Debug.LogWarning($"modeDropdown が未初期化のまま hook が呼ばれました（値: {newValue}）");
        }
    }

    // UIイベントから呼ぶ
    public void OnDifficultyDropdownChanged(int value)
    {
        Debug.Log($"Dropdown変更検知: {value}");
        CmdSetDifficulty(value); // クライアントからサーバーへ
    }

    public void OnModeDropdownChanged(int value)
    {
        Debug.Log($"Dropdown変更検知: {value}");

        CmdSetMode(value);
    }
}
