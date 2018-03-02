using UnityEngine;
using System.Collections;
using Pathfinding;
using Pathfinding.RVO;
using System.Collections.Generic;

/** RVO Character Controller.
 * Designed to be used as a drop-in replacement for the Unity Character Controller,
 * it supports almost all of the same functions and fields with the exception
 * that due to the nature of the RVO implementation, desired velocity is set in the Move function
 * and is assumed to stay the same until something else is requested (as opposed to reset every frame).
 * 
 * For documentation of many of the variables of this class: refer to the Pathfinding.RVO.IAgent interface.
 * 
 * \note Requires an RVOSimulator in the scene
 * 
 * \see Pathfinding.RVO.IAgent
 * \see RVOSimulator
 * 
 * \astarpro 
 */
[AddComponentMenu("Local Avoidance/RVO Controller")]
public class RVOController : MonoBehaviour {

	public float radius = 5;
	public float maxSpeed = 2;
	public float height = 1;
	public bool locked = false;
	
	public float agentTimeHorizon = 2;
	public float obstacleTimeHorizon = 2;
	public float neighbourDist = 10;
	
	public bool debug = false;
	
	public LayerMask mask = -1;
	
	public float wallAvoidForce = 1;
	public float falloff = 1;
	
	public Vector3 center;
	
	private IAgent rvoAgent;
	
	private Simulator simulator;
	
	private float adjustedY = 0;
	
	private Transform tr;
	
	Vector3 desiredVelocity;
	
	/** Position for the previous frame */
	private Vector3 lastPosition;
	
	public Vector3 position {
		get { return rvoAgent.InterpolatedPosition; }
	}
	
	public Vector3 velocity {
		get { return rvoAgent.Velocity; }
	}
	
	public void OnDisable () {
		//Remove the agent from the simulation but keep the reference
		//this component might get enabled and then we can simply
		//add it to the simulation again
		simulator.RemoveAgent (rvoAgent);
	}
	
	public void Awake () {
		tr = transform;
		
		RVOSimulator sim = FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
		if (sim == null) {
			Debug.LogError ("No RVOSimulator component found in the scene. Please add one.");
			return;
		}
		simulator = sim.GetSimulator ();
	}
	
	public void OnEnable () {
		//We might have an rvoAgent
		//which was disabled previously
		//if so, we can simply add it to the simulation again
		if (rvoAgent != null) {
			simulator.AddAgent (rvoAgent);
		} else {
			rvoAgent = simulator.AddAgent (transform.position);
		}
		
		UpdateAgentProperties ();
		rvoAgent.Position = transform.position;
		adjustedY = rvoAgent.Position.y;
	}
	
	protected void UpdateAgentProperties () {
		rvoAgent.Radius = radius;
		rvoAgent.MaxSpeed = maxSpeed;
		rvoAgent.Height = height;
		rvoAgent.AgentTimeHorizon = agentTimeHorizon;
		rvoAgent.ObstacleTimeHorizon = obstacleTimeHorizon;
		rvoAgent.Locked = locked;
		rvoAgent.DebugDraw = debug;
	}
	
	public void Move (Vector3 vel) {
		desiredVelocity = vel;
	}
	
	public void Teleport (Vector3 pos) {
		tr.position = pos;
		lastPosition = pos;
		//rvoAgent.Position = pos;
		rvoAgent.Teleport (pos);
		adjustedY = pos.y;
	}
	
	public void Update () {
		if (lastPosition != tr.position) {
			Teleport (tr.position);
		}
		
		UpdateAgentProperties ();
		
		RaycastHit hit;
		
		//The non-interpolated position
		Vector3 realPos = rvoAgent.InterpolatedPosition;
		realPos.y = adjustedY;
		
		if (Physics.Raycast	(realPos + Vector3.up*height*0.5f,Vector3.down, out hit, float.PositiveInfinity, mask)) {
			adjustedY = hit.point.y;
		} else {
			adjustedY = 0;
		}
		realPos.y = adjustedY;
		
		rvoAgent.Position = new Vector3(rvoAgent.Position.x, adjustedY, rvoAgent.Position.z);
		
		List<ObstacleVertex> obst = rvoAgent.NeighbourObstacles;
		
		Vector3 force = Vector3.zero;
		
		for (int i=0;i<obst.Count;i++) {
			Vector3 a = obst[i].position;
			Vector3 b = obst[i].next.position;
			
			Vector3 closest = position - Mathfx.NearestPointStrict (a,b,position);
			
			if (closest == a || closest == b) continue;
			
			float dist = closest.sqrMagnitude;
			closest /= dist*falloff;
			force += closest;
		}
		
#if ASTARDEBUG
		Debug.DrawRay (position, desiredVelocity + force*wallAvoidForce);
#endif
		rvoAgent.DesiredVelocity = desiredVelocity + force*wallAvoidForce; 

		tr.position = rvoAgent.InterpolatedPosition + Vector3.up*height*0.5f + center;
		lastPosition = tr.position;
	}
}
