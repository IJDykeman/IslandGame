using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CubeAnimator;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld.CharactersAndAI;

namespace IslandGame.GameWorld
{
    [Serializable] 
    public class Character : Actor
    {
        
        Job job;
        readonly float jumpPower = .2f;
        float walkspeed = 1.0f / 10.0f;
        BodyType bodyType;
        JobType currentJobType = JobType.none;
        private bool isWalkingOverride = false;

        float timeSinceLastSwing = float.MaxValue/3f;
        float timeBetweenSwings = 35;



        protected enum BodyType
        {
            Minotuar,
            Ghoul
        }

        public Character(AxisAlignedBoundingBox nAABB, Faction nFaction)
        {
            
            //modelLocation = new Vector3(0, -.2f, 0);
            job = new UnemployedJob();
            physics = new PhysicsHandler(nAABB);
            faction = nFaction;
            if (faction == Faction.friendly)
            {
                bodyType = BodyType.Minotuar;              
            }
            else
            {
                bodyType = BodyType.Ghoul;
                walkspeed *= .9f;
            }

            setupBodyPartGroupGivenCurrentJob();
        }

        void setupBodyPartGroupGivenCurrentJob()
        {
            switch (bodyType)
            {
                case BodyType.Ghoul:
                    setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\ghoul\ghoul.chr");
                    break;
                case BodyType.Minotuar:
                    switch (currentJobType)
                    {

                        case JobType.combat:
                            setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\armedMinotuar.chr");
                            break;
                        case JobType.agriculture:
                            setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\farmMinotuar.chr");
                            break;
                        case JobType.mining:
                            setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\mineMinotuar.chr");
                            break;
                        case JobType.building:
                            setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\buildMinotuar.chr");
                            break;
                        case JobType.logging:
                            setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\axeMinotuar.chr");
                            break;
                        default:
                            setupAnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\minotuar.chr");
                            break;
                    }
                    
                    break;
                default:
                    break;
            }
        }


        public override List<ActorAction> update(CharacterTaskTracker taskTracker)
        {
            updateSwingResetTime();
            performOceanPhysics();

            List<ActorAction> actions = new List<ActorAction>();
            setRootPartLocation(physics.AABB.middle());

            List<AnimationType> animations = UpdateJobTasks(actions, taskTracker);
            if (isWalkingOverride && !(job is CaptainingBoatJob))
            {
                animations.Add(AnimationType.walking);
            }
            updateAnimations(animations);

            if (isDead())
            {
                actions.Add(new ActorDieAction(this));
            }


            return actions;
        }

        private void performOceanPhysics()
        {

            if (getFootLocation().Y <.1)
            {

                Vector3 newVelocity = getVelocity();
                Vector3 newLocation = getFootLocation();
                newLocation.Y = .1f;
                setFootLocation(newLocation);
                if (newVelocity.Y < 0)
                {
                    newVelocity.Y = 0;
                    setVelocity(newVelocity);
                }
            }
        }

        private List<AnimationType> UpdateJobTasks(List<ActorAction> actions, CharacterTaskTracker taskTracker)
        {
            CharacterTask.Task toDo = job.getCurrentTask(taskTracker);

            List<AnimationType> animations = new List<AnimationType>();

            switch (toDo.taskType)
            {
                case CharacterTask.Type.NoTask:
                    runPhysics(actions);
                    animations.Add(AnimationType.standing);
                    break;

                case CharacterTask.Type.StepToBlock:
                    updateStepTask((CharacterTask.StepToBlock)toDo, animations);
                    break;
                case CharacterTask.Type.DestoryBlock:
                    animations.Add(AnimationType.standing);
                    CharacterTask.DestroyBlock destroyBlock = (CharacterTask.DestroyBlock)toDo;
                    destroyBlock.updateTime();
                    if (destroyBlock.readyToDestroy())
                    {
                        destroyBlock.destructionWasOrdered();
                        actions.Add(new ActorStrikeBlockAction(this, destroyBlock.getBlockToDestroy(),JobType.mining));
                    }
                    StartHammerAnimationIfPossible();
                    break;
                case CharacterTask.Type.BuildBlock:
                    animations.Add(AnimationType.standing);
                    CharacterTask.BuildBlock buildBlock = (CharacterTask.BuildBlock)toDo;
                    actions.Add(new ActorStrikeBlockAction(this, buildBlock.getBlockLocToBuild(), JobType.building));
                    StartHammerAnimationIfPossible();
                    break;
                case CharacterTask.Type.ChopBlockForFrame:
                    animations.Add(AnimationType.standing);
                    CharacterTask.ChopBlockForFrame chopBlock = (CharacterTask.ChopBlockForFrame)toDo;
                    actions.Add(new ActorStrikeBlockAction(this, chopBlock.getBlockToChop(), JobType.logging));
                    StartHammerAnimationIfPossible();
                    break;
                case CharacterTask.Type.ObjectBuildForFrame:
                    CharacterTask.ObjectBuildForFrame objectBuildTask = (CharacterTask.ObjectBuildForFrame)toDo;
                    objectBuildTask.getSiteToWorkOn().buildForAFrame();
                    if (objectBuildTask.getSiteToWorkOn().isReadyToBeBuilt())
                    {
                        actions.Add(new ActorPlaceBoatAction(objectBuildTask.getSiteToWorkOn().getObjectLoc()));
                    }
                    StartHammerAnimationIfPossible();
                    break;
                case CharacterTask.Type.CaptainBoat:
                    setRotationWithGivenDeltaVec(((CharacterTask.CaptainBoat)toDo).getBoat().getFootLocation() - getFootLocation());
                    setFootLocation(((CharacterTask.CaptainBoat)toDo).getBoat().getLocation()+ new Vector3(0,.6f,0));
                    break;
                case CharacterTask.Type.GetInBoat:
                    getInBoat(((CharacterTask.GetInBoat)toDo).getBoat());
                    break;
                case CharacterTask.Type.SwitchJob:
                    runPhysics(actions);
                    animations.Add(AnimationType.walking);
                    setJobAndCheckUseability(((CharacterTask.SwitchJob)toDo).getNewJob());
                    break;
                case CharacterTask.Type.WalkTowardPoint:
                    
                    animations.Add(AnimationType.walking);

                    physics.gravitate();
                    Vector3 move = getWalkTowardDeltaVec(((CharacterTask.WalkTowardPoint)toDo).getTargetLoc()) + physics.velocity;
                    actions.Add(getMoveToActionWithMoveByVector(move));

                    
                    setRotationWithGivenDeltaVec(((CharacterTask.WalkTowardPoint)toDo).getTargetLoc() - getLocation());
                    //positionQueue = getHammerAnimation();
                    break;
                case CharacterTask.Type.LookTowardPoint:

                    animations.Add(AnimationType.standing);
                    setRotationWithGivenDeltaVec(((CharacterTask.LookTowardPoint)toDo).getTargetLoc() - getLocation());
                    //positionQueue = getHammerAnimation();
                    break;
                case CharacterTask.Type.DoStrikeOfWorkAlongRay:

                    animations.Add(AnimationType.standing);
                    setRotationWithGivenDeltaVec(((CharacterTask.DoStrikeOfWorkAlongRay)toDo).getStrikeDirectionNormal());
                    if (canSwing())
                    {
                        CharacterTask.DoStrikeOfWorkAlongRay strike = (CharacterTask.DoStrikeOfWorkAlongRay)toDo;
                        StartStrikeAnimation();
                        swing();
                        actions.Add(new ActorStrikeAlongRayAction(this, strike.getStrikeOrigen(), strike.getStrikeDistance(), strike.getStrikeDirectionNormal(), JobType.combat));
                    }
                    break;
                case CharacterTask.Type.MakeFarmBlockGrow:
                    actions.Add(new ActorStrikeBlockAction(this,((CharacterTask.MakeFarmBlockGrow)toDo).getBlockToFarm(),JobType.agriculture));
                    StartHammerAnimationIfPossible();
                    break;
                case CharacterTask.Type.PlaceResource:
                    CharacterTask.PlaceResource placeResource = (CharacterTask.PlaceResource)toDo;
                    actions.Add(new ActorPlaceResourceAction(placeResource.getLocToPlaceResource(),placeResource.getTypeToPlace()));
                    StartHammerAnimationIfPossible();
                    break;


            }
            if (job.isComplete())
            {
                job = new UnemployedJob();
            }
            return animations;
        }

        public void StartHammerAnimationIfPossible()
        {
            if (canSwing())
            {
                swing();
                StartHammerAnimation();
            }
        }

        private Vector3 getWalkTowardDeltaVec(Vector3 target)
        {
            Vector3 delta = target - getFootLocation();
            delta.Y = 0;
            delta.Normalize();
            delta *= getSpeed();
            return delta;
        }



        private void updateStepTask(CharacterTask.StepToBlock stepTask, List<AnimationType> result)
        {
            Vector3 newFootLoc = LinearLocationInterpolator.interpolate(getFootLocation(), stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f), walkspeed);
            setFootLocation(newFootLoc);
            setRotationWithGivenDeltaVec(stepTask.getGoalLoc().toWorldSpaceVector3()+ new Vector3(.5f,.5f,.5f) - getFootLocation());

            if (LinearLocationInterpolator.isLinearInterpolationComplete(getFootLocation(), stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f), walkspeed))
            {
                stepTask.taskWasCompleted();
            }
            result.Add(AnimationType.walking);
        }

        public void jump()
        {
            physics.velocity.Y += jumpPower;
        }

        public float getSpeed()
        {
            return walkspeed;
        }

        public void setJobAndCheckUseability(Job newJob)
        {
            job.jobWasCancelled();
            setJobType(newJob.getJobType());
            if (newJob.isUseable())
            {
                job = newJob;
            }
        }

        private void setJobType(JobType newJobType)
        {
            if (newJobType != JobType.none && newJobType != currentJobType)
            {
                currentJobType = newJobType;
                setupBodyPartGroupGivenCurrentJob();
            }
        }

        public override BlockLoc? blockClaimedToWorkOn()
        {
            if (job is MultiBlockOngoingJob)
            {
                return ((MultiBlockOngoingJob)job).getCurrentGoalBlock();
            }
            return null;
        }

        public ActorAction getRightClickAction(Vector3 nearPoint, Vector3 farPoint)
        {
            return new ActorRightClickAction(nearPoint, farPoint, this);
        }

        public ActorAction getLeftClickAction(Vector3 nearPoint, Vector3 farPoint)
        {
            return new ActorStrikeAlongRayAction(this, nearPoint, getStrikeRange(), farPoint - nearPoint, getJobType());
        }


        public override ActorAction getMoveToActionWithMoveByVector(Vector3 moveBy)
        {
            if (job is CaptainingBoatJob)
            {
                return new ActorSetShipVelocity(((CaptainingBoatJob)job).getBoat(), moveBy);
            }
            else
            {
                return getActorMoveToActionFromDeltaVec(ref moveBy);
            }
        }

        public void rightClickedBoat(Boat boat)
        {
            getInBoat(boat);
        }

        private void getInBoat(Boat boat)
        {
            job = new CaptainingBoatJob(boat);
        }

        public void pathAlongOceanWithOceanPath(List<BlockLoc> path)
        {
            if (job is CaptainingBoatJob)
            {
                ((CaptainingBoatJob)job).setTravelPath(path);
            }
        }

        public float getStrikeRange()
        {
            return 1.3f;
        }

        public void setIsWalkingOverride(bool nisWalking)
        {
            isWalkingOverride = nisWalking;
        }

        public void wasJustEmbodiedByPlayer()
        {
            if (job is CaptainingBoatJob)
            {
                ((CaptainingBoatJob)job).cancelPathing();
            }
        }

        public override Job getJobWhenClicked(Character nWorker, IslandPathingProfile pathingProfile, ActorStateProfile actorState)
        {
            if (areHostile(nWorker.getFaction(), getFaction()))
            {
                return new AttackActorJob(this, pathingProfile, actorState, nWorker);
            }
            else
            {
                return new UnemployedJob();
            }
        }

        public JobType getJobType()
        {
            return currentJobType;
        }

        public bool canSwing()
        {
            return timeSinceLastSwing > timeBetweenSwings;
        }

        void swing()
        {
            timeSinceLastSwing = 0;
        }

        void updateSwingResetTime()
        {
            timeSinceLastSwing++;
        }

        public void quitCurrentJob()
        {
            if (!(job is CaptainingBoatJob))
            {
                setJobAndCheckUseability(new UnemployedJob());
            }
            else
            {
                ((CaptainingBoatJob)job).cancelPathing();
            }
        }
    }
}
