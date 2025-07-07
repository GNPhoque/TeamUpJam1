using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] new private CinemachineCamera camera;
	[SerializeField] private CinemachineTargetGroup cameraTargetGroup;
	[SerializeField] private PlayerSkillList playerSkillList;

	private Transform cameraTarget;
	private HashSet<PlayerController> players = new HashSet<PlayerController>();

	public static GameManager instance;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	private void Update()
	{
		if(cameraTarget != null && players.Count == 2)
		{
			cameraTarget.position = (players.ElementAt(0).transform.position + players.ElementAt(1).transform.position) / 2f;
		}
	}
	#endregion

	public void AddPlayer(PlayerController pc)
	{
		if (players.Count == 0)
		{
			pc.skill1 = playerSkillList.playerSkills[0];
			pc.skill1.Init(pc);
			pc.gameObject.layer = LayerMask.NameToLayer("Ground");
		}
		players.Add(pc);

		camera.Follow = cameraTargetGroup.transform;
		cameraTargetGroup.AddMember(pc.transform, 1f, 5f);
	}

	public void RemovePlayer(PlayerController pc)
	{
		if (players.Contains(pc))
		{
			players.Remove(pc);
			cameraTargetGroup.RemoveMember(pc.transform);
		}
	}

	private void UpdateCameraTarget()
	{

		if (players.Count == 1)
		{
			camera.Follow = players.ElementAt(0).transform;
		}
		if (players.Count == 2)
		{
			cameraTarget.position = (players.ElementAt(0).transform.position + players.ElementAt(1).transform.position) / 2f;
			camera.Follow = cameraTarget;
		}
	}
}
