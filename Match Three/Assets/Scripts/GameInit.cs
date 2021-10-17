using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    private Vector3 com1 = new Vector3(-11.8f, 1.4f, 4.8f);
    private Vector3 com2 = new Vector3(11.8f, 1.4f, 4.8f);
    public static ArrayList players = new ArrayList();
    // Start is called before the first frame update
    void Start()
    {
        InitGame();
        //Debug.Log(player);
    }

    public void InitGame()
    {
        foreach(GameObject item in players)
        {
            Destroy(item);
        }
        var player = Resources.Load("player") as GameObject;

        var dx = 1.5f;
        var dz = -1.5f;
        //Team 1
        for (var i = 0; i < 7; i++)
        {
            for (var j = 0; j < 7; j++)
            {
                var obj = Instantiate(player, com1 + new Vector3(dx * i, 0, dz * j), Quaternion.identity);
                obj.GetComponent<PlayerController>().team = PlayerController.Team.Player1;
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
                var obj = Instantiate(player, com2 + new Vector3(-dx * i, 0, dz * j), Quaternion.identity);
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
