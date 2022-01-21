using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public PlayerController owner;
    private float _speed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyMe());
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position -= new Vector3(owner.team == PlayerController.Team.Player1 ? -_speed : _speed, 0, 0);
    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(10f);
        if(this.gameObject)
            Destroy(this.gameObject);
    }
}
