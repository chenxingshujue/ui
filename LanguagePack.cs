using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace User{
	public class LanguagePack {
		static Dictionary<int,string> langdic ;
		static Dictionary<string,int> langdic_id;
		static int max_search_count = 20;
		static CsvWriter writer ;
		static string filepath = "/Editor/language.csv";
		static System.Text.Encoding encoding;
		static int max_id =0;
		public static void Init(int client_id){
			encoding = GetFileEncodeType (Application.dataPath + filepath);
			string csv = File.ReadAllText (Application.dataPath + filepath,encoding);
			CsvReader reader = new CsvReader ();
			langdic = new Dictionary<int, string> ();
			langdic_id = new Dictionary<string, int> ();
			writer = new CsvWriter ();
			if(!string.IsNullOrEmpty(csv.Trim())){
				foreach (var row in reader.Read(csv)) {
					string value;
					int id = int.Parse (row [0]);
					if (id > client_id && id < client_id + 1000000) {
						max_id = Mathf.Max (max_id, id);
					}
					langdic.TryGetValue (id, out value);
					if (string.IsNullOrEmpty (value)) {
						langdic.Add (id, row[1]);
						langdic_id.Add(row[1],id);
						writer.AddRow (row);
					} else {
						Debug.LogError ("language.csv 中有重复id：" +id);
					}
				}
			}
			if (max_id == 0) {
				max_id = client_id;
			}
		}
		public static string GetTextById(int id){
			string text = string.Empty;
			if (langdic != null) {
				langdic.TryGetValue (id,out text);
			}
			return text;
		}

		public static int GetIdByText(string text){
			int id = 0;
			if (langdic_id != null) {
				langdic_id.TryGetValue (text,out id);
			}
			return id;
		}
		
		public static int AddText(string text){
			if (string.IsNullOrEmpty (text)) {
				return 0;
			}
			int id = ++max_id;
			langdic.Add (id, text);
			langdic_id.Add (text, id);
			writer.AddRow (new string[]{id.ToString(), text });
			string contents = writer.Write ();
			File.WriteAllText (Application.dataPath + filepath,contents,encoding);
			return id;
		}
		
		public static List<string> SearchText(string text,out List<int> ids){
			List<string> list = new List<string> ();
			ids = new List<int> ();
			foreach(var kp in langdic){
				if (kp.Value.Contains (text)) {
					list.Add (kp.Value);
					ids.Add (kp.Key);
					if (list.Count > max_search_count) {
						break;
					}
				}
			}
			return list;
		}
		
		
		public static System.Text.Encoding  GetFileEncodeType(string filename)
		{
			System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
			byte[] buffer = br.ReadBytes(2);
			fs.Close ();
			if(buffer[0]>=0xEF)
			{
				if(buffer[0]==0xEF && buffer[1]==0xBB)
				{
					return System.Text.Encoding.UTF8;
				}
				else if(buffer[0]==0xFE && buffer[1]==0xFF)
				{
					return System.Text.Encoding.BigEndianUnicode;
				}
				else if(buffer[0]==0xFF && buffer[1]==0xFE)
				{
					return System.Text.Encoding.Unicode;
				}
				else
				{
					return System.Text.Encoding.Default;
				}
			}
			else
			{
				return System.Text.Encoding.Default;
			}
		}


	}


}