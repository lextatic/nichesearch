using System.Collections.Generic;
using MultiObjectiveGA;

namespace NSGAII
{
	/// <summary>
	/// Sorting function interface used to implement the sorting inside the same Pareto optimal front.
	/// </summary>
	/// 
	/// <remarks><para>The interfase should be implemented by all classes, which implement
	/// a particular sorting for Pareto optimal fronts.</para></remarks>
	/// 
	public interface ISortingFunction
	{
		void Sort(List<MOChromosome> front);
	}
}
