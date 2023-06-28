using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldExitBehaviour : MonoBehaviour
{
    public Collider2D Collider;

    private Queue<Rigidbody2D> colliding;

    // Start is called before the first frame update
    void Start() {
        colliding = new Queue<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if( colliding.Count > 0 &&
            !colliding.Peek().IsTouching( Collider ) )
            Destroy( colliding.Dequeue().gameObject );
    }

    private void OnTriggerEnter2D( Collider2D other ) {
        if( gameObject.layer != other.gameObject.layer )
            return;

        if( !colliding.Contains( other.attachedRigidbody ) ) {
            colliding.Enqueue( other.attachedRigidbody );
        } else
            Debug.Log( "Exit Trigger detected a collider that was already registered!" );
    }
}
