using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetrimino : MonoBehaviour
{
    public GameObject[] Tetriminos;
    // Start is called before the first frame update
    void Start()
    {
        NewTetrimino();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewTetrimino()
    {
        Instantiate(Tetriminos[Random.Range(0, Tetriminos.Length)], transform.position, Quaternion.identity);
    }
}
