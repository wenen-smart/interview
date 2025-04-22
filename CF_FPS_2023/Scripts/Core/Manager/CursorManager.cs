using UnityEngine;
public class CursorManager:MonoBehaviour
{
    public bool isLockCursor=true;
    public void Awake()
    {
        //不可见的
        CtrlCursorState(isLockCursor);
    }
    public void OnValidate()
    {
        if (isLockCursor)
        {
            CtrlCursorState(true);
        }
        else
        {
            CtrlCursorState(false);
        }
    }
    public void CtrlCursorState(bool enable)
    {
        if (enable)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus&&isLockCursor)
        {
            CtrlCursorState(true);
        }
    }
}
