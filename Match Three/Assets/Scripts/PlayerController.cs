using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //private GameInit _gameInit;
    public Team team = Team.None;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;
        Vector3 offset = Vector3.zero;
        if(team == Team.Player1)
        {
            offset = new Vector3(1, 0, 0) * _speed;
        }
        //else if(team == Team.Player2)
        //{
        //    offset = new Vector3(-1, 0, 0) * _speed;

        //}
        transform.position += offset;
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
