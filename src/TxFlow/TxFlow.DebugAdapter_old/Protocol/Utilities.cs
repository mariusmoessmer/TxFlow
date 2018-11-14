/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace TxFlow.DebugAdapter.Protocol
{
	public class Utilities
	{
		private static readonly Regex VARIABLE = new Regex(@"\{(\w+)\}");


		public static string ExpandVariables(string format, dynamic variables, bool underscoredOnly = true)
		{
			if (variables == null) {
				variables = new { };
			}
			Type type = variables.GetType();
			return VARIABLE.Replace(format, match => {
				string name = match.Groups[1].Value;
				if (!underscoredOnly || name.StartsWith("_")) {

					PropertyInfo property = type.GetProperty(name);
					if (property != null) {
						object value = property.GetValue(variables, null);
						return value.ToString();
					}
					return '{' + name + ": not found}";
				}
				return match.Groups[0].Value;
			});
		}
	}
}
