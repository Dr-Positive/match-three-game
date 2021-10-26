using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    private Vector3 com1 = new Vector3(-11.8f, 1.4f, 4.8f);
    private Vector3 com2 = new Vector3(11.8f, 1.4f, 4.8f);
    public static ArrayList players = new ArrayList();
    public PlayerController selectedPlayer;
    // Start is called before the first frame update
    void Start()
    {
        InitGame();
        //Debug.Log(player);
    }

    public void doWhith(PlayerController player)
    {
        if (!selectedPlayer)
        {
            selectedPlayer = player;
        }
        else
        {
            if (!selectedPlayer.mapPosition.canSwith(player.mapPosition))
            {
                selectedPlayer = null;
                return;
            }
            var pos1 = selectedPlayer.GetComponent<Transform>().position;
            var pos2 = player.GetComponent<Transform>().position;
            var mapPos1 = selectedPlayer.mapPosition;
            var mapPos2 = player.mapPosition;
            player.GetComponent<Transform>().position = pos1;
            selectedPlayer.GetComponent<Transform>().position = pos2;
            player.mapPosition = mapPos1;
            selectedPlayer.mapPosition = mapPos2;
            //Debug.Log(pos1);
            //Debug.Log(pos2);
            selectedPlayer = null;
        }
    }

    public void InitGame()
    {
        foreach(GameObject item in players)
        {
            Destroy(item);
        }
        var player = Resources.Load("player") as GameObject;
        //var player2 = Resources.Load("player2") as GameObject;
        var player3 = Resources.Load("player3") as GameObject;
        var models = new List<GameObject>() { player, player3 };
        var dx = 1.5f;
        var dz = -1.5f;
        //Team 1
        for (var i = 0; i < 7; i++)
        {
            for (var j = 0; j < 7; j++)
            {
                var index = Random.Range(0, models.Count);
                Debug.Log(index);
                var model = models[index];
                var obj = Instantiate(model, com1 + new Vector3(dx * i, 0, dz * j), Quaternion.identity);
                obj.GetComponent<PlayerController>().team = PlayerController.Team.Player1;
                obj.GetComponent<PlayerController>().mapPosition = new MapPosition(j, i);
                obj.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                players.Add(obj);
                //Rigidbody rb = obj.AddComponent<Rigidbody>();
                //obj.AddComponent<BoxCollider>();
                //obj.AddComponent<PlayerController>();
            }
        }

        //Team 2
        for (var i = 0; i < 7; i++)
        {
            for (var j = 0; j < 7; j++)
            {
                var index = Random.Range(0, models.Count);
                var model = models[index];
                var obj = Instantiate(model, com2 + new Vector3(-dx * i, 0, dz * j), Quaternion.identity);
                obj.GetComponent<PlayerController>().team = PlayerController.Team.Player2;
                players.Add(obj);
                //obj.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
                //Rigidbody rb = obj.AddComponent<Rigidbody>();
                //obj.AddComponent<BoxCollider>();
                //obj.AddComponent<PlayerController>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    { 

    }
}
