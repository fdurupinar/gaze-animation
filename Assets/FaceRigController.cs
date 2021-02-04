using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bone {
    public Transform transform;
    public Vector3 initPos;
    public Quaternion initRot;

    public Bone(Transform t) {
        transform = t;
        initPos = new Vector3(t.position.x, t.position.y, t.position.z);
        initRot = t.rotation;
        
    }
}

public enum BoneType {
    RightInnerBrow,
    LeftInnerBrow,
    RightOuterBrow,
    LeftOuterBrow,
    RightEyelidUpper,
    LeftEyelidUpper,
    RightEyelidLower,
    LeftEyelidLower,
    RightCheek,
    LeftCheek,
    RightEye,
    LeftEye,
    RightNostril,
    LeftNostril,
    RightLipUpper,
    LeftLipUpper,
    RightLipLower,
    LeftLipLower,
    RightLipCorner,
    LeftLipCorner,
    Jaw,
    JawEnd,
    TongueBack,
    TongueTip,
    Head,
    Neck,
    Neck1
    
};



public class FaceRigController : MonoBehaviour {

    public Transform Root;
    [SerializeField]
    public Dictionary<string, Bone> faceBones = new Dictionary<string, Bone>();
    
    public int actionUnitInd;

    private bool _eyesOpening = false;

    // Start is called before the first frame update
    void Start() {
        
        AssignBones(Root);
    }

    // Update is called once per frame
    void Update() {



        switch(actionUnitInd) {
            case 0:
                ResetAll(); 
                break;

            case 41:
                LidDroop(0.005f, 10f); 
                break;
            case 42: //SLIT
                Slit(10f);
                break;
            case 43: //EYES CLOSED
                EyesClosed(10f);
                break;
            case 44: 
                Squint(10f);
                break;
            case 45: //BLINK
                Blink(20f);
                break;
            case 61:
                EyesLeftRight(-30f, 50f); //left turn
                break;
            case 62:
                EyesLeftRight(30f, 50f); //left turn
                break;
            case 63:
                EyesUpDown(-30f, 50); //up turn
                break;
            case 64:
                EyesUpDown(30f, 50); //down turn
                break;
        }
        
    }

    public void SaveInitialTransforms() {

    }
    public void AssignBones(Transform bone) {

        if(!bone)
            return;

        string boneName = bone.name.ToUpper();


        faceBones[boneName] = new Bone(bone);
                

        foreach(Transform child in bone) {
            AssignBones(child);

        }


    }



    public bool LidDroop(float amount, float speed) {
        Transform leftEyelidUpper = faceBones["LEFTEYELIDUPPER"].transform;
        Transform rightEyelidUpper = faceBones["RIGHTEYELIDUPPER"].transform;

        Vector3 leftEyelidUpperTargetPos = faceBones["LEFTEYELIDUPPER"].initPos + Vector3.down * amount;
        Vector3 rightEyelidUpperTargetPos = faceBones["RIGHTEYELIDUPPER"].initPos + Vector3.down * amount;


        leftEyelidUpper.position = Vector3.Lerp(leftEyelidUpper.position, leftEyelidUpperTargetPos, Time.deltaTime * speed);
        rightEyelidUpper.position = Vector3.Lerp(rightEyelidUpper.position, rightEyelidUpperTargetPos, Time.deltaTime * speed);

        
        return Mathf.Abs((leftEyelidUpper.position - leftEyelidUpperTargetPos).magnitude) <  0.0001f;
           
    }
    public bool Slit(float speed) {
        return LidDroop(0.009f, 10f);
    }
    public bool EyesClosed(float speed) {
        return LidDroop(0.012f, 10f);
    }

    public bool Squint( float speed) {
        float amount = 0.008f;

        
        Transform leftEyelidLower = faceBones["LEFTEYELIDLOWER"].transform;
        Transform rightEyelidLower = faceBones["RIGHTEYELIDLOWER"].transform;


        Vector3 leftEyelidLowerTargetPos = faceBones["LEFTEYELIDLOWER"].initPos + Vector3.up * amount / 5f;
        Vector3 rightEyelidLowerTargetPos = faceBones["RIGHTEYELIDLOWER"].initPos + Vector3.up * amount / 5f;


        leftEyelidLower.position = Vector3.Lerp(leftEyelidLower.position, leftEyelidLowerTargetPos, Time.deltaTime * speed);
        rightEyelidLower.position = Vector3.Lerp(rightEyelidLower.position, rightEyelidLowerTargetPos, Time.deltaTime * speed);

        return LidDroop(amount, speed);

    }

    public void Blink(float speed) {
        
        if(!_eyesOpening) {
            bool eyesClosed = EyesClosed(speed);
            if(eyesClosed)
                _eyesOpening = true;
            
            
        }
        else { 
        
            //Eyes opening
            Transform leftEyelidUpper = faceBones["LEFTEYELIDUPPER"].transform;
            Transform rightEyelidUpper = faceBones["RIGHTEYELIDUPPER"].transform;

            leftEyelidUpper.position = Vector3.Lerp(leftEyelidUpper.position, faceBones["LEFTEYELIDUPPER"].initPos, Time.deltaTime * speed);
            rightEyelidUpper.position = Vector3.Lerp(rightEyelidUpper.position, faceBones["RIGHTEYELIDUPPER"].initPos, Time.deltaTime * speed);
            if(Mathf.Abs((leftEyelidUpper.position - faceBones["LEFTEYELIDUPPER"].initPos).magnitude) < 0.0001f)
                _eyesOpening = false;
        }

    }

    public void EyesLeftRight(float maxAngle, float speed) {

        Quaternion targetRotation = Quaternion.Euler(0, maxAngle, 0);
        Transform leftEye = faceBones["LEFTEYE"].transform;
        Transform rightEye = faceBones["RIGHTEYE"].transform;



        leftEye.rotation = Quaternion.RotateTowards(leftEye.rotation, targetRotation, Time.deltaTime * speed);
        rightEye.rotation = Quaternion.RotateTowards(rightEye.rotation, targetRotation, Time.deltaTime * speed);

    }


    public void EyesUpDown(float maxAngle, float speed) {

        Transform leftEye = faceBones["LEFTEYE"].transform;
        Transform rightEye = faceBones["RIGHTEYE"].transform;


        Quaternion targetRotationEye = Quaternion.Euler(maxAngle, 0, 0);

        leftEye.rotation = Quaternion.RotateTowards(leftEye.rotation, targetRotationEye, Time.deltaTime * speed);
        rightEye.rotation = Quaternion.RotateTowards(rightEye.rotation, targetRotationEye, Time.deltaTime * speed);


        LidDroop(0.008f, speed/3f);
    
    }



    public void ResetAll() {
        foreach(string boneName in faceBones.Keys) {
            faceBones[boneName].transform.position = faceBones[boneName].initPos;
            faceBones[boneName].transform.rotation = faceBones[boneName].initRot;
        }

    }


}
