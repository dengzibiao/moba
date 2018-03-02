using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace Pathfinding {
//Binary Heap
	
#if false
	/** Binary heap implementation. Binary heaps are really fast for ordering nodes in a way that makes it possible to get the node with the lowest F score. Also known as a priority queue.
	 * \see http://en.wikipedia.org/wiki/Binary_heap
	 */
	public class BinaryHeap { 
		public Node[] binaryHeap; 
		public int numberOfItems; 
	
		public BinaryHeap( int numberOfElements ) { 
			binaryHeap = new Node[numberOfElements]; 
			numberOfItems = 2;
		} 
		
		/** Adds a node to the heap */
		public void Add(Node node) {
			
			if (node == null) {
				Debug.Log ("Sending null node to BinaryHeap");
				return;
			}
			
			if (numberOfItems == binaryHeap.Length) {
				Debug.Log ("Forced to discard nodes because of binary heap size limit, please consider increasing the size ("+numberOfItems +" "+binaryHeap.Length+")");
				numberOfItems--;
			}
			
			binaryHeap[numberOfItems] = node;
			//node.heapIndex = numberOfItems;//Heap index
			
			int bubbleIndex = numberOfItems;
			uint nodeF = node.f;
			
			while (bubbleIndex != 1) {
				int parentIndex = bubbleIndex / 2;
				
				/*if (binaryHeap[parentIndex] == null) {
					Debug.Log ("WUT!!");
					return;
				}*/
				
				if (nodeF <= binaryHeap[parentIndex].f) {
				   
					//binaryHeap[bubbleIndex].f <= binaryHeap[parentIndex].f) { /** \todo Wouldn't it be more efficient with '<' instead of '<=' ? * /
					//Node tmpValue = binaryHeap[parentIndex];
					
					//tmpValue.heapIndex = bubbleIndex;//HeapIndex
					
					binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
					binaryHeap[parentIndex] = node;//binaryHeap[bubbleIndex];
					
					//binaryHeap[bubbleIndex].heapIndex = bubbleIndex; //Heap index
					//binaryHeap[parentIndex].heapIndex = parentIndex; //Heap index
					
					bubbleIndex = parentIndex;
				} else {
				/*if (binaryHeap[bubbleIndex].f <= binaryHeap[parentIndex].f) { /** \todo Wouldn't it be more efficient with '<' instead of '<=' ? *
					Node tmpValue = binaryHeap[parentIndex];
					
					//tmpValue.heapIndex = bubbleIndex;//HeapIndex
					
					
					binaryHeap[parentIndex] = binaryHeap[bubbleIndex];
					binaryHeap[bubbleIndex] = tmpValue;
					
					bubbleIndex = parentIndex;
				} else {*/
					break;
				}
			}
								 
			numberOfItems++;
		}
		
		/** Returns the node with the lowest F score from the heap */
		public Node Remove() {
			numberOfItems--;
			Node returnItem = binaryHeap[1];
			
		 	//returnItem.heapIndex = 0;//Heap index
			
			binaryHeap[1] = binaryHeap[numberOfItems];
			//binaryHeap[1].heapIndex = 1;//Heap index
			
			int swapItem = 1, parent = 1;
			
			do {
				parent = swapItem;
				int p2 = parent * 2;
				if ((p2 + 1) <= numberOfItems) {
					// Both children exist
					if (binaryHeap[parent].f >= binaryHeap[p2].f) {
						swapItem = p2;//2 * parent;
					}
					if (binaryHeap[swapItem].f >= binaryHeap[p2 + 1].f) {
						swapItem = p2 + 1;
					}
				} else if ((p2) <= numberOfItems) {
					// Only one child exists
					if (binaryHeap[parent].f >= binaryHeap[p2].f) {
						swapItem = p2;
					}
				}
				
				// One if the parent's children are smaller or equal, swap them
				if (parent != swapItem) {
					Node tmpIndex = binaryHeap[parent];
					//tmpIndex.heapIndex = swapItem;//Heap index
					
					binaryHeap[parent] = binaryHeap[swapItem];
					binaryHeap[swapItem] = tmpIndex;
					
					//binaryHeap[parent].heapIndex = parent;//Heap index
				}
			} while (parent != swapItem);
			
			return returnItem;
		}
		
		/** \deprecated Use #Add instead */
		public void BubbleDown (Node node) {
			
			//int bubbleIndex = node.heapIndex;
			int bubbleIndex = 0;
			
			if (bubbleIndex < 1 || bubbleIndex > numberOfItems) {
				Debug.LogError ("Node is not in the heap (index "+bubbleIndex+")");
				Add (node);
				return;
			}
			
			while (bubbleIndex != 1) {
				int parentIndex = bubbleIndex / 2;
				
				/* Can be optimized to use 'node' instead of 'binaryHeap[bubbleIndex]' */
				if (binaryHeap[bubbleIndex].f <= binaryHeap[parentIndex].f) {
					Node tmpValue = binaryHeap[parentIndex];
					binaryHeap[parentIndex] = binaryHeap[bubbleIndex];
					binaryHeap[bubbleIndex] = tmpValue;
					
					//binaryHeap[parentIndex].heapIndex = parentIndex;
					//binaryHeap[bubbleIndex].heapIndex = bubbleIndex;
					
					bubbleIndex = parentIndex;
				} else {
					return;
				}
			}
		}
		
		/** Rebuilds the heap by trickeling down all items. Called after the hTarget on a path has been changed */
		public void Rebuild () {
if DEBUG
			int changes = 0;
endif
			
			for (int i=2;i<numberOfItems;i++) {
				int bubbleIndex = i;
				Node node = binaryHeap[i];
				uint nodeF = node.f;
				while (bubbleIndex != 1) {
					int parentIndex = bubbleIndex / 2;
					
					if (nodeF < binaryHeap[parentIndex].f) {
						//Node tmpValue = binaryHeap[parentIndex];
						binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
						binaryHeap[parentIndex] = node;
						bubbleIndex = parentIndex;
if DEBUG
						changes++;
endif
					} else {
						break;
					}
				}
				
			}
			
if DEBUG
			Debug.Log ("+++ Rebuilt Heap - "+changes+" changes +++");
endif
			
		}
		
		/** Rearranges a node in the heap which has got it's F score changed (only works for a lower F score). \warning This is slow, often it is more efficient to just add the node to the heap again */
		public void Rearrange (Node node) {
			
			for (int i=0;i<numberOfItems;i++) {
				if (binaryHeap[i] == node) {
					
					int bubbleIndex = i;
					while (bubbleIndex != 1) {
						int parentIndex = bubbleIndex / 2;
						
						if (binaryHeap[bubbleIndex].f <= binaryHeap[parentIndex].f) {
							Node tmpValue = binaryHeap[parentIndex];
							binaryHeap[parentIndex] = binaryHeap[bubbleIndex];
							binaryHeap[bubbleIndex] = tmpValue;
							bubbleIndex = parentIndex;
						} else {
							return;
						}
					}
				}
			}
		}
		
		/** Returns a nicely formatted string describing the tree structure. '!!!' marks after a value means that the tree is not correct at that node (i.e it should be swapped with it's parent) */
		public override string ToString () {
			System.Text.StringBuilder text = new System.Text.StringBuilder ();
			
			text.Append ("\n=== Writing Binary Heap ===\n");
			text.Append ("Number of items: ").Append (numberOfItems-2);
			text.Append ("Capacity: ").Append (binaryHeap.Length);
			text.Append ("\n");
			if (numberOfItems > 2) {
				WriteBranch (1,1,text);
			}
			text.Append ("\n\n");
			return text.ToString ();
		}
		
		/** Writes a branch of the tree to a StringBuilder. Used by #ToString */
		private void WriteBranch (int index, int depth, System.Text.StringBuilder text) {
			text.Append ("\n");
			for (int i=0;i<depth;i++) {
				text.Append ("   ");
			}
			
			text.Append (binaryHeap[index].f);
			
			if (index > 1) {
				int parentIndex = index / 2;
						
				if (binaryHeap[index].f < binaryHeap[parentIndex].f) {
					text.Append ("	!!!");
				}
			}
			
			int p2 = index * 2;
			if ((p2 + 1) <= numberOfItems) {
				// Both children exist
				WriteBranch (p2,depth+1,text);
				WriteBranch (p2+1,depth+1,text);
			} else if (p2 <= numberOfItems) {
				// Only one child exists
				WriteBranch (p2,depth+1,text);
			}
		}
		
	}
#endif
	
	/** Binary heap implementation. Binary heaps are really fast for ordering nodes in a way that makes it possible to get the node with the lowest F score. Also known as a priority queue.
	 * \see http://en.wikipedia.org/wiki/Binary_heap
	 */
	public class BinaryHeapM { 
		private NodeRun[] binaryHeap; 
		public int numberOfItems; 
		
		public float growthFactor = 2;
		
		public BinaryHeapM ( int numberOfElements ) { 
			binaryHeap = new NodeRun[numberOfElements]; 
			numberOfItems = 2;
		} 
		
		public void Clear () {
			numberOfItems = 1;
		}
		
		public NodeRun GetNode (int i) {
			return binaryHeap[i];
		}
		
		/** Adds a node to the heap */
		public void Add(NodeRun node) {
			
			if (node == null) throw new System.ArgumentNullException ("Sending null node to BinaryHeap");
			
			if (numberOfItems == binaryHeap.Length) {
				int newSize = System.Math.Max(binaryHeap.Length+4,(int)System.Math.Round(binaryHeap.Length*growthFactor));
				if (newSize > 1<<18) {
					throw new System.Exception ("Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. " +
						"\nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
				}
				
				NodeRun[] tmp = new NodeRun[newSize];
				for (int i=0;i<binaryHeap.Length;i++) {
					tmp[i] = binaryHeap[i];
				}
#if ASTARDEBUG
				Debug.Log ("Resizing binary heap to "+newSize);
#endif
				binaryHeap = tmp;
				
				//Debug.Log ("Forced to discard nodes because of binary heap size limit, please consider increasing the size ("+numberOfItems +" "+binaryHeap.Length+")");
				//numberOfItems--;
			}
			
			binaryHeap[numberOfItems] = node;
			//node.heapIndex = numberOfItems;//Heap index
			
			int bubbleIndex = numberOfItems;
			uint nodeF = node.f;
			
			while (bubbleIndex != 1) {
				int parentIndex = bubbleIndex / 2;
				
				if (nodeF < binaryHeap[parentIndex].f) {
				   	
					//binaryHeap[bubbleIndex].f <= binaryHeap[parentIndex].f) { /* \todo Wouldn't it be more efficient with '<' instead of '<=' ? * /
					//Node tmpValue = binaryHeap[parentIndex];
					
					//tmpValue.heapIndex = bubbleIndex;//HeapIndex
					
					binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
					binaryHeap[parentIndex] = node;//binaryHeap[bubbleIndex];
					
					//binaryHeap[bubbleIndex].heapIndex = bubbleIndex; //Heap index
					//binaryHeap[parentIndex].heapIndex = parentIndex; //Heap index
					
					bubbleIndex = parentIndex;
				} else {
				/*if (binaryHeap[bubbleIndex].f <= binaryHeap[parentIndex].f) { /* \todo Wouldn't it be more efficient with '<' instead of '<=' ? *
					Node tmpValue = binaryHeap[parentIndex];
					
					//tmpValue.heapIndex = bubbleIndex;//HeapIndex
					
					
					binaryHeap[parentIndex] = binaryHeap[bubbleIndex];
					binaryHeap[bubbleIndex] = tmpValue;
					
					bubbleIndex = parentIndex;
				} else {*/
					break;
				}
			}
								 
			numberOfItems++;
		}
		
		/** Returns the node with the lowest F score from the heap */
		public NodeRun Remove() {
			numberOfItems--;
			NodeRun returnItem = binaryHeap[1];
			
		 	//returnItem.heapIndex = 0;//Heap index
			
			binaryHeap[1] = binaryHeap[numberOfItems];
			//binaryHeap[1].heapIndex = 1;//Heap index
			
			int swapItem = 1, parent = 1;
			
			do {
				parent = swapItem;
				int p2 = parent * 2;
				if (p2 + 1 <= numberOfItems) {
					// Both children exist
					if (binaryHeap[parent].f >= binaryHeap[p2].f) {
						swapItem = p2;//2 * parent;
					}
					if (binaryHeap[swapItem].f >= binaryHeap[p2 + 1].f) {
						swapItem = p2 + 1;
					}
				} else if ((p2) <= numberOfItems) {
					// Only one child exists
					if (binaryHeap[parent].f >= binaryHeap[p2].f) {
						swapItem = p2;
					}
				}
				
				// One if the parent's children are smaller or equal, swap them
				if (parent != swapItem) {
					NodeRun tmpIndex = binaryHeap[parent];
					//tmpIndex.heapIndex = swapItem;//Heap index
					
					binaryHeap[parent] = binaryHeap[swapItem];
					binaryHeap[swapItem] = tmpIndex;
					
					//binaryHeap[parent].heapIndex = parent;//Heap index
				}
			} while (parent != swapItem);
			
			return returnItem;
		}
		
		/** Rebuilds the heap by trickeling down all items.
		 * Usually called after the hTarget on a path has been changed */
		public void Rebuild () {
#if ASTARDEBUG
			int changes = 0;
#endif
			
			for (int i=2;i<numberOfItems;i++) {
				int bubbleIndex = i;
				NodeRun node = binaryHeap[i];
				uint nodeF = node.f;
				while (bubbleIndex != 1) {
					int parentIndex = bubbleIndex / 2;
					
					if (nodeF < binaryHeap[parentIndex].f) {
						//Node tmpValue = binaryHeap[parentIndex];
						binaryHeap[bubbleIndex] = binaryHeap[parentIndex];
						binaryHeap[parentIndex] = node;
						bubbleIndex = parentIndex;
#if ASTARDEBUG
						changes++;
#endif
					} else {
						break;
					}
				}
				
			}
			
#if ASTARDEBUG
			Debug.Log ("+++ Rebuilt Heap - "+changes+" changes +++");
#endif
			
		}
	}
}