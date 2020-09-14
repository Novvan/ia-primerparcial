using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject menu;
    public GameObject missions;
    public GameObject win;
    public GameObject tip;
    public bool moveEnabled;
    private float _maxTipTime = 5f;
    private float _currentTime;
    private void Start()
    {
        moveEnabled = false;
        Cursor.visible = true;
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void PlayGameButton()
    {
        menu.SetActive(false);
        moveEnabled = true;
        Cursor.visible = false;
        Time.timeScale = 1;
        missions.SetActive(true);
    }
    public void PlayGameButtonFive()
    {
        menu.SetActive(false);
        moveEnabled = true;
        Cursor.visible = false;
        player.GetComponent<Player>().keys = 5;
        Time.timeScale = 1;
        missions.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void WinGame()
    {
        moveEnabled = false;
        Cursor.visible = true;
        Time.timeScale = 0;
        win.SetActive(true);
        missions.SetActive(false);
    }

    private void Update()
    {
        if (_currentTime >= _maxTipTime) tip.SetActive(false);
        else _currentTime += Time.deltaTime;
    }
}
