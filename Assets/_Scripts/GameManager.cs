using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObj = new object();
    public static bool IsInitialized => instance != null;

    public static T Instance
    {
        get
        {
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public struct PlayerInfo
{
    public PlayerInfo(int InID, RaycastCarController InController)
    {
        id = InID;
        Controller = InController;
        RoundWon = 0;
    }

    public int id;
    public RaycastCarController Controller;
    public int RoundWon;
}

public class GameManager : Singleton<GameManager>
{
    [Header("Game Settings")]
    [SerializeField] private float CountDown;
    [SerializeField] private int MaxTurns;
    [SerializeField] private bool CountDownOn;

    [Header("Game References")]
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private List<RaycastCarController> PlayersDebug = new List<RaycastCarController>();
    private Dictionary<int, PlayerInfo> Players = new Dictionary<int, PlayerInfo>();


    void Update()
    {
        if (CountDownOn)
        {
            CountDown -= Time.deltaTime;
            if (CountDown <= 0)
            {
                StartGame();
            }
        }
    }

    void SetPlayersPosition()
    {
        foreach (var Player in Players)
        {
            Player.Value.Controller.transform.position = SpawnPoints[Player.Value.id].position;
        }
    }

    public int JoinLobby(RaycastCarController Controller)
    {
        int ID = Players.Count;
        print(ID);
        PlayersDebug.Add(Controller);
        Players.Add(ID, new PlayerInfo(ID, Controller));

        if (Players.Count > 1)
        {
            if (!CountDownOn)
            {
                CountDownOn = true;
            }
        }

        return ID;
    }

    public void OnRoundWon(int ID)
    {
        PlayerInfo playerInfo = Players[ID];
        playerInfo.RoundWon++;

        Players[ID] = playerInfo;

        foreach (var Player in Players)
        {
            if (Player.Value.RoundWon >= MaxTurns * 0.5f + 1)
            {
                SceneManager.LoadScene("StartGameLoading");
                return;
            }
        }

        SetPlayersPosition();
    }

    public void StartGame()
    {
        SetPlayersPosition();
    }
}
