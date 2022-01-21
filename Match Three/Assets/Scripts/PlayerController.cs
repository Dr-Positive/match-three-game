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
    public bool Equals(MapPosition mapPosition)
    {
        return x == mapPosition.x && y == mapPosition.y;
    }

}

public class PlayerController : MonoBehaviour
{
    public enum Team {
        Player1,
        Player2,
        None,
    };
    public enum Fraction {
        Cavalryman,
        Halberdiers,
        Knight,
        Maceman,
        Spearman,
        None
    };
    //private float _speed = 0.05f;
    private Rigidbody _rb;
    private GameController _gameController;
    public MapPosition mapPosition;
    public PlaceController placeController;
    public Team team = Team.None;
    public Fraction fraction;
    public AudioClip Sound;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
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

    public void DoShot(PlayerController owner = null)
    {
        var bulletObj = Resources.Load("Bullet") as GameObject;
        var bulletController = bulletObj.GetComponent<BulletController>();
        bulletController.owner = owner?owner:this;
        Instantiate(bulletObj, this.transform.position + new Vector3(team == Team.Player1? 0.6f: -0.6f, 0.2f,0), Quaternion.identity);
    }

    public void SwithPlaceColor(Color color)
    {
        placeController.renderer.material.color = color;
    }

    void OnMouseDown()
    {
        //DoShot();
        if (_gameController.CurrentPlayer != (GameController.Players)team) return;
        //Debug.Log(_gameController);
        //Debug.Log(mapPosition.ToString());
        SwithPlaceColor(Color.blue);
        _gameController.DoSwith(this);
        _gameController.FindMatches();
    }

    public void Reset()
    {
        //GetComponent<AudioSource>().PlayOneShot(Sound);
        Debug.Log("123");
        Destroy(transform.gameObject);
        placeController.isEmpty = true;
        placeController.viewModel = null;
        SwithPlaceColor(Color.white);
    }

    public void moveTo(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    public bool CanKill(Fraction enemuFraction)
    {
        return fraction switch
        {
            Fraction.Cavalryman => enemuFraction == Fraction.Maceman || enemuFraction == Fraction.Knight,
            Fraction.Halberdiers => enemuFraction == Fraction.Cavalryman || enemuFraction == Fraction.Maceman,
            Fraction.Knight => enemuFraction == Fraction.Spearman || enemuFraction == Fraction.Halberdiers,
            Fraction.Maceman => enemuFraction == Fraction.Knight || enemuFraction == Fraction.Spearman,
            Fraction.Spearman => enemuFraction == Fraction.Halberdiers || enemuFraction == Fraction.Cavalryman,
            _ => false,
        };
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(1);
        var bulletController = collision.gameObject.GetComponent<BulletController>();
        if (!bulletController) { return; }
        else if (team == bulletController.owner.team)
        {
            Destroy(bulletController.gameObject);
            DoShot(bulletController.owner);
            //bulletController.GetComponent<Rigidbody>().isKinematic = true;
            return;
        }
        if (bulletController.owner.CanKill(fraction))
        {
            Reset();
        }
        else
        {
            Destroy(bulletController.gameObject);
            bulletController.owner.Reset();
        }
    }

}
