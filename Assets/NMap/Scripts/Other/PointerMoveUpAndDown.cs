using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerMoveUpAndDown : MonoBehaviour
{
    // Make the pointer move up and down
    GameObject self;
    public float speed = 1.0f;
    public float distance = 1.0f;
    private Vector2 startPosition;
    private float time;
    void Start()
    {
        self = this.gameObject;
        startPosition = self.transform.position;
        time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speed;
        float y = Mathf.Sin(time) * distance;
        self.transform.position = new Vector2(startPosition.x, startPosition.y + y);
    }

}
