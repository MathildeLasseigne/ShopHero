using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileParent : MonoBehaviour
{
    [SerializeField] private Tile tile;

    public Tile GetTile()
    {
        return tile;
    }
}
