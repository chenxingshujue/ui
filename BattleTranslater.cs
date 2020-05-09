using System;
using UnityEngine;
using BattleProtoCmd;
using System.Collections.Generic;
using Config2;
using sw.util;
using System.IO;
using System.Diagnostics;
using ProtoBuf;
using sw.game.cmd;


namespace User
{
	public class BattleTranslater
	{
		static BattleTranslater m_instance = null;
		public static BattleTranslater Instance{
			get{
				if (m_instance == null) {
					m_instance = new BattleTranslater ();
				}
				return m_instance;
			}
		}
		string lastpath = null;
		const string WRAP = "\r\n";
		Dictionary<uint,BattleHeroInfo> heros = new Dictionary<uint, BattleHeroInfo> ();
		Dictionary<RecordType,string> record_template =  new Dictionary<RecordType,string> () ;
		BattleTranslater()
		{
			record_template.Add (RecordType.BattleAtkRec, "发动攻击,使用技能{0},目标效果:{1}");
			record_template.Add (RecordType.BattleBufAddRec, "给{0}增加buff{1}");
			record_template.Add (RecordType.BattleBufRemoveRec, "给{0}移除buff{1}");
			record_template.Add (RecordType.BattleDieRec, "{0}阵亡");
			record_template.Add (RecordType.BattleHpRec, "{0}血量{1}");
			record_template.Add (RecordType.BattleMpRec, "{0}当前怒气{1}");
		}

		double receiveBattleIndex;
		public void Start(){
			//receiveBattleIndex = EventDispatcher.AddEventListener(BattleEventType.BATTLE_DATA_RECEIVE,OnReceiveBattleData);
		}
		public void Stop()
		{
			EventDispatcher.RemoveEventListener(BattleEventType.BATTLE_DATA_RECEIVE, receiveBattleIndex);
			receiveBattleIndex = 0;
		}


		string TranslateRecord(ClientBattleRecord value)
		{
			string temple = record_template [value.Type];
			switch (value.Type) 
			{
			case RecordType.BattleAtkRec:
				string text = "";
				foreach (BattleAtkTar tar in value.battleAtkRec.tarList) 
				{
					string name = GetHeroNameByTempid(tar.defId);
					if (tar.byLuck == 0) {
						text += string.Format ("{0}闪避了", name);
					} else if (tar.byLuck == 1) {
						text += string.Format ("{0}受到{1}点伤害", name,tar.dmg);
					} else if (tar.byLuck == 2) {
						text += string.Format ("{0}受到{1}点暴击伤害", name,tar.dmg);
					}
					text += ",";
				}
				return string.Format (temple, GetSkillNameById (value.battleAtkRec.skillId), text);
				
			case RecordType.BattleBufAddRec:
				text = "";
				foreach (uint tempid in value.battleBufAddRec.tempList) 
				{
					text += GetHeroNameByTempid (tempid);
					text += ",";
				}
				return string.Format (temple,text,GetBuffNameById(value.battleBufAddRec.buffId));
			case RecordType.BattleBufRemoveRec:
				text = "";
				foreach (uint tempid in value.battleBufRemoveRec.tempList) 
				{
					text += GetHeroNameByHeroid (tempid);
					text += ",";
				}
				return string.Format (temple,text,GetBuffNameById(value.battleBufRemoveRec.buffId));
		
			case RecordType.BattleDieRec:
				return string.Format (temple,GetHeroNameByTempid(value.battleDieRec.tempId));

			case RecordType.BattleHpRec:
				return string.Format (temple,GetHeroNameByTempid(value.battleHpRec.tempId),value.battleHpRec.hp);

			case RecordType.BattleMpRec:
				return string.Format (temple,GetHeroNameByTempid(value.battleMpRec.tempId),value.battleMpRec.curMp);

			default:
				return string.Empty;

			}
			return string.Empty;
		}

	    private void OnReceiveBattleData(params object[] arg)
	    {
	        BattleData data = arg[0] as BattleData;
	        if (data == null)
	            return;

			string text = "";
			text += "战斗id(battleID)：" + data.battleId;
			text += WRAP;
			text += "攻击方初始状态：";
			text += WRAP;

			heros.Clear ();
			for(int i =0;i<data.atkTeam.heroList.Count;i++)
			{
				BattleHeroInfo info = data.atkTeam.heroList[i];
				heros.Add (info.tempId,info);
				string template = "{0} heroId({1}),tempId({2}),位置{3},血量{4}/{6},怒气{5}";
				text += string.Format (template,GetHeroNameByHeroid(info.heroId),info.heroId, info.tempId, info.pos, info.hp, info.mp, info.maxHp);
				text += WRAP;
			}

			text += "防御方方初始状态：";
			text += WRAP;
			for(int i =0;i<data.defTeam.heroList.Count;i++)
			{
				BattleHeroInfo info = data.defTeam.heroList[i];
				heros.Add (info.tempId,info);
				string template = "{0} heroId({1}),tempId({2}),位置{3},血量{4}/{6},怒气{5}";
				text += string.Format (template,GetHeroNameByHeroid(info.heroId),info.heroId, info.tempId, info.pos, info.hp, info.mp, info.maxHp);
				text += WRAP;
			}

			for (int i = 0;i < data.roundList.Count;i++)
			{
				text += string.Format("第{0}回合:",data.roundList[i].round);
				text += WRAP;
				for(int j = 0;j<data.roundList[i].records.Count;j++)
				{
					BattleHeroRecords records = data.roundList [i].records [j];
					BattleHeroInfo info = heros[records.tempId];
					string template  = "{0}({1})  ";
					text += string.Format (template, GetHeroNameByHeroid(info.heroId),info.pos);
					for (int k = 0; k < records.recordList.Count; k++) 
					{
						ClientBattleRecord record = (ClientBattleRecord) records.recordList [k];
						text += TranslateRecord(record);
						text += WRAP;
					}	
				}
			}

			text += "战斗结束";

			string folder = FileUtilNew.GetLocalPath () + "/battlelogs/";
			if (!Directory.Exists (folder)) {
				Directory.CreateDirectory (folder);
			} else {
				string[] files = Directory.GetFiles (folder,"*.txt");
				if (files.Length > 100) {
					Directory.Delete (folder, true);
					Directory.CreateDirectory (folder);
				}
			}
			lastpath = folder + data.battleId;
			File.WriteAllText (lastpath +".txt", text, System.Text.UTF8Encoding.UTF8);

	    }
		public void OpenLastLog(){
			if (lastpath == null) {
				return;
			}
			if (!File.Exists (lastpath +".txt")) {
				return;
			}
			Process.Start ("notepad",lastpath +".txt");
		}

		public void Replay(){
			if (lastpath == null) {
				return;
			}
			if (!File.Exists (lastpath )) {
				return;
			}
			byte[] str = File.ReadAllBytes (lastpath);

			BattleUserCmdRecordProcessor pro = new BattleUserCmdRecordProcessor();
			BattleData battleData = pro.ParseStr(str);
			Stop();
            EventDispatcher.DispatchEvent(BattleEventType.BATTLE_DATA_RECEIVE, battleData);
            Start();
		}

		public static void SaveByteStr(ulong id,byte[] cmd){
			string folder = FileUtilNew.GetLocalPath () + "/battlelogs/";
			if (! Directory.Exists(folder)) {
				Directory.CreateDirectory (folder);
			}
			string path  = folder + id ;
			File.WriteAllBytes (path, cmd);
		}


		string GetHeroNameByHeroid(uint heroid)
		{
			NpcConfig config = ConfigAsset2.Instance.GetById<NpcConfig>(heroid.ToString());
			if (config != null) 
			{
				return config.name;
			}
			return heroid.ToString();
		}

		string GetHeroNameByTempid(uint tempid)
		{
			return GetHeroNameByHeroid (heros [tempid].heroId);
		}

		string GetSkillNameById(uint skillid)
		{
			string id = skillid.ToString ().Substring (0, 6);
			SkillConfig config = ConfigAsset2.Instance.GetById<SkillConfig>(id);
			if (config != null) {
				return config.name;
			}
			return skillid.ToString();
		}

		string GetBuffNameById(uint buffid)
		{
			return "xx";
		}
	}
}

