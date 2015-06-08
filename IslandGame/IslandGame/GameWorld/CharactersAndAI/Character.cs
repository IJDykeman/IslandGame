using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CubeAnimator;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld.CharactersAndAI;
using System.Diagnostics;

namespace IslandGame.GameWorld
{
    [Serializable] 
    public class Character : Actor
    {
        
        Job job;
        
        float walkSpeedWhilePathing = 1.0f / 10.0f;
        float walkForce = 1f / 20f;
        BodyType bodyType;
        JobType currentJobType = JobType.none;
        private bool isWalkingOverride = false;

        CharacterLoad load;

        float timeSinceLastSwing = float.MaxValue/3f;
        float timeBetweenSwings = 35;
        private string currentBodyPath = "";



        protected enum BodyType
        {
            Minotuar,
            Ghoul
        }

        public Character(AxisAlignedBoundingBox nAABB, Faction nFaction)
        {

            load = new CharacterLoad();
            job = new UnemployedJob();
            physics = new PhysicsHandler(nAABB);
            faction = nFaction;
            if (faction == Faction.friendly)
            {
                bodyType = BodyType.Minotuar;
                switchBodies(ContentDistributor.getEmptyString() + "minotuar.chr");
            }
            else
            {
                bodyType = BodyType.Ghoul;
                switchBodies(ContentDistributor.getEmptyString() + @"ghoul\ghoul.chr");
                walkSpeedWhilePathing *= .9f;
            }

        }

        void setupBodyPartGroupGivenCurrentJob()
        {
            switch (bodyType)
            {
                case BodyType.Ghoul:
                    switchBodies(ContentDistributor.getEmptyString() + @"ghoul\ghoul.chr");
                    break;
                case BodyType.Minotuar:
                    switch (currentJobType)
                    {

                        case JobType.combat:
                            switchBodies(ContentDistributor.getEmptyString() + "armedMinotuar.chr");
                            break;
                        case JobType.agriculture:
                            switchBodies(ContentDistributor.getEmptyString() + "farmMinotuar.chr");
                            break;
                        case JobType.mining:
                            switchBodies(ContentDistributor.getEmptyString() + "mineMinotuar.chr");
                            break;
                        case JobType.building:
                            switchBodies(ContentDistributor.getEmptyString() + "buildMinotuar.chr");
                            break;
                        case JobType.logging:
                            switchBodies(ContentDistributor.getEmptyString() + "axeMinotuar.chr");
                            break;
                        case JobType.CarryingSomething:
                            handleBodyPartChangeForCarryingItem();
                            break;

                            
                        default:
                            //switchBodies(ContentDistributor.getEmptyString() + "minotuar.chr");
                            break;


                    }
                    break;
            }

        }

        void switchBodies(String nPath)
        {
            if (!nPath.Equals( currentBodyPath))
            {
                currentBodyPath = nPath;
                setupAnimatedBodyPartGroup(nPath);
            }
        }

        private void handleBodyPartChangeForCarryingItem()
        {
            if (load.isCaryingItem())
            {
                switch (load.getLoad())
                {
                    case ResourceBlock.ResourceType.standardBlock:
                        switchBodies(ContentDistributor.getEmptyString() + "carryingStandardBlockMinotuar.chr");
                        break;
                    case ResourceBlock.ResourceType.Wheat:
                        switchBodies(ContentDistributor.getEmptyString() + "carryingWheatMinotuar.chr");
                        break;
                    case ResourceBlock.ResourceType.Wood:
                        switchBodies(ContentDistributor.getEmptyString() + "carryingLogMinotuar.chr");
                        break;

                }

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

            setupBodyPartGroupGivenCurrentJob();

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
                    //runPhysics(actions);
                    animations.Add(AnimationType.standing);
                    break;

                case CharacterTask.Type.StepToBlock:
                    updateStepTask((CharacterTask.StepToBlock)toDo, animations);
                    setVelocity(new Vector3());
                    break;

                case CharacterTask.Type.DestoryBlock:
                    animations.Add(AnimationType.standing);
                    CharacterTask.DestroyBlock destroyBlock = (CharacterTask.DestroyBlock)toDo;
                    actions.Add(new ActorStrikeBlockAction(this, destroyBlock.getBlockToDestroy(),JobType.mining));
                    
                    StartHammerAnimationIfPossible();
                    break;

                case CharacterTask.Type.BuildBlock:
                    animations.Add(AnimationType.standing);
                    CharacterTask.BuildBlock buildBlock = (CharacterTask.BuildBlock)toDo;
                    actions.Add(new ActorPlaceBlockAction(buildBlock.getBlockLocToBuild(), buildBlock.getBlockTypeToBuild()));
                    StartHammerAnimationIfPossible();
                    dropItem();
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
                    //setRotationWithGivenDeltaVec(((CharacterTask.CaptainBoat)toDo).getBoat().getFootLocation() - getFootLocation());
                    setFootLocation(((CharacterTask.CaptainBoat)toDo).getBoat().getLocation()+ new Vector3(0,.6f,0));
                    setVelocity(new Vector3());
                    break;
                case CharacterTask.Type.GetInBoat:
                    getInBoat(((CharacterTask.GetInBoat)toDo).getBoat());
                    break;
                case CharacterTask.Type.SwitchJob:
                    setJobAndCheckUseability(((CharacterTask.SwitchJob)toDo).getNewJob());
                    break;

                case CharacterTask.Type.WalkTowardPoint:
                    animations.Add(AnimationType.walking);

                    if (World.getPathingProfile().isActorStanding(this))
                    {
                        Vector3 move = getWalkTowardDeltaVec(((CharacterTask.WalkTowardPoint)toDo).getTargetLoc());
                        move.Normalize();
                        move *= getWalkSpeedWhilePathing();
                        actions.Add(getAddVelocityAction(move, false));
                    }

                    
                    setRotationWithGivenDeltaVec(((CharacterTask.WalkTowardPoint)toDo).getTargetLoc() - getLocation());

                    break;
                case CharacterTask.Type.LookTowardPoint:

                    animations.Add(AnimationType.standing);
                    setRotationWithGivenDeltaVec(((CharacterTask.LookTowardPoint)toDo).getTargetLoc() - getLocation());
                    break;
                case CharacterTask.Type.DoStrikeOfWorkAlongRay:

                    animations.Add(AnimationType.standing);
                    setRotationWithGivenDeltaVec(((CharacterTask.DoStrikeOfWorkAlongRay)toDo).getStrikeDirectionNormal());
                    if (canSwing())
                    {
                        CharacterTask.DoStrikeOfWorkAlongRay strike = (CharacterTask.DoStrikeOfWorkAlongRay)toDo;
                        StartStrikeAnimation();
                        swing();
                        actions.Add(new ActorStrikeAlongRayAction(this, strike.getStrikeOrigen(), strike.getStrikeDistance(), strike.getStrikeDirectionNormal(), JobType.combat,0));
                    }
                    break;
                case CharacterTask.Type.MakeFarmBlockGrow:
                    actions.Add(new ActorStrikeBlockAction(this,((CharacterTask.MakeFarmBlockGrow)toDo).getBlockToFarm(),JobType.agriculture));
                    StartHammerAnimationIfPossible();
                    break;
                case CharacterTask.Type.HarvestFarmBlock:
                    CharacterTask.HarvestFarmBlock harvestTask = (CharacterTask.HarvestFarmBlock)toDo;
                    actions.Add(new ActorStrikeBlockAction(this, ((CharacterTask.HarvestFarmBlock)toDo).getBlockToFarm(),
                        JobType.agriculture));
                    setJobAndCheckUseability(new CarryResourceToStockpileKickoffJob(ResourceBlock.ResourceType.Wheat,this,
                        new FarmingKickoffJob(harvestTask.getFarm(),this,harvestTask.getWorkingProfile()),harvestTask.getWorkingProfile()));
                    StartHammerAnimationIfPossible();
                    pickUpItem(ResourceBlock.ResourceType.Wheat);
                    break;
                case CharacterTask.Type.PlaceResource:
                    CharacterTask.PlaceResource placeResource = (CharacterTask.PlaceResource)toDo;
                    actions.Add(new ActorPlaceResourceAction(placeResource.getLocToPlaceResource(),placeResource.getTypeToPlace()));
                    StartHammerAnimationIfPossible();
                    dropLoad();
                    break;
                case CharacterTask.Type.PickUpResource:
                    CharacterTask.PickUpResource pickUpResource = (CharacterTask.PickUpResource)toDo;
                    actions.Add(new ActorPickUpResourceAction(pickUpResource.getLocToPlaceResource(), pickUpResource.getTypeToPlace()));
                    load.pickUpItem(pickUpResource.getTypeToPlace());
                    StartHammerAnimationIfPossible();
                    pickUpItem(pickUpResource.getTypeToPlace());
                    break;
                default:
                    throw new Exception("unhandled task");


            }
            if (job.isComplete())
            {
                job = new UnemployedJob();
            }
            return animations;
        }

        private void dropItem()
        {
            load.dropItem();
            setupBodyPartGroupGivenCurrentJob();
        }

        public void StartHammerAnimationIfPossible()
        {
            if (canSwing())
            {
                swing();
                StartHammerAnimation();
            }
        }

         public void startHammerAnimation()
        {
            StartHammerAnimation();
        }

        private Vector3 getWalkTowardDeltaVec(Vector3 target)
        {
            Vector3 delta = target - getFootLocation();
            delta.Y = 0;
            delta.Normalize();
            delta *= getWalkSpeedWhilePathing();
            return delta;
        }

        private void updateStepTask(CharacterTask.StepToBlock stepTask, List<AnimationType> result)
        {
            Vector3 newFootLoc = LinearLocationInterpolator.interpolate(getFootLocation(), stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f), walkSpeedWhilePathing);
            setFootLocation(newFootLoc);
            setRotationWithGivenDeltaVec(stepTask.getGoalLoc().toWorldSpaceVector3()+ new Vector3(.5f,.5f,.5f) - getFootLocation());

            if (LinearLocationInterpolator.isLinearInterpolationComplete(getFootLocation(), stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f), walkSpeedWhilePathing))
            {
                stepTask.taskWasCompleted();
            }
            result.Add(AnimationType.walking);
        }

        public float getWalkSpeedWhilePathing()
        {
            return walkSpeedWhilePathing;
        }

        public float getWalkForceUnderDirectControl()
        {
            return walkForce;
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

        public void setJobType(JobType newJobType)
        {
            if ( newJobType != currentJobType && newJobType != JobType.none)
            {
                currentJobType = newJobType;
                setupBodyPartGroupGivenCurrentJob();
            }
        }

        public override List<BlockLoc> blockClaimedToWorkOn()
        {
            List<BlockLoc> result = job.getGoalBlock();
            return result;
        }

        public ActorAction getRightClickAction(Vector3 nearPoint, Vector3 farPoint)
        {
            return new ActorRightClickAction(nearPoint, farPoint, this);
        }

        public ActorAction getLeftClickAction(Vector3 nearPoint, Vector3 farPoint, byte currentlySelectedBlockType)
        {
            if (isCarryingItem())
            {
                return new ActorPlaceResourceAlongRay(this, nearPoint, getStrikeRange(), farPoint - nearPoint, getLoad(), currentlySelectedBlockType);
            }
            return new ActorStrikeAlongRayAction(this, nearPoint, getStrikeRange(), farPoint - nearPoint, getJobType(), currentlySelectedBlockType);
        }

        public override ActorAction getAddVelocityAction(Vector3 toAddToVelocity, bool isFootPropelled)
        {
            if (job is CaptainingBoatJob)
            {
                return new ActorAddToShipVelocity(((CaptainingBoatJob)job).getBoat(), toAddToVelocity);
            }
            else
            {
                return getActorMoveToActionFromDeltaVec(ref toAddToVelocity, isFootPropelled);
            }
        }

        public void rightClickedBoat(Boat boat)
        {
            getInBoat(boat);
        }

        private void getInBoat(Boat boat)
        {
            setJobAndCheckUseability(new CaptainingBoatJob(boat));
        }

        public void pathAlongOceanWithOceanPath(Path path)
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

        private JobType getJobType()
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

        public void dropLoad()
        {
            load.dropItem();
        }

        public void pickUpItem(ResourceBlock.ResourceType nItem)
        {
            load.pickUpItem(nItem);
            setupBodyPartGroupGivenCurrentJob();
        }

        public bool isCarryingItem()
        {
            return load.isCaryingItem();
        }

        public ResourceBlock.ResourceType getLoad()
        {
            return load.getLoad();
        }

        public override bool canBeKnockedBack()
        {
            return !(job is TravelAlongPath);
        }

        public override void updatePhysics(float coefficientOfFricton)
        {
            physics.update(coefficientOfFricton, canBeKnockedBack());
        }

        public override void getHit(float damage, Vector3 hitForceDirection, Actor striker)
        {
            recieveDamage(damage);
            hitForceDirection = applyKnockBack(hitForceDirection);
            setJobAndCheckUseability(new AttackActorJob(striker, World.getPathingProfile(), World.getActorProfile(), this));

        }
        
    }
}
