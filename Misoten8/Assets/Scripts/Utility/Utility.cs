using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

/// <summary>
/// misoten8Utility 名前空間
/// 製作者：実川
/// </summary>
namespace Misoten8Utility
{
	public static class FanMath
	{
		public static bool OverBorder(int fanPoint, Define.FanLevel fanLevel) => fanPoint >= Define.FanPointArray[(int)fanLevel];
		public static int GetFanScore(Define.FanLevel fanLevel) => Define.FanScoreArray[(int)fanLevel];
	}

	public static class ArrayExtensions
	{
		public static bool IsNullOrEmpty<T>(this T[] target)
		{
			if (target == null)
				return true;

			return target.Count() == 0;
		}
	}

	public static class EnumerableExtensions
	{
		public static Decimal[] SumDoubleArray(this IEnumerable<Decimal[][]> element, int arraySize)
		{
			Decimal[] result = element
				.SelectMany(x => x.Select(e =>
				{
					Decimal num1 = new Decimal(0);
					foreach (Decimal num2 in e)
						num1 += num2;
					return num1;
				})).ToArray();

			return result;
		}

		/// <summary>
		/// 選択した範囲の要素を取り出す
		/// </summary>
		public static IEnumerable<T> ElementsRange<T>(this IEnumerable<T> element, int beginIndex, int endIndex)
		{
			return element.Skip(beginIndex).Take(endIndex - beginIndex);
		}

		/// <summary>
		/// 最大値の要素が格納されている番号を取得する
		/// </summary>
		public static int FindIndexMax<T>(this IEnumerable<T> element)
		{
			return element
				.Select((v, i) => new { Value = v, Index = i })
				.First(e => e.Value.Equals(element.Select(s => s).Max())).Index;
		}

		/// <summary>
		/// 最大値の要素が格納されている番号を取得する
		/// </summary>
		public static int FindIndexMin<T>(this IEnumerable<T> element)
		{
			return element
				.Select((v, i) => new { Value = v, Index = i })
				.First(e => e.Value.Equals(element.Select(s => s).Min())).Index;
		}
	}
}
