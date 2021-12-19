namespace Lab4
{
	public class MaterialParameters
	{
		public double vp { get; set; }
		public double vs { get; set; }
		public double ro { get; set; }
		public double p { get; set; }
		public double f1 { get; set; }
		public double f2 { get; set; }
		public double limit { get; set; }

		public MaterialParameters(double vp, double vs, double ro, double p, double f1, double f2, double limit)
		{
			this.vp = vp;
			this.vs = vs;
			this.ro = ro;
			this.p = p;
			this.f1 = f1;
			this.f2 = f2;
			this.limit = limit;
		}

		public MaterialParameters()
		{
			
		}
		
		public static double GetMu(double Vp, double Vs, double Ro)
		{
			return Ro * Vs * Vs;
		}
		public static double GetLambda(double Vp, double Vs, double Ro, double mu)
		{
			return Ro * Vp * Vp - 2 * mu;
		}
		public static double[,] CreateExternalStressField()
		{
			double[,] externalStressField = new double[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 10 } };
			return externalStressField;
		}
		public static double[,] FindMineralElasticityModule(double lambda, double mu) //скорость проточных и параллельных волн в минералах. Плотность кг/м3
		{
			double[,] C = new double[6, 6]
			{
				{lambda+2*mu, lambda, lambda, 0, 0, 0},
				{lambda, lambda + 2 * mu, lambda, 0, 0, 0},
				{lambda, lambda, lambda + 2 * mu, 0, 0, 0},
				{0, 0, 0, mu, 0, 0},
				{0, 0, 0, 0, mu, 0},
				{0, 0, 0, 0, 0, mu}
			};
			return C;
		}

		public static double[,] FindMineralElasticityModuleWithAddition(double lambda, double mu1, double p, double f1, double f2) //p Задается пользователем - среднее давление газа или жидкости во включениях минерала
		{
			var K1 = lambda + 2 * mu1 / 3;
			var K2 = p;
			var mu2 = 0;
			var Kmwp = K1 + (f2 / (1 / (K2 - K1) + f1 * (K1 + 4 * mu1 / 3)));
			var mumwp = mu1 + (f2 / (1 / (mu2 - mu1) + (2 * f1 * (K1 + 2 * mu1) / (5 * mu1 * (K1 + 4 * mu1 / 3)))));
			var lambdamwp = Kmwp - 2 * mumwp / 3;
			double[,] Cmwp = FindMineralElasticityModule(lambdamwp, mumwp);
			return Cmwp;
		}

		public static double[,] FindInversedMatrix(double[,] Cwmp)
		{
			var Matrix = new Matrix(6, 6);

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					Matrix[i, j] = Cwmp[i, j];
				}
			}

			var reverseMatrix = Matrix.CreateInvertibleMatrix();
			double[,] Smwp = new double[6, 6];

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					Smwp[i, j] = reverseMatrix[i, j];
				}
			}
			return Smwp;
		}

		public static double[,,,] ConvertElasticModuleMineralsTensor(double[,] Cm)
		{
			double[,,,] Cijkl = new double[3, 3, 3, 3];

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					int i1 = 0;
					int j1 = 0;
					int k1 = 0;
					int l1 = 0;
					switch (i)
					{
						case 0:
							i1 = 0;
							j1 = 0;
							break;
						case 1:
							i1 = 1;
							j1 = 1;
							break;
						case 2:
							i1 = 2;
							j1 = 2;
							break;
					}
					switch (j)
					{
						case 0:
							k1 = 0;
							l1 = 0;
							break;
						case 1:
							k1 = 1;
							l1 = 1;
							break;
						case 2:
							k1 = 2;
							l1 = 2;
							break;
					}
					if (i == 3 && j == 3)
					{
						Cijkl[1, 2, 1, 2] = Cm[i, j];
						Cijkl[2, 1, 1, 2] = Cm[i, j];
						Cijkl[1, 2, 2, 1] = Cm[i, j];
						Cijkl[2, 1, 2, 1] = Cm[i, j];
					}
					if (i == 4 && j == 4)
					{
						Cijkl[2, 0, 2, 0] = Cm[i, j];
						Cijkl[2, 0, 0, 2] = Cm[i, j];
						Cijkl[0, 2, 2, 0] = Cm[i, j];
						Cijkl[0, 2, 0, 2] = Cm[i, j];
					}
					if (i == 5 && j == 5)
					{
						Cijkl[0, 1, 0, 1] = Cm[i, j];
						Cijkl[0, 1, 1, 0] = Cm[i, j];
						Cijkl[1, 0, 0, 1] = Cm[i, j];
						Cijkl[1, 0, 1, 0] = Cm[i, j];
					}
					Cijkl[i1, j1, k1, l1] = Cm[i, j];
				}
			}
			return Cijkl;
		}

		public static double[,,,] ConvertElasticModuleMineralsTensorWithAddition(double[,] Smwp)
		{
			double[,,,] Sijkl = new double[3, 3, 3, 3];

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					int i1 = 0;
					int j1 = 0;
					int k1 = 0;
					int l1 = 0;
					switch (i)
					{
						case 0:
							i1 = 0;
							j1 = 0;
							break;
						case 1:
							i1 = 1;
							j1 = 1;
							break;
						case 2:
							i1 = 2;
							j1 = 2;
							break;
					}
					switch (j)
					{
						case 0:
							k1 = 0;
							l1 = 0;
							break;
						case 1:
							k1 = 1;
							l1 = 1;
							break;
						case 2:
							k1 = 2;
							l1 = 2;
							break;
					}
					if (i == 3 && j == 3)
					{
						Sijkl[1, 2, 1, 2] = Smwp[i, j] * 4;
						Sijkl[2, 1, 1, 2] = Smwp[i, j] * 4;
						Sijkl[1, 2, 2, 1] = Smwp[i, j] * 4;
						Sijkl[2, 1, 2, 1] = Smwp[i, j] * 4;
					}
					if (i == 4 && j == 4)
					{
						Sijkl[2, 0, 2, 0] = Smwp[i, j] * 4;
						Sijkl[2, 0, 0, 2] = Smwp[i, j] * 4;
						Sijkl[0, 2, 2, 0] = Smwp[i, j] * 4;
						Sijkl[0, 2, 0, 2] = Smwp[i, j] * 4;
					}
					if (i == 5 && j == 5)
					{
						Sijkl[0, 1, 0, 1] = Smwp[i, j] * 4;
						Sijkl[0, 1, 1, 0] = Smwp[i, j] * 4;
						Sijkl[1, 0, 0, 1] = Smwp[i, j] * 4;
						Sijkl[1, 0, 1, 0] = Smwp[i, j] * 4;
					}
					Sijkl[i1, j1, k1, l1] = Smwp[i, j];
				}
			}
			return Sijkl;
		}

		private static double[,] FindSigmaMN(double[,,,] Cmnij, double[,,,] Sijkl, double[,] Sigma_kl)
		{
			double[,] tensorE = new double[3, 3];

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						for (int l = 0; l < 3; l++)
						{
							tensorE[i, j] += Sijkl[i, j, k, l] * Sigma_kl[k, l];
						}
					}
				}
			}

			double[,] Sigma_mn = new double[3, 3];

			for (int m = 0; m < 3; m++)
			{
				for (int n = 0; n < 3; n++)
				{
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							Sigma_mn[m, n] += Cmnij[m, n, i, j] * tensorE[i, j];
						}
					}
				}
			}

			return Sigma_mn;
		}

		public static double FindSigma33(double[,,,] Cmnij, double[,,,] Sijkl, double[,] Sigma_kl, double limit)
		{
			var sigma_mn = FindSigmaMN(Cmnij, Sijkl, Sigma_kl);
			if (sigma_mn[2, 2] >= limit)
			{
				return Sigma_kl[2, 2];
			}
			else
			{
				Sigma_kl[2, 2] += 1;
				return FindSigma33(Cmnij, Sijkl, Sigma_kl, limit);
			}
		}
	}
}

