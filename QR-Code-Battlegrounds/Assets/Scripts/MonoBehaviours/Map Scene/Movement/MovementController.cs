using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Animator anim;
	public float speed = 3.0F;  
    private Vector3 moveDirection = Vector3.zero;
    private CharacterControllersController ccc_instance;
    private CharacterController cc;
    
    private static bool isenabled_private = true;
    public static bool isenabled
    {
        set { isenabled_private = value; }
    }
    private void Start()
    {
        ccc_instance = CharacterControllersController.Instance;
        cc = ccc_instance.GetActiveController();
    }
    void Update()
    {
        MovementUpdate();
    }

    private bool isWalking = false;

    void MovementUpdate()
    {
        if (isenabled_private)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection *= speed; 

            if (moveDirection.magnitude > 0.001)
            {
                cc = ccc_instance.GetActiveController();

                isWalking = true;
                if (cc.gameObject.GetComponent<isPlayer>())
                    anim.SetBool("IsWalking", true);

                cc.Move(moveDirection * Time.deltaTime);
            } else
            {
                if (isWalking)
                {
                    isWalking = false;
                    anim.SetBool("IsWalking", false);
                }
            }
        }
    }
}
