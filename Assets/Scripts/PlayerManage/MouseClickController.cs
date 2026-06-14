using UnityEngine;
using UnityEngine.Events;

public class MouseClickController : MonoBehaviour
{
    public Vector3 clickPosition;
    public UnityEvent<Vector3> OnClick;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                Vector3 clickWorldPosition = hitInfo.point;
                Debug.Log(clickWorldPosition);

                clickPosition = clickWorldPosition;

                OnClick.Invoke(clickPosition);
            }
        }

        if (clickPosition != Vector3.zero)
        {
            DebugExtension.DebugWireSphere(clickPosition, Color.yellow, 0.6f);
        }
    }
}