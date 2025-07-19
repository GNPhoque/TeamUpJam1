using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	public FusedPlayerInputManager fusedPlayerInputManager;
	[SerializeField] private float deathDuration;

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
		if(pc.attacker == EPlayerType.Duck)
		{
			fusedPlayerInputManager.SetDuckVisuals(pc.gameObject.transform.GetChild(0).gameObject);
		}
		else if(pc.attacker == EPlayerType.Beaver)
		{
			fusedPlayerInputManager.SetBeaverVisuals(pc.gameObject.transform.GetChild(0).gameObject);
		}

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

	public void RespawnPlayers()
	{
		StartCoroutine(RespawnPlayer(players.ElementAt(0)));
		StartCoroutine(RespawnPlayer(players.ElementAt(1)));
	}

	public IEnumerator RespawnPlayer(PlayerController pc)
	{
		pc.ResetInputMovement();
		//TODO  : despawn, anim, damage, respawn
		pc.transform.GetChild(0).gameObject.SetActive(false); //fade out
		pc.canMove = false;
		yield return new WaitForSeconds(deathDuration);
		pc.transform.position = spawnPos;
		camera.Follow = cameraTargetGroup.transform;
		yield return new WaitForSeconds(deathDuration);
		pc.transform.GetChild(0).gameObject.SetActive(true);
		pc.canMove = true;
	}

	public void UpdateCheckPoint(Vector2 newPos)
	{
		spawnPos = newPos;
	}
}
