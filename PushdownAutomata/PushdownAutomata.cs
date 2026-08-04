using System.Collections.Generic;
using Extensions.Other;
using UnityEngine;

namespace Extensions.PushdownAutomata
{
    public class PushdownAutomata<T> : MonoBehaviourUpdatable where T : class
    {
        private readonly Stack<PushdownState<T>> _currentState = new();
        public readonly T Parent;

        /**
         * <summary>
         * Creates a machine that drives itself from the Unity loop, attaching an updater to the
         * parent's GameObject. If the parent is not a MonoBehaviour there is nothing to attach to,
         * so the machine is driven externally and the caller must tick it.
         * </summary>
         */
        public PushdownAutomata(T parent) : base((parent as MonoBehaviour)?.gameObject)
        {
            Parent = parent;
        }

        /**
         * <summary>
         * Creates a machine with an explicit choice of who drives it. Pass true to opt out of the
         * Unity loop and tick it yourself, which is what you want when the machine has to run at a
         * precise point relative to other systems rather than whenever Unity gets to it.
         * </summary>
         * <param name="parent">Host the states receive.</param>
         * <param name="externallyDriven">True to attach no updater and require manual ticking.</param>
         */
        public PushdownAutomata(T parent, bool externallyDriven)
            : base(externallyDriven ? null : (parent as MonoBehaviour)?.gameObject)
        {
            Parent = parent;
        }

        public override void Update()
        {
            if (_currentState.Count > 0)
            {
                _currentState.Peek().OnUpdate();
            }
        }

        public override void FixedUpdate()
        {
            if (_currentState.Count > 0)
            {
                _currentState.Peek().OnFixedUpdate();
            }
        }

        public override void LateUpdate()
        {
            if (_currentState.Count > 0)
            {
                _currentState.Peek().OnLateUpdate();
            }
        }

        public void ChangeState(PushdownState<T> newState)
        {
            RemoveTop();
            AddNewState(newState);
        }

        public void Interrupt(PushdownState<T> newState)
        {
            _currentState.Peek().OnInterrupt();
            AddNewState(newState);
        }

        public void ResumePrevious()
        {
            RemoveTop();
            if (_currentState.Count > 0)
            {
                _currentState.Peek().OnResume();
            }
        }
        
        public void ResumeBase()
        {
            while (_currentState.Count > 0 && !_currentState.Peek().doNotRemove)
            {
                _currentState.Peek().OnExit();
                _currentState.Pop();
            }

            if (_currentState.Count > 0)
                _currentState.Peek().OnResume();
        }

        public PushdownState<T> GetCurrentState()
        {
            return _currentState?.Count > 0 ? _currentState.Peek() : null;
        }
        
        public bool IsState<TState>() where TState : PushdownState<T>
        {
            return _currentState.Count > 0 && _currentState.Peek() is TState;
        }

        private void RemoveTop()
        {
            if (_currentState.Count > 0 && !_currentState.Peek().doNotRemove)
            {
                _currentState.Peek().OnExit();
                _currentState.Pop();
            }
        }

        private void AddNewState(PushdownState<T> newState)
        {
            _currentState.Push(newState);
            _currentState.Peek().OnStateEnter(Parent);
        }

        public void ClearStates()
        {
            while (_currentState.Count > 0 && !_currentState.Peek().doNotRemove)
            {
                _currentState.Peek().OnExit();
                _currentState.Pop();
            }
        }

        public void PrintStates()
        {
            string output = ((Parent as UnityEngine.Object)?.name ?? Parent.GetType().Name) + " States: ";

            foreach (var state in _currentState.ToArray())
            {
                output += state.ToString();
                output += " -- ";
            }

            Debug.Log(output[..^4]);


        }

        public override void OnTriggerEnter(Collider other)
        {
            if (_currentState.Count > 0)
            {
                _currentState.Peek().OnTriggerEnter(other);
            }
        }

        public override void OnCollisionEnter(Collision collision)
        {
            if (_currentState.Count > 0)
            {
                _currentState.Peek().OnCollisionEnter(collision);
            }
        }
    }
}

