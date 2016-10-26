using System;
using System.Collections.Generic;
using System.Linq;
using MultiObjectiveGA;

namespace NSGAII
{
	/// <summary>
	/// Crowding distance method.
	/// </summary>
	/// 
	/// <remarks><para>Sort individuals from a specified front based on the Crowding Distance function.</para></remarks> 
	/// 
	public class CrowdingDistance : ISortingFunction
	{
		private ObjectiveMinMax[] _minMax;

		private int[] _objectiveIndexes;

		public struct ObjectiveMinMax
		{
			public double MinValue;
			public double MaxValue;
			
			public ObjectiveMinMax(int minValue, int maxValue)
			{
				MinValue = minValue;
				MaxValue = maxValue;
			}
		}

		private class CrowdingDistanceChromosome
		{
			public MOChromosome Chromosome;
			public double CrowdDistance;

			public CrowdingDistanceChromosome(MOChromosome chromosome, double crowdDistance)
			{
				Chromosome = chromosome;
				CrowdDistance = crowdDistance;
			}
		}

		public CrowdingDistance(ObjectiveMinMax[] minMax, int[] objectiveIndexes)
		{
			_minMax = minMax;
			_objectiveIndexes = objectiveIndexes;
		}

		#region NSGAIISortingFunction implementation

		public void Sort (List<MOChromosome> front)
		{
			if(_minMax.Length != _objectiveIndexes.Length)
				throw new ArgumentException( "Invalid MinMax size. Must be the same size as the objectives list in your NSGAII." );

			List<CrowdingDistanceChromosome> auxFront = new List<CrowdingDistanceChromosome>();

			for (int i = 0; i < front.Count; i++)
			{
				auxFront.Add(new CrowdingDistanceChromosome(front[i], 0));
			}

			for (int i = 0; i < _minMax.Length; i++)
			{
				int index = _objectiveIndexes[i];

				auxFront.Sort((x, y) => x.Chromosome.Objectives[index].CompareTo(y.Chromosome.Objectives[index]));
				
				auxFront[0].CrowdDistance = double.PositiveInfinity;
				auxFront[auxFront.Count - 1].CrowdDistance = double.PositiveInfinity;
				
				for (int j = 1; j < auxFront.Count - 1; j++)
				{
					auxFront[j].CrowdDistance = auxFront[j].CrowdDistance + (auxFront[j + 1].Chromosome.Objectives[index] - auxFront[j - 1].Chromosome.Objectives[index]) / (_minMax[i].MaxValue - _minMax[i].MinValue);
				}
			}

			auxFront.Sort((x, y) => y.CrowdDistance.CompareTo(x.CrowdDistance));

			front.Clear();

			for(int i = 0; i < auxFront.Count; i++)
			{
				front.Add(auxFront[i].Chromosome);
			}
		}

		#endregion
	}
}