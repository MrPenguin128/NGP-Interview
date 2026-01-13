using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    public class InteractionHandler : Singleton<InteractionHandler>
    {
        //------------------------------------
        //          PREMADE ASSET
        //------------------------------------

        [SerializeField] float checkRadius;
        [SerializeField] LayerMask interactionLayer;
        Collider[] objects = new Collider[10];
        IInteractiveObject interactiveObject;
        public static bool CanInteract { get; private set; } = true;

        // Update is called once per frame
        void Update()
        {
            if (!CanInteract)
            {
                if (interactiveObject != null)
                {
                    interactiveObject?.OnExit();
                    interactiveObject = null;
                }
                return;
            }
            objects = new Collider[10];
            int count = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, objects, interactionLayer);
            if (count > 0)
            {
                float minDistance = float.MaxValue;
                int closestObjectIndex = 0;

                for (int i = 0; i < count; i++)
                {
                    float distance = Vector3.SqrMagnitude(objects[i].transform.position - transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestObjectIndex = i;
                    }
                }
                var closestObject = objects[closestObjectIndex].GetComponent<IInteractiveObject>();
                if (closestObject != null && closestObject != interactiveObject && closestObject.CanInteract)
                {
                    if(interactiveObject as MonoBehaviour != null)
                        interactiveObject?.OnExit();
                    interactiveObject = closestObject;
                    interactiveObject.OnEnter();
                }
            }
            else if (interactiveObject as MonoBehaviour != null)
            {
                interactiveObject?.OnExit();
                interactiveObject = null;
            }else
                interactiveObject = null;

        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.started || interactiveObject == null) return;
            interactiveObject.OnInteract(GameManager.Player);
        }


        public static void BlockInteraction() => CanInteract = false;
        public static void AllowInteraction() => CanInteract = true;
    }
}