using System;
using System.IO;


namespace Lab4
{
	public class BurnerParameters
	{
		public static MaterialParameters GetMaterialFromTable()
		{
			Console.WriteLine("Выберите минерал \n" +
				"1) Кварц \n" +
				"2) Ортоклаз \n" +
				"3) Альбит\n" +
				"4) Анорит\n" +
				"5) Кальцит\n" +
				"6) Доломит\n" +
				"7) Биотит\n" +
				"8) Мусковит\n" +
				"9) Роговая обманка\n" +
				"10) Эпидот\n" +
				"11) Диопсид\n" +
				"12) Оливин\n" +
				"13) Альмандин\n" +
				"14) Корунд\n" +
				"15) Магнетит\n" +
				"16) Гемвтит\n" +
				"17) Галенит\n" +
				"18) Малибденит\n" +
				"19) Сфалерит\n" +
				"20) Пирит\n");

			string[] lines = File.ReadAllLines("materials.txt");
			string line = lines[Convert.ToInt32(Console.ReadLine()) - 1];
			var materialParameters = line.Split(" ");
			var material = new MaterialParameters();
			material.vp = Convert.ToDouble(materialParameters[0]) * 1000;
			material.vs = Convert.ToDouble(materialParameters[1]) * 1000;
			material.ro = Convert.ToDouble(materialParameters[2]);
			Console.WriteLine("Введите среднее давление газа или жидкости во включениях минерала: p(Па) = ");
			material.p = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите объемную долю твердого вещества в долях: f1 = ");
			material.f1 = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите объемную долю примеси в долях: f2 = ");
			material.f2 = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите предел прочности минерала (Па)");
			material.limit = Convert.ToDouble(Console.ReadLine());
			return material;
		}

		public static MaterialParameters GetNewMaterial()
		{
			Console.WriteLine("Введите скорость параллельных волн в минерале: Vp = ");
			var vp = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите скорость проточных волн в минералах: Vs = ");
			var vs = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите плотность минерала: Ro = ");
			var ro = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите среднее давление газа или жидкости во включениях минерала: p = ");
			var p = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите объемную долю минерала: f1 = ");
			var f1 = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите объемную долю примеси: f2 = ");
			var f2 = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите пределов прочности минерала");
			var limit = Convert.ToDouble(Console.ReadLine());
			var material = new MaterialParameters(vp, vs, ro, p, f1, f2, limit);
			return material;
		}

		private static double FindMaterialStrength(MaterialParameters material)
		{
			var externalStressField = MaterialParameters.CreateExternalStressField();
			var mu = MaterialParameters.GetMu(material.vp, material.vs, material.ro);
			var lambda = MaterialParameters.GetLambda(material.vp, material.vs, material.ro, mu);
			var mineralElasticityModule = MaterialParameters.FindMineralElasticityModule(lambda, mu, true);
			var mineralElasticityModuleWithAddition = MaterialParameters.FindMineralElasticityModuleWithAddition(lambda, mu, material.p, material.f1, material.f2);
			var inversedMatrix = MaterialParameters.FindInversedMatrix(mineralElasticityModuleWithAddition);
			var mineralElasticModuleTensor = MaterialParameters.ConvertElasticModuleMineralsTensor(mineralElasticityModule);
			var mineralElasticModuleTensorWithAddition = MaterialParameters.ConvertElasticModuleMineralsTensorWithAddition(inversedMatrix);
			var sigma33 = MaterialParameters.FindSigma33(mineralElasticModuleTensor, mineralElasticModuleTensorWithAddition, externalStressField, material.limit);
			Console.WriteLine($"Предел прочности материала := {sigma33}");
			return sigma33;
		}

		public static double FindOptimalMaterialStrength(MaterialParameters material1, MaterialParameters material2)
		{
			Console.WriteLine("\n \nРезультаты каждого шага алгоритма для первого минерала");
			var materialStrength1 = FindMaterialStrength(material1);
			Console.WriteLine("\n \nВывод каждого шага алгоритма для второго минерала");
			var materialStrength2 = FindMaterialStrength(material2);
			var optimeMaterialStrength = Math.Max(materialStrength1, materialStrength2);
			Console.WriteLine($"\n \nКонечный предел прочности материала := {optimeMaterialStrength}");
			return optimeMaterialStrength;
		}

		public static LRFuzzyNumber FindDrivePower(MaterialParameters material1, MaterialParameters material2)
		{
			var sigma = Math.Pow(FindOptimalMaterialStrength(material1, material2), 2);
			Console.WriteLine("Введите альфу и бету для предела прочности материала \n альфа =");
			var sigma_a = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("бета =");
			var sigma_b = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите длинну камеры дробления дробилки (м) =");
			var L = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите модуль упругости (Па) =");
			var E = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите диаметр загружаемых кусков материала (м) =");
			var D = Math.Pow(Convert.ToDouble(Console.ReadLine()), 2);
			Console.WriteLine("Введите диаметр куска готовго продукта (м) =");
			var d = Math.Pow(Convert.ToDouble(Console.ReadLine()), 2);
			Console.WriteLine("Введите частоту вращения эксцентрикового вала (об/с) =");
			var N = Math.Pow(Convert.ToDouble(Console.ReadLine()), 2);

			LRFuzzyNumber fuzzySigma = new LRFuzzyNumber(sigma, sigma_a, sigma_b);
			LRFuzzyNumber fuzzyL = new LRFuzzyNumber(L, 0, 0);
			LRFuzzyNumber fuzzyE = new LRFuzzyNumber(E, 0, 0);
			LRFuzzyNumber fuzzyD = new LRFuzzyNumber(D-d, 0, 0);
			LRFuzzyNumber fuzzyN = new LRFuzzyNumber(N, 0, 0);

			var a = LRFuzzyNumbersOperations.Mupltiply(fuzzySigma, fuzzyL);
			var b = LRFuzzyNumbersOperations.Mupltiply(a, fuzzyD);
			var c = LRFuzzyNumbersOperations.Divide(b, fuzzyE);
			var result = LRFuzzyNumbersOperations.Mupltiply(c, fuzzyN);
			return result;
		}
	}
}
