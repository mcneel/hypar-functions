using Elements;
using Elements.Geometry;
using Rhino.FileIO;
using System.Collections.Generic;

namespace RhinoHead
{
    public static class RhinoHead
    {
        /// <summary>
        /// The RhinoHead function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A RhinoHeadOutputs instance containing computed results and the model with any new elements.</returns>
        public static RhinoHeadOutputs Execute(Dictionary<string, Model> inputModels, RhinoHeadInputs input)
        {
            var doc = File3dm.Read("hello_mesh.3dm");
            //var doc = File3dm.Read("/home/ec2-user/hypar_test/StarterFunction/models/hello_mesh.3dm");

            var meshes = new List<MeshElement>();

            foreach (var obj in doc.Objects)
            {
                if (!(obj.Geometry is Rhino.Geometry.Mesh m)) continue;

                m.Compact(); // remove any unreferenced vertices

                var mesh = new Mesh();

                foreach (var v in m.Vertices)
                {
                    var x = (double)v.X;
                    var y = (double)v.Y;
                    var z = (double)v.Z;
                    mesh.AddVertex(new Vector3(x, y, z));
                }

                foreach (var f in m.Faces)
                {
                    if (f.IsTriangle)
                    {
                        mesh.AddTriangle(mesh.Vertices[f.A], mesh.Vertices[f.B], mesh.Vertices[f.C]);
                    }
                    else
                    {
                        mesh.AddTriangle(mesh.Vertices[f.A], mesh.Vertices[f.B], mesh.Vertices[f.C]);
                        mesh.AddTriangle(mesh.Vertices[f.C], mesh.Vertices[f.D], mesh.Vertices[f.A]);
                    }
                }

                mesh.ComputeNormals();
                var shiny = new Material("shiny", Colors.Red, 1.0, 0.9);
                meshes.Add(new MeshElement(mesh, shiny));
            }

            var output = new RhinoHeadOutputs((double)doc.Objects.Count);

            output.Model.AddElements(meshes);
            return output;
        }
    }
}