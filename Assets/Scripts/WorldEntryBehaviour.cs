using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntryBehaviour : MonoBehaviour
{
    public Collider2D Collider;
    public GameObject WorldContainer;
    public List<GameObject> PrefabsSideLeft,
        PrefabsSideRight,
        PrefabsSlice,
        PrefabsObstacle;

    private static readonly float[] SLICE_SPAWN_CHANCE = new float[] {
        0.25f,
        0.325f,
        0.1f,
        0.325f
    };

    public int MinimumSliceCount { set => slicesMin = value; }
    public int MaximumSliceCount { set => slicesMax = value; }

    public enum WaitStage
    {
        PostDelay,  // = 0
        Colliding,  // = 1
        PreDelay,   // = 2
        Draw        // = 3
    }

    private WaitStage waitStage = WaitStage.PostDelay;

    private static readonly float DELAY_DRAW = 0.2f;
    private static readonly float SPRITE_SIZE = 2.56f;
    private static readonly Vector3 SPRITE_SIZE_OFFSET = new( SPRITE_SIZE, 0, 0 );

    private int slicesMin, slicesMax;
    private float waitTimeStart;
    //private float debugTimeStart;

    // Start is called before the first frame update
    void Start() {
        slicesMin = 2;
        slicesMax = 14;
        waitTimeStart = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch( waitStage ) {
            case WaitStage.PostDelay:
                if( Collider.IsTouchingLayers() )
                    waitStage++;
                break;
            case WaitStage.Colliding:
                if( !Collider.IsTouchingLayers() ) {
                    waitTimeStart = Time.realtimeSinceStartup;
                    //debugTimeStart = Time.realtimeSinceStartup;
                    waitStage++;
                }
                break;
            case WaitStage.PreDelay:
                if( waitTimeStart + DELAY_DRAW < Time.realtimeSinceStartup ) {
                    waitTimeStart = 0;
                    waitStage++;
                }
                break;
            case WaitStage.Draw:
                Transform tf = new GameObject( "Chunk" ).transform;
                tf.position = transform.position;
                tf.parent = WorldContainer.transform;

                GenerateChunk( tf );

                waitTimeStart = Time.realtimeSinceStartup;
                waitStage++;
                break;
        }

        if( waitStage > WaitStage.Draw )
            waitStage = WaitStage.PostDelay;
    }

    private void GenerateChunk( Transform parent ) {
        int length = Random.Range( slicesMin, 1 + slicesMax );
        int[] type = new int[ length ];
        int rocksSpawned = 0;
        float rand;

        //TODO: Improve this. This is disgusting.
        for( int i = 0; i < length; i++ ) {
            rand = Random.Range( 0, 1.0f );

            if( rand < SLICE_SPAWN_CHANCE[ 0 ] )
                type[ i ] = 0;
            else if( rand < SLICE_SPAWN_CHANCE[ 0 ] + SLICE_SPAWN_CHANCE[ 1 ] )
                type[ i ] = 1;
            else if( rand < SLICE_SPAWN_CHANCE[ 0 ] + SLICE_SPAWN_CHANCE[ 1 ] + SLICE_SPAWN_CHANCE[ 2 ] )
                type[ i ] = 2;
            else
                type[ i ] = 3;

            if( i > 0 && type[ i ] == type[ i - 1 ] )
                i--;
        }

        Vector3 position = parent.position + new Vector3( SPRITE_SIZE * 0.5f, 0, 0 );
        GameObject previous = Instantiate( PrefabsSideLeft[ Random.Range( 0, PrefabsSideLeft.Count ) ], parent );
        previous.transform.position = position;

        for( int i = 0; i < length; i++ ) {
            position += SPRITE_SIZE_OFFSET;
            previous = Instantiate( PrefabsSlice[ type[ i ] ], parent );
            previous.transform.position = position;

            float rand2 = Random.Range( 0, 1.0f );

            // Disgusting 2.0
            if( rand2 <= 0.1f && rocksSpawned < 1 && i > 4 && i < length - 2 ) {
                GameObject tempgo = Instantiate( PrefabsObstacle[ 0 ], parent );
                tempgo.transform.position = position + new Vector3( 0, SPRITE_SIZE, 0.1f );
                rocksSpawned++;
            }
            if( rand2 <= 0.4f ) {
                Vector3 myScale = previous.transform.localScale;
                myScale.x *= -1;
                previous.transform.localScale = myScale;
            }
        }

        position += SPRITE_SIZE_OFFSET;
        previous = Instantiate( PrefabsSideRight[ Random.Range( 0, PrefabsSideRight.Count ) ], parent );
        previous.transform.position = position;
    }
}
