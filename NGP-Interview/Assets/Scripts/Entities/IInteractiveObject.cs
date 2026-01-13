using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public interface IInteractiveObject
    {
        public bool CanInteract { get; }
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnInteract(BaseEntity entity);
    }
}