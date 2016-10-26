using MultiObjectiveGA;
using System;

namespace NoveltySearch
{
	/// <summary>
	/// Novelty chromosome.
	/// </summary>
	/// 
	/// <remarks><para>Chromosome used by the PopulationNovelty.</para></remarks></remarks>
	[Serializable]
	public abstract class NoveltyChromosome : MOChromosome
	{   
	    public double Sparseness
	    {
	        get { return Objectives[1]; }
	        set { Objectives[1] = value; }
	    }

	    public double LocalCompetition
	    {
	        get { return Objectives[2]; }
	        set { Objectives[2] = value; }
	    }

	    public NoveltyChromosome() : base(3) { }
	}
}