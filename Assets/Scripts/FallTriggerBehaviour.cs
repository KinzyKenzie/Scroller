using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTriggerBehaviour : MonoBehaviour
{
    private static bool playerDead = false;
    private static GameProgression script;

    // Start is called before the first frame update
    void Start() {
        if( script == null )
            FindGameWorldScript( transform.parent.parent.parent, out script );
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if( playerDead || !collision.transform.CompareTag( "Player" ) )
            return;

        playerDead = true;
        Debug.Log( "Player has hit a FallTrigger." );

        script.TriggerPlayerDeath();
    }

    Transform FindGameWorldScript( Transform tf, out GameProgression component ) {
        if( tf.TryGetComponent( out component ) )
            return tf;
        else
            return FindGameWorldScript( tf.parent, out component );
    }
}
