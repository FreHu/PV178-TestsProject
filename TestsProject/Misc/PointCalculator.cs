namespace TestsProject.Misc
{
	public class PointCalculator
	{
		public static double CalculatePoints(int value, int correct, int incorrect,int totalCorrect)
		{
			if (value == 0)
			{
				return 0;
			}
			return (correct - incorrect)*value/totalCorrect;
		}
	}
}