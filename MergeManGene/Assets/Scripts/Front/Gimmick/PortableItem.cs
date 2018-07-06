using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MargeManGene {
	public class PortableItem : Item {
		
		public override GraspedReaction Reaction {
			get {
				return GraspedReaction.PULL_TO_PLAYER;
			}
		}

		Rigidbody2D rig_;


		public override void OnGraspedEnter(Player arg_player) {
			SetAbleRelease (false);
			SetAbleGrasp (false);
			rig_ = GetComponent<Rigidbody2D>();
			rig_.simulated = false;

            //新しい音でして
            //AudioManager.Instance.QuickPlaySE("SE_PlayerHand_grab");
		}

		public override void OnGraspedStay(Player arg_player) {

			this.transform.position = arg_player.PlayableArm.TopPosition;

			if (!arg_player.PlayableArm.IsUsing()) {
				SetAbleRelease(true);
			}
			
		}

		public override void OnGraspedExit(Player arg_player) {
			SetAbleGrasp (true);
			SetAbleRelease (false);
			rig_ = GetComponent<Rigidbody2D>();
			rig_.simulated = true;
		}
	}
}