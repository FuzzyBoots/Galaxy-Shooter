using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static float rBound = 11.5f;
    [SerializeField]
    public static float lBound = -11.5f;
    [SerializeField]
    public static float uBound = 6f;
    [SerializeField]
    public static float dBound = -6f;

    [SerializeField]
    static Camera _camera;

    private static bool _gameOver = false;
    public static bool GameOver { get => _gameOver; set => _gameOver = value; }

    private void Start()
    {
        GenerateCollidersAcrossScreen();
    }

    void GenerateCollidersAcrossScreen()
    {
        Debug.Log("Making Colliders");
        Vector2[] colliderpoints;

        EdgeCollider2D upperEdge = new GameObject("upperEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = upperEdge.points;
        colliderpoints[0] = new Vector2(lBound, uBound);
        colliderpoints[1] = new Vector2(rBound, uBound);
        upperEdge.points = colliderpoints;

        EdgeCollider2D lowerEdge = new GameObject("lowerEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = lowerEdge.points;
        colliderpoints[0] = new Vector2(lBound, dBound);
        colliderpoints[1] = new Vector2(rBound, dBound);
        lowerEdge.points = colliderpoints;

        EdgeCollider2D leftEdge = new GameObject("leftEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = leftEdge.points;
        colliderpoints[0] = new Vector2(lBound, dBound);
        colliderpoints[1] = new Vector2(lBound, uBound);
        leftEdge.points = colliderpoints;

        EdgeCollider2D rightEdge = new GameObject("rightEdge").AddComponent<EdgeCollider2D>();

        colliderpoints = rightEdge.points;
        colliderpoints[0] = new Vector2(rBound, dBound);
        colliderpoints[1] = new Vector2(rBound, uBound);
        rightEdge.points = colliderpoints;
    }
}


