using UnityEngine;

namespace Extensions.PushdownAutomata
{
    public abstract class PushdownState<T> where T : MonoBehaviour
    {
        public bool doNotRemove = false;

        protected T Host;

        public void OnStateEnter(T parent)
        {
            Host = parent;
            OnEnter();
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnLateUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnInterrupt()
        {
        }

        public virtual void OnResume()
        {
        }

        public virtual void OnTriggerEnter(Collider other)
        {
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
        }
    }
}