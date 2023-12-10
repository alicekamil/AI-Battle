using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Nodes
{
    public class Compare : DecoratorNode
    {
        public enum Operator
        {
            Equals, 
            NotEquals,
            GreaterThan,
            LessThan,
        };

        public string           m_key = "VariableName";
        public Operator         m_operator = Operator.LessThan;
        public int              m_iValue = 5;
        public bool             m_bCompareEveryFrame = true;

        private bool            m_bResult = false;
        static string[]         sm_opCodes = new string[] { " == ", " != ", " > ", " < " };

        #region Properties

        public override string Description => m_key + sm_opCodes[(int)m_operator] + m_iValue.ToString() + (m_bCompareEveryFrame ? " (EF)" : "");

        #endregion

        protected void UpdateResult()
        {
            m_bResult = false;
            int iValue = 0;
            object value;
            if (Tree != null &&
                Tree.Blackboard != null &&
                Tree.Blackboard.TryGetValue(m_key, out value))
            {
                // get value from object
                if (value is int) iValue = (int)value;
                if (value is bool) iValue = (bool)value ? 1 : 0;
                else if (value is float) iValue = Mathf.RoundToInt((float)value);
                else if (value is double) iValue = Mathf.RoundToInt((float)value);
                else if (value is IEnumerable)
                {
                    IEnumerable v = value as IEnumerable;
                    IEnumerator e = v.GetEnumerator();
                    while (e.MoveNext())
                    {
                        iValue++;
                    }
                }
            }

            switch (m_operator)
            {
                case Operator.Equals:
                    m_bResult = iValue == m_iValue;
                    break;

                case Operator.NotEquals:
                    m_bResult = iValue != m_iValue;
                    break;

                case Operator.GreaterThan:
                    m_bResult = iValue > m_iValue;
                    break;

                case Operator.LessThan:
                    m_bResult = iValue < m_iValue;
                    break;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (!m_bCompareEveryFrame)
            {
                UpdateResult();
            }
        }

        protected override State OnUpdate()
        {
            // compare every frame?
            if (m_bCompareEveryFrame)
            {
                UpdateResult();
            }

            if (!m_bResult)
            {
                m_state = State.Failure;
            }
            else  if (m_child != null)
            {
                m_state = m_child.Update();
            }

            return m_state;
        }
    }
}