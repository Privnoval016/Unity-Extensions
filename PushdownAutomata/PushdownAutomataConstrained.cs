using System.Collections.Generic;
using Extensions.Other;
using UnityEngine;

namespace Extensions.PushdownAutomata
{
    /**
     * <summary>
     * A more constrained version of PushdownAutomata that enforces type safety on states.
     * </summary>
     */
    public class PushdownAutomata<THost, TState> : MonoBehaviourUpdatable 
                                                   where THost : MonoBehaviour where TState : PushdownState<THost>
    {
        private readonly Stack<TState> _currentState = new();
        public readonly THost Parent;

        public PushdownAutomata(THost parent) : base(parent.gameObject)
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

        public void ChangeState(TState newState)
        {
            RemoveTop();
            AddNewState(newState);
        }

        public void Interrupt(TState newState)
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
                _currentState.Pop();
            }
            
            if (_currentState.Count > 0)
                _currentState.Peek().OnResume();
        }

        public TState GetCurrentState()
        {
            return _currentState?.Count > 0 ? _currentState.Peek() : null;
        }
        
        public bool IsState<TState2>() where TState2 : TState
        {
            return _currentState.Count > 0 && _currentState.Peek() is TState2;
        }

        private void RemoveTop()
        {
            if (_currentState.Count > 0 && !_currentState.Peek().doNotRemove)
            {
                _currentState.Peek().OnExit();
                _currentState.Pop();
            }
        }

        private void AddNewState(TState newState)
        {
            _currentState.Push(newState);
            _currentState.Peek().OnStateEnter(Parent);
        }

        public void ClearStates()
        {
            while (_currentState.Count > 0 && !_currentState.Peek().doNotRemove)
            {
                _currentState.Pop();
            }
        }

        public void PrintStates()
        {
            string output = Parent.name + " States: ";

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