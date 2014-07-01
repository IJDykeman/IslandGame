using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.menus
{
    public enum MenuActionType
    {
        NewGame,
        Quit,
        Save,
        Load,
        Continue,
        ExcavationHudButtonClick,
        FarmHudButtonClick,
        WoodBuildHudClick,
        WoodStorageHudClick,
        WheatStorageHudClick,
        ColorSelection,
        PlayerBuildHudClick,
        PlayerBuildBoatHudClick,
        JobTypeSwitch,
        NewCharacterHudClick,
        StoneStorageHudClick,
        
    }

    public class MenuAction
    { 
        public MenuActionType type;

    }



    public class NewGameMenuAction : MenuAction
    {
        public NewGameMenuAction()
        {
            type = MenuActionType.NewGame;
        }
    }

    public class QuitMenuAction : MenuAction
    {
        public QuitMenuAction()
        {
            type = MenuActionType.Quit;
        }
    }

    public class SaveMenuAction : MenuAction
    {
        public SaveMenuAction()
        {
            type = MenuActionType.Save;
        }
    }

    public class ContinueMenuAction : MenuAction
    {
        public ContinueMenuAction()
        {
            type = MenuActionType.Continue;
        }
    }
    
    public class LoadMenuAction : MenuAction
    {
        public LoadMenuAction()
        {
            type = MenuActionType.Load;
        }
    }

    public class ExcavationHudButtonClick : MenuAction
    {
        public ExcavationHudButtonClick()
        {
            type = MenuActionType.ExcavationHudButtonClick;
        }
    }

    public class FarmHudButtonClick : MenuAction
    {
        public FarmHudButtonClick()
        {
            type = MenuActionType.FarmHudButtonClick;
        }
    }

    public class WoodBuildHudClick : MenuAction
    {
        public WoodBuildHudClick()
        {
            type = MenuActionType.WoodBuildHudClick;
        }
    }

    public class PlayerBuildHudClick : MenuAction
    {
        public PlayerBuildHudClick()
        {
            type = MenuActionType.PlayerBuildHudClick;
        }
    }

    public class PlayerBuildBoatHudClick : MenuAction
    {
        public PlayerBuildBoatHudClick()
        {
            type = MenuActionType.PlayerBuildBoatHudClick;
        }
    }

    public class PlayerPlaceWoodStorageHudClick : MenuAction
    {
        public PlayerPlaceWoodStorageHudClick()
        {
            type = MenuActionType.WoodStorageHudClick;
        }
    }

    public class PlayerPlaceWheatStorageHudClick : MenuAction
    {
        public PlayerPlaceWheatStorageHudClick()
        {
            type = MenuActionType.WheatStorageHudClick;
        }
    }

    public class PlayerPlaceStoneStorageHudClick : MenuAction
    {
        public PlayerPlaceStoneStorageHudClick()
        {
            type = MenuActionType.StoneStorageHudClick;
        }
    }

    public class PlayerPlaceNewCharacterHudClick : MenuAction
    {
        public PlayerPlaceNewCharacterHudClick()
        {
            type = MenuActionType.NewCharacterHudClick;
        }
    }

    public class ColorSelection : MenuAction
    {
        public byte selectedColor;
        public ColorSelection(byte nSelectedColor)
        {
            selectedColor = nSelectedColor;
            type = MenuActionType.ColorSelection;
        }
    }

    public class JobTypeSwitch : MenuAction
    {
        IslandGame.GameWorld.JobType newType = GameWorld.JobType.none;
        public JobTypeSwitch(IslandGame.GameWorld.JobType nNewType)
        {
            newType = nNewType;
            type = MenuActionType.JobTypeSwitch;
        }

        public IslandGame.GameWorld.JobType getJobType()
        {
            return newType;
        }
    }
}
