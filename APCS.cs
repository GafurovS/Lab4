using System;




namespace Lab4
{
	class APCS
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Выберите действие для первого минерала\n" +
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

			var destructionTemperature = BurnerParameters.FindDestructionTemperature(material1, material2);
			Console.WriteLine($"Температура разрушения породы = ({destructionTemperature.m};{destructionTemperature.a};{destructionTemperature.b}) градусов Цельсия");
		}
	}
}
