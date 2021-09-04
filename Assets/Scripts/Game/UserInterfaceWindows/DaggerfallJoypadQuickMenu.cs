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
        KeyCode toggleClosedBinding;
        //protected Rect mapRect = new Rect(222, 178, 39, 22);
        public DaggerfallJoypadQuickMenu(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
         : base(uiManager, previous)
        {
            //StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
           
        }


        protected override void Setup()
        {
            ParentPanel.BackgroundColor = ScreenDimColor;
            //setup buttons when opened
            //Button mapButton = DaggerfallUI.AddButton(mapRect, NativePanel);
            //mapButton.OnMouseClick += MapButton_OnMouseClick;
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

       // protected void MapButton_OnMouseClick(BaseScreenComponent sender, Vector2 position) {
         //   CloseWindow();
          //  DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenAutomap);
      //  }
    }
}
