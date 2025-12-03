using UnityEngine;
using UnityEngine.UI;

public class GameSettingsUI : MonoBehaviour
{
    [Header("難易度パネル")]
    public Dropdown difficultyDropdown;
    [Header("モード設定")]
    public Dropdown modeDropdown;

    // プレイヤーの GameSettings を受け取って紐付ける
    public void Bind(GameSettings settings)
    {
        Debug.Log("Bind呼ばれた");

        // GameSettings 側に UI を渡す
        settings.difficultyDropdown = difficultyDropdown;
        settings.modeDropdown = modeDropdown;

        // UIイベントをプレイヤーの処理に接続
        difficultyDropdown.onValueChanged.AddListener(settings.OnDifficultyDropdownChanged);
        modeDropdown.onValueChanged.AddListener(settings.OnModeDropdownChanged);

        // 初期値を反映
        difficultyDropdown.SetValueWithoutNotify(settings.difficulty);
        modeDropdown.SetValueWithoutNotify(settings.mode);

    }
}
