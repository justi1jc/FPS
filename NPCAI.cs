using UnityEngine;
using System.Collections;

public interface NPCAI {
   void Begin(NPC npc);
   void Pause();
   void Resume();
   ItemData GetData();
   void LoadData(ItemData dat);
}
