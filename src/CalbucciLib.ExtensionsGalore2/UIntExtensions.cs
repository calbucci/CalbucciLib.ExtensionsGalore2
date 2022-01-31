﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.ExtensionsGalore2
{
	public static class UIntExtensions
	{
		public static string? ToLiteral(this uint u)
		{
			return ((long) u).ToLiteral();
		}

		public static int CountBits(this uint u)
		{
			if (u == 0)
				return 0;

			u -= ((u >> 1) & 0x55555555);
			u = (u & 0x33333333) + ((u >> 2) & 0x33333333);
			return (int)((((u + (u >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
		}
	}
}
