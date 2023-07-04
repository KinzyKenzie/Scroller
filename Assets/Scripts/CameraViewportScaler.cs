using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent( typeof( Camera ) )]
public class CameraViewportScaler : MonoBehaviour
{
    enum CamState
    {
        Playing,
        MainMenu,
        Transition
    }

    private Camera _camera;

    [Tooltip( "Set the base camera size, assuming the aspect ratio is 1:1." )]
    [SerializeField] private float _baseCameraSize;

    private static readonly Vector3 DESIRED_POSITION = new( 0, 0, -10 );
    private static readonly float TRANSITION_SPEED = 0.6f;

    public bool TransitionFinished { get { return transitionProgress >= 0.5f; } }

    private CamState activeState = CamState.MainMenu;
    private Vector3 transitionOffset;
    private float aspectRatio;
    private float transitionProgress;

    void Start() {
        _camera = GetComponent<Camera>();
        aspectRatio = Screen.width / (float) Screen.height;
        transitionProgress = 0.0f;
        transitionOffset = new(
            DESIRED_POSITION.x - _camera.transform.position.x,
            DESIRED_POSITION.y - _camera.transform.position.y,
            0 );

        if( Application.isPlaying )
            ScaleViewport( 0.8f );
    }

    void Update() {
        switch( activeState ) {
            case CamState.Transition:
                transitionProgress += TRANSITION_SPEED * Time.deltaTime;

                if( transitionProgress < 1.0f ) {
                    _camera.transform.position += new Vector3(
                        TRANSITION_SPEED * Time.deltaTime * transitionOffset.x,
                        0,
                        0 );
                    ScaleViewport( 0.8f + ( 0.2f * transitionProgress ) );
                } else {
                    transitionProgress = 1.0f;
                    _camera.transform.position = DESIRED_POSITION;
                    ScaleViewport( 1.0f );
                    activeState = CamState.Playing;
                }
                break;
        }
    }

    private void ScaleViewport( float scale ) {
        Vector3 cameraPosition = _camera.transform.position;
        _camera.orthographicSize = scale * ( _baseCameraSize / aspectRatio );

        cameraPosition.y = _camera.orthographicSize - ( _baseCameraSize * 0.5f );
        _camera.transform.position = cameraPosition;
    }

    public void AdvanceCameraState() {
        switch( activeState ) {
            case CamState.MainMenu:
                activeState = CamState.Transition;
                break;
            case CamState.Transition:
                activeState = CamState.Playing;
                break;
            case CamState.Playing:
                activeState = CamState.MainMenu;
                break;
        }
    }
}
