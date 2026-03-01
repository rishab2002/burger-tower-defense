using Godot;
using System;
using System.Collections.Generic;

public partial class MeshInstance3d : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Godot.Collections.Array surfaceArray = [];
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        // C# arrays cannot be resized or expanded, so use Lists to create geometry.
        List<Vector3> verts = [];
        List<Vector2> uvs = [];
        List<Vector3> normals = [];
        List<int> indices = [];

        verts.AddRange(new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 24),
            new Vector3(20, 0, 0),
            new Vector3(20, 0, 24),
        });

        uvs.AddRange(new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(10, 0),
            new Vector2(0, 10),
            new Vector2(10, 10),
        });

        normals.AddRange(new Vector3[]
        {
            Vector3.Up,
            Vector3.Up,
            Vector3.Up,
            Vector3.Up,
        });

        indices.AddRange(new int[]
{
          0, 2, 1,
          2, 3, 1,
        });

        // Convert Lists to arrays and assign to surface array
        surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

        var arrMesh = Mesh as ArrayMesh;
        if (arrMesh != null)
        {
            // Create mesh surface from mesh array
            // No blendshapes, lods, or compression used.
            arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
