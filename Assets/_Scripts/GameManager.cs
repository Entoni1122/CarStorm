using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

struct PlayerInfo
{
    public PlayerInfo(ref int InID, ref RaycastCarController InController)
    {
        id = InID;
        Controller = InController;
        RoundWon = 0;
    }

    public int id;
    public RaycastCarController Controller;
    public int RoundWon;
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float CountDown;
    [SerializeField] private int MaxTurns;
    [SerializeField] private bool CountDownOn;
    public static GameManager Self;

    [Header("Game References")]
    [SerializeField] private List<Transform> SpawnPoints;
    [SerializeField] private List<RaycastCarController> PlayersDebug = new List<RaycastCarController>();
    private Dictionary<int, PlayerInfo> Players = new Dictionary<int, PlayerInfo>();


    void Start()
    {
        if (Self == null)
        {
            Self = this;
        }
    }

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
        int ID = Players.Keys.Count;
        PlayersDebug.Add(Controller);
        Players.Add(Players.Keys.Count, new PlayerInfo(ref ID, ref Controller));

        if (Players.Keys.Count > 1) 
        {
            if (!CountDownOn)
            {
                CountDownOn = true;
            }
        }

        return Players.Keys.Count - 1;
    }

    public void OnRoundWon(int ID)
    {
        PlayerInfo PlayerInfo = Players[ID];
        PlayerInfo.RoundWon++;

        Players[ID] = PlayerInfo;

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
