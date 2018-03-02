using System;
using UnityEngine;

public interface IStatus
{
	void Init (IActorController controller, StateHelper helper);

	STATUS GetNextStatus();

	void UpdateLogic();

	void OnLeave(STATUS next);

	bool OnEnter(STATUS last);
}