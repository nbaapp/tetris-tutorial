using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public Grid gridObject;
    private bool isActive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        gridObject = GameObject.Find("Grid").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        if (Input.GetKeyDown("a"))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKeyDown("d"))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown("w"))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3 (0, 0, 1), 90);
            if (!ValidMove())
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }
        }


        if (Input.GetKey("s"))
        {
            if (Time.time - previousTime > fallTime / 10)
            {
                transform.position += new Vector3(0, -1, 0);
                previousTime = Time.time;
                if (!ValidMove())
                {
                    transform.position -= new Vector3(0, -1, 0);
                    AddToGrid();
                    CheckForLines();
                    this.enabled = false;
                    FindObjectOfType<SpawnTetrimino>().NewTetrimino();
                }
            }
        }
        else if (Time.time - previousTime > fallTime)
        {
            transform.position += new Vector3(0, -1, 0);
            previousTime = Time.time;
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();
                this.enabled = false;
                FindObjectOfType<SpawnTetrimino>().NewTetrimino();
            }
        }
    }

    void CheckForLines()
    {
        for (int i = Grid.height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {
        for(int j = 0; j < Grid.width; j++)
        {
           if (gridObject.grid[j, i] == null)
            {
                return false;
            }
        }

        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < Grid.width; j++)
        {
            GameObject parent = gridObject.grid[j, i].gameObject;
            parent.GetComponent<TetrisBlock>().HitWeakPoint(i, j);
            //Destroy(gridObject.grid[j, i].gameObject);
            gridObject.grid[j, i] = null;
        }
    }

    void HitWeakPoint(int i, int j)
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);
            if (j == roundedX && i == roundedY)
            {
                Destroy(child.gameObject);
            }
        }
        if (GetWeakPointCount() == 0)
        {
            Die();
        }
    }

    int GetWeakPointCount()
    {
        return transform.childCount;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void RowDown(int deletedRow)
    {
        Dictionary<GameObject, bool> parentsToDrop = new Dictionary<GameObject, bool>();
        for (int row = deletedRow + 1; row < Grid.height; row++)
        {
            for (int column = 0; column < Grid.width; column++)
            {
                gridObject.grid[column, row - 1] = gridObject.grid[column, row];
                gridObject.grid[column, row] = null;
                if (gridObject.grid[column, row - 1] != null)
                {
                    GameObject parent = gridObject.grid[column, row - 1].gameObject;
                    parentsToDrop[parent] = true;
                   // gridObject.grid[column, row - 1].transform.position += new Vector3(0, -1, 0);
                }
            }
        }
        foreach(GameObject blockToDrop in parentsToDrop.Keys)
        {
            blockToDrop.GetComponent<TetrisBlock>().RecoilImpact();
        }
    }

    void RecoilImpact()
    {
        gameObject.transform.position += new Vector3(0, -1, 0);

    }

    void AddToGrid()
    {
        foreach(Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            gridObject.grid[roundedX, roundedY] = transform;
        }
        isActive = false;
    }

    bool ValidMove()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= Grid.width || roundedY < 0 || roundedY >= Grid.height)
            {
                return false;
            }

            if(gridObject.grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }

        return true;
    }
}
