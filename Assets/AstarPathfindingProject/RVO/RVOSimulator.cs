using UnityEngine;
using System.Collections;
using Pathfinding.RVO;

/** Unity front end for an RVO simulator.
 * Attached to any GameObject in a scene, scripts such as the RVOController will use the
 * simulator exposed by this class to handle their movement.
 * 
 * You can have more than one of these, however most script which make use of the RVOSimulator
 * will find it by FindObjectOfType, and thus only one will be used.
 * 
 * This is only a wrapper class for a Pathfinding.RVO.Simulator which simplifies exposing it
 * for a unity scene.
 * 
 * \see Pathfinding.RVO.Simulator
 * 
 * \astarpro 
 */
[AddComponentMenu("Local Avoidance/RVO Simulator")]
public class RVOSimulator : MonoBehaviour {
	
	/** Use Double Buffering.
	 * This will only be read at Awake.
	 * \see Pathfinding.RVO.Simulator.DoubleBuffering */
	public bool doubleBuffering = true;
	
	/** Interpolate positions between simulation timesteps.
	  * If you are using agents directly, make sure you read from the InterpolatedPosition property. */
	public bool interpolation = true;
	
	/** Desired FPS for rvo simulation.
	  * It is usually not necessary to run a crowd simulation at a very high fps.
	  * Usually 10-30 fps is enough, but can be increased for better quality.
	  * The rvo simulation will never run at a higher fps than the game */
	public int desiredSimulatonFPS = 20;
	
	/** Number of ROV worker threads.
	 * If set to None, no multithreading will be used. */
	public ThreadCount workerThreads = ThreadCount.Two;
	
	/** Reference to the internal simulator */
	Pathfinding.RVO.Simulator simulator;
	
	/** Get the internal simulator.
	 * Will never be null */
	public Simulator GetSimulator () {
		if (simulator == null) {
			Awake ();
		}
		return simulator;
	}
	
	void Awake () {
		if (desiredSimulatonFPS < 1) desiredSimulatonFPS = 1;
		
		if (simulator == null) {
			int threadCount = AstarPath.CalculateThreadCount (workerThreads);
			simulator = new Pathfinding.RVO.Simulator (threadCount, doubleBuffering);
			simulator.Interpolation = interpolation;
			simulator.DesiredDeltaTime = 1.0f / desiredSimulatonFPS;
		}
	}
	
	/** Update the simulation */
	void Update () {
		if (desiredSimulatonFPS < 1) desiredSimulatonFPS = 1;
		
		GetSimulator().DesiredDeltaTime = 1.0f / desiredSimulatonFPS;
		GetSimulator().Interpolation = interpolation;
		GetSimulator().Update ();
	}
}
