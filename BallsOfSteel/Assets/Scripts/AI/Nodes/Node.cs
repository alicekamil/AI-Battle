using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Nodes
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Success,
            Failure
        }

        [SerializeField, HideInInspector]
        private int             m_ID;

        [SerializeField, HideInInspector]
        public Vector2          m_vPosition;

        [System.NonSerialized]
        public State            m_state = State.Running; // current state on this particular node

        [System.NonSerialized]
        public bool             m_bStarted = false; // has this node started execution?

        [System.NonSerialized]
        public BehaviourTree    m_tree;

        #region Properties

        public int ID => m_ID;

        public BehaviourTree Tree => m_tree;

        public virtual string Description => "";

        #endregion

        private void OnValidate()
        {
            if (m_ID == 0)
            {
                m_ID = System.Guid.NewGuid().GetHashCode();
            }
        }

        public State Update() // will run on every type of node and works the same for all of them
        {
            // has this node started?
            if (!m_bStarted)
            {
                OnStart();
                m_bStarted = true; // now it has started..
            }

            m_state = OnUpdate(); // do as many updates as the base class wants

            if (m_state != State.Running) // when its no longer running..
            {
                OnStop(); // ..then we stop
                m_bStarted = false;
            }

            return m_state;
        }

        public virtual void OnTreeStart(BehaviourTree tree)
        {
            m_tree = tree;
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.name = name;
            return clone;
        }
    }
}