using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
	public class LRFuzzyNumbersOperations
	{
		public static LRFuzzyNumber Mupltiply(LRFuzzyNumber fuzzyNumber1, LRFuzzyNumber fuzzyNumber2)
		{
			LRFuzzyNumber result = new LRFuzzyNumber();
			if (fuzzyNumber1.m > 0 && fuzzyNumber2.m > 0)
			{
				result.m = fuzzyNumber1.m * fuzzyNumber2.m;
				result.a = fuzzyNumber1.m * fuzzyNumber2.a + fuzzyNumber2.m * fuzzyNumber1.a - fuzzyNumber1.a * fuzzyNumber2.a;
				result.b = fuzzyNumber1.m * fuzzyNumber2.b + fuzzyNumber2.m * fuzzyNumber1.b - fuzzyNumber1.b * fuzzyNumber2.b;
			}
			if(fuzzyNumber1.m > 0 && fuzzyNumber2.m < 0)
			{
				result.m = fuzzyNumber1.m * fuzzyNumber2.m;
				result.a = fuzzyNumber1.m * fuzzyNumber2.a - fuzzyNumber2.m * fuzzyNumber1.b - fuzzyNumber2.a * fuzzyNumber1.b;
				result.b = fuzzyNumber1.m * fuzzyNumber2.b - fuzzyNumber2.m * fuzzyNumber1.a - fuzzyNumber1.a * fuzzyNumber2.b;
			}
			if (fuzzyNumber1.m < 0 && fuzzyNumber2.m > 0)
			{
				result.m = fuzzyNumber1.m * fuzzyNumber2.m;
				result.a = -fuzzyNumber1.m * fuzzyNumber2.b + fuzzyNumber2.m * fuzzyNumber1.a + fuzzyNumber1.a * fuzzyNumber2.b;
				result.b = -fuzzyNumber1.m * fuzzyNumber2.a + fuzzyNumber2.m * fuzzyNumber1.b - fuzzyNumber2.a * fuzzyNumber1.b;
			}
			return result;
		}

		public static LRFuzzyNumber Divide(LRFuzzyNumber fuzzyNumber1, LRFuzzyNumber fuzzyNumber2)
		{
			LRFuzzyNumber result = new LRFuzzyNumber();
			if (fuzzyNumber1.m > 0 && fuzzyNumber2.m > 0)
			{
				result.m = fuzzyNumber1.m / fuzzyNumber2.m;
				result.a = (fuzzyNumber1.m * fuzzyNumber2.b + fuzzyNumber2.m * fuzzyNumber1.a) / (fuzzyNumber2.m * (fuzzyNumber2.m + fuzzyNumber2.b));
				result.b = (fuzzyNumber1.m * fuzzyNumber2.a + fuzzyNumber2.m * fuzzyNumber1.b) / (fuzzyNumber2.m * (fuzzyNumber2.m - fuzzyNumber2.a));
			}
			if (fuzzyNumber1.m > 0 && fuzzyNumber2.m < 0)
			{
				result.m = fuzzyNumber1.m / fuzzyNumber2.m;
				result.a = (fuzzyNumber1.m * fuzzyNumber2.b - fuzzyNumber2.m * fuzzyNumber1.b) / (fuzzyNumber2.m * (fuzzyNumber2.m + fuzzyNumber2.b));
				result.b = (fuzzyNumber1.m * fuzzyNumber2.a - fuzzyNumber2.m * fuzzyNumber1.a) / (fuzzyNumber2.m * (fuzzyNumber2.m - fuzzyNumber2.a));
			}
			if (fuzzyNumber1.m < 0 && fuzzyNumber2.m > 0)
			{
				result.m = fuzzyNumber1.m / fuzzyNumber2.m;
				result.a = (-fuzzyNumber1.m * fuzzyNumber2.a + fuzzyNumber2.m * fuzzyNumber1.a) / (fuzzyNumber2.m * (fuzzyNumber2.m - fuzzyNumber2.a));
				result.b = (-fuzzyNumber1.m * fuzzyNumber2.b + fuzzyNumber2.m * fuzzyNumber1.b) / (fuzzyNumber2.m * (fuzzyNumber2.m + fuzzyNumber2.b));
			}
			if (fuzzyNumber1.m < 0 && fuzzyNumber2.m < 0)
			{
				result.m = fuzzyNumber1.m / fuzzyNumber2.m;
				result.a = (-fuzzyNumber1.m * fuzzyNumber2.a - fuzzyNumber2.m * fuzzyNumber1.b) / (fuzzyNumber2.m * (fuzzyNumber2.m - fuzzyNumber2.a));
				result.b = (-fuzzyNumber1.m * fuzzyNumber2.b + fuzzyNumber2.m * fuzzyNumber1.a) / (fuzzyNumber2.m * (fuzzyNumber2.m + fuzzyNumber2.b));
			}
			return result;
		}
	}
}
