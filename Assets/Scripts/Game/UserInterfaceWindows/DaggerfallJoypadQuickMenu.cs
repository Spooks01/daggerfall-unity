using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Banking;
using System.Linq;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    class DaggerfallJoypadQuickMenu : DaggerfallPopupWindow
    {
        Color buttonBackGroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        KeyCode toggleClosedBinding;
        protected Rect mapRect = new Rect(10, 100, 40, 40);
        protected Rect trvRect = new Rect(60, 100, 40, 40);
        protected Rect invRect = new Rect(110, 100, 40, 40);
        protected Rect charRect = new Rect(160, 100, 40, 40);
        protected Rect questRect = new Rect(210, 100, 40, 40);
        protected Rect restRect = new Rect(260, 100, 40, 40);
        protected Button mapButton;
        protected Button trvButton;
        protected Button invButton;
        protected Button charButton;
        protected Button questButton;
        protected Button restButton;
        public DaggerfallJoypadQuickMenu(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
         : base(uiManager, previous)
        {
            //StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
           
        }

        //
        protected override void Setup()
        {
            ParentPanel.BackgroundColor = ScreenDimColor;
            //setup buttons 
            mapButton = DaggerfallUI.AddButton(mapRect, NativePanel);
            mapButton.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            mapButton.Outline.Enabled = true;
            mapButton.Label.Text = "Local Map";
            mapButton.OnMouseClick += MapButton_OnMouseClick;

            trvButton = DaggerfallUI.AddButton(trvRect, NativePanel);
            trvButton.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            trvButton.Outline.Enabled = true;
            trvButton.Label.Text = "Travel Map";
            trvButton.OnMouseClick += TrvButton_OnMouseClick;

            invButton = DaggerfallUI.AddButton(invRect, NativePanel);
            invButton.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            invButton.Outline.Enabled = true;
            invButton.Label.Text = "Inventory";
            invButton.OnMouseClick += InvButton_OnMouseClick;

            charButton = DaggerfallUI.AddButton(charRect, NativePanel);
            charButton.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            charButton.Outline.Enabled = true;
            charButton.Label.Text = "Character";
            charButton.OnMouseClick += CharButton_OnMouseClick;

            questButton = DaggerfallUI.AddButton(questRect, NativePanel);
            questButton.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            questButton.Outline.Enabled = true;
            questButton.Label.Text = "Quest Log";
            questButton.OnMouseClick += QuestButton_OnMouseClick;

            restButton = DaggerfallUI.AddButton(restRect, NativePanel);
            restButton.BackgroundColor = DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor;
            restButton.Outline.Enabled = true;
            restButton.Label.Text = "Rest";
            restButton.OnMouseClick += RestButton_OnMouseClick;

            //mapButton. =
            //
            //
        }


        public override void Update()
        {
            base.Update();

            if (DaggerfallUI.Instance.HotkeySequenceProcessed == HotkeySequence.HotkeySequenceProcessStatus.NotFound)
            {
                // Toggle window closed with same hotkey used to open it
                if (InputManager.Instance.GetKeyUp(toggleClosedBinding))
                    CloseWindow();
            }
        }

        public override void OnPush()
        {
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.QuickMenu);
        }

       protected void MapButton_OnMouseClick(BaseScreenComponent sender, Vector2 position) {
           CloseWindow();
           DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenAutomap);
       }

        protected void TrvButton_OnMouseClick(BaseScreenComponent sender, Vector2 position) {
            CloseWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenTravelMapWindow);
        }

        protected void InvButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
        }

        protected void CharButton_OnMouseClick(BaseScreenComponent sender, Vector2 position) {
            CloseWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
        }
        protected void QuestButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenQuestJournalWindow);
        }

        protected void RestButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenRestWindow);
        }

    }
}
