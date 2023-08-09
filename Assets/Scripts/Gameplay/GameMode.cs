using UnityEngine;

[CreateAssetMenu(fileName = "new GameMode", menuName = "GameMode")]
public class GameMode : ScriptableObject, IGameMode
{
    public int minPlayers;
    public int maxPlayers;
    public int MinPlayers { get => minPlayers; set => minPlayers = value; }
    public int MaxPlayers { get => maxPlayers; set => maxPlayers = value; }
}

public interface IGameMode
{
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
}