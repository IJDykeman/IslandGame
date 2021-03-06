﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class CharacterLoad
    {
        private ResourceBlock.ResourceType? carriedResource;

        public CharacterLoad()
        {
            carriedResource = null;
        }

        public bool isCaryingItem()
        {
            return carriedResource.HasValue;
        }

        public ResourceBlock.ResourceType getLoad()
        {
            if (isCaryingItem())
            {
                return (ResourceBlock.ResourceType)carriedResource;
            }
            else
            {
                throw new Exception("no resource being carried");
            }
        }

        public void dropItem()
        {
            carriedResource = null;
        }

        public void pickUpItem(ResourceBlock.ResourceType nCarriedResource)
        {
            dropItem();
            carriedResource = nCarriedResource;

        }

    }
}
