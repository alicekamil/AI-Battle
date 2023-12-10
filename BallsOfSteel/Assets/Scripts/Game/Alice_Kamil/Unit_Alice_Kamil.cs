using System;
using AI;
using Game;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Alice_Kamil
{
    public class Unit_Alice_Kamil : Unit
    {
        #region Properties

        public new Team_Alice_Kamil Team => base.Team as Team_Alice_Kamil;

        private List<Battlefield.Node> m_path;

        #endregion
        protected override Unit SelectTarget(List<Unit> enemiesInRange)
        {
            return enemiesInRange[Random.Range(0, enemiesInRange.Count)];
        }

        protected override GraphUtils.Path GetPathToTarget()
        {
            return Team.GetShortestPath(CurrentNode, TargetNode);
        }
    }
}