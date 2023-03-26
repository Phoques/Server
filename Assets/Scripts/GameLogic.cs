using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
  private static GameLogic _gameLogicInstance;
    public static GameLogic GameLogicInstance
    {
        //Property read is the instance 

        get => _gameLogicInstance;
        private set
        {
            //Property private write sets to instance to the value if null
            if (_gameLogicInstance == null)
            {
                _gameLogicInstance = value;
            }
            //Property private write sets instance to destroy other instance if instance already exists
            else if (_gameLogicInstance != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }
    public GameObject PlayerPrefab => playerPrefab;
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    private void Awake()
    {
        //sets the singleton to this 
        GameLogicInstance = this;
    }
}
