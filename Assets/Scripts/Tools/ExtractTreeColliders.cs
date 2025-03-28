using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class ExtractTreeColliders : MonoBehaviour {
    [SerializeField] private Terrain terrain;

    private void Reset() {
        terrain = GetComponent<Terrain>();
        Extract();
    }

    [ContextMenu("Extract")]
    public void Extract() {
        for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++) {
            TreePrototype tree = terrain.terrainData.treePrototypes[i];
            // Get all instances matching the prefab index
            TreeInstance[] instances = terrain.terrainData.treeInstances
                .Where(x => x.prototypeIndex == i).ToArray();

            for (int j = 0; j < instances.Length; j++) {
                // Un-normalize positions so they're in world-space
                Vector3 worldPosition = Vector3.Scale(instances[j].position, terrain.terrainData.size) + terrain.GetPosition();

                // Create the collider object at the correct world position
                GameObject obj = Instantiate(tree.prefab);
                obj.transform.position = worldPosition; // Set the object's position to the worldPosition

                // // Fetch the collider from the prefab object parent
                // CapsuleCollider prefabCollider = tree.prefab.GetComponent<CapsuleCollider>();
                // if (!prefabCollider) continue;

                obj.name = tree.prefab.name + j;
                // CapsuleCollider objCollider = obj.AddComponent<CapsuleCollider>();
                // objCollider.center = prefabCollider.center;
                // objCollider.height = prefabCollider.height;
                // objCollider.radius = prefabCollider.radius;

                if (terrain.preserveTreePrototypeLayers)
                    obj.layer = tree.prefab.layer;
                else
                    obj.layer = terrain.gameObject.layer;
            }
        }
    }
}

