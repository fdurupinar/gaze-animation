//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;


//static class Constants {
//    public const float timeStep = 0.0167f;
    
//}


//public enum pType {
//    O,
//    C,
//    E,
//    A,
//    N
//};

//public class AgentController : MonoBehaviour
//{

//    FaceScript faceController;
//    OVRLipSyncContext lipsyncContext;
//    public Transform rightEye;
//    public Transform leftEye;
    

//    public PupilController rightPC;
//    public PupilController leftPC;
    


//    public Transform lookAtTarget;
//    public Vector3 initialLookAtTargetPosition;

//    public float theta = 0f;

//   [Range(0, 1)]
//    public float lookAtIndX;
//    [Range(0, 1)]
//    public float lookAtIndY;
//    public const float maxViewingDist = 10f;

//    [Range(1, 3)]
//    public int[] personality = new int[5]; //only takes 1, 2, 3
//    static float gazeXAmp = 0.5f;
//    static float gazeYAmp = 0.1f;
//    List<Vector2> gazeCoordinates = new List<Vector2>();
//    List<int> eventArr = new List<int>();
//    List<Vector2> pupilArr = new List<Vector2>();

//    public int gazeCoordInd = 0;

//    public double timePassed = 0;

//    private void Awake() {
//        eventArr = new List<int>();
//    }

//    // Start is called before the first frame update
//    void Start() {
        

//        GameObject body = GetChildGameObject(gameObject, "Body");
//        GameObject body_default = GetChildGameObject(gameObject, "default");
//        GameObject body_eyelashes = GetChildGameObject(gameObject, "Eyelashes");

//        faceController = body.AddComponent<FaceScript>();
//        faceController.meshRenderer = body.GetComponentInChildren<SkinnedMeshRenderer>();
//        faceController.meshRendererEyes = body_default.GetComponent<SkinnedMeshRenderer>();
//        if(body_eyelashes)
//            faceController.meshRendererEyelashes = body_eyelashes.GetComponent<SkinnedMeshRenderer>();





//        //leftEye = GetChildGameObject(gameObject, "mixamorig:LeftEye").transform;
//        leftEye = GetChildGameObject(gameObject, "LeftEye").transform;

//        //rightEye = GetChildGameObject(gameObject, "mixamorig:RightEye").transform;
//        rightEye = GetChildGameObject(gameObject, "RightEye").transform;

//        if(!leftEye || !rightEye) {
//            Debug.Log("Eyes not found");
//            return;
//        }

//        leftPC = leftEye.GetComponentInChildren<PupilController>();
//        rightPC = rightEye.GetComponentInChildren<PupilController>();


//        if(!leftPC || !rightPC) {
//            Debug.Log("Pupil controllers not found");
//            return;
//        }


//        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
//        if(cameras == null) {
//            Debug.Log("Main Camera not found");
//            return;
//        }

        
//        if(!lookAtTarget) {
//            Debug.Log("Assign a look at target");
//            return;
//        }

//        lookAtTarget.transform.position = cameras[0].transform.position;        
//        initialLookAtTargetPosition = cameras[0].transform.position;
//        UpdatePersonalityCurves();

//        faceController.InitShapeKeys();

//        //faceController.StartTalking();



//        // make sure there is a phoneme context assigned to this object
//        lipsyncContext = GetComponent<OVRLipSyncContext>();
//        ReadGazeData();

        
        

//    }

//    // Update is called once per frame
//    void Update() {


//        //LookatTarget will be updated by the GazeClient
        

//        //UpdateGazeDirection(lookAtIndX, lookAtIndY);

//        if((lipsyncContext != null)) {
//            // get the current viseme frame
//            OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
//            if(frame != null) {
//                for(int i = 0; i < faceController.v.Length; i++) {
//                    faceController.v[i] = frame.Visemes[i];
//                }
//            }

//        }
//    }


//    private void FixedUpdate() {
//        //fixedDeltaTime = 0.0167 //60Hz

//        //TODO: blinking is on now
//        if(eventArr[gazeCoordInd] == 1) {

//            faceController.blinkInProgress = true;
//        }
//        //else

//        UpdateGazeDirection(gazeCoordinates[gazeCoordInd].x, gazeCoordinates[gazeCoordInd].y);

//        Vector3 negZ = new Vector3(1, 1, -1);
//        leftEye.LookAt(Vector3.Scale(lookAtTarget.position, negZ));
//        rightEye.LookAt(Vector3.Scale(lookAtTarget.position, negZ));


//        //constant comes from 4mm --> 0.016 in unity
//        //TODO: pupils are controlled now
//        rightPC.Radius = pupilArr[gazeCoordInd].x * 0.0003f;
//        leftPC.Radius = pupilArr[gazeCoordInd].y * 0.0003f;

//        gazeCoordInd++;
//        if(gazeCoordInd >= gazeCoordinates.Count)
//            gazeCoordInd = 0;
        
//    }

    


//    void ReadGazeData() {

//        //1. Determine number of lines to create an index array for valid and invalid data
//        //   This will be used for the blink and pupil size data
//        string file_ext = personality[0].ToString() + "_"
//           + personality[1].ToString() + "_"
//           + personality[2].ToString() + "_"
//           + personality[3].ToString() + "_"
//           + personality[4].ToString();

//        print(file_ext);


//        reader = new StreamReader(File.OpenRead("data/gaze_positions_" + file_ext + ".csv"));

//        reader.ReadLine(); //skip first line
//        int lineCnt = 0;        
//        while(!reader.EndOfStream) {
//            reader.ReadLine(); 
//            lineCnt++;
//        }


//        //2. Allocate data for the valid index array
//        //bool[] validIndArr = new bool[lineCnt]; //indices with invalid values will be discarded


//        ////3. Read position data which is not nan
//        //List<float> xArr = new List<float>();
//        //List<float> yArr = new List<float>();
//        //filename is equal to the personality values
       
//        float x, y;
//        int i = 0;
//        reader = new StreamReader(File.OpenRead("data/gaze_positions_"+ file_ext+ ".csv"));
//        reader.ReadLine(); //skip first line
//        while(!reader.EndOfStream) {
//            string line = reader.ReadLine();
//            string[] tokens = line.Split(',');
//            x = 0;
//            y = 0;
//            //if(!tokens[1].Equals("nan") && !tokens[2].Equals("nan")) {
//            //x = float.Parse(tokens[1]); //TODO: it was 1 and 2 before, because of timestep
//            //y = float.Parse(tokens[2]);

//            if(!tokens[0].Equals("nan") && !tokens[1].Equals("nan")) {
//                x = float.Parse(tokens[0]); //TODO: it was 1 and 2 before, because of timestep
//                y = float.Parse(tokens[1]);

//                //    if(x < -0.5 || x > 1.5 || y < -0.5 || y > 1.5)
//                //        validIndArr[i] = false;
//                //    else {

//                //        validIndArr[i] = true;
//                //    }



//                //}
//                //else
//                //    validIndArr[i] = false;


//                gazeCoordinates.Add(new Vector2(x, y));
//            }

//            i++;

//        }

//        //TODO: can be open or not
//        ////3. Fix invalid indices
//        //int prevValidInd = 0;
//        //i = 0;
//        //while( i < lineCnt) {
//        //    if(validIndArr[i]) {
//        //        prevValidInd = i;
//        //        i++;
//        //    }
//        //    else {
//        //        while(validIndArr[i] == false) {
//        //            i++;
//        //        }
//        //        //linearly interpolate
//        //        for(int j = prevValidInd+1; j < i; j++) {
//        //            gazeCoordinates[j] = gazeCoordinates[prevValidInd] + (gazeCoordinates[i] - gazeCoordinates[prevValidInd]) * (float)(j-prevValidInd) / (i - prevValidInd);
//        //        }



//        //    }



//        //}


//        ////4. Determine the boundaries for valid data.
//        //// Discard data outside 25th and 75th percentiles
//        //float minX = MathDefs.Percentile(xArr, 0.25f);
//        //float maxX = MathDefs.Percentile(xArr, 0.75f);

//        //float minY = MathDefs.Percentile(yArr, 0.25f);
//        //float maxY = MathDefs.Percentile(yArr, 0.75f);

//        //xArr.Clear();
//        //yArr.Clear();

//        //reader = new StreamReader(File.OpenRead("data/gaze_positions.csv"));
//        //reader.ReadLine(); //skip first line
//        //i = 0;
//        //while(!reader.EndOfStream) {
//        //    string line = reader.ReadLine();
//        //    string[] tokens = line.Split(',');

//        //    if(!tokens[1].Equals("nan") && !tokens[2].Equals("nan")) {
//        //        x = float.Parse(tokens[1]);
//        //        y = float.Parse(tokens[2]);

//        //        if(x >= minX && x <= maxX && y >= minY && y <= maxY) {
//        //            gazeCoordinates.Add(new Vector2(x, y));
//        //            xArr.Add(x);
//        //            yArr.Add(y);
//        //            validIndArr[i] = true;
//        //        }
//        //        else //now eliminate the ones outside this range as well
//        //            validIndArr[i] = false;

//        //    }
//        //    else
//        //        validIndArr[i] = false;

//        //    i++;
//        //}

//        //Debug.Log(gazeCoordinates.Count);




//        //Read blink data
//        reader = new StreamReader(File.OpenRead("data/events.csv"));
//        reader.ReadLine(); //skip first line

//        i = 0;
//        while(!reader.EndOfStream) {
//            string token = reader.ReadLine();
//            //if(validIndArr[i]) {
//                if(token == "Blink") {
//                    eventArr.Add(1); //indicating blink
//                }
//                else
//                    eventArr.Add(0); //indicating other
//            //}
//            //else
//            //    eventArr.Add(0); //indicating other

//            i++;
//        }
//        //Debug.Log(eventArr.Count);


        
//        //Read gaze data
//        reader = new StreamReader(File.OpenRead("data/pupil_diameter.csv"));
//        reader.ReadLine(); //skip first line
//        i = 0;
//        while(!reader.EndOfStream) {
//            string line = reader.ReadLine();
//            string[] tokens = line.Split(',');
            
//            //if(validIndArr[i]) {
//                x = 4f;
//                y = 4;
//                if(!tokens[0].Equals("nan")) //TODO: therse were  1 and 2 before
//                    x = float.Parse(tokens[0]);
//                if(!tokens[1].Equals("nan"))
//                    y = float.Parse(tokens[1]);

//                //x refers to right pupil dimension
//                //y refers to left pupil dimension
//                pupilArr.Add(new Vector2(x, y));
//            //}
//            i++;
//        }
//        //Debug.Log(pupilArr.Count);


//    }

//    void OnDrawGizmos() {

    

//        if(!leftEye || !rightEye)
//            return;
        
//        float dist = maxViewingDist;

//        Vector3[,] coords = new Vector3[2, 2];


//        Vector3 pos = (leftEye.position + rightEye.position)*0.5f; //take the middle point



//        float xDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 30f)) * dist; // somewhere bw 120-200
//        float yDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 23f)) * dist;


//        float minX = -xDist+ pos.x;
//        float maxX =  xDist + pos.x;
//        float minY = -yDist + pos.y;
//        float maxY = yDist + pos.y;



//        coords[0, 0] = new Vector3(minX, minY, pos.z + dist);
//        coords[1, 0] = new Vector3(maxX, minY, pos.z + dist);
//        coords[1, 1] = new Vector3(maxX, maxY, pos.z + dist);
//        coords[0, 1] = new Vector3(minX, maxY, pos.z + dist);


//        Gizmos.DrawLine(pos, coords[0, 0]);
//        Gizmos.DrawLine(pos, coords[0, 1]);
//        Gizmos.DrawLine(pos, coords[1, 0]);
//        Gizmos.DrawLine(pos, coords[1, 1]);
//        Gizmos.DrawLine(coords[0, 0], coords[0, 1]);
//        Gizmos.DrawLine(coords[0, 1], coords[1, 1]);
//        Gizmos.DrawLine(coords[1, 1], coords[1, 0]);
//        Gizmos.DrawLine(coords[1, 0], coords[0, 0]);

//        Gizmos.DrawSphere(new Vector3(lookAtTarget.position.x, lookAtTarget.position.y, lookAtTarget.position.z), 0.1f);


//    }

//    void UpdateGazeDirection(float x, float y) {
//        if(!leftEye || !rightEye)
//            return;

//        float dist = maxViewingDist;
        

//        Vector3 pos = (leftEye.position + rightEye.position) * 0.5f; //take the middle point

        
//        //https://imotions.com/hardware/smi-eye-tracking-glasses/
//        //SMI eye tracking glasses field of view, should be 60 and 46 for hor and ver
//        //It's supposed to be 80 and 60 in newer systems
//        float xDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 30f)) * dist; // somewhere bw 120-200
//        float yDist = Mathf.Abs(Mathf.Tan(Mathf.Deg2Rad * 23f)) * dist ;


//        //Changes between 0 and 1

//        float lookAtX = (2 * x - 1) * xDist  + pos.x;//x * 2 * xDist + minX;
//        float lookAtY = (2 * y - 1) * yDist + pos.y; //y * 2 * yDist + minY;
//        float lookAtZ = dist + pos.z;

//        lookAtTarget.position = new Vector3(lookAtX, lookAtY, lookAtZ);


//    }


//    GameObject GetChildGameObject(GameObject root, string name) {

//        Transform[] transforms = root.transform.GetComponentsInChildren<Transform>();
//        foreach(Transform t in transforms)
//               if(t.gameObject.name.ToUpper().Equals(name.ToUpper()))
//                return t.gameObject;
//        return null;
//    }


//    AnimationCurve  gazeCurve;    
//    float cumTime;
//    public bool isHorizontal; //if motion is vertical or horizontal
//    public void UpdatePersonalityCurves() {

//        Keyframe[] keys = new Keyframe[5];

//        float animLength = 5f;

//        float k4 = animLength + Random.Range(-animLength/8f, animLength / 8f) ;
//        float k0 = 0f;
//        float k1 = k4 / 4f + Random.Range(-animLength / 8f, animLength / 8f);
//        float k2 = k4 / 2f + Random.Range(-animLength / 8f, animLength / 8f);
//        float k3 = k4 * 3f/4f + Random.Range(-animLength / 8f, animLength / 8f);
        


//        //Randomize
//        //k1 = Random.Range(k0, k1 + k1 / 4f);
//        //k2 = Random.Range(k2 - k2 / 4f, k2 + k2 / 4f);
//        //k3 = Random.Range(k2, k3);
//        //k4 = Random.Range(k2, k3);


//        //k1 = Random.Range(k0, k1 + k1 / 4f);
//        //k2 = Random.Range(k2 - k2 / 4f, k2 + k2 / 4f);
//        //k3 = Random.Range(k2, k3);
//        //k4 = Random.Range(k3, k4);



//        if(personality[(int)pType.E] < 0) {
//            isHorizontal = false;

//            //float k1 = Random.Range(0f, 1f);
//            //float k2 = Random.Range(3f, 3.5f);
//            //float k3 = Random.Range(3.5f, 5f);

//            //float k0 = 0f;
//            //float k1 = animLength / 5f;
//            //float k2 = animLength - k1;
//            //float k3 = animLength;

            
            

//            keys[0] = new Keyframe(k0, 0);
//            keys[1] = new Keyframe(k1, -gazeYAmp);
//            keys[2] = new Keyframe(k2, -gazeYAmp);
//            keys[3] = new Keyframe(k3, -gazeYAmp);
//            keys[4] = new Keyframe(k4, 0);
            
//            gazeCurve = new AnimationCurve(keys);

//        }
//        else if(personality[(int)pType.E] >= 0) {
            
//            isHorizontal = (Random.Range(0f, 1f) >= 0.5) ? true : false;
//            float sign = (Random.Range(0f, 1f) >= 0.5f) ? -1f : 1f;
//            float amp;
//            if(isHorizontal)
//                amp = sign * gazeXAmp;
//            else
//                amp = sign * gazeYAmp;



//            //float k0 = 0f;
//            //float k1 = animLength / 5f;
//            //float k2 = animLength - k1;
//            //float k3 = animLength;

//            //float k1 = Random.Range(2f, 2.5f);            
//            //float k3 = Random.Range(3f, 5f);
//            //float k4 = Random.Range(4f, 5f); //length of animation

            

//            keys[0] = new Keyframe(0, 0);
//            keys[1] = new Keyframe(k1, 0);
//            keys[2] = new Keyframe(k2, amp);
//            keys[3] = new Keyframe(k3, 0);
//            keys[4] = new Keyframe(k4, 0);


//            gazeCurve = new AnimationCurve(keys);

//        }

//            gazeCurve.preWrapMode = WrapMode.PingPong;
//        gazeCurve.postWrapMode = WrapMode.PingPong;

//    }
    
//    public void UpdateGazeDirectionByCurves() {

//        if(Time.time > cumTime) { //update keyframes when motion is over
//            UpdatePersonalityCurves();
//            cumTime += gazeCurve.keys[gazeCurve.keys.Length-1].time;
//        }


//        float step = gazeCurve.Evaluate(Time.time);
//        if(isHorizontal)
//            lookAtTarget.position = new Vector3(initialLookAtTargetPosition.x + step, initialLookAtTargetPosition.y , initialLookAtTargetPosition.z);
//        else
//            lookAtTarget.position = new Vector3(initialLookAtTargetPosition.x, initialLookAtTargetPosition.y + step, initialLookAtTargetPosition.z);

        
//        leftEye.LookAt(lookAtTarget);
//        rightEye.LookAt(lookAtTarget);


//    }







//}
