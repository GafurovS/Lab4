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
			material.vp = Convert.ToDouble(materialParameters[0]);
			material.vs = Convert.ToDouble(materialParameters[1]);
			material.ro = Convert.ToDouble(materialParameters[2]);
			Console.WriteLine("Введите среднее давление газа или жидкости во включениях минерала: p(Па) = ");
			material.p = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите объемную долю твердого вещества от 0 до 1: f1 = ");
			material.f1 = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите объемную долю примеси 0 до 1: f2 = ");
			material.f2 = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите предел прочности минерала");
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
			Console.WriteLine("Результаты каждого шага алгоритма для первого минерала");
			var materialStrength1 = FindMaterialStrength(material1);
			Console.WriteLine("Нажмите любую клавишу для вывода каждого шага алгоритма для второго минерала");
			Console.ReadKey();
			var materialStrength2 = FindMaterialStrength(material2);
			var optimeMaterialStrength = Math.Max(materialStrength1, materialStrength2);
			Console.WriteLine($"Конечный предел прочности материала := {optimeMaterialStrength}");
			return optimeMaterialStrength;
		}

		public static LRFuzzyNumber FindDestructionTemperature(MaterialParameters material1, MaterialParameters material2)
		{
			var sigma = 1.5 * FindOptimalMaterialStrength(material1, material2);
			Console.WriteLine("Введите альфу и бету для предела прочности материала \n альфа =");
			var sigma_a = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("бета =");
			var sigma_b = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите коэффицент Пуассона от 0 до 1 =");
			var gamma = 1 - Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите коэффицент теплового расширения породы(1/С) =");
			var beta = Convert.ToDouble(Console.ReadLine());
			Console.WriteLine("Введите модуль упругости(Па) =");
			var e = Convert.ToDouble(Console.ReadLine());

			LRFuzzyNumber fuzzySigma = new LRFuzzyNumber(sigma, sigma_a, sigma_b);
			LRFuzzyNumber fuzzyGamma = new LRFuzzyNumber(gamma, 0, 0);
			LRFuzzyNumber fuzzyBeta = new LRFuzzyNumber(beta, 0, 0);
			LRFuzzyNumber fuzzyE = new LRFuzzyNumber(e, 0, 0);

			var a = LRFuzzyNumbersOperations.Mupltiply(fuzzySigma, fuzzyGamma);
			var b = LRFuzzyNumbersOperations.Mupltiply(fuzzyBeta, fuzzyE);
			var result = LRFuzzyNumbersOperations.Divide(a, b);
			return result;
		}
	}
}
