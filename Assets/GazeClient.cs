using UnityEngine;

[RequireComponent(typeof(AgentController))]
public class GazeClient : MonoBehaviour {
    private GazeRequester _gazeRequester;


    AgentController _agentController;
    private void Start() {
        _gazeRequester = new GazeRequester();
        _gazeRequester.Start();

        _agentController = this.GetComponent<AgentController>();

    }


    //private void Update() {
    //    _gazeRequester.data = _agentController.lookAtTarget;
        
    //}

    //private void OnDestroy() {
    //    _gazeRequester.Stop();
    //}

    //public void SetData(string message) {

    //    _agentController.lookAtTarget = message[0];

    //}
}