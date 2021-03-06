using System;


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
			var mu = Ro * Vs * Vs;
			Console.WriteLine($"Mu = {mu}");
			return mu;
		}
		public static double GetLambda(double Vp, double Vs, double Ro, double mu)
		{
			var lambda = Ro * Vp * Vp - 2 * mu;
			Console.WriteLine($"Lambda = {lambda}");
			return lambda;
		}
		public static double[,] CreateExternalStressField()
		{
			double[,] externalStressField = new double[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
			return externalStressField;
		}
		public static double[,] FindMineralElasticityModule(double lambda, double mu, bool write = false) 
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
			if (write)
			{
				Console.WriteLine("\nШаг 3. Тензор модулей упругости минерала (в матричной форме):");
				WriteArray(C);
			}
			return C;
		}

		public static double[,] FindMineralElasticityModuleWithAddition(double lambda, double mu1, double p, double f1, double f2)
		{
			var K1 = lambda + 2 * mu1 / 3;
			var K2 = p;
			var mu2 = 0;
			var Kmwp = K1 + (f2 / (1 / (K2 - K1) + f1 * (K1 + 4 * mu1 / 3)));
			var mumwp = mu1 + (f2 / (1 / (mu2 - mu1) + (2 * f1 * (K1 + 2 * mu1) / (5 * mu1 * (K1 + 4 * mu1 / 3)))));
			var lambdamwp = Kmwp - 2 * mumwp / 3;
			double[,] Cmwp = FindMineralElasticityModule(lambdamwp, mumwp);
			Console.WriteLine("\nШаг 4. Эффективный тензор модулей упругости минералов с газовыми или жидкостными включениями(в матричной форме):");
			WriteArray(Cmwp);
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
			Console.WriteLine("\nШаг 5. Обратная матрица Smwp:");
			WriteArray(Smwp);
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
						continue;
					}
					if (i == 4 && j == 4)
					{
						Cijkl[2, 0, 2, 0] = Cm[i, j];
						Cijkl[2, 0, 0, 2] = Cm[i, j];
						Cijkl[0, 2, 2, 0] = Cm[i, j];
						Cijkl[0, 2, 0, 2] = Cm[i, j];
						continue;
					}
					if (i == 5 && j == 5)
					{
						Cijkl[0, 1, 0, 1] = Cm[i, j];
						Cijkl[0, 1, 1, 0] = Cm[i, j];
						Cijkl[1, 0, 0, 1] = Cm[i, j];
						Cijkl[1, 0, 1, 0] = Cm[i, j];
						continue;
					}
					Cijkl[i1, j1, k1, l1] = Cm[i, j];
				}
			}
			Console.WriteLine("\nШаг 6. Преобразованный тензор модулей упругости минерала:");
			WriteArray(Cijkl);
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
						continue;
					}
					if (i == 4 && j == 4)
					{
						Sijkl[2, 0, 2, 0] = Smwp[i, j] * 4;
						Sijkl[2, 0, 0, 2] = Smwp[i, j] * 4;
						Sijkl[0, 2, 2, 0] = Smwp[i, j] * 4;
						Sijkl[0, 2, 0, 2] = Smwp[i, j] * 4;
						continue;
					}
					if (i == 5 && j == 5)
					{
						Sijkl[0, 1, 0, 1] = Smwp[i, j] * 4;
						Sijkl[0, 1, 1, 0] = Smwp[i, j] * 4;
						Sijkl[1, 0, 0, 1] = Smwp[i, j] * 4;
						Sijkl[1, 0, 1, 0] = Smwp[i, j] * 4;
						continue;
					}
					Sijkl[i1, j1, k1, l1] = Smwp[i, j];
					
				}
			}
			Console.WriteLine("\nШаг 7. Преобразованный тензор модулей упругости минерала с газовыми или жидкостными включениями: ");
			WriteArray(Sijkl);
			return Sijkl;
		}

		public static double FindSigma33(double[,,,] Cmnij, double[,,,] Sijkl, double[,] Sigma_kl, double limit)
		{
			double[,] Sigma_mn = new double[3, 3];
			do
			{
				Sigma_kl[2, 2] += 1000;
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
				if(Sigma_kl[2,2] == 2000)
				{
					Console.WriteLine("\nШаг 8. Начальня сигма (m):");
					WriteArray(Sigma_mn);
				}
			} while (Sigma_mn[2, 2] < limit);
			Console.WriteLine("\nШаг 9. Конечная сигма (m)");
			WriteArray(Sigma_mn);
			return Sigma_kl[2, 2];
		}

		public static void WriteArray(double[,] array)
		{
			var a = 25;
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					if (array[i,j] != 0)
					{
						var b = array[i, j].ToString().Length;
						var c = a - b + 1;
						Console.Write(array[i, j] + new string(' ', c));
					}
					else
					{
						Console.Write(array[i, j] + new string(' ', a));
					}
				}
				Console.WriteLine();
			}
		}

		public static void WriteArray(double[,,,] array)
		{
			var a = 25;
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					for (int k = 0; k < array.GetLength(2); k++)
					{
						for (int l = 0; l < array.GetLength(3); l++)
						{
							if (array[i, j, k, l] != 0)
							{
								var b = array[i, j, k, l].ToString().Length;
								var c = a - b + 1;
								Console.Write(array[i, j, k, l] + new string(' ', c - 3));
							}
							else
							{
								Console.Write(array[i, j, k, l] + new string(' ', a - 3));
							}
						}
					}
					Console.WriteLine();
				}
			}
		}
	}
}

