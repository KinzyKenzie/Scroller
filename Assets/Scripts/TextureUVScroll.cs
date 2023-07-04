using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureUVScroll : MonoBehaviour
{
    private static readonly float DISTANCE_LIMIT = 20.48f;

    public float ScrollSpeed = 1.0f;

    [HideInInspector]
    public float BaseSpeed;

    private Vector3 movePosition, globalScale;

    // Start is called before the first frame update
    void Start() {
        BaseSpeed = ScrollSpeed;
        movePosition = Vector3.zero;
        globalScale = transform.lossyScale;

        this.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        movePosition = transform.position;
        movePosition.x -= Time.deltaTime * ScrollSpeed;

        if( movePosition.x < -DISTANCE_LIMIT * globalScale.x )
            movePosition.x = -movePosition.x;

        transform.position = movePosition;
    }
}
