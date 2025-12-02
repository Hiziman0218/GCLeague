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
        Anabuki
    }

    //UIの種類
    public enum UIType
    {
        HUD,     //ゲーム中にずっと表示されるUI
        Timer,   //回答中の制限時間を表示するUI
        StartUI, //ゲーム開始時にゲームの設定を表示するUI
        QuizUI,  //クイズ内容を表示するUI
        CorrectUI,   //正解時に表示されるUI
        IncorrectUI, //不正解時に表示されるUI
    }
}