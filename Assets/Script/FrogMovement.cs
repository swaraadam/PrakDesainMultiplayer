using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{

    public float jumpBehaviour;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(jumpingFrog());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator jumpingFrog()
    {
        transform.Rotate(new Vector3(0, transform.rotation.y + (Random.Range(-55, 55)), 0));
        yield return new WaitForSeconds(2f);
        transform.GetComponent<Rigidbody>().AddForce(transform.forward * jumpBehaviour);
        transform.GetComponent<Rigidbody>().AddForce(transform.up * jumpBehaviour);
        StartCoroutine(jumpingFrog());
    }
}
