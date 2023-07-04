using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameProgression : MonoBehaviour
{
    public enum PlayState
    {
        Waiting,
        Transition,
        Running,
        GameOver
    }

    public GameObject Camera;
    public GameObject Player;
    public GameObject ScoreLabelContainer;
    public Transform WorldContainer;
    public List<GameObject> ManagedScrollObjects = new();

    [NonSerialized]
    public PlayState _playState = PlayState.Waiting;

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
    private bool playerIsDead;
    private int scoreOut;
    private float scoreActual;

    // Start is called before the first frame update
    void Start() {
        scrollSpeed = 6.0f;
        playerIsDead = false;
        scoreOut = 0;
        scoreActual = 0;
        ScoreLabel = ScoreLabelContainer.GetComponent<TextMeshProUGUI>();

        //TODO: Replace with idle pose
        Player.GetComponent<Animator>().enabled = false;

        foreach( GameObject go in ManagedScrollObjects ) {
            uvScrolls.Add( go.GetComponent<TextureUVScroll>() );
        }
    }

    // Update is called once per frame
    void Update() {
        switch( _playState ) {
            case PlayState.Running:
                Progress();
                break;
            case PlayState.Waiting:
                if( SceneManager.sceneCount == 1 ) {
                    _playState = PlayState.Transition;
                    Camera.GetComponent<CameraViewportScaler>().AdvanceCameraState();
                }
                break;
            case PlayState.Transition:
                if( Camera.GetComponent<CameraViewportScaler>().TransitionFinished ) {
                    StartPlaying();
                }
                break;
        }
    }

    void StartPlaying() {
        _playState = PlayState.Running;
        Player.GetComponent<Animator>().enabled = true;
        ScrollSpeedChanged( scrollSpeed );
    }

    void Progress() {
        for( int i = WorldContainer.childCount - 1; i >= 0; i-- ) {
            Transform go = WorldContainer.GetChild( i );

            if( go.childCount == 0 ) {
                Destroy( go.gameObject );
                if( !playerIsDead && scrollSpeed < WORLD_SPEED_LIMIT )
                    ScrollSpeed += 0.5f;
            } else
                go.position += new Vector3( -ScrollSpeed * Time.deltaTime, 0, 0 );
        }

        scoreActual += SCORE_SCALAR * Time.deltaTime * ScrollSpeed;
        if( !playerIsDead && scoreOut != Mathf.FloorToInt( scoreActual ) ) {
            scoreOut = Mathf.FloorToInt( scoreActual );
            ScoreLabel.text = "" + scoreOut;
        }
    }

    void FixedUpdate() {
        if( playerIsDead )
            scrollSpeed = ScrollSpeed < 0.1f ? 0.0f : ScrollSpeed *= 0.9f;
    }

    private void ScrollSpeedChanged( float value ) {
        foreach( TextureUVScroll texture in uvScrolls ) {
            texture.enabled = true;
            texture.ScrollSpeed = 0.2f * value * texture.BaseSpeed;
        }
        Player.GetComponent<Animator>().speed = value * 0.15f;
    }

    public void TriggerPlayerDeath() {
        playerIsDead = true;
        Player.GetComponent<PlayerBehaviour>().Die();
    }
}
