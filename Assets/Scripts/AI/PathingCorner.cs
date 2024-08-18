using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.AI
{
    public class PathingCorner : MonoBehaviour
    {
        [SerializeField] private Vector3 _destinationArea = Vector3.one;

        public Vector3 GetDestination()
        {
            float spawnRangeX = _destinationArea.x / 2;
            float spawnRangeZ = _destinationArea.z / 2;
            Vector3 destination = new Vector3(transform.localPosition.x + Random.Range(-spawnRangeX, spawnRangeX), transform.localPosition.y, transform.localPosition.z + Random.Range(-spawnRangeZ, spawnRangeZ));
            return destination;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, _destinationArea);
        }
    }
}
