namespace Game.Enum
{
    //ゲームの状態
    public enum GameState
    {
        GameStart,      //ゲーム開始時のゲーム内容表示
        Question,       //クイズの出題
        Thinking,       //プレイヤーの回答中
        Judging,        //回答判定
        Standby,        //待機
        CorrectAnswer,  //正解
        IncorrectAnswer,//不正解
        GameClear,      //ゲームクリア
        GameOver,       //ゲームオーバー
    }

    //クイズのタイプ(通常クイズか穴吹クイズ)
    public enum QuizType
    {
        Normal,
        Anabuki,
    }
}