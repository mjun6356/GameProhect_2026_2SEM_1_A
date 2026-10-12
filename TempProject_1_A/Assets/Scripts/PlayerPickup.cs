using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]
    [SerializeField]private Animator animator;

    [SerializeField] private Playercontroller playerController;

    [Header("아이템 탐색")]
    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]

    [SerializeField] private float pickupDuration = 1.2f;

    [SerializeField] private float pickupMoment = 0.55f;

    [SerializeField] private InteractionUI interactionUI;

    private PickupItem targetItem;

    private Door targetDoor;

    private NPC targetNPC;
    private bool isPickingUp;








    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickingUp) return;

        targetItem = FindClosestItem();
        targetDoor = FindClosestDoor();
        targetNPC = FindClosestNPC();

        if (targetItem == null ) return;

        Keyboard keyboard = Keyboard.current;

        if( keyboard == null ) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Interact();
        }
        
    }

    private PickupItem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        PickupItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemacollider in colliders)
        {
            PickupItem item =itemacollider.GetComponentInParent<PickupItem>();

            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position); 

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }
        return closestItem;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        interactionUI.HideGuide();

        PickupItem itemToCollect = targetItem;

        Vector3 itemDirection = itemToCollect.transform.position - transform.position;
        itemDirection.y= 0f;

        if (itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }

        playerController.ChangeState(PlayerState.Pickup);

        animator.CrossFade("PickUp01", 0.1f);
        yield return new WaitForSeconds(pickupMoment);

        if (itemToCollect != null)
        {
            itemToCollect.Collect();
        }

        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        animator.CrossFade("Blend Tree", 0.1f);

        playerController.ChangeState(PlayerState.Nomal);

        targetItem = null;
        targetNPC = null;
        targetDoor = null;
        isPickingUp=false;



    }

    private Door FindClosestDoor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders)
        {
            Door door = targetCollider.GetComponent<Door>();

            if (door == null) continue;

            float distance = Vector3.Distance(transform.position, door.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDoor = door;
            }
        }
        return closestDoor;
    }

    private NPC FindClosestNPC()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders)
        {
            NPC npc = targetCollider.GetComponent<NPC>();

            if (npc == null) continue;

            float distance = Vector3.Distance(transform.position, npc.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }
        return closestNPC;

    }


    private void showGuide()
    {
        if(targetItem != null)
        {
            interactionUI.ShowGuide("[E] " + targetItem.GetItemName() + " 줍기");
        }
        else if (targetDoor != null)
        {
            interactionUI.ShowGuide(targetDoor.GetGuideText());
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowGuide(targetNPC.GetGuideText());
        }
        else
        {
            interactionUI.HideGuide();
        }
    }

    private void Interact()
    {
        if (targetItem != null)
        {
            StartCoroutine(PickupAnimation());
        }
        else if (targetDoor != null)
        {
            targetDoor.Interact();
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowDialogue(targetNPC.GetNPCName(), targetNPC.GetMessage());
        }
        
    }



    
}
