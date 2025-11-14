using System;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] protected GameObject root; //このUIの親オブジェクト

    protected bool m_isShowClear = false; //表示が完了したか
    protected bool m_isHideClear = false; //非表示が完了したか
    protected Action ShowEvent; //表示時のイベント
    protected Action HideEvent; //非表示時のイベント

    /// <summary>
    /// UIを表示
    /// </summary>
    public void Show()
    {
        //表示イベントが設定されていれば実行
        if(ShowEvent != null)
        {
            ShowEvent.Invoke();
            return;
        }
        //そうでなければそのまま表示
        root?.SetActive(true);
    }

    /// <summary>
    /// UIを非表示
    /// </summary>
    public void Hide()
    {
        //非表示イベントが設定されていれば実行
        if(HideEvent != null)
        {
            HideEvent.Invoke();
            return;
        }
        //そうでなければそのまま非表示
        root?.SetActive(false);
    }

    /// <summary>
    /// 表示が完了しているかを取得
    /// </summary>
    /// <returns></returns>
    public bool IsShowClear()
    {
        //イベントが設定されていればこのフラグを、設定されていなければ有効かを返却
        return m_isShowClear;
    }

    /// <summary>
    /// 非表示が完了しているか
    /// </summary>
    /// <returns></returns>
    public bool IsHideClear()
    {
        //イベントが設定されていればこのフラグを、設定されていなければ無効かを返却
        return m_isHideClear;
    }
}
