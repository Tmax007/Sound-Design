using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateGroundCollider : MonoBehaviour
{
    Tilemap tm;

    public GameObject boxColliderPrefab;

    void Start()
    {
        tm = GetComponent<Tilemap>();

        foreach (var pos in tm.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tm.CellToWorld(localPlace) + new Vector3(tm.tileAnchor.x, 0, tm.tileAnchor.y);
            if (tm.HasTile(localPlace))
            {
                GameObject newCollider = Instantiate(boxColliderPrefab, place, Quaternion.identity);
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
