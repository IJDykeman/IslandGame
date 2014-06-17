using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld;

namespace IslandGame
{



    public static class PlayerAction
    {
        public enum Type
        {
            LeftClick,
            ReleaseLeftMouseButton,
            MoveTo,
            RightClick,
            DoubleClick,
            Save,
            NewGame,
            Quit,
            LoadGame,
            PlaceExcavationMarker,
            ExcavationMouseover,
            StartDragging,
            FinishDragging,
            Dragging,
            PlaceWoodBlockPlan,
            RemoveWoodBlockPlan,
            PlayerBuildBlock,
            PlayerDestoryBlock,
            BoatPlacementMouseover,
            BoatPlacement,
            DisembodyCharacter,
            DeselectCharacter,
            MoveBy,
            setCameraLocation,
            BlockPlanPlacementHover
        }

        public abstract class Action
        {
            public Type type;
        }

        public class MouseAction : Action
        {
            protected Vector3 nearPoint;
            protected Vector3 farPoint;

            public Ray getRay()
            {
                Ray result =  new Ray(nearPoint, farPoint - nearPoint);
                result.Direction.Normalize();
                return result;
            }
        }

        public class LeftClick : MouseAction
        {
            public LeftClick(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.LeftClick;
            }
        }

        public class ReleaseLeftMouseButton : MouseAction
        {
            public ReleaseLeftMouseButton(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;
                type = Type.ReleaseLeftMouseButton;
            }
        }

        public class PlaceExcavationMarker : MouseAction
        {
            public PlaceExcavationMarker(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.PlaceExcavationMarker;
            }
        }


        public class StartDragging : Dragging
        {
            public StartDragging(Vector3 nNearPoint, Vector3 nFarPoint, DragType nDragType)
            {
                dragType = nDragType;
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.StartDragging;
            }


        }
        public class FinishDragging : Dragging
        {
            public FinishDragging(Vector3 nNearPoint, Vector3 nFarPoint, DragType nDragType)
            {
                dragType = nDragType;
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.FinishDragging;
            }


        }
        public class Dragging : MouseAction
        {
            public enum DragType
            {
                farm,
                storage
            }

            protected DragType dragType;
            public Dragging(){}
            public Dragging(Vector3 nNearPoint, Vector3 nFarPoint, DragType nDragType)
            {
                dragType = nDragType;
                nearPoint = nNearPoint;
                farPoint = nFarPoint;
                
                type = Type.Dragging;
            }
            public Dragging.DragType getType()
            {
                return dragType;
            }
        }

        public class PlaceWoodBlockPlan : MouseAction
        {
            public PlaceWoodBlockPlan(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.PlaceWoodBlockPlan;
            }
        }

        public class RemoveWoodBlockPlan : MouseAction
        {

            public RemoveWoodBlockPlan(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.RemoveWoodBlockPlan;
            }
        }

        public class ExcavationMouseHover : MouseAction
        {
            public ExcavationMouseHover(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.ExcavationMouseover;
            }
        }

        public class BlockPlanPlacementHover : MouseAction
        {
            public BlockPlanPlacementHover(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.BlockPlanPlacementHover;
            }
        }

        public class BoatPlacementHover : MouseAction
        {
            public BoatPlacementHover(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.BoatPlacementMouseover;
            }
        }

        public class BoatPlacement : MouseAction
        {
            public BoatPlacement(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;
                type = Type.BoatPlacement;
            }
        }

        public class DoubleClick : MouseAction
        {
            public DoubleClick(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.DoubleClick;
            }
        }

        public class RightClick : MouseAction
        {
            public RightClick(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;

                type = Type.RightClick;
            }
        }

        public class PlayerBuildBlock : MouseAction
        {


            public PlayerBuildBlock(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;
                type = Type.PlayerBuildBlock;
            }
        }

        public class PlayerDestroyBlock : MouseAction
        {
            public PlayerDestroyBlock(Vector3 nNearPoint, Vector3 nFarPoint)
            {
                nearPoint = nNearPoint;
                farPoint = nFarPoint;
                type = Type.PlayerDestoryBlock;
            }
        }

        public class MoveTo : Action
        {
            public AxisAlignedBoundingBox currentAABB;
            public AxisAlignedBoundingBox desiredAABB;

            public MoveTo(AxisAlignedBoundingBox nCurrentAABB, AxisAlignedBoundingBox nDesiredAABB)
            {
                currentAABB = nCurrentAABB;
                desiredAABB = nDesiredAABB;
                type = Type.MoveTo;
            }
        }

        public class MoveBy : Action
        {
            public Vector3 moveBy;

            public MoveBy(Vector3 nMoveBy)
            {
                moveBy = nMoveBy;
                type = Type.MoveBy;
            }
        }

        public class SetCameraLocation : Action
        {
            public Vector3 newCameraLocation;
            float neckAdjustment = 0;

            public SetCameraLocation(Vector3 nNewCameraLocation)
            {
                newCameraLocation = nNewCameraLocation;
                type = Type.setCameraLocation;
            }

            public SetCameraLocation(Vector3 nNewCameraLocation, float nNeckAdjustment)
            {
                newCameraLocation = nNewCameraLocation;
                type = Type.setCameraLocation;
                neckAdjustment = nNeckAdjustment;
            }

            public float getNeckAdjustment()
            {
                return neckAdjustment;
            }
        }

        public class Save : Action
        {
            public Save()
            {
                type = Type.Save;
            }
        }

        public class NewGame : Action
        {
            public NewGame()
            {

                type = Type.NewGame;
            }
        }

        public class Quit : Action
        {
            public Quit()
            {
                type = Type.Quit;
            }
        }

        public class LoadGame : Action
        {
            public LoadGame()
            {
                type = Type.LoadGame;
            }
        }

        public class DisembodyCharacter : Action
        {
            public DisembodyCharacter()
            {
                type = Type.DisembodyCharacter;
            }
        }

        public class DeselectCharacter : Action
        {
            public DeselectCharacter()
            {
                type = Type.DeselectCharacter;
            }
        }
        




    }

}
