/// <summary>
/// misoten8Utility 名前空間
/// 製作者：実川
/// </summary>
namespace Misoten8Utility
{
	public static class IntExtensions
	{
		public static void Loop(this int count, System.Action<int> action)
		{
			for (int i = 1; i <= count; i++) action(i);
		}

		public static bool IsOutRange(this int count, int min, int max)
		{
			if (count < min) return true;
			if (count > max) return true;
			return false;
		}
	}
}