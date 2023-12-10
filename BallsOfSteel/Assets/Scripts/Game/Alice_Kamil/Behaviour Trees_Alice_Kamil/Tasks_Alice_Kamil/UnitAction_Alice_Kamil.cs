using System.Collections;
using System.Collections.Generic;
using AI;
using AI.Nodes;
using System.Linq;
using Graphs;
using Game;
using UnityEngine;

namespace Alice_Kamil
{
        public abstract class UnitAction_Alice_Kamil : ActionNode
        {
            
            #region Properties
            public Team_Alice_Kamil Team => Tree?.TargetBrain.GetComponent<Team_Alice_Kamil>();
            
            #endregion

            protected override void OnStart()
            {

            }
        }
}
