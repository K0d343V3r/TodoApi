using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Helpers
{
    public static class PositionAdjustor
    {
        public static void AdjustForCreate(
            ISortable entity, IList<ISortable> entityPeers, params IList<ISortable>[] entityChildren)
        {
            // adjust position for entity being created and its children
            AdjustEntityPositions(entity, entityPeers.Count, entityChildren);

            // adjust positions for entity peers
            AdjustPeerPositions(entityPeers, entity.Position, true);
        }

        private static void AdjustPeerPositions(IList<ISortable> entityPeers, int position, bool add)
        {
            for (int i = add ? position : position + 1; i < entityPeers.Count; i++)
            {
                entityPeers[i].Position = add ? entityPeers[i].Position + 1 : entityPeers[i].Position - 1;
            }
        }

        private static void AdjustEntityPositions(ISortable entity, int count, IList<ISortable>[] entityChildren)
        {
            if (entity.Position < 0 || entity.Position > count)
            {
                // given index outside list scope, assume append
                entity.Position = count;
            }

            // adjust child positions to match collection position
            foreach (var child in entityChildren)
            {
                for (int i = 0; i < child.Count; i++)
                {
                    child[i].Position = i;
                }
            }
        }

        public static void AdjustForDelete(ISortable entity, IList<ISortable> entityPeers)
        {
            // adjust positions for entity peers
            AdjustPeerPositions(entityPeers, entity.Position, false);
        }

        public static void AdjustForUpdate(
            ISortable newEntity, IList<ISortable> entityPeers, ISortable oldEntity, params IList<ISortable>[] entityChildren)
        {
            // adjust position for entity being update and its children
            AdjustEntityPositions(newEntity, entityPeers.Count, entityChildren);

            // adjust positions for entity peers
            AdjustPeerPositions(entityPeers, oldEntity.Position, newEntity.Position);
        }

        private static void AdjustPeerPositions(IList<ISortable> entityPeers, int oldPosition, int newPosition)
        {
            if (newPosition != oldPosition)
            {
                int start, end, step;
                if (newPosition < oldPosition)
                {
                    start = newPosition;
                    end = oldPosition - 1;
                    step = 1;
                }
                else
                {
                    start = oldPosition + 1;
                    end = newPosition;
                    step = -1;
                }
                for (int i = start; i <= end; i++)
                {
                    entityPeers[i].Position += step;
                }
            }
        }
    }
}
