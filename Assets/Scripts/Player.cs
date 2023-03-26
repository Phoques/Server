using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class Player : MonoBehaviour
{
//dictionary of all players
 public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    
    public static void Spawn(ushort id, string username)
    {
        foreach(Player otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }
        Player player = Instantiate(GameLogic.GameLogicInstance.PlayerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<Player>();
        // the ? is like an if else statement, taking the first value, 'id' if true, or 'username' if false;
        player.name = $"Player{id}({(string.IsNullOrEmpty(username) ? $"Guest{id}" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest{id}" : username;

        player.SendSpawned();
        list.Add(id, player);
    }
    private void OnDestroy()
    {
        list.Remove(Id);
    }
  #region Messages
      [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    private void SendSpawned()
    {
        NetworkManager.NetworkManagerInstance.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable,(ushort)ServerToClientId.playerSpawned)));
    }
    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.NetworkManagerInstance.Server.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable,(ushort)ServerToClientId.playerSpawned)), toClientId);

    }
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);

        return message;
    }
  #endregion
}
