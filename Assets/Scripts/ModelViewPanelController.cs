using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModelViewPanelController : MonoBehaviour
{
    public RectTransform modelContainer;
    public float dragSensitivity;
    public float pinchSensitivity;
    public float resetSpeed;
    public float resetDoneMargin;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                modelContainer.transform.Rotate(touch.deltaPosition.y * dragSensitivity, -touch.deltaPosition.x * dragSensitivity, 0, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchA = Input.GetTouch(0);
            Touch touchB = Input.GetTouch(1);

            Vector2 touchAPrevPos = touchA.position - touchA.deltaPosition;
            Vector2 touchBPrevPos = touchB.position - touchB.deltaPosition;

            float prevTouchDeltaMag = (touchAPrevPos - touchBPrevPos).magnitude;
            float touchDeltaMag = (touchA.position - touchB.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            mainCamera.fieldOfView += deltaMagnitudeDiff * pinchSensitivity;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 10, 60);
        }
    }

    public void ResetObject()
    {
        StartCoroutine(Co_ResetObject());
    }

    public void QuickResetObject()
    {
        mainCamera.fieldOfView = 60;
        modelContainer.transform.rotation = Quaternion.identity;
    }

    private IEnumerator Co_ResetObject()
    {
        while(mainCamera.fieldOfView < 60 - resetDoneMargin || modelContainer.rotation.eulerAngles.magnitude > resetDoneMargin)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 60, Time.deltaTime * resetSpeed);
            modelContainer.transform.rotation = Quaternion.Lerp(modelContainer.transform.rotation, Quaternion.identity, Time.deltaTime * resetSpeed);

            yield return null;
        }
    }
}
