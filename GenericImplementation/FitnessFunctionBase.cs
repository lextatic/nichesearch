using AForge.Genetic;

namespace GeneticImplementation
{
	public abstract class FitnessFunctionBase : IFitnessFunction
	{
		public abstract double Evaluate (IChromosome chromosome);
	}
}
