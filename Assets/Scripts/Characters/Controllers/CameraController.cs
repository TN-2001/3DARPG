using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] // 通常時シネマ
    private CinemachineVirtualCamera normalCinema = null;
    [SerializeField] // ロックオンシネマ
    private CinemachineVirtualCamera lockOnCinema = null;


    public void ResetNormalCamera(Transform transform)
    {
        CinemachinePOV pov = normalCinema.GetCinemachineComponent<CinemachinePOV>();
        pov.m_HorizontalAxis.Value = transform.eulerAngles.y;
        pov.m_VerticalAxis.Value = transform.eulerAngles.x;
    }
    public void ResetNormalCamera()
    {
        ResetNormalCamera(normalCinema.Follow);
    }

    public void IsMove(bool isMove)
    {
        if(isMove){
            normalCinema.GetComponent<CinemachineInputProvider>().enabled = true;
        }
        else{
            normalCinema.GetComponent<CinemachineInputProvider>().enabled = false;
        }
    }

    public void LockOn(Transform transform)
    {
        if(lockOnCinema.LookAt != transform){
            normalCinema.gameObject.SetActive(false);
            lockOnCinema.gameObject.SetActive(true);
            lockOnCinema.LookAt = transform;
        }
    }

    public void LockOff()
    {
        if(lockOnCinema.LookAt != lockOnCinema.Follow){
            ResetNormalCamera(Camera.main.transform);
            lockOnCinema.LookAt = lockOnCinema.Follow;
            normalCinema.gameObject.SetActive(true);
            lockOnCinema.gameObject.SetActive(false);
        }
    }
}
