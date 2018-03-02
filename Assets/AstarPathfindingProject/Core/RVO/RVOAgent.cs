//#define ASTARDEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.RVO {
	/** Exposes properties of an Agent class.
	  * \astarpro */
	public interface IAgent {
		/** Interpolated position of agent.
		 * Will be interpolated if the interpolation setting is enabled on the simulator.
		 */
		Vector3 InterpolatedPosition {get;}
		
		/** Position of the agent.
		 * This can be changed manually if you need to reposition the agent, but if you are reading from InterpolatedPosition, it will not update interpolated position
		 * until the next simulation step.
		 * \see Position
		 */
		Vector3 Position {get; set;}
		
		/** Desired velocity of the agent.
		 * Usually you set this once per frame. The agent will try move as close to the desired velocity as possible.
		 * Will take effect at the next simulation step.
		 */
		Vector3 DesiredVelocity {get; set;}
		/** Velocity of the agent.
		 * Can be used to set the rotation of the rendered agent.
		 * But smoothing is recommended if you do so since it might be a bit unstable when the velocity is very low.
		 * 
		 * You can set this variable manually,but it is not recommended to do so unless
		 * you have good reasons since it might degrade the quality of the simulation.
		 */
		Vector3 Velocity {get; set;}
		
		/** Locked agents will not move */
		bool Locked {get; set;}
		
		/** Radius of the agent.
		 * Agents are modelled as circles/cylinders */
		float Radius {get; set;}
		
		/** Height of the agent */
		float Height {get; set;}
		
		/** Max speed of the agent. In units per second  */
		float MaxSpeed {get; set;}
		
		/** Max distance to other agents to take them into account.
		 * Decreasing this value can lead to better performance, increasing it can lead to better quality of the simulation.
		 */
		float NeighbourDist {get; set;}
		
		/** Max number of estimated seconds to look into the future for collisions with agents.
		  * As it turns out, this variable is also very good for controling agent avoidance priorities.
		  * Agents with lower values will avoid other agents less and thus you can make 'high priority agents' by
		  * giving them a lower value.
		  */
		float AgentTimeHorizon {get; set;}
		/** Max number of estimated seconds to look into the future for collisions with obstacles */
		float ObstacleTimeHorizon {get; set;}
		
		/** Debug drawing */
		bool DebugDraw {get; set;}
		
		/** Max number of agents to take into account.
		 * Decreasing this value can lead to better performance, increasing it can lead to better quality of the simulation.
		 */
		int MaxNeighbours {get; set;}
		
		/** List of obstacle segments which were close to the agent during the last simulation step.
		 * Can be used to apply additional wall avoidance forces for example.
		 * Segments are formed by the obstacle vertex and its .next property.
		 */
		List<ObstacleVertex> NeighbourObstacles {get; }
		
		/** Teleports the agent to a new position.
		 * Just setting the position can cause strange effects when using interpolation.
		 */
		void Teleport (Vector3 pos);
	}
	
	/** RVO Agent.
	 * Handles calculation of an individual agent's velocity.
	 * \astarpro 
	 */
	public class Agent : IAgent {
	
		Vector3 smoothPos;
		
		public Vector3 Position {
			get;
			set;
		}
		
		public Vector3 InterpolatedPosition {
			get { return smoothPos; }
		}
		
		public Vector3 DesiredVelocity { get; set; }
		
		public void Teleport (Vector3 pos) {
			Position = pos;
			smoothPos = pos;
			prevSmoothPos = pos;
		}
		
		//Current values for double buffer calculation
		
		public float radius, height, maxSpeed, neighbourDist, agentTimeHorizon, obstacleTimeHorizon, weight;
		public bool locked = false;
		
		public int maxNeighbours;
		public Vector3 position, desiredVelocity, prevSmoothPos;
		
		public bool Locked {get; set;}
		public float Radius {get; set;}
		public float Height {get; set;}
		public float MaxSpeed {get; set;}
		public float NeighbourDist {get; set;}
		public float AgentTimeHorizon {get; set;}
		public float ObstacleTimeHorizon {get; set;}
		public Vector3 Velocity {get; set;}
		public bool DebugDraw {get; set;}
		
		public int MaxNeighbours {get; set;}
		
		private Vector3 velocity;
		private Vector3 newVelocity;
		
		/** Simulator which handles this agent.
		 * Used by this script as a reference and to prevent
		 * adding this agent to multiple simulations.
		 */
		public Simulator simulator;
		
		List<Agent> neighbours = new List<Agent>();
		List<float> neighbourDists = new List<float>();
		List<ObstacleVertex> obstaclesBuffered = new List<ObstacleVertex>();
		List<ObstacleVertex> obstacles = new List<ObstacleVertex>();
		List<float> obstacleDists = new List<float>();
		
		List<Line> orcaLines = new List<Line>();
		
		List<Line> projLines = new List<Line>();
		
		public List<ObstacleVertex> NeighbourObstacles {
			get {
				return obstaclesBuffered;
			}
		}
		
		public Agent (Vector3 pos) {
			MaxSpeed = 2;
			NeighbourDist = 15;
			AgentTimeHorizon = 2;
			ObstacleTimeHorizon = 2;
			Height = 5;
			Radius = 5;
			MaxNeighbours = 10;
			Locked = false;
			
			position = pos;
			Position = position;
			prevSmoothPos = position;
			smoothPos = position;
		}
		
		public void PreUpdatePosition () {
			position = position + Velocity*simulator.PrevDeltaTime;
		}
		
		public void BufferSwitch () {
			// <==
			radius = Radius;
			height = Height;
			maxSpeed = MaxSpeed;
			neighbourDist = NeighbourDist;
			agentTimeHorizon = AgentTimeHorizon;
			obstacleTimeHorizon = ObstacleTimeHorizon;
			maxNeighbours = MaxNeighbours;
			desiredVelocity = DesiredVelocity;
			locked = Locked;
			
			//position = Position;
			
			// ==>
			Velocity = velocity;
			List<ObstacleVertex> tmp = obstaclesBuffered;
			obstaclesBuffered = obstacles;
			obstacles = tmp;
		}
		
		// Update is called once per frame
		public void Update () {
			velocity = newVelocity;
			
			prevSmoothPos = smoothPos;
			
			//Note the case P/p
			position = Position;
			position = position + velocity * simulator.DeltaTime;
			Position = position;
		}
		
		public void Interpolate (float t) {
			if (t == 1.0f) smoothPos = position;
			else smoothPos = prevSmoothPos + (position-prevSmoothPos)*t;
		}
		
		public static System.Diagnostics.Stopwatch watch1 = new System.Diagnostics.Stopwatch();
		public static System.Diagnostics.Stopwatch watch2 = new System.Diagnostics.Stopwatch();
		
		public void CalculateNeighbours () {
			neighbours.Clear ();
			neighbourDists.Clear ();
			float rangeSq;
			
			//watch1.Start ();
			if (MaxNeighbours > 0) {
				rangeSq = neighbourDist*neighbourDist;// 
				
				simulator.KDTree.GetAgentNeighbours (this, rangeSq);
			}
			//watch1.Stop ();
			
			obstacles.Clear ();
			obstacleDists.Clear ();
			
			//watch2.Start ();
			rangeSq = (obstacleTimeHorizon * maxSpeed + radius);
			rangeSq *= rangeSq;
			simulator.KDTree.GetObstacleNeighbours (this, rangeSq);
			//watch2.Stop ();
			
		}
		
		public float det (Vector3 a, Vector3 b) {
			return a.x * b.z - a.z * b.x;
		}
		
		public float det (Vector2 a, Vector2 b) {
			return a.x * b.y - a.y * b.x;
		}
		
		public void CalculateVelocity () {
			orcaLines.Clear ();
			
			if (locked) {
				newVelocity = new Vector3(0,0,0);
				return;
			}
			
			Vector2 velocity2D = new Vector2 (Velocity.x, Velocity.z);
			
#if ASTARDEBUG
			if (DebugDraw) {
				Debug.DrawRay (position,desiredVelocity,Color.green);
				Debug.DrawRay (position,Velocity,Color.blue);
			}
#endif
			
			for (int i=0;i<obstacles.Count;i++) {
				float invTimeHorizonObst = 1.0f / obstacleTimeHorizon;
				
				ObstacleVertex ob1 = obstacles[i];
				ObstacleVertex ob2 = ob1.next;
				
				float agentRadius = radius;
				
				float obstacleYMin = System.Math.Min (ob1.position.y,ob2.position.y);
				float obstacleYMax = System.Math.Max (ob1.position.y+ob1.height,ob2.position.y+ob2.height);
				
				float intersectYMin = System.Math.Max (obstacleYMin, position.y);
				float intersectYMax = System.Math.Min (obstacleYMax, position.y+height);
				
				if (intersectYMax - intersectYMin < 0) {
					continue;
				}
				
				Vector2 dir1 = new Vector2 (ob1.position.x - position.x, ob1.position.z - position.z);
				Vector2 dir2 = new Vector2 (ob2.position.x - position.x, ob2.position.z - position.z);
				
				
				Vector2 obstVector = new Vector2 (ob2.position.x - ob1.position.x,ob2.position.z - ob1.position.z);
				
				/* 
				 * Check if velocity obstacle of obstacle is already taken care of by
				 * previously constructed obstacle ORCA lines.
				 */
				bool alreadyCovered = false;
			
				for (int j = 0; j < orcaLines.Count; j++) {
					if (det(invTimeHorizonObst * dir1 - orcaLines[j].point, orcaLines[j].dir) - invTimeHorizonObst * agentRadius >= -float.Epsilon && det(invTimeHorizonObst * dir2 - orcaLines[j].point, orcaLines[j].dir) - invTimeHorizonObst * agentRadius >=  -float.Epsilon) {
						alreadyCovered = true;
						break;
					}
				}
			
				if (alreadyCovered) {
					continue;
				}
				
#if ASTARDEBUG
				Debug.DrawLine (ob1.position+Vector3.up,ob2.position+Vector3.up,Color.black);
				Debug.DrawRay (ob1.position + Vector3.up, new Vector3 (ob1.dir.x, 0, ob1.dir.y), Color.red);
				Debug.DrawRay (ob2.position + Vector3.up, new Vector3 (ob2.dir.x, 0, ob2.dir.y), Color.red);
#endif
				
				/* 
				   * Compute legs. When obliquely viewed, both legs can come from a single
				   * vertex. Legs can never point into neighboring edge when convex vertex,
				   * take cutoff-line of neighboring edge instead. If velocity projected on
				   * "foreign" leg, no constraint is added. Legs extend cut-off line when
				   * nonconvex vertex.
				   */
				
				float distSq1 = dir1.sqrMagnitude;
				float distSq2 = dir2.sqrMagnitude;
				
				float radiusSq = agentRadius*agentRadius;
				
				Vector2 leftLegDir, rightLegDir;
				
				float s = Vector2.Dot (-dir1, obstVector) / obstVector.sqrMagnitude;
				float distSqLine = (-dir1 - s*obstVector).sqrMagnitude;
				
				//invTimeHorizonObst = 1.0f / simulator.DeltaTime;
				
				//ORCA Line
				Line line;
				
				if (s < 0 && distSq1 <= radiusSq) {
					/* Left Vertex Collision */
					
					if (ob1.convex) {
						line.point = new Vector2(0, 0);
                        line.dir = (new Vector2(-dir1.y, dir1.x)).normalized;
                        orcaLines.Add(line);
					}
					continue;
				} else if (s > 1.0f && distSq2 <= radiusSq) {
					/* Right Vertex Collision */
					
					if (ob2.convex && det(dir2, ob2.dir) >= 0) {
						line.point = new Vector2(0, 0);
                        line.dir = (new Vector2(-dir2.y, dir2.x)).normalized;
                        orcaLines.Add(line);
					}
					continue;
				} else if (s >= 0 && s < 1 && distSqLine <= radiusSq) {
					/* Obstacle Segment Collision */
					
					line.point = new Vector2(0, 0);
                    line.dir = -ob1.dir;
                    orcaLines.Add(line);
					continue;
				}
				
				/* 
                 * No collision.  
                 * Compute legs. When obliquely viewed, both legs can come from a single
                 * vertex. Legs extend cut-off line when nonconvex vertex.
                 */
				
				if (s < 0 && distSqLine <= radiusSq) {
					/* No Collision, but obstacle viewed obliquely so that left vertex
					 * defines velocity obstacle
					 */
					if (!ob1.convex) continue;
					
					ob2 = ob1;
					
					//Wait wut... what is being done here?
					float leg1 = (float)System.Math.Sqrt (distSq1 - radiusSq);
					
					leftLegDir = new Vector2(dir1.x*leg1 - dir1.y*agentRadius, dir1.x*agentRadius + dir1.y*leg1) / distSq1;
					rightLegDir = new Vector2 (dir1.x*leg1 + dir1.y*agentRadius, -dir1.x * agentRadius + dir1.y * leg1) / distSq1;
				
				} else if (s > 1 && distSqLine <= radiusSq) {
					/* No Collision, but obstacle viewed obliquely so that right vertex
					 * defines velocity obstacle
					 */
					if (!ob1.convex) continue;
					
					ob1 = ob2;
					
					//Wait wut... what is being done here?
					float leg2 = (float)System.Math.Sqrt (distSq2 - radiusSq);
					
					leftLegDir = new Vector2(dir2.x*leg2 - dir2.y*agentRadius, dir2.x*agentRadius + dir2.y*leg2) / distSq2;
					rightLegDir = new Vector2 (dir2.x*leg2 + dir2.y*agentRadius, -dir2.x * agentRadius + dir2.y * leg2) / distSq2;
				
				} else {
					/* Usual situation */
					
					if (ob1.convex) {
						float leg1 = (float)System.Math.Sqrt (distSq1 - radiusSq);
						
						leftLegDir = new Vector2(dir1.x*leg1 - dir1.y*agentRadius, dir1.x*agentRadius + dir1.y*leg1) / distSq1;
					} else {
						/* Left vertex non-convex; left leg extends cut-off line. */
						leftLegDir = -ob1.dir;
					}
					
					if (ob2.convex) {
						float leg2 = (float)System.Math.Sqrt (distSq2 - radiusSq);
						rightLegDir = new Vector2 (dir2.x*leg2 + dir2.y*agentRadius, -dir2.x * agentRadius + dir2.y * leg2) / distSq2;
					} else {
						/* Right vertex non-convex; right leg extends cut-off line. */
						rightLegDir = ob1.dir;//(ob1.convex ? obstVector.normalized : -leftLegDir);
					}
				}
				
				/* 
				 * Legs can never point into neighboring edge when convex vertex,
				 * take cutoff-line of neighboring edge instead. If velocity projected on
				 * "foreign" leg, no constraint is added. 
				 */
				
				ObstacleVertex leftNeighbor = ob1.prev;
				
				bool isLeftLegForeign = false;
				bool isRightLegForeign = false;

				if (ob1.convex && det(leftLegDir, -leftNeighbor.dir) >= 0.0f)
				{
					/* Left leg points into obstacle. */
					leftLegDir = -leftNeighbor.dir;
					isLeftLegForeign = true;
				}

				if (ob2.convex && det(rightLegDir, ob2.dir) <= 0.0f)
				{
					/* Right leg points into obstacle. */
					rightLegDir = ob2.dir;
					isRightLegForeign = true;
				}

				/* Compute cut-off centers. */
				Vector2 leftCutoff = invTimeHorizonObst * new Vector2 (ob1.position.x - position.x,ob1.position.z-position.z);
				Vector2 rightCutoff = invTimeHorizonObst * new Vector2 (ob2.position.x - position.x,ob2.position.z-position.z);
				Vector2 cutoffVec = rightCutoff - leftCutoff;

				/* Project current velocity on velocity obstacle. */

				/* Check if current velocity is projected on cutoff circles. */
				float t = (ob1 == ob2 ? 0.5f : Vector2.Dot ((velocity2D - leftCutoff) ,cutoffVec)) / cutoffVec.sqrMagnitude;
				float tLeft = Vector2.Dot (velocity2D - leftCutoff , leftLegDir);
				float tRight = Vector2.Dot (velocity2D - rightCutoff , rightLegDir);

				if ((t < 0.0f && tLeft < 0.0f) || (ob1 == ob2 && tLeft < 0.0f && tRight < 0.0f))
				{
					/* Project on left cut-off circle. */
					Vector2 unitW = (velocity2D - leftCutoff).normalized;

					line.dir = new Vector2(unitW.y, -unitW.x);
					line.point = leftCutoff + agentRadius * invTimeHorizonObst * unitW;
					orcaLines.Add(line);
					continue;
				}
				else if (t > 1.0f && tRight < 0.0f)
				{
					/* Project on right cut-off circle. */
					Vector2 unitW = (velocity2D - rightCutoff).normalized;
	
					line.dir = new Vector2(unitW.y, -unitW.x);
					line.point = rightCutoff + agentRadius * invTimeHorizonObst * unitW;
					orcaLines.Add(line);
					continue;
				}
				
				
				/* 
				 * Project on left leg, right leg, or cut-off line, whichever is closest
				 * to velocity.
				 */
				float distSqCutoff = ((t < 0.0f || t > 1.0f || ob1 == ob2) ?
				                      float.PositiveInfinity : (velocity2D - (leftCutoff + t * cutoffVec)).sqrMagnitude);
				float distSqLeft = ((tLeft < 0.0f) ?
				                    float.PositiveInfinity : (velocity2D - (leftCutoff + tLeft * leftLegDir)).sqrMagnitude);
				float distSqRight = ((tRight < 0.0f) ?
				                     float.PositiveInfinity : (velocity2D - (rightCutoff + tRight * rightLegDir)).sqrMagnitude);

				if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight)
				{
					/* Project on cut-off line. */
					line.dir = -ob1.dir;
					line.point = leftCutoff + agentRadius * invTimeHorizonObst * new Vector2(-line.dir.y, line.dir.x);
					orcaLines.Add(line);
					continue;
				}
				else if (distSqLeft <= distSqRight)
				{
					/* Project on left leg. */
					if (isLeftLegForeign)
					{
						continue;
					}

					line.dir = leftLegDir;
					line.point = leftCutoff + agentRadius * invTimeHorizonObst * new Vector2(-line.dir.y, line.dir.x);
					orcaLines.Add(line);
					continue;
				}
				else
				{
					/* Project on right leg. */
					if (isRightLegForeign)
					{
						continue;
					}

					line.dir = -rightLegDir;
					line.point = rightCutoff + agentRadius * invTimeHorizonObst * new Vector2(-line.dir.y, line.dir.x);
					orcaLines.Add(line);
					continue;
				}
			}
			
			
			//Phase II

			float invTimeHorizon = 1.0f / agentTimeHorizon;
			
			int numObstacleLines = orcaLines.Count;
			
			/* Create agent ORCA lines. */
			for (int i = 0; i < neighbours.Count; ++i)
			{
				Agent other = neighbours[i];
				
				//Debug.DrawLine (position,other.position,Color.red);
				
				float maxY = System.Math.Min (position.y+height,other.position.y+other.height);
				float minY = System.Math.Max (position.y,other.position.y);
				
				//The agents cannot collide since they
				//are on different y-levels
				if (maxY - minY < 0) {
					continue;
				}
				
				Vector3 relativePosition3D = (other.position - position);
				Vector3 relativeVelocity3D = Velocity - other.Velocity;
				
				Vector2 relativePosition = new Vector2 (relativePosition3D.x,relativePosition3D.z);
				Vector2 relativeVelocity = new Vector2 (relativeVelocity3D.x,relativeVelocity3D.z);
				
				float distSq = relativePosition.sqrMagnitude;
				float combinedRadius = radius + other.radius;
				float combinedRadiusSq = combinedRadius*combinedRadius;

				Line line;
				Vector2 u;
				
				if (distSq > combinedRadiusSq)
				{
					/* No collision. */
					Vector2 w = relativeVelocity - invTimeHorizon * relativePosition;
					/* Vector from cutoff center to relative velocity. */
					float wLengthSq = w.sqrMagnitude;

					float dotProduct1 = Vector2.Dot (w, relativePosition);

					if (dotProduct1 < 0.0f && dotProduct1*dotProduct1 > combinedRadiusSq * wLengthSq)
					{
						/* Project on cut-off circle. */
						float wLength = (float)System.Math.Sqrt (wLengthSq);
						
						//Normalized w
						Vector2 unitW = w / wLength;

						line.dir = new Vector2(unitW.y, -unitW.x);
						u = (combinedRadius * invTimeHorizon - wLength) * unitW;
						
#if ASTARDEBUG
						if (DebugDraw)
							Debug.DrawRay (position, u,Color.red);	
#endif
					}
					else
					{
						/* Project on legs. */
						float leg = (float)System.Math.Sqrt (distSq - combinedRadiusSq);

						if (det(relativePosition, w) > 0.0f)
						{
							/* Project on left leg. */
							line.dir = new Vector2(relativePosition.x * leg - relativePosition.y * combinedRadius, relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
						}
						else
						{
							/* Project on right leg. */
							line.dir = -new Vector2(relativePosition.x * leg + relativePosition.y * combinedRadius, -relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
						}

						float dotProduct2 = Vector2.Dot (relativeVelocity, line.dir);

						u = dotProduct2 * line.dir - relativeVelocity;
					}
				} else {
					/* Collision. Project on cut-off circle of time timeStep. */
					float invTimeStep = 1.0f / simulator.DeltaTime;

					/* Vector from cutoff center to relative velocity. */
					Vector2 w = relativeVelocity - invTimeStep * relativePosition;
					//vel - pos/delta
					
					float wLength = w.magnitude;
					
					//Normalized w
					Vector2 unitW = w / wLength;

					line.dir = new Vector2(unitW.y, -unitW.x);
					u = (combinedRadius * invTimeStep - wLength) * unitW;
					
					
				}
				
				if (other.locked)
					line.point = velocity2D + 1.0f * u;

				line.point = velocity2D + 0.5f * u;
				
				orcaLines.Add(line);
			}
			
#if ASTARDEBUG
			if (DebugDraw) {
				for (int i=0;i<orcaLines.Count;i++) {
					Debug.DrawRay (position+new Vector3 (orcaLines[i].point.x,0,orcaLines[i].point.y)-new Vector3 (orcaLines[i].dir.x,0,orcaLines[i].dir.y)*100,new Vector3 (orcaLines[i].dir.x,0,orcaLines[i].dir.y)*200,Color.blue);
				}
			}
#endif
			
			Vector2 resultVelocity = Vector2.zero;
			
			int lineFail = LinearProgram2(orcaLines, maxSpeed, new Vector2 (desiredVelocity.x,desiredVelocity.z), false, ref resultVelocity);

		    if (lineFail < orcaLines.Count)
			{
				LinearProgram3(orcaLines, numObstacleLines, lineFail, maxSpeed, ref resultVelocity);
			}
			
			newVelocity = new Vector3(resultVelocity.x,0,resultVelocity.y);
			newVelocity = Vector3.ClampMagnitude (newVelocity, maxSpeed);
			
#if ASTARDEBUG
			if (DebugDraw)
				Debug.DrawRay (position,newVelocity,Color.magenta);
#endif
		}
		
		private float Sqr (float v) { return v*v; }
		
		public float InsertAgentNeighbour (Agent agent, float rangeSq) {
			if (this == agent) return rangeSq;
			
			//2D Dist
			float dist = Sqr(agent.position.x-position.x) + Sqr(agent.position.z - position.z);
			
			if (dist < rangeSq) {
				if (neighbours.Count < maxNeighbours) {
					neighbours.Add (agent);
					neighbourDists.Add (dist);
				}
				/** \bug This starts to compare with a lower agent if neighbours.Count >= maxNeighbours */
				int i = neighbours.Count-1;
				while ( i != 0 && dist < neighbourDists[i-1]) {
					neighbours[i] = neighbours[i-1];
					neighbourDists[i] = neighbourDists[i-1];
					i--;
				}
				neighbours[i] = agent;
				neighbourDists[i] = dist;
				
				if (neighbours.Count == maxNeighbours) {
					rangeSq = neighbourDists[neighbourDists.Count-1];
				}
			}
			return rangeSq;
		}
		
		private static float DistSqPointLineSegment(Vector3 a, Vector3 b, Vector3 c) {
			return DistSqPointLineSegment ( new Vector2(a.x,a.z), new Vector2(b.x,b.z), new Vector2(c.x,c.z));
		}
		
		private static float DistSqPointLineSegment(Vector2 a, Vector2 b, Vector2 c) {
			float r = Vector2.Dot (c - a,  b - a) / (b - a).sqrMagnitude;
			
			if (r < 0.0f)
				return (c - a).sqrMagnitude;
			else if (r > 1.0f)
				return (c - b).sqrMagnitude;
			else
				return (c - (a + r * (b - a))).sqrMagnitude;
		}
		
		public void InsertObstacleNeighbour (ObstacleVertex ob1, float rangeSq) {

			ObstacleVertex ob2 = ob1.next;
			
			float dist = Mathfx.DistancePointSegmentStrict (ob1.position,ob2.position, Position);
			
			if (dist < rangeSq) {
				obstacles.Add (ob1);
				obstacleDists.Add (dist);
				
				int i = obstacles.Count-1;
				while ( i != 0 && dist < obstacleDists[i-1]) {
					obstacles[i] = obstacles[i-1];
					obstacleDists[i] = obstacleDists[i-1];
					i--;
				}
				obstacles[i] = ob1;
				obstacleDists[i] = dist;
			}
		}
		
		bool LinearProgram1(List<Line> lines, int lineNo, float radius, Vector2 optVelocity, bool directionOpt, ref Vector2 result) {
			float dotProduct = Vector2.Dot (lines[lineNo].point, lines[lineNo].dir);
			float discriminant = dotProduct*dotProduct + radius*radius - lines[lineNo].point.sqrMagnitude;

			if (discriminant < 0.0f) {
				/* Max speed circle fully invalidates line lineNo. */
				return false;
			}

			float sqrtDiscriminant = (float)System.Math.Sqrt (discriminant);
			float tLeft = -dotProduct - sqrtDiscriminant;
			float tRight = -dotProduct + sqrtDiscriminant;
			
			for (int i = 0; i < lineNo; i++) {
				//Calculate Intersection
				float denominator = det(lines[lineNo].dir, lines[i].dir);
				float numerator = det(lines[i].dir, lines[lineNo].point - lines[i].point);

				if (System.Math.Abs(denominator) <= float.Epsilon) {
					/* Lines lineNo and i are (almost) parallel. */
					if (numerator < 0.0f) {
						return false;
					} else {
						continue;
					}
				}

				float t = numerator / denominator;

				if (denominator >= 0.0f) {
					/* Line i bounds line lineNo on the right. */
					tRight = System.Math.Min(tRight, t);
				} else {
					/* Line i bounds line lineNo on the left. */
					tLeft = System.Math.Max(tLeft, t);
				}

				if (tLeft > tRight) {
					return false;
				}
			}

			if (directionOpt)  {
				/* Optimize direction. */
				if (Vector2.Dot (optVelocity, lines[lineNo].dir) > 0.0f) {
					/* Take right extreme. */
					result = lines[lineNo].point + tRight * lines[lineNo].dir;
				} else {
					/* Take left extreme. */
					result = lines[lineNo].point + tLeft * lines[lineNo].dir;
				}
			} else {
				/* Optimize closest point. */
				float t = Vector2.Dot (lines[lineNo].dir, optVelocity - lines[lineNo].point);

				if (t < tLeft) {
					result = lines[lineNo].point + tLeft * lines[lineNo].dir;
				} else if (t > tRight) {
					result = lines[lineNo].point + tRight * lines[lineNo].dir;
				} else {
					result = lines[lineNo].point + t * lines[lineNo].dir;
				}
			}

			return true;
		}
		
		int LinearProgram2(List<Line> lines, float radius, Vector2 optVelocity, bool directionOpt, ref Vector2 result) {
			if (directionOpt) {
				/* Optimize direction. Note that the optimization velocity is of unit
				 * length in this case.
				 */
				result = optVelocity * radius;
			}
			else if (optVelocity.sqrMagnitude > radius*radius) {
				/* Optimize closest point and outside circle. */
				result = optVelocity.normalized * radius;
			} else {
				/* Optimize closest point and inside circle. */
				result = optVelocity;
			}

			for (int i = 0; i < lines.Count; ++i) {
				if (det(lines[i].dir, lines[i].point - result) > 0.0f) {
					/* Result does not satisfy constraint i. Compute new optimal result. */
					Vector2 tempResult = result;
					if (!LinearProgram1(lines, i, radius, optVelocity, directionOpt, ref result)) {
						result = tempResult;
						return i;
					}
				}
			}

			return lines.Count;
		}
		
		void LinearProgram3(List<Line> lines, int numObstLines, int beginLine, float radius, ref Vector2 result) {
			float distance = 0.0f;

			for (int i = beginLine; i < lines.Count; i++) {
				if (det(lines[i].dir, lines[i].point - result) > distance) {
					/* Result does not satisfy constraint of line i. */
					//std.vector<Line> projLines(lines.begin(), lines.begin() + numObstLines);
					projLines.Clear ();
					
					for (int j = 0; j < numObstLines; ++j) {
						projLines.Add(lines[j]);
					}

					for (int j = numObstLines; j < i; ++j) {
						Line line;

						float determinant = det(lines[i].dir, lines[j].dir);

						if (System.Math.Abs(determinant) <= float.Epsilon) {
							/* Line i and line j are parallel. */
							if (Vector2.Dot (lines[i].dir, lines[j].dir) > 0.0f) {
								/* Line i and line j point in the same direction. */
								continue;
							} else {
								/* Line i and line j point in opposite direction. */
								line.point = 0.5f * (lines[i].point + lines[j].point);
							}
						} else {
							line.point = lines[i].point + (det(lines[j].dir, lines[i].point - lines[j].point) / determinant) * lines[i].dir;
						}

						line.dir = (lines[j].dir - lines[i].dir).normalized;
						projLines.Add(line);
					}

					Vector2 tempResult = result;
					if (LinearProgram2(projLines, radius, new Vector2(-lines[i].dir.y, lines[i].dir.x), true, ref result) < projLines.Count) {
						/* This should in principle not happen.  The result is by definition
						 * already in the feasible region of this linear program. If it fails,
						 * it is due to small floating point error, and the current result is
						 * kept.
						 */
						result = tempResult;
					}

					distance = det(lines[i].dir, lines[i].point - result);
				}
			}
		}
	}
}