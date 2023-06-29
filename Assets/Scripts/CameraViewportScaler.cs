using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent( typeof( Camera ) )]
public class CameraViewportScaler : MonoBehaviour
{
    private Camera _camera;

    [Tooltip( "Set the base camera size, assuming the aspect ratio is 1:1." )]
    [SerializeField] private float _baseCameraSize;

    private float previousRatio;

    private void Awake() {
        _camera = GetComponent<Camera>();
        previousRatio = 1.0f;

        if( Application.isPlaying )
            ScaleViewport();
    }

    private void Update() {
#if UNITY_EDITOR
        if( _camera )
            ScaleViewport();
#endif
    }

    private void ScaleViewport() {
        float ratio = Screen.width / (float) Screen.height;
        if( ratio != previousRatio ) {

            Vector3 cameraPosition = _camera.transform.position;
            _camera.orthographicSize = _baseCameraSize / ratio;

            cameraPosition.y = _camera.orthographicSize - 5.0f;
            _camera.transform.position = cameraPosition;
        }
        previousRatio = ratio;
    }
}
