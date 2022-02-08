using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridLineRenderer
{
    public class GridLineRenderer : MonoBehaviour
    {
        public Vector3Int gridSize = new Vector3Int(5, 5, 5);
        public Vector3 gridSpacing = new Vector3(1.0f, 1.0f, 1.0f);
        public Vector3 gridPivot = new Vector3(0.5f, 0.5f, 0.5f);
        public Material lineMaterial;
        public float lineWidth = 0.02f;
        public bool disableXParallelLines = false;
        public bool disableYParallelLines = false;
        public bool disableZParallelLines = false;
        [SerializeField] bool autoUpdate = true;

        void Reset()
        {
            if (autoUpdate)
            {
                CreateGrid();
            }
        }

        public void DestroyGrid()
        {
            foreach (var child in transform.OfType<Transform>().ToArray())
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
#else
                Destroy(child.gameObject);
#endif
            }
        }

        public void CreateGrid()
        {
            if (gridSize.x < 0 || gridSize.y < 0 || gridSize.z < 0)
            {
                Debug.Log("GridLineRenderer: Grid size must be bigger than or equal to 0.");
                return;
            }

            if (lineWidth <= 0.0f)
            {
                Debug.Log("Line Width must be bigger than 0.");
                return;
            }

            DestroyGrid();

            var corner = new Vector3(
                -gridPivot.x * this.gridSize.x * gridSpacing.x,
                -gridPivot.y * this.gridSize.y * gridSpacing.y,
                -gridPivot.z * this.gridSize.z * gridSpacing.z
            );

            if (!disableXParallelLines)
            {
                var x0 = corner.x;
                var x1 = corner.x + gridSize.x * gridSpacing.x;
                for (var yi = 0; yi < gridSize.y + 1; ++yi)
                {
                    var y = corner.y + yi * gridSpacing.y;
                    for (var zi = 0; zi < gridSize.z + 1; ++zi)
                    {
                        var z = corner.z + zi * gridSpacing.z;
                        CreateLine(new Vector3(x0, y, z), new Vector3(x1, y, z), $"Line_y{yi}_z{zi}");
                    }
                }
            }
            if (!disableYParallelLines)
            {
                var y0 = corner.y;
                var y1 = corner.y + gridSize.y * gridSpacing.y;
                for (var zi = 0; zi <gridSize.z + 1; ++zi)
                {
                    var z = corner.z + zi * gridSpacing.z;
                    for (var xi = 0; xi < gridSize.x + 1; ++xi)
                    {
                        var x = corner.x + xi * gridSpacing.x;
                        CreateLine(new Vector3(x, y0, z), new Vector3(x, y1, z),  $"Line_z{zi}_x{xi}");
                    }
                }
            }
            if (!disableZParallelLines)
            {
                var z0 = corner.z;
                var z1 = corner.z + gridSize.z * gridSpacing.z;
                for (var xi = 0; xi < gridSize.x + 1; ++xi)
                {
                    var x = corner.x + xi * gridSpacing.x;
                    for (var yi = 0; yi < gridSize.y + 1; ++yi)
                    {
                        var y = corner.y + yi * gridSpacing.y;
                        CreateLine(new Vector3(x, y, z0), new Vector3(x, y, z1),  $"Line_x{xi}_y{yi}");
                    }
                }
            }
        }

        GameObject CreateLine(Vector3 p0, Vector3 p1, string name)
        {
            var line = new GameObject(name);
            line.transform.SetParent(transform, false);
            line.hideFlags = HideFlags.NotEditable;
            var lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.SetPositions(new Vector3[]{ p0, p1 });
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
            lineRenderer.useWorldSpace = false;
            return line;
        }
    }
}
