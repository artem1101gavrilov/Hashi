using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private Game game;
    [SerializeField] private GameObject Button2;
    [SerializeField] private GameObject Button3;

    private void Start()
    {
        game = GetComponent<Game>();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Game2()
    {
        Game.MaxLine = 2;
        game.CreateGame();
        CloseButtons();
    }

    public void Game3()
    {
        Game.MaxLine = 3;
        game.CreateGame();
        CloseButtons();
    }

    private void CloseButtons()
    {
        Button2.SetActive(false);
        Button3.SetActive(false);
    }
}
