using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    private bool canGetHit = true;

    [Header("Assign")]
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerBullet" && canGetHit)
        {
            StartCoroutine(Damage(other.gameObject));
        }
    }

    IEnumerator Damage(GameObject obj)
    {
        canGetHit = false;
        health -= 1;
        Destroy(obj.gameObject);
        if (health <= 0)
        {
            //explosion
            Instantiate(explosion,obj.transform.position,Quaternion.identity);

            Destroy(gameObject);
            yield break;
        }

        yield return new WaitForSeconds(0.2f);
        canGetHit = true;
    }
}
