using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI; // Required when Using UI elements.

static class Constants {
    public const float timeStep = 0.0167f;
    
}


public enum pType {
    O,
    C,
    E,
    A,
    N
};

public class AgentController : MonoBehaviour
{

    FaceScript faceController;
    //OVRLipSyncContext lipsyncContext;
    public Transform rightEye;
    public Transform leftEye;
    

    public PupilController rightPC;
    public PupilController leftPC;
    


    public Transform lookAtTarget;
    

    public float theta = 0f;

   [Range(0, 1)]
    public float lookAtIndX;
    [Range(0, 1)]
    public float lookAtIndY;
    public const float maxViewingDist = 50;

    [Range(1, 3)]
    public int[] personality = new int[5]; //only takes 1, 2, 3    
    List<Vector2> gazeCoordinates = new List<Vector2>();
    List<int> eventArr = new List<int>();
    List<Vector2> pupilArr = new List<Vector2>();

    public int gazeCoordInd = 0;

    public double timePassed = 0;


    public Slider sliderO;
    public Slider sliderC;
    public Slider sliderE;
    public Slider sliderA;
    public Slider sliderN;


    bool enableBlinks = false;
    public float blinkRate;
    public float blinkVar;
    public float blinkMean;
    public float blinkMin;
    public float blinkMax;



    // Start is called before the first frame update
    void Start() {




        blinkRate = 0.7f;
        blinkVar = 0.012f;
        blinkMean = 0.5f;

        blinkMin = 0.06f;
        blinkMax = 4f;

        GameObject body = GetChildGameObject(gameObject, "Body");
        GameObject body_default = GetChildGameObject(gameObject, "default");
        GameObject body_eyelashes = GetChildGameObject(gameObject, "Eyelashes");

        

        faceController = body.AddComponent<FaceScript>();
        faceController.meshRenderer = body.GetComponentInChildren<SkinnedMeshRenderer>();
        faceController.meshRendererEyes = body_default.GetComponent<SkinnedMeshRenderer>();
        if(body_eyelashes)
            faceController.meshRendererEyelashes = body_eyelashes.GetComponent<SkinnedMeshRenderer>();
        faceController.Init();


        //leftEye = GetChildGameObject(gameObject, "mixamorig:LeftEye").transform;
        leftEye = GetChildGameObject(gameObject, "LeftEye").transform;

        //rightEye = GetChildGameObject(gameObject, "mixamorig:RightEye").transform;
        rightEye = GetChildGameObject(gameObject, "RightEye").transform;

        if(!leftEye || !rightEye) {
            Debug.Log("Eyes not found");
            return;
        }

        leftPC = leftEye.GetComponentInChildren<PupilController>();
        rightPC = rightEye.GetComponentInChildren<PupilController>();


        if(!leftPC || !rightPC) {
            Debug.Log("Pupil controllers not found");
            return;
        }


        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        if(cameras == null) {
            Debug.Log("Main Camera not found");
            return;
        }

        
        if(!lookAtTarget) {
            Debug.Log("Assign a look at target");
            return;
        }

        lookAtTarget.transform.position = cameras[0].transform.position;        


        faceController.InitShapeKeys();

        //faceController.StartTalking();

        for(int i = 0; i < 5; i++)
            personality[i] = 2;

        // make sure there is a phoneme context assigned to this object
        //lipsyncContext = GetComponent<OVRLipSyncContext>();
        ReadGazeData();

    }

    // Update is called once per frame
    void Update() {

        

        //UpdateGazeDirection(lookAtIndX, lookAtIndY);

        //if((lipsyncContext != null)) {
        //    // get the current viseme frame
        //    OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
        //    if(frame != null) {
        //        for(int i = 0; i < faceController.v.Length; i++) {
        //            faceController.v[i] = frame.Visemes[i];
        //        }
        //    }

        //}
    }

    
    private void FixedUpdate() {
        //fixedDeltaTime = 0.0167 //60Hz

        if(enableBlinks) {
            //TODO ac
            //variance is too high between max and min so for now just assign mean value
            float blinkDur = blinkMean;

            faceController.blinkCloseSpeed = 2f / blinkDur;
            faceController.blinkOpenSpeed = faceController.blinkCloseSpeed;


            float prob = blinkRate / 60f;

            if(!faceController.blinkInProgress && MathDefs.GetRandomNumber(0, 1) <= prob) {
                faceController.blinkInProgress = true;
                //float blinkDur = MathDefs.GaussianDist(blinkMean, blinkVar);
                //if(blinkDur < blinkMin)
                //    blinkDur = blinkMin;
                //if(blinkDur > blinkMax)
                //    blinkDur = blinkMax;



                faceController.blinkCloseSpeed = 2f / blinkDur;
                faceController.blinkOpenSpeed = faceController.blinkCloseSpeed;
                faceController.blinkInProgress = true;

                //Debug.Log(gazeCoordInd);
            }

        }

        // do this based on blink rate and blink duration
        // compute probability at each frame:
        //
        ////TODO AC
        //if(eventArr[gazeCoordInd] == 1) {

        //    //Debug.Log(gazeCoordInd);
        //    faceController.blinkInProgress = true;
        //}

        //TODO: ac
        UpdateGazeDirection(gazeCoordinates[gazeCoordInd].x, gazeCoordinates[gazeCoordInd].y);


        //We do this because of the eyeball model's default orientation
        leftEye.LookAt(new Vector3(-lookAtTarget.position.x, -lookAtTarget.position.y, -lookAtTarget.position.z));
        rightEye.LookAt(new Vector3(-lookAtTarget.position.x, -lookAtTarget.position.y, -lookAtTarget.position.z));



        faceController.UpdateEyelids(gazeCoordinates[gazeCoordInd].y);

        //}


        //constant comes from 4mm --> 0.0016 in unity
        //TODO: ac
        //rightPC.Radius = pupilArr[gazeCoordInd].x* 0.0004f;
        //leftPC.Radius = pupilArr[gazeCoordInd].y* 0.0004f;

        gazeCoordInd++;
        if(gazeCoordInd >= gazeCoordinates.Count)
            gazeCoordInd = 0;
        
    }


    void ReadGazeData() {


        //TODO: test a single personality dimension for now
        string p_str = "N";

        if(personality[(int)pType.N] != 2)
            p_str = "N";
        else if(personality[(int)pType.E] != 2)
            p_str = "E";
        else if(personality[(int)pType.O] != 2)
            p_str = "O";
        else if(personality[(int)pType.A] != 2)
            p_str = "A";
        else if(personality[(int)pType.C] != 2)
            p_str = "C";

        string p_type = "dim_" + p_str;

        string file_ext = personality[(int)pType.N].ToString() + "_"
           + personality[(int)pType.E].ToString() + "_"
           + personality[(int)pType.O].ToString() + "_"
           + personality[(int)pType.A].ToString() + "_"
           + personality[(int)pType.C].ToString() + "_" + p_type;


        Debug.Log(file_ext);


        float x, y;
        gazeCoordinates = new List<Vector2>();
        var reader = new StreamReader(File.OpenRead("data/gaze_positions_" + file_ext + ".csv"));

        reader.ReadLine(); //skip first line
        while(!reader.EndOfStream) {
            string line = reader.ReadLine();
            string[] tokens = line.Split(',');


            x = float.Parse(tokens[0]); 
            y = float.Parse(tokens[1]);

            gazeCoordinates.Add(new Vector2(x, y));


        }



        //TODO: open this for reading blinks from the file
        //Read blink data
        //reader = new StreamReader(File.OpenRead("data/events_" + file_ext + ".csv"));

        ////for now
        ////reader = new StreamReader(File.OpenRead("data/events_2_2_3_2_2_dim_O.csv"));

        //reader.ReadLine(); //skip first line

        //eventArr = new List<int>();
        //while(!reader.EndOfStream) {
        //    string token = reader.ReadLine();
        //    //if(validIndArr[i]) {
        //    if(token == "Blink") {
        //        eventArr.Add(1);
                
        //    }
        //    else
        //        eventArr.Add(0); //indicating other

        //}


        //Read pupil data
        pupilArr = new List<Vector2>();
        reader = new StreamReader(File.OpenRead("data/pupil_diameter_" + file_ext + ".csv"));

        reader.ReadLine(); //skip first line        
        while(!reader.EndOfStream) {
            string line = reader.ReadLine();
            string[] tokens = line.Split(',');

            //if(validIndArr[i]) {
            x = 4f;
            y = 4;
            if(!tokens[0].Equals("nan")) //TODO: therse were  1 and 2 before
                x = float.Parse(tokens[0]);
            if(!tokens[1].Equals("nan"))
                y = float.Parse(tokens[1]);

            //x refers to right pupil dimension
            //y refers to left pupil dimension
            pupilArr.Add(new Vector2(x, y));
            //}

        }


        ////Read blink parameters
        //reader = new StreamReader(File.OpenRead("data/blink_features/blink_values_" + file_ext + ".csv"));

        ////order is mean, var, min, max, rate
        //while(!reader.EndOfStream) {
        //    string line = reader.ReadLine();
        //    string[] tokens = line.Split(',');

        //    blinkMean = float.Parse(tokens[0]);
        //    blinkVar = float.Parse(tokens[1]);
        //    blinkMin = float.Parse(tokens[2]);
        //    blinkMax = float.Parse(tokens[3]);
        //    blinkRate = float.Parse(tokens[4]);

        //}
    }
        void OnDrawGizmos() {

    

        if(!leftEye || !rightEye)
            return;
        
        float dist = maxViewingDist;

        Vector3[,] coords = new Vector3[2, 2];


        Vector3 pos = (leftEye.position + rightEye.position)*0.5f; //take the middle point



        float xDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 30f)) * dist; // somewhere bw 120-200
        float yDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 23f)) * dist;


        float minX = -xDist+ pos.x;
        float maxX =  xDist + pos.x;
        float minY = -yDist + pos.y;
        float maxY = yDist + pos.y;



        coords[0, 0] = new Vector3(minX, minY, pos.z + dist);
        coords[1, 0] = new Vector3(maxX, minY, pos.z + dist);
        coords[1, 1] = new Vector3(maxX, maxY, pos.z + dist);
        coords[0, 1] = new Vector3(minX, maxY, pos.z + dist);


        Gizmos.DrawLine(pos, coords[0, 0]);
        Gizmos.DrawLine(pos, coords[0, 1]);
        Gizmos.DrawLine(pos, coords[1, 0]);
        Gizmos.DrawLine(pos, coords[1, 1]);
        Gizmos.DrawLine(coords[0, 0], coords[0, 1]);
        Gizmos.DrawLine(coords[0, 1], coords[1, 1]);
        Gizmos.DrawLine(coords[1, 1], coords[1, 0]);
        Gizmos.DrawLine(coords[1, 0], coords[0, 0]);

        Gizmos.DrawSphere(new Vector3(lookAtTarget.position.x, lookAtTarget.position.y, lookAtTarget.position.z), 0.1f);


        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, lookAtTarget.position);
    }

    void UpdateGazeDirection(float x, float y) {
        if(!leftEye || !rightEye)
            return;

        float dist = maxViewingDist;
        

        Vector3 pos = (leftEye.position + rightEye.position) * 0.5f; //take the middle point

        
        //https://imotions.com/hardware/smi-eye-tracking-glasses/
        //SMI eye tracking glasses field of view, should be 60 and 46 for hor and ver
        //It's supposed to be 80 and 60 in newer systems
        float xDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 30f)) * dist;
        float yDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 23f)) * dist;


        //Changes between 0 and 1

        float lookAtX = (2 * x - 1) * xDist  + pos.x;
        float lookAtY = (2 * y - 1) * yDist + pos.y; 
        float lookAtZ = dist + pos.z;

        
        lookAtTarget.position = new Vector3(lookAtX, lookAtY, lookAtZ);



    }


    GameObject GetChildGameObject(GameObject root, string name) {

        Transform[] transforms = root.transform.GetComponentsInChildren<Transform>();
        foreach(Transform t in transforms)
               if(t.gameObject.name.ToUpper().Equals(name.ToUpper()))
                return t.gameObject;
        return null;
    }


    public void Reset() {

        

        if(!sliderO || !sliderC || !sliderE || !sliderA || !sliderN) {
            Debug.Log("Sliders not assigned");
            return;
        }
        personality[(int)pType.O] = (int)sliderO.value;
        personality[(int)pType.C] = (int)sliderC.value;
        personality[(int)pType.E] = (int)sliderE.value;
        personality[(int)pType.A] = (int)sliderA.value;
        personality[(int)pType.N] = (int)sliderN.value;

        

        ReadGazeData();



        faceController.InitShapeKeys();

    }


   
}
