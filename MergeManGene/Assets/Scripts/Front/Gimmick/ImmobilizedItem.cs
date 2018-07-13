//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MargeManGene {
//	public class ImmobilizedItem : Item {

//		public override GraspedReaction Reaction {
//			get {
//				return GraspedReaction.REST_ARM;
//			}
//		}

//		public override void OnGraspedEnter(Player arg_player) {
//            //新しい音でございます
//            //AudioManager.Instance.QuickPlaySE("SE_PlayerHand_grab");
//		}

//		public override void OnGraspedStay(Player arg_player) {
//			SetAbleGrasp (false);
//			SetAbleRelease (true);
//		}

//		public override void OnGraspedExit(Player arg_player) {
//			SetAbleGrasp (true);
//			SetAbleRelease (false);
//		}
		
//	}
//}