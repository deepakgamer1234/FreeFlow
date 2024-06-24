
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LevelData[] levels;
    [SerializeField]
    private LevelData DefaultLevel;
    [SerializeField]
    private LevelList _allLevels;

    [HideInInspector]
    public int CurrentLevel;
    private Dictionary<string, LevelData> Levels;
    private const string MainMenu = "MainMenu";
    private const string Gameplay = "Gameplay";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            // DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            //  Destroy(gameObject);
        }
    }

    private void Init()
    {
        CurrentLevel = 1;

        Levels = new Dictionary<string, LevelData>();

        foreach (var item in _allLevels.Levels)
        {
            Levels[item.LevelName] = item;
        }
        print("qqqqqqqqqqqqqqqq" + Levels);
    }

    public LevelData GetLevel()
    {
        string levelName = "Level1" + CurrentLevel.ToString();

        print("levelllllllllll" + levelName);

        if (Levels.ContainsKey(levelName))
        {
            return Levels[levelName];
        }
        //print(Levels[levelName]);
        if (CurrentLevel >= 4)
        {
            GoToMainMenu();
        }
        return DefaultLevel;
    }

    public void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu);
    }

    public void GoToGameplay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Gameplay);
    }
}

