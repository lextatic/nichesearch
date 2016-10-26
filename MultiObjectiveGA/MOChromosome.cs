using System;
using AForge.Genetic;

namespace MultiObjectiveGA
{
	/// <summary>
	/// Chromosome that supports more than one objective score.
	/// </summary>
	[Serializable]
	public abstract class MOChromosome : IChromosome//, IEquatable<MOChromosome>
	{
	    /// <summary>
	    /// Chromosome's multi-fitness values.
	    /// </summary>
	    protected double[] _objectives;

	    public double[] Objectives
	    {
	        get { return _objectives; }
	    }

	    public MOChromosome(int objectives)
	    {
	        if (objectives < 1)
	            throw new ArgumentException("Must have at least one objective.");

	        _objectives = new double[objectives];
	    }

	    public void Evaluate(IFitnessFunction function, int objectiveIndex)
	    {
	        _objectives[objectiveIndex] = function.Evaluate(this);
	    }
	    
	    #region IChromosome implementation

	    public double Fitness
	    {
	        get { return _objectives[0]; }
	    }

	    /// <summary>
	    /// Generate random chromosome value.
	    /// </summary>
	    /// 
	    /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
	    /// </remarks>
	    /// 
	    public abstract void Generate();

	    /// <summary>
	    /// Create new random chromosome with same parameters (factory method).
	    /// </summary>
	    /// 
	    /// <remarks><para>The method creates new chromosome of the same type, but randomly
	    /// initialized. The method is useful as factory method for those classes, which work
	    /// with chromosome's interface, but not with particular chromosome class.</para></remarks>
	    /// 
	    public abstract IChromosome CreateNew();

	    /// <summary>
	    /// Clone the chromosome.
	    /// </summary>
	    /// 
	    /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
	    /// </remarks>
	    /// 
	    public abstract IChromosome Clone();

	    /// <summary>
	    /// Mutation operator.
	    /// </summary>
	    /// 
	    /// <remarks><para>The method performs chromosome's mutation, changing its part randomly.</para></remarks>
	    /// 
	    public abstract void Mutate();

	    /// <summary>
	    /// Crossover operator.
	    /// </summary>
	    /// 
	    /// <param name="pair">Pair chromosome to crossover with.</param>
	    /// 
	    /// <remarks><para>The method performs crossover between two chromosomes – interchanging some parts of chromosomes.</para></remarks>
	    /// 
	    public abstract void Crossover(IChromosome pair);

	    /// <summary>
	    /// Evaluate chromosome with specified fitness function.
	    /// </summary>
	    /// 
	    /// <param name="function">Fitness function to use for evaluation of the chromosome.</param>
	    /// 
	    /// <remarks><para>Calculates chromosome's fitness using the specifed fitness function.</para></remarks>
	    ///
	    public void Evaluate(IFitnessFunction function)
	    {
	        _objectives[0] = function.Evaluate(this);
	    }

	    #endregion

	    #region IComparable implementation

	    /// <summary>
	    /// Compare two chromosomes.
	    /// </summary>
	    /// 
	    /// <param name="o">Binary chromosome to compare to.</param>
	    /// 
	    /// <returns>Returns comparison result, which equals to 0 if fitness values
	    /// of both chromosomes are equal, 1 if fitness value of this chromosome
	    /// is less than fitness value of the specified chromosome, -1 otherwise.</returns>
	    /// 
	    public int CompareTo(object o)
	    {
	        double f = ((MOChromosome)o)._objectives[0];

	        return (_objectives[0] == f) ? 0 : (_objectives[0] < f) ? 1 : -1;
	    }

	    #endregion

	    #region IEquatable implementation

//	    public abstract bool Equals(MOChromosome other);
//
//	    public override bool Equals(object obj)
//	    {
//	        if (obj == null)
//	            return false;
//
//	        MOChromosome chromosomeObj = obj as MOChromosome;
//	        if (chromosomeObj == null)
//	            return false;
//	        else
//	            return Equals(chromosomeObj);
//	    }
//
//	    public abstract override int GetHashCode();
//
//	    public static bool operator ==(MOChromosome chromosome1, MOChromosome chromosome2)
//	    {
//	        // If left hand side is null...
//	        if (System.Object.ReferenceEquals(chromosome1, null))
//	        {
//	            // ...and right hand side is null...
//	            if (System.Object.ReferenceEquals(chromosome2, null))
//	            {
//	                //...both are null and are Equal.
//	                return true;
//	            }
//
//	            // ...right hand side is not null, therefore not Equal.
//	            return false;
//	        }
//
//	        // Return true if the fields match:
//	        return chromosome1.Equals(chromosome2);
//	    }
//
//	    public static bool operator !=(MOChromosome chromosome1, MOChromosome chromosome2)
//	    {
//	        return !(chromosome1 == chromosome2);
//	    }

	    #endregion
	}
}