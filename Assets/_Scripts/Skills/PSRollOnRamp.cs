using UnityEngine;

[CreateAssetMenu(fileName = "RollOnRamp", menuName = "PlayerSkill/RollOnRamp")]
public class PSRollOnRamp : PlayerSkill
{
	public bool isRolling;

	private Vector2 rollDirection;
	private Ramp ramp;

	public override void Init(PlayerController _pc)
	{
		pc = _pc;
	}

	public override void Trigger()
    {
        Debug.Log("Triggered Roll on ramp");
		//Detect if ramp is present
		if (!IsOnRamp())
		{
			Debug.LogError("Cancelled Skill because of is not on a ramp!");
			return;
		}
		
		if(ramp == null)
		{
			Debug.LogError("Found a ramp but could not get its component");
			return;
		}

        //Shrink
		pc.transform.localScale = Vector3.one * .5f;
		pc.canDestroyBlocks = true;
		//Move following ramp
		rollDirection = ramp.isLookingRight ? Vector2.right : Vector2.left;
		pc.OnMovementInputChanged += CancelSkill;
		ForcePlayerDirection();
		//Destroy obstacles
		//On player movement, cancel roll
	}

	private bool IsOnRamp()
	{
		RaycastHit2D rh;
		float height = pc.playerHeight / 2f;
		Vector3 leftRayStart = pc.transform.position - pc.groundRaycastSideOffset * Vector3.right;
		Vector3 rightRayStart = pc.transform.position - pc.groundRaycastSideOffset * Vector3.right;

		if (((rh = Physics2D.Raycast(pc.transform.position, Vector2.down, height, pc.groundMask)).collider != null && (ramp = rh.collider.GetComponent<Ramp>()) != null)
		|| ((rh = Physics2D.Raycast(leftRayStart, Vector2.down, height, pc.groundMask)).collider != null && (ramp = rh.collider.GetComponent<Ramp>()) != null)
		|| ((rh = Physics2D.Raycast(rightRayStart, Vector2.down, height, pc.groundMask)).collider != null && (ramp = rh.collider.GetComponent<Ramp>()) != null))
		{
			Debug.Log("Player is on a ramp");
			return true;
		}
		
		return false;
	}

	private void CancelSkill(Vector2 direction)
	{
		if(rollDirection == Vector2.right)
		{
			if(direction.x >= -.7f)
			{
				ForcePlayerDirection();
				return;
			}
		}
		else if(rollDirection == Vector2.left)
		{
			if(direction.x <= .7f)
			{
				ForcePlayerDirection();
				return;
			}
		}

		//Cancel roll
		pc.canDestroyBlocks = false;
		pc.transform.localScale = Vector3.one;
		rollDirection = Vector2.zero;
	}

	private void ForcePlayerDirection()
	{
		pc.inputMovement = rollDirection;
	}
}
