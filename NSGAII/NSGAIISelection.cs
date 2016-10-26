using AForge.Genetic;
using System;
using System.Collections.Generic;
using MultiObjectiveGA;

namespace NSGAII
{
	/// <summary>
	/// NSGA-II selection.
	/// </summary>
	/// 
	/// <remarks><para>Select individuals based on a Pareto optimal manner.</para></remarks>
	/// 
	public class NSGAIISelection : ISelectionMethod
	{
		private ISortingFunction _sortingFunction;

		private static int[] _objectiveIndexes;

		public NSGAIISelection(ISortingFunction sortingFunction, int[] objectiveIndexes)
	    {
			_sortingFunction = sortingFunction;
			_objectiveIndexes = objectiveIndexes;
	    }

	    public void ApplySelection(List<IChromosome> chromosomes, int size)
	    {
	        List<List<MOChromosome>> fronts = FastNonDominatedSort(chromosomes.ConvertAll(x => (MOChromosome)x));

			List<IChromosome> newPopulation = new List<IChromosome>();

			int currentFrontIndex = 0;

			List<MOChromosome> currentFront = fronts[currentFrontIndex];

			while (newPopulation.Count + currentFront.Count <= size)
	        {
				//UnityEngine.Debug.Log ("[" + currentFrontIndex + "] currentFront.Count: " + currentFront.Count + "\ntotal: " + (newPopulation.Count + currentFront.Count));
				for (int i = 0; i < currentFront.Count; i++)
	            {
					newPopulation.Add(currentFront[i]);
	            }

	            currentFrontIndex++;

				if(currentFrontIndex < fronts.Count)
				{
					currentFront = fronts[currentFrontIndex];
				}
	        }

			_sortingFunction.Sort(currentFront);
	            
	        int membersLeft = size - newPopulation.Count;

			for (int i = 0; i < membersLeft; i++)
	        {
				newPopulation.Add(currentFront[i]);
	        }

	        chromosomes.Clear();

			chromosomes.AddRange(newPopulation);
	    }

		private class NSGAChromosome
		{
			public MOChromosome chromosome;
			public int dominators;
			public List<NSGAChromosome> dominatedChromosomes;

			public NSGAChromosome(MOChromosome chrom)
			{
				chromosome = chrom;
				dominators = 0;
				dominatedChromosomes = new List<NSGAChromosome>();
			}

			public bool Dominates(NSGAChromosome other)
			{
				int equalObjectivesCount = 0;

				for(int i = 0; i < _objectiveIndexes.Length; i++)
				{
					int index = _objectiveIndexes[i];

					double objective1 = chromosome.Objectives[index];
					double objective2 = other.chromosome.Objectives[index];

					if(objective1 < objective2)
					{
						return false;
					}
					else if (objective1 == objective2)
					{
						equalObjectivesCount++;
					}
				}

				if(equalObjectivesCount == _objectiveIndexes.Length)
				{
					return false;
				}

				return true;
			}
		}

	    private List<List<MOChromosome>> FastNonDominatedSort(List<MOChromosome> chromosomes)
	    {
			List<NSGAChromosome> sortingChromosomes = new List<NSGAChromosome> ();
			
			for (int i = 0; i < chromosomes.Count; i++)
			{
				sortingChromosomes.Add(new NSGAChromosome(chromosomes[i]));
			}
			
			List<List<NSGAChromosome>> fronts = new List<List<NSGAChromosome>>();
			fronts.Add(new List<NSGAChromosome>());

			for (int i = 0; i < sortingChromosomes.Count; i++)
	        {
				NSGAChromosome p = sortingChromosomes[i];

				for (int j = 0; j < sortingChromosomes.Count; j++)
	            {
					NSGAChromosome q = sortingChromosomes[j];

					if (p.Dominates(q))
	                {
						p.dominatedChromosomes.Add(q);
					}
					else if (q.Dominates(p))
	                {
						p.dominators++;
					}
	            }

				if(p.dominators == 0)
	            {
					fronts[0].Add(p);
	            }
	        }

			int currentFrontIndex = 0;
			List<NSGAChromosome> currentFront = fronts[currentFrontIndex];

			while(currentFront.Count > 0)
	        {
				List<NSGAChromosome> nextFront = new List<NSGAChromosome>();

				for(int i = 0; i < currentFront.Count; i++)
	            {
					NSGAChromosome p = currentFront[i];

					for (int j = 0; j < p.dominatedChromosomes.Count; j++)
	                {
						p.dominatedChromosomes[j].dominators--;

						if(p.dominatedChromosomes[j].dominators == 0)
	                    {
							nextFront.Add(p.dominatedChromosomes[j]);
	                    }
	                }
	            }

				fronts.Add(nextFront);

	            currentFrontIndex++;
				currentFront = fronts[currentFrontIndex];
	        }

	        fronts.RemoveAt(fronts.Count - 1);

			List<List<MOChromosome>> finalFronts = new List<List<MOChromosome>> ();

			int finalCount = 0;

			for (int i = 0; i < fronts.Count; i++)
			{
				finalFronts.Add (new List<MOChromosome> ());

				for (int j = 0; j < fronts[i].Count; j++)
				{
					finalFronts [i].Add (fronts [i] [j].chromosome);
					finalCount++;
				}
			}

			return finalFronts;
	    }
	}
}