using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneController : MonoBehaviour
{
    public string tagToIgnore = "Character Collider";
    public AudioClip collisionSound;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagToIgnore))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

        if (collision.collider.CompareTag("TerrainGround"))
        {
            AudioSource.PlayClipAtPoint(collisionSound, transform.position);
        }
    }
}
