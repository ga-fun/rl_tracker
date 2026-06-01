namespace GuillaumeAst.Utils;

public static class Maths
{
	public const double Precision = 0.1f;

	public static bool DoublesAreEqual(double a, double b)
	{
		return a - Precision <= b && b <= a + Precision;
	}
}