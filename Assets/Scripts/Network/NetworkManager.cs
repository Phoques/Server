using Riptide; // The Riptide DLL, and XML needs to be added to the Unity project to allow these usings. (The package does all this)
using Riptide.Utils;
using UnityEngine;

//All of these scripts are what the package does automatically. (Mostly)
public enum ServerToClientId : ushort
{
    playerSpawned = 1,
}

public enum ClientToServerId: ushort
{
    name = 1,
}
public class NetworkManager : MonoBehaviour
{
    
    // We want to make sure there is only one instance of our network manager. 
    //We are creating a private static instance of our NetworkManager and a public static Property to control connection.
    
    private static NetworkManager _networkManagerInstance;
    public static NetworkManager NetworkManagerInstance 
    {
        //Property Read is the instance 
        get => _networkManagerInstance;
        //Property Write
        private set
        {
            //if we dont have an instance then set the instance to this
            if (_networkManagerInstance == null)
            {
                _networkManagerInstance = value;
            }
            //else if we do have a  network manager and the instance we have isnt us
            else if (_networkManagerInstance != value)
            {
                //Destroy the network manager that isnt the instance
                // the $ allows you to put variables within curly brackets to then post in the debug, else it just is wanting a string. or just ())
                //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }
    public Server Server { get; private set; }
    //ushort is a smaller int that starts at 0, cannot be in the negative (see website for more details regarding longs shorts ushorts ulongs https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/)
    //port number aka something like 8080
    [SerializeField] private ushort s_port;
    //how many players we can connect aka how manay peeps can join 
    [SerializeField] private ushort s_maxClientCount;
    //this s_ seems to ber personal, could mean short, could mean server?
    private void Awake()
    {
        //when object that this script is on set instance to this 
        NetworkManagerInstance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        //Logs what the network is doing
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
         //creates a new server
        Server = new Server();
        //starts the server at port x with x amount of max clients
        Server.Start(s_port, s_maxClientCount);
        //When a client leaves the server run the playerleft function
        Server.ClientDisconnected += PlayerLeft;
    }
      // Checking server activity at set intervals 
    private void FixedUpdate()
    {
        Server.Update();
    }
    // When the game closes it kills connection to the server 
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
       private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        //When a player leaves a server Destroy the player object and remove from list
        Destroy(Player.list[e.Client.Id].gameObject);
    } 
}
