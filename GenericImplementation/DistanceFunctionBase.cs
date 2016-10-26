using NoveltySearch;
using AForge.Genetic;

namespace GeneticImplementation
{
	public abstract class DistanceFunctionBase : IDistanceFunction
	{
		public abstract double Distance (IChromosome chromosome1, IChromosome chromosome2);
	}
}
