using AForge.Genetic;

namespace NoveltySearch
{
	/// <summary>
	/// Distance function interface.
	/// </summary>
	/// 
	/// <remarks><para>The interface should be implemented by all distance function
	/// classes, which are supposed to be used for calculation of chromosomes
	/// behavioural distance values. All distance functions should return positive (<b>greater
	/// then zero</b>) value, which indicates how distant are the two chromosomes from each other.</para>
	/// <para>The greater the value, the further they are.</para>
	/// </remarks>
	/// 
	public interface IDistanceFunction
	{
	    /// <summary>
	    /// Calculates the chromosomes behavioural distance.
	    /// </summary>
	    /// 
	    /// <param name="chromosome1">First chromosome to compare distance to.</param>
		/// <param name="chromosome2">Second chromosome to compare distance to.</param>
	    /// 
	    /// <returns>Returns the calculated distance between the two chromosomes.</returns>
	    ///
		/// <remarks><para>The method calculates the behavioural distance of the two specified
		/// chromosomes.</para></remarks>
	    ///
		double Distance( IChromosome chromosome1, IChromosome chromosome2 );
	}
}