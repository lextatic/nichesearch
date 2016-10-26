using System;
using System.Collections.Generic;
using System.Linq;
using AForge;
using AForge.Genetic;

namespace NoveltySearch
{
	/// <summary>
	/// Population novelty.
	/// </summary>
	/// 
	/// <remarks><para>Implementation of the Progressive Mininimal Criteria Novelty Search</para></remarks>
	/// 
	public class PopulationNovelty : Population
	{
	    private IDistanceFunction _distanceFunction;
	    private List<NoveltyChromosome> _archive = new List<NoveltyChromosome>();
	    
		private Dictionary<IChromosome, Dictionary<IChromosome, double>> _distanceMatrix = new Dictionary<IChromosome, Dictionary<IChromosome, double>>();

	    private int _kNearestNeighbours;
	    private int _archiveSizeLimit = 500;
		private double _sparsenessThreshold = 1250;
		private int _epochAjustFrequency = 5;
		private int _idealIndividualsAdded = 4;
		private float _sparsenessThresholdIncrease = 1.20f;
		private float _sparsenessThresholdReduction = 0.95f;
		private double _minimalCriteria = 0;

		private int _addedToArchiveCount = 0;
		private int _epochCount = 0;

		private float _smoothingParameter = 0f;
		private float _acceptedIndividualsPercentage = 0f;

		public IDistanceFunction DistanceFunction
	    {
	        get { return _distanceFunction; }
	        set { _distanceFunction = value; }
	    }

	    public int KNearestNeighbours
	    {
	        get { return _kNearestNeighbours; }
	    }

	    public int ArchiveSize
	    {
	        get { return _archive.Count; }
	    }

	    public int ArchiveSizeLimit
	    {
	        get { return _archiveSizeLimit; }
	        set { _archiveSizeLimit = value; }
	    }

	    public double SparsenessThreshold
	    {
			get { return _sparsenessThreshold; }
			set { _sparsenessThreshold = value; }
	    }

		public int EpochAjustFrequency
		{
			get { return _epochAjustFrequency; }
			set { _epochAjustFrequency = value; }
		}

		public int IdealIndividualsAdded
		{
			get { return _idealIndividualsAdded; }
			set { _idealIndividualsAdded = value; }
		}

		public float SparsenessThresholdIncrease
		{
			get { return _sparsenessThresholdIncrease; }
			set { _sparsenessThresholdIncrease = value; }
		}

		public float SparsenessThresholdReduction
		{
			get { return _sparsenessThresholdReduction; }
			set { _sparsenessThresholdReduction = value; }
		}

	    public double MinimalCriteria
	    {
			get { return _minimalCriteria; }
			set { _minimalCriteria = value; }
	    }

		public float SmoothingParameter
		{
			get { return _smoothingParameter; }
			set { _smoothingParameter = Math.Max (0, Math.Min (value, 1)); }
		}

		public float AcceptedIndividualsPercentage
		{
			get { return _acceptedIndividualsPercentage; }
			set { _acceptedIndividualsPercentage = Math.Max (0, Math.Min (value, 1)); }
		}

	    public List<NoveltyChromosome> Archive
	    {
	        get { return _archive; }
	    }

	    public PopulationNovelty(int size,
	                        NoveltyChromosome ancestor,
	                        IFitnessFunction fitnessFunction,
	                        IDistanceFunction distanceFunction,
	                        ISelectionMethod selectionMethod,
	                        int kNearestNeighbours) : base (size, ancestor, fitnessFunction, selectionMethod)
	    {
	        if (kNearestNeighbours < 2)
	            throw new ArgumentException("Too small nearest neighbours was specified.");

			_distanceFunction = distanceFunction;
	        _kNearestNeighbours = kNearestNeighbours;
	    }

	    public override void Regenerate()
	    {
			base.Regenerate();

	        _archive.Clear();
	    }

		public override void Selection()
	    {
			try
			{
				CalculateSparseness();
			}
			catch(Exception e)
			{
				UnityEngine.Debug.Log(e.Message);
			}

			//
			// Calculates the dynamic minimal criteria
			//
			population.Sort((x, y) => y.Fitness.CompareTo(x.Fitness));
			_minimalCriteria = _minimalCriteria + Math.Max (0, (population [(int)(_acceptedIndividualsPercentage * population.Count - 0.5f)].Fitness - _minimalCriteria) * _smoothingParameter);

			base.Selection();

			//
			// Limpa o dictionary _distanceMatrix
			//
			clearDistanceMatrix();

//			try
//			{
//				CalculateSparseness2();
//			}
//			catch (Exception e)
//			{
//				UnityEngine.Debug.Log(e.Message);
//			}

			//
			// Counts the epoch to ajust the sparsenessThreshold
			// In 10 epochs, if more than 4 individuals were add to the archive, increase the sparsenessThreshold by 20%
			// In the same time, if no individual were add to the archive, decrese the sparsenessThreshold by 5%
			//
			_epochCount++;

			if (_epochCount >= _epochAjustFrequency)
			{
				if (_addedToArchiveCount > _idealIndividualsAdded)
				{
					_sparsenessThreshold *= _sparsenessThresholdIncrease;
				}
				else if (_addedToArchiveCount <= 0)
				{
					_sparsenessThreshold *= _sparsenessThresholdReduction;
				}

				_addedToArchiveCount = 0;
				_epochCount = 0;
			}
	    }

		private void CalculateSparseness()
	    {
			population = population.OrderByDescending(x => x.Fitness).ToList();

			// CALCULATE ALL SPARSENESS HERE
	        Parallel.For(0, population.Count, i => 
	        {	
				double[] distances = new double[population.Count + _archive.Count];

	            NoveltyChromosome auxChromosome = (NoveltyChromosome) this[i];

				for (int j = 0; j < population.Count; j++)
	            {
					distances[j] = findDistance(auxChromosome, this[j]);
	            }

				for (int j = 0; j < _archive.Count; j++)
	            {
					distances[population.Count + j] = findDistance(auxChromosome, _archive[j]);
	            }

				double distSum = 0;

				double[] closestDistances = new double[_kNearestNeighbours];
				double[] closestFitness = new double[_kNearestNeighbours];

				for (int k = 0; k < _kNearestNeighbours; k++)
	            {
	                closestDistances[k] = int.MaxValue;
	            }

				for (int j = 0; j < population.Count + _archive.Count; j++)
	            {
					double currentDistance = distances[j];

	                //
	                // If it's better than the worst saved distances
	                //
					if (currentDistance < closestDistances[_kNearestNeighbours - 1] && (i != j))
	                {
	                    bool shift = false;

	                    double auxDistance1 = 0;
	                    double auxDistance2 = 0;

						double auxFitness1 = 0;
		                double auxFitness2 = 0;

						for (int k = 0; k < _kNearestNeighbours; k++)
	                    {
	                        //
	                        // Save the new distance at its proper place in the closestDistances array
	                        //
	                        if (shift == false)
	                        {
								if (currentDistance < closestDistances[k])
	                            {
	                                shift = true;

	                                auxDistance1 = closestDistances[k];
									auxFitness1 = closestFitness[k];

									closestDistances[k] = currentDistance;

									if (j < population.Count)
	                                {
									    closestFitness[k] = this[j].Fitness;
	                                }
									else if (auxChromosome != _archive[j - population.Count])
	                                {
										closestFitness[k] = _archive[j - population.Count].Fitness;
	                                }
	                            }
	                        }

	                        //
	                        // And shift the rest keeping the array closestDistances always ordered
	                        //
	                        else
	                        {
								auxDistance2 = closestDistances[k];
	                            closestDistances[k] = auxDistance1;
								auxDistance1 = auxDistance2;

								if (j < population.Count || auxChromosome != _archive[j - population.Count])
								{
									auxFitness2 = closestFitness[k];
									closestFitness[k] = auxFitness1;
			                        auxFitness1 = auxFitness2;
								}
	                        }
	                    }
	                }
	            }

				auxChromosome.LocalCompetition = 0;

				//
	            // Make a sum of all those _nearestNeighbours distances.
	            //
				for (int k = 0; k < _kNearestNeighbours; k++)
	            {
	                distSum += closestDistances[k];

					if (auxChromosome.Fitness > closestFitness[k])
	                {
	                    auxChromosome.LocalCompetition++;
	                }
	            }

	            //
	            // Claculate the average
	            //
	            auxChromosome.Sparseness = distSum / KNearestNeighbours;

				if (auxChromosome.Fitness < _minimalCriteria)
	            {
	                auxChromosome.LocalCompetition = 0;
	            }
	        });

			for (int i = 0; i < population.Count; i++)
	        {
				NoveltyChromosome currentChromosome = (NoveltyChromosome) this[i];

				if (currentChromosome.Sparseness > _sparsenessThreshold)
	            {
					_archive.Add(currentChromosome);

					//
					// Count as added to archive.
					//
					_addedToArchiveCount++;

	                if (_archive.Count > _archiveSizeLimit)
	                {
	                    _archive.RemoveAt(0);
	                }
	            }
	        }
	    }

		private double findDistance(IChromosome chromosome1, IChromosome chromosome2)
		{
			double distance;

			Dictionary<IChromosome, double> distances;

			lock (_distanceMatrix)
			{
				if (_distanceMatrix.TryGetValue(chromosome1, out distances))
				{
					// chromosome2 existe na lista do chromosome1
					if (distances.TryGetValue(chromosome2, out distance))
					{
						return distance;
					}
				}
			}

			// calcula a distancia
			distance = _distanceFunction.Distance(chromosome1, chromosome2);

			lock (_distanceMatrix)
			{
				// chromosome1 existe na lista
				if (_distanceMatrix.TryGetValue(chromosome1, out distances))
				{
					// Adiciona na lista de distancias do chromosome1 para o chromosome2
					distances[chromosome2] = distance;

					// Checa se o Chromosome2 existe na lista geral
					if (_distanceMatrix.TryGetValue(chromosome2, out distances))
					{
						distances[chromosome1] = distance;
					}
					else
					{
						// Cria a lista de distancias do chromosome2 e adiciona a distancia para o cromosome1 nela
						distances = new Dictionary<IChromosome, double>();

						distances[chromosome1] =  distance;

						try
						{
							_distanceMatrix.Add(chromosome2, distances);
						}
						catch
						{
							_distanceMatrix[chromosome2][chromosome1] = distance;
						}
					}
				}
				// chromosome1 não existe na lista
				else
				{
					// calcula a distância
					distance = _distanceFunction.Distance(chromosome1, chromosome2);

					// cria a lista de distancias do chromosome1 e adiciona a distancia para o chromosome2 nela
					distances = new Dictionary<IChromosome, double>();

					distances[chromosome2] = distance;

					try
					{
						_distanceMatrix.Add(chromosome1, distances);
					}
					catch
					{
						_distanceMatrix[chromosome1][chromosome2] = distance;
					}

					// chromosome2 existe na lista
					if (_distanceMatrix.TryGetValue(chromosome2, out distances))
					{
						// Adiciona na lista de distâncias do chromosome2 para o chromosome1
						distances[chromosome1] = distance;
					}
					// chromosome2 não existe na lista
					else
					{
						// Cria a lista de distâncias para o chromosome2 e adiciona a lista para o chromosome1 nela
						distances = new Dictionary<IChromosome, double>();

						distances[chromosome1] = distance;

						try
						{
							_distanceMatrix.Add(chromosome2, distances);
						}
						catch
						{
							_distanceMatrix[chromosome2][chromosome1] = distance;
						}
					}
				}
			}

			return distance;
		}

		public void clearDistanceMatrix()
		{
			var itemsToRemove = _distanceMatrix.Where(f => (!population.Contains(f.Key) && !_archive.Contains((NoveltyChromosome)f.Key))).ToList();
			
			foreach (var item in itemsToRemove)
				_distanceMatrix.Remove(item.Key);

			foreach (KeyValuePair<IChromosome, Dictionary<IChromosome, double>> pair in _distanceMatrix)
			{
				foreach (var item in itemsToRemove)
					pair.Value.Remove(item.Key);
			}
		}
	}
}