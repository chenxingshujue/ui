using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;


public class CsvConfig
{
	public char Delimiter { get; private set; }
	public string NewLineMark { get; private set; }
	public char QuotationMark { get; private set; }

	public CsvConfig(char delimiter, string newLineMark, char quotationMark)
	{
		Delimiter = delimiter;
		NewLineMark = newLineMark;
		QuotationMark = quotationMark;
	}

	// useful configs

	public static CsvConfig Default
	{
		get { return new CsvConfig(',', "\r\n", '\"'); }
	}

	// etc.
}
public class CsvWriter
{
	private CsvConfig m_config;
	private StringBuilder m_csvContents;

	public CsvWriter(CsvConfig config = null)
	{
		if (config == null)
			m_config = CsvConfig.Default;
		else
			m_config = config;

		m_csvContents = new StringBuilder();
	}

	public void AddRow(IEnumerable<string> cells)
	{
		int i = 0;
		foreach (string cell in cells)
		{
			m_csvContents.Append(ParseCell(cell));
			m_csvContents.Append(m_config.Delimiter);

			i++;
		}

		m_csvContents.Length--; // remove last delimiter
		m_csvContents.Append("\r\n");
	}

	private string ParseCell(string cell)
	{
		// cells cannot be multi-line
		cell = cell.Replace("\r", "");
		cell = cell.Replace("\n", "");

		if ( ! NeedsToBeEscaped(cell))
			return cell;

		// double every quotation mark
		cell = cell.Replace(m_config.QuotationMark.ToString(), string.Format("{0}{0}", m_config.QuotationMark));

		// add quotation marks at the beginning and at the end
		cell = m_config.QuotationMark + cell + m_config.QuotationMark;

		return cell;
	}

	private bool NeedsToBeEscaped(string cell)
	{
		if (cell.Contains(m_config.QuotationMark.ToString()))
			return true;

		if (cell.Contains(m_config.Delimiter.ToString()))
			return true;

		return false;
	}

	public string Write()
	{
		return m_csvContents.ToString();
	}
}

public class CsvReader
{
	private CsvConfig m_config;

	public CsvReader(CsvConfig config = null)
	{
		if (config == null)
			m_config = CsvConfig.Default;
		else
			m_config = config;
	}

	public IEnumerable<string[]> Read(string csvFileContents)
	{
		using (StringReader reader = new StringReader(csvFileContents))
		{
			while (true)
			{
				string line = reader.ReadLine();
				if (line == null)
					break;
				yield return ParseLine(line);
			}
		}
	}

	private string[] ParseLine(string line)
	{
		Stack<string> result = new Stack<string>();

		int i = 0;
		while (true)
		{
			string cell = ParseNextCell(line, ref i);
			if (cell == null)
				break;
			result.Push(cell);
		}

		// remove last elements if they're empty
		while (string.IsNullOrEmpty(result.Peek()))
		{
			result.Pop();
		}

		var resultAsArray = result.ToArray();
		Array.Reverse(resultAsArray);
		return resultAsArray;
	}

	// returns iterator after delimiter or after end of string
	private string ParseNextCell(string line, ref int i)
	{
		if (i >= line.Length)
			return null;

		if (line[i] != m_config.QuotationMark)
			return ParseNotEscapedCell(line, ref i);
		else
			return ParseEscapedCell(line, ref i);
	}

	// returns iterator after delimiter or after end of string
	private string ParseNotEscapedCell(string line, ref int i)
	{
		StringBuilder sb = new StringBuilder();
		while (true)
		{
			if (i >= line.Length) // return iterator after end of string
				break;
			if (line[i] == m_config.Delimiter)
			{
				i++; // return iterator after delimiter
				break;
			}
			sb.Append(line[i]);
			i++;
		}
		return sb.ToString();
	}

	// returns iterator after delimiter or after end of string
	private string ParseEscapedCell(string line, ref int i)
	{
		i++; // omit first character (quotation mark)
		StringBuilder sb = new StringBuilder();
		while (true)
		{
			if (i >= line.Length)
				break;
			if (line[i] == m_config.QuotationMark)
			{
				i++; // we're more interested in the next character
				if (i >= line.Length)
				{
					// quotation mark was closing cell;
					// return iterator after end of string
					break;
				}
				if (line[i] == m_config.Delimiter)
				{
					// quotation mark was closing cell;
					// return iterator after delimiter
					i++;
					break;
				}
				if (line[i] == m_config.QuotationMark)
				{
					// it was doubled (escaped) quotation mark;
					// do nothing -- we've already skipped first quotation mark
				}

			}
			sb.Append(line[i]);
			i++;
		}

		return sb.ToString();
	}
}