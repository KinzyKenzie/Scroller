using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Rigidbody2D Physics;
    public Collider2D LeftFoot,
                      RightFoot;

    //private static readonly float GRAVITY_FALL_SPEED = 2.0f / 3;
    //private static readonly float FLOAT_REDUCTION = 0.995f;
    private static readonly float VELOCITY_SWITCH_VALUE = 0.195f,
                                  PLAYER_JUMP_STRENGTH = 10,
                                  PLAYER_HEIGHT = 0.4f;

    [HideInInspector]
    public bool playerJumpDisableOverride;

    public enum JumpState
    {
        Waiting,    // Awaiting player input. Ready to jump!
        Jumping     // Y-position is increasing
    }

    private JumpState jumpState = JumpState.Waiting;
    private List<RaycastHit2D> rcHits;

    // Start is called before the first frame update
    void Start() {
        playerJumpDisableOverride = false;
        rcHits = new List<RaycastHit2D>();
    }

    // Update is called once per frame
    void Update() {

        switch( jumpState ) {
            case JumpState.Waiting:
                if( Input.GetKey( KeyCode.Space ) && !playerJumpDisableOverride && IsGrounded() ) {
                    Physics.velocity = PLAYER_JUMP_STRENGTH * Vector2.up;
                    jumpState = JumpState.Jumping;
                }
                break;
            case JumpState.Jumping:
                if( Physics.velocity.y < VELOCITY_SWITCH_VALUE ) {
                    jumpState = JumpState.Waiting;
                }
                break;
        }

        if( transform.position.y < -8.0f ) {
            Die();
            Physics.simulated = false;
            Physics.velocity = Vector2.zero;
            transform.position = new Vector3( 0, -7, 0 );
        }
    }

    private bool IsGrounded() {
        bool response = false;

        rcHits.AddRange( Physics2D.RaycastAll( ( (Vector2) transform.position ) + ( transform.lossyScale * LeftFoot.offset ), Vector2.down, 2.5f, 1 << LayerMask.NameToLayer( "World" ) ) );
        rcHits.AddRange( Physics2D.RaycastAll( ( (Vector2) transform.position ) + ( transform.lossyScale * RightFoot.offset ), Vector2.down, 2.5f, 1 << LayerMask.NameToLayer( "World" ) ) );

        foreach( RaycastHit2D hit in rcHits ) {
            if( hit.distance < PLAYER_HEIGHT )
                response = true;
        }

        rcHits.Clear();
        return response;
    }

    public void Die() {
        playerJumpDisableOverride = true;
        Physics.constraints = RigidbodyConstraints2D.FreezeRotation;
        Physics.velocity += Vector2.right;

        Debug.Log( "Player has died." );
    }
}
