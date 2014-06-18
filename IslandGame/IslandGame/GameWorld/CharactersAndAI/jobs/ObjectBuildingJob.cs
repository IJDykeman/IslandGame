using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class ObjectBuildingJob : Job
    {
        protected ObjectBuildJobSite objectBuildSite;
        protected Character workerCharacter;


        public ObjectBuildingJob(Character nWorker, ObjectBuildJobSite nObjectBuildSite)
        {
            objectBuildSite = nObjectBuildSite;
            workerCharacter = nWorker;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            return new CharacterTask.ObjectBuildForFrame(objectBuildSite);       
        }

        public override bool isUseable()
        {
            return true;
        }

        public override bool isComplete()
        {
            return objectBuildSite.isComplete();
        }




    }
}
