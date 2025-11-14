using UnityEngine;

[System.Serializable] //インスペクタで表示可能に設定
public class Quiz
{
    [Tooltip("問題文")]
    [SerializeField] private string m_questionText;
    [Tooltip("回答1")]
    [SerializeField] private string m_choice1;
    [Tooltip("回答2")]
    [SerializeField] private string m_choice2;
    [Tooltip("どちらが正解か(0か1か)")]
    [SerializeField] private int m_correctIndex;
    [Tooltip("難易度(1から5)")]
    [Range(1, 5)]
    [SerializeField] private int m_difficulty;
    [Tooltip("問題画像")]
    [SerializeField] private Sprite m_questionImage;
    [Tooltip("回答1画像")]
    [SerializeField] private Sprite m_choice1Image;
    [Tooltip("回答2画像")]
    [SerializeField] private Sprite m_choice2Image;

    public string QuestionText => m_questionText;
    public string Choice1 => m_choice1;
    public string Choice2 => m_choice2;
    public int CorrectIndex => m_correctIndex;
    public int Difficulty => m_difficulty;
    public Sprite QuestionImage => m_questionImage;
    public Sprite Choice1Image => m_choice1Image;
    public Sprite Choice2Image => m_choice2Image;
}
