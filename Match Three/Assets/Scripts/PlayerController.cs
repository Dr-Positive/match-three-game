using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition
{
    public int x;
    public int y;

    public MapPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool canSwith(MapPosition position)
    {
        var offsetX = Mathf.Abs(x - position.x);
        var offsetY = Mathf.Abs(y - position.y);
        if ((offsetX == 1 && offsetY == 0) || (offsetX == 0 && offsetY == 1)) return true;
        return false;

    }

    public override string ToString()
    {
        return @$"(x:{x} y:{y})";
    }

}

public class PlayerController : MonoBehaviour
{
    public enum Team {
        None,
        Player1,
        Player2
    }
    private bool _isDead = false;
    private float _speed = 0.05f;
    private Rigidbody _rb;
    private GameInit _gameInit;
    public MapPosition mapPosition;
    //private GameInit _gameInit;
    public Team team = Team.None;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _gameInit = GameObject.Find("Floor").GetComponent<GameInit>();
        //Debug.Log(_gameInit);

    }

    // Update is called once per frame
    void Update()
    {
        //if (_isDead) return;
        //Vector3 offset = Vector3.zero;
        //if(team == Team.Player1)
        //{
        //    offset = new Vector3(1, 0, 0) * _speed;
        //}
        ////else if(team == Team.Player2)
        ////{
        ////    offset = new Vector3(-1, 0, 0) * _speed;

        ////}
        //transform.position += offset;
    }

    void OnMouseDown()
    {
        //Debug.Log();
        Debug.Log(mapPosition.ToString());
        _gameInit.doWhith(this);
    }

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.name == "KillZone") 
        {
            Destroy(transform.gameObject);
            GameInit.players.Remove(transform.gameObject);
            return;
        }
        if (team == Team.Player2) return;
        //Debug.Log(collision.gameObject.name);
        var player2 = collision.gameObject.GetComponent<PlayerController>();
        if (player2)
        {
            var i = Random.Range(0, 2);
            Debug.Log(i);
            //var isKill =  i == 0;
            if (i == 0)
            {
                Destroy(collision.gameObject);
                GameInit.players.Remove(collision.gameObject);

            }
            else if(i == 1)
            {
                Destroy(transform.gameObject);
                GameInit.players.Remove(transform.gameObject);

            }
            //else
            //{
            //    Destroy(collision.gameObject);
            //    Destroy(transform.gameObject);
            //    GameInit.players.Remove(collision.gameObject);
            //    GameInit.players.Remove(transform.gameObject);
            //}

        }
    }

}
