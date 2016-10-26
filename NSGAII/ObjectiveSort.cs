using System;
using System.Collections.Generic;
using System.Linq;
using MultiObjectiveGA;

namespace NSGAII
{
	/// <summary>
	/// Objective sort method.
	/// </summary>
	/// 
	/// <remarks><para>Sort individuals from a specified front based on a selected objective value.</para></remarks> 
	/// 
	public class ObjectiveSort : ISortingFunction
	{
		private int _objectiveIndex;

		public ObjectiveSort(int objectiveIndex)
		{
			_objectiveIndex = objectiveIndex;
		}

		#region NSGAIISortingFunction implementation

		public void Sort (List<MOChromosome> front)
		{
			if(_objectiveIndex >= front[0].Objectives.Length)
				throw new ArgumentException( "Invalid ObjectiveIndex. Must be a valid index for your MOChromosome objectives." );

			front.Sort((x, y) => ((MOChromosome)y).Objectives[_objectiveIndex].CompareTo(((MOChromosome)x).Objectives[_objectiveIndex]));
		}

		#endregion
	}
}