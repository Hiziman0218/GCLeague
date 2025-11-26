using UnityEngine;
using UnityEngine.UI;

public class Timer : UIBase
{
    [Header("UI")]
    [Tooltip("c‚è‰ñ“šŠÔ")]
    [SerializeField] Text m_timer;

    private float m_time;

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        //¬”“_ˆÈ‰º‚ğØ‚èÌ‚Ä‚½”’l‚ğİ’è
        m_timer.text = $"{Mathf.Floor(m_time/* - 1f*/)}";
    }

    public void SetTime(float time)
    {
        m_time = time;
    }
}
