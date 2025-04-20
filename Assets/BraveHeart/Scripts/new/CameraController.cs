using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   
    public Transform target = null;

   
    [Range(0, 1)]
    public float linearSpeed = 1;
   
    [Range(2, 10)]
    public float distanceFromTarget = 5;
  
    [Range(1, 50)]
    public float speed = 5;
  
    public float xOffset = 0.5f;


    private float yMouse;
    private float xMouse;

    // Start is called before the first frame update
    void Start()
    {
        if (target != null)
        {
            gameObject.layer = target.gameObject.layer = 2;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void LateUpdate()
    {
        if (target != null)
        {
            xMouse += Input.GetAxis("Mouse X") * linearSpeed;
            yMouse -= Input.GetAxis("Mouse Y") * linearSpeed;
            yMouse = Mathf.Clamp(yMouse, -30, 80);

            distanceFromTarget -= Input.GetAxis("Mouse ScrollWheel") * 10;
            distanceFromTarget = Mathf.Clamp(distanceFromTarget, 2, 15);
           
            Quaternion targetRotation = Quaternion.Euler(yMouse, xMouse, 0);
         
            CamCheck(out RaycastHit hit, out float dis);
            Vector3 targetPostion = target.position + targetRotation * new Vector3(xOffset, 0, -dis) + target.GetComponent<CapsuleCollider>().center * 1.75f;
          
            speed = target.GetComponent<Rigidbody>().velocity.magnitude > 0.1f ?
   Mathf.Lerp(speed, 7, 5f * Time.deltaTime) : Mathf.Lerp(speed, 25, 5f * Time.deltaTime);
          
            transform.position = Vector3.Lerp(transform.position, targetPostion, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 25f);

        }
    }
    private void CamCheck(out RaycastHit raycast, out float dis)
    {
        
#if UNITY_EDITOR
        Debug.DrawLine(target.position + target.GetComponent<CapsuleCollider>().center * 1.75f,
            target.position + target.GetComponent<CapsuleCollider>().center * 1.75f +
            (transform.position - target.position - target.GetComponent<CapsuleCollider>().center * 1.75f).normalized * distanceFromTarget
            , Color.blue);
#endif
       
        if (Physics.Raycast(target.position + target.GetComponent<CapsuleCollider>().center * 1.75f,
          (transform.position - target.position - target.GetComponent<CapsuleCollider>().center * 1.75f).normalized, out raycast,
           distanceFromTarget, ~Physics.IgnoreRaycastLayer))
        {
            dis = Vector3.Distance(target.position + target.GetComponent<CapsuleCollider>().center * 1.75f + new Vector3(xOffset, 0, 0), raycast.point);
        }
        else
            dis = distanceFromTarget;
    }
    public void CursorArise()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && Cursor.visible == false)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        if (target != null)
        {

        }
    }
}
