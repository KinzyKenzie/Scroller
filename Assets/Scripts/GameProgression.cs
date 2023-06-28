using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameProgression : MonoBehaviour
{
    public GameObject Player;
    public GameObject ScoreLabelContainer;
    public Transform WorldContainer;
    public List<GameObject> ManagedScrollObjects = new();

    private static readonly int WORLD_SPEED_LIMIT = 18,
                                WORLD_SPEED_BASE = 6;
    private static readonly float SCORE_SCALAR = 1.0f / WORLD_SPEED_BASE;
    private static readonly List<TextureUVScroll> uvScrolls = new();
    private TextMeshProUGUI ScoreLabel;

    public float ScrollSpeed {
        get {
            return scrollSpeed;
        }
        set {
            ScrollSpeedChanged( value );
            scrollSpeed = value;
        }
    }
    private protected float scrollSpeed;
    private bool gameOver;
    private int scoreOut;
    private float scoreActual;

    // Start is called before the first frame update
    void Start() {
        scrollSpeed = 6.0f;
        gameOver = false;
        scoreOut = 0;
        scoreActual = 0;
        Player.GetComponent<Animator>().speed = scrollSpeed * 0.15f;
        ScoreLabel = ScoreLabelContainer.GetComponent<TextMeshProUGUI>();

        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 144;

        foreach( GameObject go in ManagedScrollObjects ) {
            uvScrolls.Add( go.GetComponent<TextureUVScroll>() );
        }
    }

    // Update is called once per frame
    void Update() {
        for( int i = WorldContainer.childCount - 1; i >= 0; i-- ) {
            Transform go = WorldContainer.GetChild( i );

            if( go.childCount == 0 ) {
                Destroy( go.gameObject );
                if( !gameOver && scrollSpeed < WORLD_SPEED_LIMIT )
                    ScrollSpeed += 0.5f;
                Debug.Log( "Scroll Speed: " + scrollSpeed );
            } else
                go.position += new Vector3( -ScrollSpeed * Time.deltaTime, 0, 0 );
        }

        scoreActual += SCORE_SCALAR * Time.deltaTime * ScrollSpeed;
        if( !gameOver && scoreOut != Mathf.FloorToInt( scoreActual ) ) {
            scoreOut = Mathf.FloorToInt( scoreActual );
            ScoreLabel.text = "" + scoreOut;
        }
    }

    void FixedUpdate() {
        if( gameOver )
            scrollSpeed = ScrollSpeed < 0.1f ? 0.0f : ScrollSpeed *= 0.9f;
    }

    private void ScrollSpeedChanged( float value ) {
        foreach( TextureUVScroll texture in uvScrolls ) {
            texture.ScrollSpeed = 0.2f * value * texture.BaseSpeed;
        }
        Player.GetComponent<Animator>().speed = value * 0.15f;
    }

    public void TriggerPlayerDeath() {
        gameOver = true;
        Player.GetComponent<PlayerBehaviour>().Die();
    }
}
