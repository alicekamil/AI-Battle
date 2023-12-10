using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Nodes
{
    public class HasValue : DecoratorNode
    {
        public string           m_key = "VariableName";
        public bool             m_bInvert = false;
        public bool             m_bCheckEveryFrame = true;

        private bool            m_bResult = false;

        #region Properties

        public override string Description => (m_bInvert ? "Don't have " : "Has ") + m_key + (m_bCheckEveryFrame ? " (EF)" : "");

        #endregion

        protected override void OnStart()
        {
            base.OnStart();

            if (!m_bCheckEveryFrame)
            {
                UpdateResult();
            }
        }

        private void UpdateResult()
        {
            m_bResult = Tree != null && Tree.Blackboard != null && Tree.Blackboard.ContainsKey(m_key) != m_bInvert;
        }

        protected override State OnUpdate()
        {
            if (m_bCheckEveryFrame)
            {
                UpdateResult();
            }

            if (!m_bResult)
            {
                m_state = State.Failure;
            }
            else if (m_child != null)
            {
                m_state = m_child.Update();
            }

            return m_state;
        }
    }
}