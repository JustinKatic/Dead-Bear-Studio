using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerController : MonoBehaviourPun
{
    [Header("Info")]
    public int id;
    private int curAttackerId;

    public float moveSpeed;
    public float turnSpeed = 15;
    Animator animator;
    Camera mainCamera;

    Vector2 input;

    public Player photonPlayer;


    private void Update()
    {
        if (!photonView.IsMine)
            return;

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        //Set the animators blend tree to correct animation based of inputs, with 0.1 smooth added
        animator.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
        animator.SetFloat("InputY", input.y, 0.1f, Time.deltaTime);

        //set the players rotation to the direction of the camera with a slerp smoothness
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);

    }


    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        // is this not our local player?
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
        else
        {
            animator = GetComponent<Animator>();
            mainCamera = GetComponentInChildren<Camera>();
            //GameUI.instance.Initialize(this);
        }
    }

}
