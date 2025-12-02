using UnityEngine;

public class LocalMenuController : MonoBehaviour
{
    public GameObject menuPanel;
    private bool isOpen = false;

    void Start()
    {
        if (menuPanel == null)
        {
            // シーン内の "MenuPanel" を名前で探す
            menuPanel = GameObject.Find("MenuPanel");
        }
    }

    void Update()
    {
        //Escapeキーでメニューの開閉
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuPanel)
            {
                // シーン内の "MenuPanel" を名前で探す
                menuPanel = GameObject.Find("MenuPanel");
            }

            isOpen = !isOpen;
            menuPanel.SetActive(isOpen);
        }
    }
}
