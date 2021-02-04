using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilController : MonoBehaviour
{

    Mesh mesh;
    Vector3[] originalVertices;
    

    bool [] isPupilBorder;

    [SerializeField]
    float radius;
    public float Radius   // property
     {
        get { return radius; }   // get method
        set { radius = value; }  // set method
    }
    // Start is called before the first frame update
    void Start() {

        mesh = GetComponent<MeshFilter>().sharedMesh;
        originalVertices = (Vector3[])mesh.vertices.Clone();//new Vector3[mesh.vertices.Length];

        //Assign pupil circumference vertices so that we will only update them

        isPupilBorder = new bool[originalVertices.Length];

        //find center of the iris
        Vector3 pCenter = Vector3.zero;
        float maxZ = -1000f;
        float minZ = 1000f;
        for(int i = 0; i < originalVertices.Length; i++) {
            //pCenter += originalVertices[i];
            if(originalVertices[i].z > maxZ)
                maxZ = originalVertices[i].z;

            if(originalVertices[i].z < minZ)
                minZ = originalVertices[i].z;
        }

        int cnt = 0;
        for(int i = 0; i < originalVertices.Length; i++) {

            if(Mathf.Abs(maxZ - originalVertices[i].z) < 0.000215) {
                pCenter += originalVertices[i];
                cnt++;
            }

        }

        pCenter /= cnt;


        for(int i = 0; i < originalVertices.Length; i++) {
            Vector3 dVec = originalVertices[i] - pCenter;
            float dMag = Vector3.Magnitude(dVec);

            if(dMag < 0.002f && Mathf.Abs(minZ - originalVertices[i].z) < 0.000215){ //close to the border //Vector3.Dot(dVec, Vector3.back) >= 0){//&& dMag > 0.0017f) { //if a pupil vertex
                isPupilBorder[i] = true;
            }
            else
                isPupilBorder[i] = false;
        }
    }   

    // void OnDrawGizmos() {

    //    Gizmos.color = Color.magenta;

    //    Vector3 pCenter = Vector3.zero;
    //    int circVerts = 0;
    //    for(int i = 0; i < originalVertices.Length; i++) {

    //        if(isPupilBorder[i]) {
    //            pCenter += originalVertices[i];
    //            Gizmos.DrawSphere(originalVertices[i], 0.001f);
    //        }
    //    }
    //    pCenter /= (float)circVerts;

    //    Gizmos.DrawSphere(pCenter, 10f);

    //}
    void ResetMesh() {
        mesh.vertices = originalVertices;
        mesh.RecalculateNormals();
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
     
        Vector3[] vertices = (Vector3[])originalVertices.Clone();

        //find center of the iris
        Vector3 pCenter = Vector3.zero;

        int circVerts = 0;
        for(int i = 0; i < vertices.Length; i++) {
            if(isPupilBorder[i]) {
                pCenter += vertices[i];
                circVerts++;
            }
        }
        pCenter /= (float)circVerts;
        
        
        for(int i = 0; i < vertices.Length; i++) {
            Vector3 dVec = vertices[i] - pCenter;
            float dMag = Vector3.Magnitude(dVec);

            if(isPupilBorder[i]) {
                vertices[i] = dVec.normalized * radius;
            }

            

            //if(dMag < radius) {
            //    //if(isOnCircumference[i]) {
            //    vertices[i] += dVec.normalized * radius;
            //    //vertices[i] += dVec.normalized * radius; //* Time.fixedDeltaTime;
            //    //vertices[i] += dVec * Time.fixedDeltaTime;

            //}
            //else if(isPupil[i]) {
            //    vertices[i] -= dVec.normalized * radius;
            //}

        }

            mesh.vertices = vertices;

        
        mesh.RecalculateNormals();

    }

    private void OnApplicationQuit() {
        ResetMesh();
    }
}
