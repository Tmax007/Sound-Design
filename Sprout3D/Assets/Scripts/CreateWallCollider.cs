using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateWallCollider : MonoBehaviour
{
    Tilemap tm;

    public GameObject boxColliderPrefab;

    void Start()
    {
        tm = GetComponent<Tilemap>();

        foreach (var pos in tm.cellBounds.allPositionsWithin)
        {
            Vector3Int locationInGrid = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 worldLocation = tm.CellToWorld(locationInGrid) + new Vector3(tm.tileAnchor.x, 0, tm.tileAnchor.y + 0.5f);
            if (tm.HasTile(locationInGrid))
            {
                GameObject newCollider = Instantiate(boxColliderPrefab, worldLocation, Quaternion.identity);
            }
        }

        Debug.Log(tm.orientation.ToString());
        Debug.Log(tm.orientationMatrix.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
