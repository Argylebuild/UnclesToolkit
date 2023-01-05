namespace Argyle.UnclesToolkit
{
	public abstract class IntUtility
	{
		public static int MinBetween(int a, int b) =>
			a < b ? a : b;

		public static int MaxBetween(int a, int b) =>
			a > b ? a : b;

	}
}