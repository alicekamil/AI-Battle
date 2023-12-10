using System.Collections;
using System.Collections.Generic;
using Alice_Kamil;
using Game;
using Graphs;
using UnityEngine;

namespace Alice_Kamil
{
    public class Aggressive : UnitAction_Alice_Kamil
    {
        #region Properties

        #endregion

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            foreach (Unit unit in Team.Units)
            {
                if (unit.ClosestEnemy != null)
                {
                    Battlefield.Node bestNode = null;
                    float influencevalue = float.MinValue;
                    
                    // Get all the attack nodes within distance(4) of each units closest enemy's node
                    List<Battlefield.Node> attackNodes = new List<Battlefield.Node>(
                        GraphUtils.GetNodesWithinDistance(Battlefield.Instance, unit.ClosestEnemy.CurrentNode, 4));
                    Dictionary<Battlefield.Node, float> influencevalues =
                        Tree.Blackboard.GetValue<Dictionary<Battlefield.Node, float>>("Influence", null);
                    
                    if (influencevalues != null)
                    {
                        foreach (var attackNode in attackNodes)
                        {
                            if (bestNode == null || influencevalues[attackNode] > influencevalue)
                            {
                                influencevalue = influencevalues[attackNode];
                                bestNode = attackNode;
                            }
                        }
                    }

                    unit.TargetNode = bestNode;
                }
            }
            return State.Success;
        }
    }
}