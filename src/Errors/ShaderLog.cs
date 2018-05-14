using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.GLSL.Errors
{
	public class ShaderLogLine
	{
		public string Type = string.Empty;
		public int FileNumber = -1;
		public int LineNumber = -1;
		public string Message = string.Empty;
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (string.Empty != Type)
			{
				sb.Append(Type + ": ");
			}
			if (-1 != LineNumber)
			{
				sb.Append("Line ");
				sb.Append(LineNumber.ToString());
				sb.Append(" : ");
			}
			sb.Append(Message);
			return sb.ToString();
		}
	}

	public static class ShaderLog
	{
		public const string ERROR_TOKEN = "ERROR";

		public static List<ShaderLogLine> Parse(string log)
		{
			//parse error log
			char[] splitChars = new char[] { '\n' };
			var errorLines = new List<ShaderLogLine>();
			var otherLines = new List<ShaderLogLine>();
			foreach (var line in log.Split(splitChars, StringSplitOptions.RemoveEmptyEntries))
			{
				ShaderLogLine logLine;
				try
				{
					logLine = ParseLogLineNVIDIA(line);
				}
				catch
				{
					logLine = ParseLogLine(line);
				}
				if (logLine.Type.StartsWith(ERROR_TOKEN))
				{
					errorLines.Add(logLine);
				}
				else
				{
					otherLines.Add(logLine);
				}
			}
			//first error messages, then all others
			errorLines.AddRange(otherLines);
			return errorLines;
		}

		private static ShaderLogLine ParseLogLine(string line)
		{
			ShaderLogLine logLine = new ShaderLogLine();
			char[] splitChars = new char[] { ':' };
			var elements = line.Split(splitChars, 4);
			switch(elements.Length)
			{
				case 4:
					logLine.Type = ParseType(elements[0]);
					logLine.FileNumber = ParseNumber(elements[1]);
					logLine.LineNumber = ParseNumber(elements[2]);
					logLine.Message = elements[3];
					break;
				case 3:
					logLine.Type = ParseType(elements[0]);
					logLine.Message = elements[1] + ":" + elements[2];
					break;
				case 2:
					logLine.Type = ParseType(elements[0]);
					logLine.Message = elements[1];
					break;
				case 1:
					logLine.Message = elements[0];
					break;
				default:
					throw new ArgumentException(line);
			}
			logLine.Message = logLine.Message.Trim();
			return logLine;
		}

		private static ShaderLogLine ParseLogLineNVIDIA(string line)
		{
			ShaderLogLine logLine = new ShaderLogLine();
			char[] splitChars = new char[] { ':' };
			var elements = line.Split(splitChars, 3);
			switch (elements.Length)
			{
				case 3:
					logLine.FileNumber = ParseNVFileNumber(elements[0]);
					logLine.LineNumber = ParseNVLineNumber(elements[0]);
					logLine.Type = ParseNVType(elements[1]);
					logLine.Message = elements[1] + ":" + elements[2];
					break;
				default:
					throw new ArgumentException(line);
			}
			logLine.Message = logLine.Message.Trim();
			return logLine;
		}

		private static string ParseNVType(string v)
		{
			char[] splitChars = new char[] { ' ', '\t' };
			var elements = v.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
			return ParseType(elements[0]);
		}

		private static int ParseNVLineNumber(string v)
		{
			char[] splitChars = new char[] { '(',')', ' ', '\t' };
			var elements = v.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
			return ParseNumber(elements[1]);
		}

		private static int ParseNVFileNumber(string v)
		{
			char[] splitChars = new char[] { '(', ')', ' ', '\t' };
			var elements = v.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
			return ParseNumber(elements[0]);
		}

		private static string ParseType(string typeString)
		{
			return typeString.ToUpperInvariant().Trim();
		}

		private static int ParseNumber(string number)
		{
			if (int.TryParse(number, out int output))
			{
				return output;
			}
			else
			{
				return -1;
			}
		}
	}
}
