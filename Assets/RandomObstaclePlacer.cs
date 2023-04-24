using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObstaclePlacer : MonoBehaviour
{
  
    public int maxObstacles = 100;
    public Material defaultMaterial;
    Transform[] ChildTransforms;

    public void ResetAllObstacles()
    {
        ChildTransforms = GetComponentsInChildren<Transform>();

        if (ChildTransforms.Length == 1)
        {
            return;
        }

        for (int i = 0; i < ChildTransforms.Length; i++)
        {
            if (i == 0)
            {
                continue;
            }

            ChildTransforms[i].gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            ChildTransforms[i].transform.parent = null;
            ChildTransforms[i].gameObject.SetActive(false);
        }
     
    }
    public void GenerateRandomObstacleLayout(Vector3 objetivePosition)
    {
        int numObstalces = Random.Range(maxObstacles / 2, maxObstacles);

        for (int i = 0; i < numObstalces; i++)
        {
            Vector3 newPos = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(0, 5), transform.position.z + Random.Range(-10, 10));

            ///Safe Area
            if (Vector3.Distance(transform.position, newPos)<3f)
            {
                continue;
            }
            if (Vector3.Distance(objetivePosition, newPos) <3f)
            {
                continue;
            }

            GameObject obstacleGO = Pools.instance.GrabFromPool("Obstacle", newPos, Quaternion.identity);
            obstacleGO.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
            obstacleGO.transform.parent = transform;
        }
    }
}
