using UnityEngine;

public class FlowController : MonoBehaviour
{
    public void QuitApplication()
    {
        Debug.Log("[FlowController] Quitting application...");
        Application.Quit();
    }
}
