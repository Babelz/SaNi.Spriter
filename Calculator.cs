using System;
using Microsoft.Xna.Framework;

namespace SaNi.Spriter
{

	/// <summary>
	/// A utility class which provides methods to calculate Spriter specific issues,
	/// like linear interpolation and rotation around a parent object.
	/// Other interpolation types are coming with the next releases of Spriter.
	/// 
	/// @author Trixt0r
	/// 
	/// </summary>

	public class Calculator
	{

		public static readonly float PI = (float)Math.PI;

		/// <summary>
		/// Calculates the smallest difference between angle a and b. </summary>
		/// <param name="a"> first angle (in degrees) </param>
		/// <param name="b"> second angle (in degrees) </param>
		/// <returns> Smallest difference between a and b (between 180� and -180�). </returns>
		public static float AngleDifference(float a, float b)
		{
			return ((((a - b) % 360) + 540) % 360) - 180;
		}

		/// <param name="x1"> x coordinate of first point. </param>
		/// <param name="y1"> y coordinate of first point. </param>
		/// <param name="x2"> x coordinate of second point. </param>
		/// <param name="y2"> y coordinate of second point. </param>
		/// <returns> Angle between the two given points. </returns>
		public static float AngleBetween(float x1, float y1, float x2, float y2)
		{
			return (float)MathHelper.ToDegrees((float) Math.Atan2(y2 - y1,x2 - x1));
		}

		/// <param name="x1"> x coordinate of first point. </param>
		/// <param name="y1"> y coordinate of first point. </param>
		/// <param name="x2"> x coordinate of second point. </param>
		/// <param name="y2"> y coordinate of second point. </param>
		/// <returns> Distance between the two given points. </returns>
		public static float DistanceBetween(float x1, float y1, float x2, float y2)
		{
			float xDiff = x2 - x1;
			float yDiff = y2 - y1;
			return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
		}

		/// <summary>
		/// Solves the equation a*x^3 + b*x^2 + c*x +d = 0. </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		/// <param name="c"> </param>
		/// <param name="d"> </param>
		/// <returns> the solution of the cubic function </returns>
		public static float? SolveCubic(float a, float b, float c, float d)
		{
			if (a == 0)
			{
				return SolveQuadratic(b, c, d);
			}
			if (d == 0)
			{
				return 0f;
			}

			b /= a;
			c /= a;
			d /= a;
			float squaredB = Squared(b);
			float q = (3f * c - squaredB) / 9f;
			float r = (-27f * d + b * (9f * c - 2f * squaredB)) / 54f;
			float disc = Cubed(q) + Squared(r);
			float term1 = b / 3f;

			if (disc > 0)
			{
				float s = r + Sqrt(disc);
				s = (s < 0) ? - CubicRoot(-s) : CubicRoot(s);
				float t = r - Sqrt(disc);
				t = (t < 0) ? - CubicRoot(-t) : CubicRoot(t);

				float result = -term1 + s + t;
				if (result >= 0 && result <= 1)
				{
					return result;
				}
			}
			else if (disc == 0)
			{
				float r13 = (r < 0) ? - CubicRoot(-r) : CubicRoot(r);

				float result = -term1 + 2f * r13;
				if (result >= 0 && result <= 1)
				{
					return result;
				}

				result = -(r13 + term1);
				if (result >= 0 && result <= 1)
				{
					return result;
				}
			}
			else
			{
				q = -q;
				float dum1 = q * q * q;
				dum1 = Acos(r / Sqrt(dum1));
				float r13 = 2f * Sqrt(q);

				float result = -term1 + r13 * Cosf(dum1 / 3f);
				if (result >= 0 && result <= 1)
				{
					return result;
				}

				result = -term1 + r13 * Cosf((dum1 + 2f * PI) / 3f);
				if (result >= 0 && result <= 1)
				{
					return result;
				}

				result = -term1 + r13 * Cosf((dum1 + 4f * PI) / 3f);
				if (result >= 0 && result <= 1)
				{
					return result;
				}
			}

			return null;
		}

		/// <summary>
		/// Solves the equation a*x^2 + b*x + c = 0 </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		/// <param name="c"> </param>
		/// <returns> the solution for the quadratic function </returns>
		public static float? SolveQuadratic(float a, float b, float c)
		{
			float squaredB = Squared(b);
			float twoA = 2 * a;
			float fourAC = 4 * a * c;
			float result = (-b + Sqrt(squaredB - fourAC)) / twoA;
			if (result >= 0 && result <= 1)
			{
				return result;
			}

			result = (-b - Sqrt(squaredB - fourAC)) / twoA;
			if (result >= 0 && result <= 1)
			{
				return result;
			}

			return null;
		}

		/// <summary>
		/// Returns the square of the given value. </summary>
		/// <param name="f"> the value </param>
		/// <returns> the square of the value </returns>
		public static float Squared(float f)
		{
			return f * f;
		}

		/// <summary>
		/// Returns the cubed value of the given one. </summary>
		/// <param name="f"> the value </param>
		/// <returns> the cubed value </returns>
		public static float Cubed(float f)
		{
			return f * f * f;
		}

		/// <summary>
		/// Returns the cubic root of the given value. </summary>
		/// <param name="f"> the value </param>
		/// <returns> the cubic root </returns>
		public static float CubicRoot(float f)
		{
			return (float) Math.Pow(f, 1f / 3f);
		}

		/// <summary>
		/// Returns the square root of the given value. </summary>
		/// <param name="x"> the value </param>
		/// <returns> the square root </returns>
		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt(x);
		}

		/// <summary>
		/// Returns the arc cosine at the given value. </summary>
		/// <param name="x"> the value </param>
		/// <returns> the arc cosine </returns>
		public static float Acos(float x)
		{
			return (float)Math.Acos(x);
		}

		private const int SIN_BITS = 14; // 16KB. Adjust for accuracy.
		private static readonly int SIN_MASK = ~(-1 << SIN_BITS);
		private static readonly int SIN_COUNT = SIN_MASK + 1;

		private static readonly float RadFull = PI * 2;
		private const float DegFull = 360;
		private static readonly float RadToIndex = SIN_COUNT / RadFull;
		private static readonly float DegToIndex = SIN_COUNT / DegFull;

		/// <summary>
		/// multiply by this to convert from radians to degrees </summary>
		public static readonly float RadiansToDegrees = 180f / PI;
		public static readonly float RadDeg = RadiansToDegrees;
		/// <summary>
		/// multiply by this to convert from degrees to radians </summary>
		public static readonly float DegreesToRadians = PI / 180;
		public static readonly float DegRad = DegreesToRadians;

		private class Sin
		{
			internal static readonly float[] Table = new float[SIN_COUNT];
			static Sin()
			{
				for (int i = 0; i < SIN_COUNT; i++)
				{
					Table[i] = (float)Math.Sin((i + 0.5f) / SIN_COUNT * RadFull);
				}
				for (int i = 0; i < 360; i += 90)
				{
					Table[(int)(i * DegToIndex) & SIN_MASK] = (float)Math.Sin(i * DegreesToRadians);
				}
			}
		}

		/// <summary>
		/// Returns the sine in radians from a lookup table. </summary>
		public static float Sinf(float radians)
		{
			return Sin.Table[(int)(radians * RadToIndex) & SIN_MASK];
		}

		/// <summary>
		/// Returns the cosine in radians from a lookup table. </summary>
		public static float Cosf(float radians)
		{
			return Sin.Table[(int)((radians + PI / 2) * RadToIndex) & SIN_MASK];
		}

		/// <summary>
		/// Returns the sine in radians from a lookup table. </summary>
		public static float SinDeg(float degrees)
		{
			return Sin.Table[(int)(degrees * DegToIndex) & SIN_MASK];
		}

		/// <summary>
		/// Returns the cosine in radians from a lookup table. </summary>
		public static float CosDeg(float degrees)
		{
			return Sin.Table[(int)((degrees + 90) * DegToIndex) & SIN_MASK];
		}

	}

}