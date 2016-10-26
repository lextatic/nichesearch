using AForge.Genetic;
using System.Collections.Generic;
using System.Linq;

namespace MultiObjectiveGA
{
	/// <summary>
	/// Elite selection method.
	/// </summary>
	/// 
	/// <remarks>Elite selection method selects specified amount of
	/// best chromosomes to the next generation.</remarks> 
	///
	public class MOEliteSelection : ISelectionMethod
	{
	    private int _objectiveIndex;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="MOEliteSelection"/> class.
	    /// </summary>
	    public MOEliteSelection( int objectiveIndex )
	    {
	        _objectiveIndex = objectiveIndex;
	    }

	    /// <summary>
	    /// Apply selection to the specified population.
	    /// </summary>
	    /// 
	    /// <param name="chromosomes">Population, which should be filtered.</param>
	    /// <param name="size">The amount of chromosomes to keep.</param>
	    /// 
	    /// <remarks>Filters specified population keeping only specified amount of best
	    /// chromosomes.</remarks>
	    /// 
	    public void ApplySelection( List<IChromosome> chromosomes, int size )
	    {
			chromosomes.Sort((x, y) => ((MOChromosome)y).Objectives[_objectiveIndex].CompareTo(((MOChromosome)x).Objectives[_objectiveIndex]));
	        
			// remove bad chromosomes
	        chromosomes.RemoveRange( size, chromosomes.Count - size );
	    }
	}
}
