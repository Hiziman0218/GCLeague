using System;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] protected GameObject root; //このUIの親オブジェクト

    [Header("デバッグ用表示")]
    [SerializeField] private bool m_isReceptionShow = false; //表示の信号を受け取ったか
    [SerializeField] private bool m_isReceptionHide = false; //非表示の信号を受け取ったか
    [SerializeField] private bool m_isShowClear = false; //表示が完了したか
    [SerializeField] private bool m_isHideClear = false; //非表示が完了したか
    protected Action ShowEvent; //表示時のイベント
    protected Action HideEvent; //非表示時のイベント

    /// <summary>
    /// UIを表示
    /// </summary>
    public void Show()
    {
        //既に表示の信号を受け取っていたなら、以降の処理を行わない
        if (m_isReceptionShow) return;
        m_isReceptionShow = true;
        m_isShowClear = false;

        //表示イベントが設定されていれば実行
        if(ShowEvent != null)
        {
            ShowEvent.Invoke();
            return;
        }
        //そうでなければそのまま表示
        root?.SetActive(true);
        ShowClear();
    }

    /// <summary>
    /// UIを非表示
    /// </summary>
    public void Hide()
    {
        //既に非表示の信号を受け取っていたなら、以降の処理を行わない
        if(m_isReceptionHide) return;
        m_isReceptionHide = true;
        m_isHideClear = false;

        //非表示イベントが設定されていれば実行
        if(HideEvent != null)
        {
            HideEvent.Invoke();
            return;
        }

        //そうでなければそのまま非表示
        root?.SetActive(false);
        HideClear();
    }

    /// <summary>
    /// 表示完了
    /// </summary>
    protected void ShowClear()
    {
        m_isReceptionShow = false;
        m_isShowClear = true;
        m_isHideClear = false;
    }

    /// <summary>
    /// 非表示完了
    /// </summary>
    protected void HideClear()
    {
        m_isReceptionHide = false;
        m_isShowClear = false;
        m_isHideClear = true;
    }

    /// <summary>
    /// 表示が完了しているかを取得
    /// </summary>
    /// <returns></returns>
    public bool IsShowClear()
    {
        //イベントが設定されていればこのフラグを、設定されていなければ有効かを返却
        if (ShowEvent != null) return m_isShowClear;
        else return gameObject.activeInHierarchy;
    }

    /// <summary>
    /// 非表示が完了しているか
    /// </summary>
    /// <returns></returns>
    public bool IsHideClear()
    {
        //イベントが設定されていればこのフラグを、設定されていなければ無効かを返却
        if(HideEvent != null) return m_isHideClear;
        else return !gameObject.activeInHierarchy;
    }
}
