using GMTK_Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundary : MonoBehaviour
{
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public bool IsWithinBoundary(Vector3 pos)
    {
        return pos.x > _renderer.bounds.min.x && pos.x < _renderer.bounds.max.x &&
               pos.z > _renderer.bounds.min.z && pos.z < _renderer.bounds.max.z;
    }
}
