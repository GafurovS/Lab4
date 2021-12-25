using System;


namespace Lab4
{
	class APCS
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Формат ввода чисел: 9999,9999");
			Console.WriteLine("\nВыберите действие для первого минерала\n" +
				"1)Выбрать минерал из таблицы \n" +
				"2)Ввести новый минерал");
			var tableNumber = Convert.ToInt32(Console.ReadLine());
			MaterialParameters material1 = new MaterialParameters();

			if (tableNumber == 1)
			{
				material1 = BurnerParameters.GetMaterialFromTable();
			}

			if(tableNumber == 2)
			{
				material1 = BurnerParameters.GetNewMaterial();
			}

			Console.WriteLine("Выберите действие для второго минерала\n" +
				"1)Выбрать минерал из таблицы \n" +
				"2)Ввести новый минерал");
			tableNumber = Convert.ToInt32(Console.ReadLine());
			MaterialParameters material2 = new MaterialParameters();

			if (tableNumber == 1)
			{
				material2 = BurnerParameters.GetMaterialFromTable();
			}

			if (tableNumber == 2)
			{
				material2 = BurnerParameters.GetNewMaterial();
			}

			var destructionTemperature = BurnerParameters.FindDrivePower(material1, material2);
			Console.WriteLine($"Мощность привода дробилки = ({destructionTemperature.m};{destructionTemperature.a};{destructionTemperature.b}) Ватт");
			Console.ReadKey();
		}
	}
}
