using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Extensions;
using RPGCharacterAnims.Lookups;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
// Requires installing the InputSystem Package from the Package Manager: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/Installation.html
using UnityEngine.InputSystem;
#endif

namespace RPGCharacterAnims
{
    public class GUIControls : MonoBehaviour
    {
        private RPGCharacterController rpgCharacterController;
        private RPGCharacterWeaponController rpgCharacterWeaponController;
		private float idleStatic;
        private bool useInstant;
        private bool useNavigation;
        private Vector3 jumpInput;
        public GameObject nav;

        private void Start()
        {
            // Get other RPG Character components.
            rpgCharacterController = GetComponent<RPGCharacterController>();
            rpgCharacterWeaponController = GetComponent<RPGCharacterWeaponController>();
        }

        private void OnGUI()
        {
			// Allow Navigation if grounded.
			if (rpgCharacterController.maintainingGround) { Navigation(); }

	        // Character is not on the ground.
	        if (!rpgCharacterController.maintainingGround) {
		        Jumping();
		        return;
	        }
			// If character can act.
			if (rpgCharacterController.canAction) {
				Idle();
				Attacks();
				Damage();
				DiveRoll();
				WeaponSwitching();
			}

			// Outputs RPG variables and Animator parameters.
			DebugRPGCharacter();
        }

		private void Idle()
		{
			GUI.Button(new Rect(540, 140, 60, 30), "Idle");
			idleStatic = GUI.HorizontalSlider(new Rect(540, 170, 60, 30), idleStatic, 0.0F, 1f);
			rpgCharacterController.animator.SetFloat(AnimationParameters.Idle, idleStatic);
		}

		private void Navigation()
        {
			// Check to make sure Navigation Action exists.
            if (!rpgCharacterController.HandlerExists(HandlerTypes.Navigation)) { return; }

            useNavigation = GUI.Toggle(new Rect(550, 105, 100, 30), useNavigation, "Navigation");

            var navChild = nav.transform.GetChild(0);
            if (useNavigation) {

				// Show the navigation pointer.
	            navChild.GetComponent<MeshRenderer>().enabled = true;
	            navChild.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                RaycastHit hit;

				#if ENABLE_INPUT_SYSTEM
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100)) {
					nav.transform.position = hit.point;
					if (Mouse.current.leftButton.wasPressedThisFrame) { rpgCharacterController.StartAction(HandlerTypes.Navigation, hit.point); }
				}
				#else
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
					nav.transform.position = hit.point;
					if (Input.GetMouseButtonDown(0)) { rpgCharacterController.StartAction(HandlerTypes.Navigation, hit.point); }
				}
				#endif
			}
			// Hide the navigation pointer.
			else {
	            navChild.GetComponent<MeshRenderer>().enabled = false;
	            navChild.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

				// End Navigation Action if possible.
				if (rpgCharacterController.CanEndAction(HandlerTypes.Navigation))
				{ rpgCharacterController.EndAction(HandlerTypes.Navigation); }
            }
        }

		private void Attacks()
		{
			// Check if Attack Action exists.
			if (!rpgCharacterController.HandlerExists(HandlerTypes.Attack)) { return; }

			// End special attack.
			if (rpgCharacterController.CanEndAction(HandlerTypes.Attack)) {
				if (GUI.Button(new Rect(235, 85, 100, 30), "End Special"))
				{ rpgCharacterController.EndAction(HandlerTypes.Attack); }
			}
			// Check if can start Attack Action.
			if (!rpgCharacterController.CanStartAction(HandlerTypes.Attack)) { return; }

			// Attack Left.
			if (rpgCharacterController.leftWeapon == Weapon.Unarmed && rpgCharacterController.rightWeapon == Weapon.Unarmed) {
				if (GUI.Button(new Rect(25, 85, 100, 30), "Attack L"))
				{ rpgCharacterController.StartAction(HandlerTypes.Attack, new AttackContext("Attack", Side.Left)); }
			}
			// Attack Right.
			if (rpgCharacterController.rightWeapon == Weapon.Unarmed && rpgCharacterController.leftWeapon == Weapon.Unarmed) {
				if (GUI.Button(new Rect(130, 85, 100, 30), "Attack R"))
				{ rpgCharacterController.StartAction(HandlerTypes.Attack, new AttackContext("Attack", Side.Right)); }
			}
			// TwoHanded Attack.
			if (rpgCharacterController.hasTwoHandedWeapon) {
				if (GUI.Button(new Rect(130, 85, 100, 30), "Attack"))
				{ rpgCharacterController.StartAction(HandlerTypes.Attack, new AttackContext("Attack", Side.None)); }
			}
		}

		private void Damage()
        {
			// Check if Get Hit Action exists and can start it.
			if (rpgCharacterController.HandlerExists(HandlerTypes.GetHit)
				&& rpgCharacterController.CanStartAction(HandlerTypes.GetHit)) {
					if (GUI.Button(new Rect(30, 240, 100, 30), "Get Hit"))
					{ rpgCharacterController.StartAction(HandlerTypes.GetHit, new HitContext()); }
			}
			// Check if Knockback Action exists and can start it.
			if (rpgCharacterController.HandlerExists(HandlerTypes.Knockback)
				&& rpgCharacterController.CanStartAction(HandlerTypes.Knockback)) {
					if (GUI.Button(new Rect(130, 240, 100, 30), "Knockback1"))
					{ rpgCharacterController.StartAction(HandlerTypes.Knockback, new HitContext((int)KnockbackType.Knockback1, Vector3.back)); }
					if (GUI.Button(new Rect(230, 240, 100, 30), "Knockback2"))
					{ rpgCharacterController.StartAction(HandlerTypes.Knockback, new HitContext((int)KnockbackType.Knockback2, Vector3.back)); }
			}
			// Check if Knockdown Action exists and can start it.
			if (rpgCharacterController.HandlerExists(HandlerTypes.Knockdown)
				&& rpgCharacterController.CanStartAction(HandlerTypes.Knockdown)) {
					if (GUI.Button(new Rect(130, 270, 100, 30), "Knockdown"))
					{ rpgCharacterController.StartAction(HandlerTypes.Knockdown, new HitContext((int)KnockdownType.Knockdown1, Vector3.back)); }
			}
        }

		private void DiveRoll()
		{
			// Check if DiveRoll Action exists and can start it.
			if (rpgCharacterController.HandlerExists(HandlerTypes.DiveRoll)) {
				if (rpgCharacterController.CanStartAction(HandlerTypes.DiveRoll)) {
					if (GUI.Button(new Rect(445, 75, 100, 30), "Dive Roll"))
					{ rpgCharacterController.StartAction(HandlerTypes.DiveRoll, DiveRollType.DiveRoll1); }
				}
			}
		}

        private void Jumping()
        {
			// Check if Jump Action exists.
			if (!rpgCharacterController.HandlerExists(HandlerTypes.Jump)) { return; }

			// Check if can start Jump Action.
			if (rpgCharacterController.CanStartAction(HandlerTypes.Jump)) {
                if (GUI.Button(new Rect(25, 175, 100, 30), "Jump")) {
                    rpgCharacterController.SetJumpInput(Vector3.up);
                    rpgCharacterController.StartAction(HandlerTypes.Jump);
                }
            }
			// Check if can start DoubleJump Action.
			if (rpgCharacterController.CanStartAction(HandlerTypes.DoubleJump)) {
                if (GUI.Button(new Rect(25, 175, 100, 30), "Jump Flip")) {
                    rpgCharacterController.SetJumpInput(Vector3.up);
                    rpgCharacterController.StartAction(HandlerTypes.DoubleJump);
                }
            }
        }

		private void DebugRPGCharacter()
		{
			// Output all controller variables.
			if (GUI.Button(new Rect(600, 20, 120, 30), "Debug Controller"))
			{ rpgCharacterController.DebugController(); }

			// Output all Animator parameters.
			if (GUI.Button(new Rect(600, 50, 120, 30), "Debug Animator"))
			{ rpgCharacterController.animator.DebugAnimatorParameters(); }
		}

        private void WeaponSwitching()
		{
			// Check if SwitchWeapon Action exists.
			if (!rpgCharacterController.HandlerExists(HandlerTypes.SwitchWeapon)) { return; }

			var doSwitch = false;

			// Create a new SwitchWeaponContext with the switch settings.
			var switchWeaponContext = new SwitchWeaponContext();

			// Unarmed.
			if (rpgCharacterController.rightWeapon != Weapon.Unarmed
				|| rpgCharacterController.leftWeapon != Weapon.Unarmed) {
				if (GUI.Button(new Rect(1115, 280, 100, 30), "Unarmed")) {
					doSwitch = true;
					switchWeaponContext.type = "Switch";
					switchWeaponContext.side = "Both";
					switchWeaponContext.leftWeapon = Weapon.Unarmed;
					switchWeaponContext.rightWeapon = Weapon.Unarmed;
				}
			}
			var offset = 310;

			// TwoHanded weapon.
			foreach (var weapon in WeaponGroupings.TwoHandedWeapons) {
				if (rpgCharacterController.rightWeapon != weapon) {
					var label = weapon.ToString();
					if (label.StartsWith("TwoHand")) { label = label.Replace("TwoHand", "2H "); }
					if (GUI.Button(new Rect(1115, offset, 100, 30), label)) {
						doSwitch = true;
						switchWeaponContext.type = "Switch";
						switchWeaponContext.side = "None";
						switchWeaponContext.leftWeapon = Weapon.Unarmed;
						switchWeaponContext.rightWeapon = weapon;
					}
				}
				offset += 30;
			}
			// Instant weapon toggle.
			useInstant = GUI.Toggle(new Rect(1000, 310, 100, 30), useInstant, "Instant");
			if (useInstant) { switchWeaponContext.type = "Instant"; }

			// Perform the weapon switch using the SwitchWeaponContext created earlier.
			if (doSwitch) { rpgCharacterController.TryStartAction(HandlerTypes.SwitchWeapon, switchWeaponContext); }
		}
	}
}