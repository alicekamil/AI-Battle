using System;
using Game;
using Graphs;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using AI;
using AI.Nodes;
using UnityEngine;


namespace Alice_Kamil
{
    [RequireComponent(typeof(Brain))]
    public class Team_Alice_Kamil : Team
    {
        [SerializeField] private Color m_myFancyColor;

        private Brain m_brain;

        private Team[] m_teams = new Team[2];

        public Dictionary<Battlefield.Node, float> influenceLookup = new Dictionary<Battlefield.Node, float>();
        public Dictionary<Battlefield.Node, float> firePowerLookup = new Dictionary<Battlefield.Node, float>();


        #region Properties

        public Brain Brain => m_brain;

        public Blackboard Blackboard => m_brain.Tree.Blackboard;

        public override Color Color => m_myFancyColor;

        #endregion


        protected override void Start()
        {
            base.Start();
            //StartCoroutine(MoveUnits());

            m_brain = GetComponent<Brain>();

            m_teams[0] = this;
            m_teams[1] = EnemyTeam; // This seems stupid..
            
            Blackboard.SetValue("CollectedHealth", 15);
            
            //start coroutines
            StartCoroutine(UpdateCollectedHealth());
            StartCoroutine(UpdateInfluenceMap());
            StartCoroutine(UpdateFirePower());
        }
        IEnumerator UpdateInfluenceMap()
        {
            while (true)
            {
                CalculateInfluenceMap();
                Blackboard.SetValue("Influence", influenceLookup);
                yield return null;
            }
        }
        IEnumerator UpdateFirePower()
        {
            while (true)
            {
                CalculateFirePower();
                Blackboard.SetValue("FirePower", firePowerLookup);
                yield return null;
            }
        }
        IEnumerator UpdateCollectedHealth()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                float fCollectedHealth = CalculateCollectedHealth();
                Blackboard.SetValue("CollectedHealth", fCollectedHealth);
            }
        }

        IEnumerator MoveUnits()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                foreach (Unit unit in Units)
                {
                    unit.TargetNode =
                        GraphUtils.GetClosestNode<Battlefield.Node>(Battlefield.Instance, GetEnemyCentrePoint());
                }
            }
        }

        private float CalculateCollectedHealth()
        {
            float data = 0;
            foreach (Unit unit in Units)
            {
                data += unit.Health;
            }

            return data;
        }

        private void CalculateFirePower() // How many enemies are in shooting range if we stand on this tile? (1-all, 0 zero)
        {
            if (EnemyTeam != null)
            {
                foreach (Battlefield.Node node in Battlefield.Instance.Nodes)
                {
                    float fScore = 0;

                    foreach (Unit unit in EnemyTeam.Units)
                    {
                        float fDistance = Vector3.Distance(unit.transform.position, node.WorldPosition);
                        if (fDistance < Unit.FIRE_RANGE)
                        {
                            fScore += 1.0f;
                        }
                    }

                    fScore /= 5;

                    if (!firePowerLookup.ContainsKey(node))
                    {
                        firePowerLookup.Add(node, fScore);
                    }
                    else
                        firePowerLookup[node] = fScore; // update
                }
            }
        }

        private void CalculateInfluenceMap() // Given a range, how How much influence does "this" team command over "this" tile?
        {
            influenceLookup.Clear();
            foreach (Battlefield.Node node in Battlefield.Instance.Nodes)
            {
                float fScore = 0.0f;

                for (int i = 0; i < m_teams.Length; i++)
                {
                    foreach (Unit unit in m_teams[i].Units)
                    {
                        float fDistance = Vector3.Distance(unit.transform.position, node.WorldPosition);
                        if (fDistance < Unit.FIRE_RANGE)
                        {
                            fScore += ((1.0f - (fDistance / Unit.FIRE_RANGE)) * (i == 0 ? 1.0f : -1.0f)) /
                                      m_teams[i].Units.Count();
                        }
                    }
                }

                if (!influenceLookup.ContainsKey(node))
                {
                    influenceLookup.Add(node, fScore);
                }
                else
                    influenceLookup[node] = fScore; // update
            }
        }

        private Vector3 GetEnemyFurthestAway()
        {
            Vector3 centerPoint = GetEnemyCentrePoint();
            Vector3 furthestEnemy = new Vector3(0, 0, 0);
            float maxDistance = 3;

            foreach (Unit enemy in EnemyTeam.Units)
            {
                if (enemy == null)
                    continue;
                float distanceToCenter = Vector3.Distance(centerPoint, enemy.transform.position);

                if (distanceToCenter > maxDistance)
                {
                    Debug.Log("distance is greater than max");
                    maxDistance = distanceToCenter;
                    furthestEnemy = enemy.transform.position;
                }
            }

            return furthestEnemy;
        }
        private Vector3 GetEnemyCentrePoint()
        {
            Vector3 collectedCentrePoints = new Vector3(0, 0, 0);
            foreach (Unit enemy in EnemyTeam.Units)
            {
                if (enemy == null)
                    continue;
                collectedCentrePoints += enemy.transform.position;
            }

            return collectedCentrePoints / EnemyTeam.Units.Count();
        }
        public new GraphUtils.Path GetShortestPath(Battlefield.Node start, Battlefield.Node goal)
        {
            if (start == null ||
                goal == null ||
                start == goal ||
                Battlefield.Instance == null)
            {
                Debug.Log("cant return shortest path!");
                return null;
            }

            // initialize pathfinding
            foreach (Battlefield.Node node in Battlefield.Instance.Nodes)
            {
                node?.ResetPathfinding();
            }

            // add start node
            start.m_fDistance = 0.0f;
            start.m_fRemainingDistance = Battlefield.Instance.Heuristic(goal, start);
            List<Battlefield.Node> open = new List<Battlefield.Node>();
            HashSet<Battlefield.Node> closed = new HashSet<Battlefield.Node>();
            open.Add(start);

            // search
            while (open.Count > 0)
            {
                // get next node (the one with the least remaining distance)
                Battlefield.Node current = open[0];
                for (int i = 1; i < open.Count; ++i)
                {
                    if (open[i].m_fRemainingDistance < current.m_fRemainingDistance)
                    {
                        current = open[i];
                    }
                }

                open.Remove(current);
                closed.Add(current);

                // found goal?
                if (current == goal)
                {
                    // construct path
                    GraphUtils.Path path = new GraphUtils.Path();
                    while (current != null)
                    {
                        path.Add(current.m_parentLink);
                        current = current != null && current.m_parentLink != null ? current.m_parentLink.Source : null;
                    }

                    path.RemoveAll(l => l == null); // HACK: check if path contains null links
                    path.Reverse();
                    return path;
                }
                else
                {
                    foreach (Battlefield.Link link in current.Links)
                    {
                        if (link.Target is Battlefield.Node target)
                        {
                            if (!closed.Contains(target) &&
                                target.Unit == null)
                            {
                                float newDistance = current.m_fDistance +
                                                    Vector3.Distance(current.WorldPosition, target.WorldPosition) +
                                                    target.AdditionalCost;
                                float newRemainingDistance =
                                    newDistance + Battlefield.Instance.Heuristic(target, start);

                                if (open.Contains(target))
                                {
                                    if (newRemainingDistance < target.m_fRemainingDistance)
                                    {
                                        // re-parent neighbor node
                                        target.m_fRemainingDistance = newRemainingDistance;
                                        target.m_fDistance = newDistance;
                                        target.m_parentLink = link;
                                    }
                                }
                                else
                                {
                                    // add target to openlist
                                    target.m_fRemainingDistance = newRemainingDistance;
                                    target.m_fDistance = newDistance;
                                    target.m_parentLink = link;
                                    open.Add(target);
                                }
                            }
                        }
                    }
                }
            }
            // no path found :(
            return null;
        }
    }
}