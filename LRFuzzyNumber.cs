namespace Lab4
{
	public class LRFuzzyNumber
	{
		public LRFuzzyNumber(double m, double a, double b)
		{
			this.m = m;
			this.a = a;
			this.b = b;
		}

		public LRFuzzyNumber()
		{
		}

		public double m { get; set; }
		public double a { get; set; }
		public double b { get; set; }
	}
}
