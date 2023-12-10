using System.Collections;
using System.Collections.Generic;
using Game;
using Graphs;
using UnityEngine;
using UnityEngine.Android;

namespace Alice_Kamil
{
    
    public class Defensive : UnitAction_Alice_Kamil
    {
        
        protected override void OnStop()
        {
            
        }
        
        protected override State OnUpdate()
        {
            List<Battlefield.Node> lowFirePowerNodes = new List<Battlefield.Node>();
            Dictionary<Battlefield.Node, float> firePowerData = Tree.Blackboard.GetValue<Dictionary<Battlefield.Node,float>>("FirePower", null);
            
            foreach (var data in firePowerData)
            {
                if (data.Value == 0.2f)
                {
                    lowFirePowerNodes.Add(data.Key);
                }
            }
            foreach (Unit unit in Team.Units)
            {
                Battlefield.Node bestNode = null;
                if (unit.ClosestEnemy == null)
                    continue;
                List<Battlefield.Node> safeNodes = new List<Battlefield.Node>(
                    GraphUtils.GetNodesWithinDistance(Battlefield.Instance, unit.ClosestEnemy.CurrentNode, 40)); // Nodes we can reach
                foreach (var lowfpnode in lowFirePowerNodes) 
                {
                    foreach (var snode in Battlefield.Instance.Nodes) 
                    {
                        if (snode is Battlefield.Node castedNode)
                        {
                            if (lowfpnode == castedNode)
                            {
                                bestNode = castedNode;
                                break;
                            }
                        }
                    }
                    if (bestNode != null)
                        break;
                }
                unit.TargetNode = bestNode;
            }
            return State.Success;
        }
    }
}
