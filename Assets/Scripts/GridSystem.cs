using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private LayerMask _mask;
    private Vector3 pos = Vector3.zero;
    private void Update()
    {
        pos = transform.position;
        pos.y -= 1f;
        if(Physics.CheckSphere(pos, radius, _mask))
        {
            Debug.Log("deneme");
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pos, radius);
    }
}
