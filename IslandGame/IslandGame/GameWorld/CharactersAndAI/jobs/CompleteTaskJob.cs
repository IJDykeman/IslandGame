using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    class CompleteTaskJob : Job
    {
        
        private CharacterTask.Task task;
        private Character workerCharacter;


        public CompleteTaskJob(Character nWorker, CharacterTask.Task nTask)
        {
            task = nTask;
            workerCharacter = nWorker;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            return task;  
        }

        public override bool isUseable()
        {
            return true;
        }

        public override bool isComplete()
        {
            return false;
        }
    }
}
