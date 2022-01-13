using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceController : MonoBehaviour
{
    

    private Vector3 com1 = new Vector3(-11.7f, 1.6f, 4.7f);
    private Vector3 com2 = new Vector3(11.7f, 1.4f, 4.7f);
    private GameObject Player1;
    private GameObject Player2;
    private GameObject Player3;
    private List<GameObject> Models;
    private float dX = 1.5f;
    private float dZ = -1.5f;
    public int PlaceX;
    public int PlaceY;
    public PlayerController.Team PlaceTeam;
    public Renderer renderer;
    public GameObject viewModel;
    public bool isEmpty = true;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        Player1 = Resources.Load("player") as GameObject;
        Player2 = Resources.Load("player2") as GameObject;
        Player3 = Resources.Load("player3") as GameObject;
        Models = new List<GameObject>() { Player1, Player2, Player3 };
        RenderModel();

    }

    public Vector3 getPositionForModel(bool isFirstTeam) => (isFirstTeam ? com1 : com2) + new Vector3((isFirstTeam ? dX : -dX) * PlaceY, 0, dZ * PlaceX);

    public MapPosition GetCurrentMapPosition() => new MapPosition(PlaceX, PlaceY);

    public void RenderModel()
    {
        var index = UnityEngine.Random.Range(0, Models.Count);
        var model = Models[index];
        var dx = dX;
        var dz = dZ;
        var isFirstTeam = PlaceTeam == PlayerController.Team.Player1;
        //var originalPos = isFirstTeam ? com1 : com2;
        //if (!isFirstTeam) dx = -dx;
        var obj = Instantiate(model, getPositionForModel(isFirstTeam), Quaternion.identity);
        var playerController = obj.GetComponent<PlayerController>();
        playerController.team = PlaceTeam;
        playerController.mapPosition = GetCurrentMapPosition();
        playerController.placeController = this;
        playerController.fraction = (PlayerController.Fraction)index;
        viewModel = obj;
        if (isFirstTeam)
        {
            obj.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        }
        isEmpty = false;
    }

    private void OnMouseOver()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
