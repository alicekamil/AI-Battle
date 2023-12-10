using System.Collections;
using System.Collections.Generic;
using AI.Nodes;
using Graphs;
using System.Linq;
using Game;
using UnityEngine;

namespace Alice_Kamil
{
    public class FocusEnemyCentrePoint : UnitAction_Alice_Kamil
    {
        private Battlefield.Node m_targetNode;
        
        
        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            /*Vector3 collectedCentrePoints = new Vector3(0, 0, 0);
            Vector3 newCenterPoint = new Vector3(0, 0, 0);
            foreach (Unit unit in Team.EnemyTeam.Units) //?
            {
                if (unit == null)
                    continue;
                collectedCentrePoints += unit.transform.position;
            }

            newCenterPoint = collectedCentrePoints / Team.EnemyTeam.Units.Count();
            m_targetNode = GraphUtils.GetClosestNode<Battlefield.Node>(Battlefield.Instance, newCenterPoint);

            //assign target
            TargetNode = m_targetNode;

            return Unit.CurrentNode != Unit.TargetNode ? State.Running : State.Success;*/
            
            return State.Success;
            //calculate a target node
        }
    }
}
