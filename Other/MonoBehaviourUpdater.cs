using System;
using UnityEngine;

namespace Extensions.Other
{
    [AddComponentMenu("")]
    public class MonoBehaviourUpdater : MonoBehaviour
    {
        private MonoBehaviourUpdatable updatable;

        public void SetUpdatable(MonoBehaviourUpdatable updatable)
        {
            this.updatable = updatable;
        }

        private void Update()
        {
            updatable?.Update();
        }

        private void FixedUpdate()
        {
            updatable?.FixedUpdate();
        }

        private void LateUpdate()
        {
            updatable?.LateUpdate();
        }

        public void RemoveUpdatable()
        {
            updatable = null;
        }

        public bool HasUpdatable()
        {
            return updatable != null;
        }

        private void OnTriggerEnter(Collider other)
        {
            updatable?.OnTriggerEnter(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            updatable?.OnCollisionEnter(collision);
        }
    }

    public abstract class MonoBehaviourUpdatable : ITickable
    {
        private MonoBehaviourUpdater updater;
        private GameObject gameObject;

        /** <summary>True when this drives itself from the Unity loop rather than being ticked by a caller.</summary> */
        public bool IsSelfDriven => updater != null;

        /** <summary>Creates a self-driven instance on a new GameObject named after the type.</summary> */
        protected MonoBehaviourUpdatable()
        {
            gameObject = new GameObject();
            gameObject.name = GetType().Name;

            updater = gameObject.AddComponent<MonoBehaviourUpdater>();
            updater.SetUpdatable(this);
        }

        /**
         * <summary>
         * Creates an instance driven by an updater on the given GameObject.
         * <para>
         * Pass null to opt out entirely. Nothing is attached and no GameObject is created, so
         * <see cref="Update" />, <see cref="FixedUpdate" /> and <see cref="LateUpdate" /> only run
         * when a caller invokes them. Use that where tick order matters relative to other systems.
         * </para>
         * </summary>
         * <param name="g">GameObject to attach an updater to, or null to be driven externally.</param>
         */
        protected MonoBehaviourUpdatable(GameObject g)
        {
            if (g == null) return;

            gameObject = g;
            updater = g.AddComponent<MonoBehaviourUpdater>();
            updater.SetUpdatable(this);
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void OnTriggerEnter(Collider other)
        {
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
        }

        /** <summary>Destroys the GameObject this drives itself from. Does nothing when driven externally.</summary> */
        public void Destroy()
        {
            if (gameObject == null) return;

            GameObject.Destroy(gameObject);
        }
    }
}
