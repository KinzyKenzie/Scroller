using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent( typeof( Camera ) )]
public class CameraViewportScaler : MonoBehaviour
{
    private static readonly Rect RAW_RECT = new() {
        x = 0,
        y = 0,
        width = 1,
        height = 1
    };

    private Camera _camera;

    [Tooltip( "Set the target aspect ratio." )]
    [SerializeField] private float _targetAspectRatio;

    private void Awake() {
        _camera = GetComponent<Camera>();

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
        float windowaspect = Screen.width / (float) Screen.height;

        if( Mathf.Abs( _targetAspectRatio - windowaspect ) > 0.01 ) {
            Debug.Log( "Current Aspect Ratio is " + windowaspect );

            Rect rect = _camera.rect;
            rect.y = 0.5f * (windowaspect - _targetAspectRatio);
            _camera.rect = rect;
        } else {
            _camera.rect = RAW_RECT;
        }
    }
}
