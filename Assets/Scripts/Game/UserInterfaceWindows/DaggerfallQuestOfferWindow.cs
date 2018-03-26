// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implementation of a (dummy) popup window that can offer quests.
    /// </summary>
    public class DaggerfallQuestOfferWindow : DaggerfallQuestPopupWindow
    {
        StaticNPC questorNPC;
        FactionFile.SocialGroups socialGroup;

        #region Constructors

        public DaggerfallQuestOfferWindow(IUserInterfaceManager uiManager, StaticNPC npc, FactionFile.SocialGroups socialGroup)
            : base(uiManager)
        {
            questorNPC = npc;
            this.socialGroup = socialGroup;

            // Remove potential questor from pool after quest has been offered
            TalkManager.Instance.RemoveMerchantQuestor(npc.Data.nameSeed);

            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            CloseWindow();
            GetQuest();
        }

        #endregion

        #region Quest handling

        protected override void GetQuest()
        {
            // Just exit if this NPC already involved in an active quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor())
            {
                CloseWindow();
                return;
            }

            // Get the faction id for affecting reputation on success/failure, and current rep
            int factionId = questorNPC.Data.factionID;
            int reputation = GameManager.Instance.PlayerEntity.FactionData.GetReputation(factionId);

            // Select a quest at random from appropriate pool
            offeredQuest = GameManager.Instance.QuestListsManager.GetSocialQuest(socialGroup, factionId, reputation);
            if (offeredQuest != null)
            {
                // Log offered quest
                Debug.LogFormat("Offering quest {0} from Social group {1} affecting factionId {2}", offeredQuest.QuestName, socialGroup, offeredQuest.FactionId);

                // Offer the quest to player
                DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);// TODO - need to provide an mcp for macros
                if (messageBox != null)
                {
                    messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                    messageBox.Show();
                }
            }
            else
            {
                ShowFailGetQuestMessage();
            }
        }

        protected override void QuestPopupMessage_OnClose()
        {
        }

        #endregion
    }
}