using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovement : MonoBehaviour
{
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        animator.SetFloat("horizontal", rigidbodyFirstPersonController.GetHorizontal());
        animator.SetFloat("vertical", rigidbodyFirstPersonController.GetVertical());
    }
}
