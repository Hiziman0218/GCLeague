using UnityEngine;
using UnityEngine.UI;

public class Timer : UIBase
{
    [Header("UI")]
    [Tooltip("残り回答時間")]
    [SerializeField] Text m_timer;

    private float m_time;

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        //小数点以下を切り捨てた数値を設定
        float currentTime = Mathf.Floor(m_time);
        //0以下の数値なら0に強制
        if (currentTime <= 0) currentTime = 0; 
        m_timer.text = $"{currentTime}";
    }

    public void SetTime(float time)
    {
        m_time = time;
    }
}
