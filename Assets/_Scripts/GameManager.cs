using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	[Header("PLAYER JOIN")]
	[SerializeField] private PlayerInputManager playerInputManager;
	[SerializeField] private GameObject player2Prefab;

	[Header("CAMERA")]
	[SerializeField] new private CinemachineCamera camera;
	[SerializeField] private CinemachineTargetGroup cameraTargetGroup;
	[SerializeField] private PlayerSkillList playerSkillList;

	private Vector3 spawnPos;
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
		playerInputManager.playerPrefab = player2Prefab;

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

	public void RespawnPlayer(PlayerController pc)
	{
		pc.transform.position = spawnPos;
	}
}
