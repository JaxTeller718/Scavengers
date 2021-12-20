﻿using GamePath;
using Platform;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UAI
{
    public static class SCoreUtils
    {
        public static void SimulateActionInstantExecution(Context _context, int _actionIdx, ItemStack _itemStack)
        {
            if (!Equals(_itemStack, ItemStack.Empty))
            {
                _context.Self.MinEventContext.ItemValue = _itemStack.itemValue;

                // starting action fire events.
                _context.Self.MinEventContext.ItemValue.FireEvent(_actionIdx == 0 ? MinEventTypes.onSelfPrimaryActionStart : MinEventTypes.onSelfSecondaryActionStart, _context.Self.MinEventContext);
                _context.Self.FireEvent(_actionIdx == 0 ? MinEventTypes.onSelfPrimaryActionStart : MinEventTypes.onSelfSecondaryActionStart, false);

                _itemStack.itemValue.ItemClass.Actions[_actionIdx].ExecuteInstantAction(_context.Self, _itemStack, false, null);

                // Ending action fire events
                _context.Self.MinEventContext.ItemValue.FireEvent(_actionIdx == 0 ? MinEventTypes.onSelfPrimaryActionEnd : MinEventTypes.onSelfSecondaryActionEnd, _context.Self.MinEventContext);
                _context.Self.FireEvent(_actionIdx == 0 ? MinEventTypes.onSelfPrimaryActionEnd : MinEventTypes.onSelfSecondaryActionEnd, false);
            }

            _context.ActionData.CurrentTask.Stop(_context);
        }

        public static void MoveBack(Context _context, Vector3 position)
        {
            _context.Self.moveHelper.SetMoveTo(position, true);
        }

        public static void HideWeapon(Context _context)
        {
            if (_context.Self.inventory.holdingItemIdx != _context.Self.inventory.DUMMY_SLOT_IDX)
            {
                _context.Self.inventory.SetHoldingItemIdx(_context.Self.inventory.DUMMY_SLOT_IDX);
                _context.Self.inventory.OnUpdate();
            }
        }

        public static void SetWeapon(Context _context)
        {
            if (_context.Self.inventory.holdingItemIdx != 0)
            {
                _context.Self.inventory.SetHoldingItemIdx(0);
                _context.Self.inventory.OnUpdate();
            }
        }

        public static bool IsBlocked(Context _context)
        {
            // Still calculating the path, let's let it finish.
            if (PathFinderThread.Instance.IsCalculatingPath(_context.Self.entityId))
                return false;
            var result = _context.Self.bodyDamage.CurrentStun == EnumEntityStunType.None && _context.Self.moveHelper.BlockedTime <= 0.3f && !_context.Self.navigator.noPathAndNotPlanningOne();
            if (result)
                return false;
            return !CheckForClosedDoor(_context);
        }

        public static void TeleportToPosition(Context _context, Vector3 position)
        {
            _context.Self.SetPosition(position);
        }
        public static void TeleportToLeader(Context _context)
        {
            var leader = EntityUtilities.GetLeaderOrOwner(_context.Self.entityId) as EntityAlive;
            if (leader == null) return;
            GameManager.Instance.World.GetRandomSpawnPositionMinMaxToPosition(leader.position, 1, 2, 3, false, out var position);
            if (position == Vector3.zero)
                position = leader.position + Vector3.back;
            _context.Self.SetPosition(position);
        }

        public static bool HasBuff(Context _context, string buff)
        {
            return !string.IsNullOrEmpty(buff) && _context.Self.Buffs.HasBuff(buff);
        }

        public static Vector3 HasHomePosition(Context _context)
        {
            if (!_context.Self.hasHome())
                return Vector3.zero;

            var homePosition = _context.Self.getHomePosition();
            var position = RandomPositionGenerator.CalcTowards(_context.Self, 5, 100, 10, homePosition.position.ToVector3());
            return position == Vector3.zero ? Vector3.zero : position;
        }

        public static bool IsAnyoneNearby(Context _context, float distance = 20f)
        {
            var nearbyEntities = new List<Entity>();

            // Search in the bounds are to try to find the most appealing entity to follow.
            var bb = new Bounds(_context.Self.position, new Vector3(distance, distance, distance));

            _context.Self.world.GetEntitiesInBounds(typeof(EntityAlive), bb, nearbyEntities);
            for (var i = nearbyEntities.Count - 1; i >= 0; i--)
            {
                var x = nearbyEntities[i] as EntityAlive;
                if (x == null) continue;
                if (x == _context.Self) continue;
                if (x.IsDead()) continue;

                // Otherwise they are an enemy.
                return true;
            }

            return false;
        }

        public static bool CanSee(EntityAlive sourceEntity, EntityAlive targetEntity)
        {
            var headPosition = sourceEntity.getHeadPosition();
            var headPosition2 = targetEntity.getHeadPosition();
            var direction = headPosition2 - headPosition;
            var seeDistance = sourceEntity.GetSeeDistance();
            if (direction.magnitude > seeDistance)
                return false;
            if (!sourceEntity.IsInViewCone(headPosition2))
                return false;

            var ray = new Ray(headPosition, direction);
            ray.origin += direction.normalized * 0.2f;
            var flag = Voxel.Raycast(sourceEntity.world, ray, seeDistance, true, true);
            if (flag)
            {
                var hitRootTransform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
                if (hitRootTransform == null)
                    return false;

                var component = hitRootTransform.GetComponent<EntityAlive>();
                if (component != null && component.IsAlive() && targetEntity == component)
                {
                    // Don't wake up the sleeping zombies if the leader is crouching.
                    var leader = EntityUtilities.GetLeaderOrOwner(sourceEntity.entityId) as EntityAlive;
                    if (leader != null && leader.IsCrouching && component.IsSleeping)
                        return false;
                    return true;
                }
            }
            return false;
        }
        public static bool IsEnemyNearby(Context _context, float distance = 20f)
        {
            var nearbyEntities = new List<Entity>();

            // Search in the bounds are to try to find the most appealing entity to follow.
            var bb = new Bounds(_context.Self.position, new Vector3(distance, distance, distance));

            _context.Self.world.GetEntitiesInBounds(typeof(EntityAlive), bb, nearbyEntities);
            for (var i = nearbyEntities.Count - 1; i >= 0; i--)
            {
                var x = nearbyEntities[i] as EntityAlive;
                if (x == null) continue;
                if (x == _context.Self) continue;
                if (x.IsDead()) continue;

                // If they are friendly
                if (EntityUtilities.CheckFaction(_context.Self.entityId, x)) continue;

                // Can we see them?
                if (!SCoreUtils.CanSee(_context.Self, x))
                    continue;
                

                // Otherwise they are an enemy.
                return true;
            }

            return false;
        }

        public static void SetCrouching(Context _context, bool crouch = false)
        {
            _context.Self.Crouching = crouch;
        }

        public static List<Vector3> ScanForTileEntities(Context _context, string _targetTypes = "")
        {
            var paths = new List<Vector3>();
            var blockPosition = _context.Self.GetBlockPosition();
            var chunkX = World.toChunkXZ(blockPosition.x);
            var chunkZ = World.toChunkXZ(blockPosition.z);

            if (string.IsNullOrEmpty(_targetTypes) || _targetTypes.ToLower().Contains("basic"))
                _targetTypes = "LandClaim, Loot, VendingMachine, Forge, Campfire, Workstation, PowerSource";
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    var chunk = (Chunk)_context.Self.world.GetChunkSync(chunkX + j, chunkZ + i);
                    if (chunk == null) continue;

                    var tileEntities = chunk.GetTileEntities();
                    foreach (var tileEntity in tileEntities.list)
                    {
                        foreach (var filterType in _targetTypes.Split(','))
                        {
                            // Parse the filter type and verify if the tile entity is in the filter.
                            var targetType = EnumUtils.Parse<TileEntityType>(filterType, true);
                            if (tileEntity.GetTileEntityType() != targetType) continue;

                            switch (tileEntity.GetTileEntityType())
                            {
                                case TileEntityType.None:
                                    continue;
                                // If the loot containers were already touched, don't path to them.
                                case TileEntityType.Loot:
                                    if (((TileEntityLootContainer)tileEntity).bTouched)
                                        continue;
                                    break;
                                case TileEntityType.SecureLoot:
                                    if (((TileEntitySecureLootContainer)tileEntity).bTouched)
                                        continue;
                                    break;
                            }

                            var position = tileEntity.ToWorldPos().ToVector3();
                            paths.Add(position);
                        }
                    }
                }
            }


            // sort the paths to keep the closes one.
            paths.Sort(new SCoreUtils.NearestPathSorter(_context.Self));
            return paths;
        }

        public static float SetSpeed(Context _context, bool panic = false)
        {
            var speed = _context.Self.GetMoveSpeed();
            if (panic)
                speed = _context.Self.GetMoveSpeedPanic();

            _context.Self.navigator.setMoveSpeed(speed);
            return speed;
        }
        public static void FindPath(Context _context, Vector3 _position, bool panic = false)
        {
            // If we have a leader, and following, match the player speed.
            var leader = EntityUtilities.GetLeaderOrOwner(_context.Self.entityId) as EntityAlive;
            if (leader != null && _context.ActionData.CurrentTask is UAITaskFollowSDX)
            {
                var tag = FastTags.Parse("running");
                panic = leader.CurrentMovementTag.Test_AllSet(tag);
            }

            var speed = SetSpeed(_context, panic);

            _position = SCoreUtils.GetMoveToLocation(_context, _position);
            
            // If there's not a lot of distance to go, don't re-path.
            var distance = Vector3.Distance(_context.Self.position, _position);
            if (distance < 0.4f)
                return;

            _context.Self.SetLookPosition(_position);
            _context.Self.RotateTo(_position.x, _position.y, _position.z, 45f, 45);

            // Path finding has to be set for Breaking Blocks so it can path through doors
            var path = PathFinderThread.Instance.GetPath(_context.Self.entityId);
            if (path.path == null && !PathFinderThread.Instance.IsCalculatingPath(_context.Self.entityId))
                _context.Self.FindPath(_position, speed, true, null);
        }

        // allows the NPC to climb ladders
        public static Vector3 GetMoveToLocation(Context _context, Vector3 position, float maxDist = 10f)
        {
            var vector = _context.Self.world.FindSupportingBlockPos(position);
            var temp = _context.Self.world.GetBlock(new Vector3i(vector));
            
            if (!(maxDist > 0f)) return vector;

            var vector2 = new Vector3(_context.Self.position.x, vector.y, _context.Self.position.z);
            var vector3 = vector - vector2;
           
            var magnitude = vector3.magnitude;

            if (!(magnitude < 3f)) return vector;
            if (magnitude <= maxDist)
            {
                return vector.y - _context.Self.position.y > 1.5f ? vector : vector2;
            }
            else
            {
                vector3 *= maxDist / magnitude;
                var vector4 = vector - vector3;
                vector4.y += 0.51f;
                var pos = World.worldToBlockPos(vector4);
                var block = _context.Self.world.GetBlock(pos);
                var block2 = block.Block;

                if (block2.IsPathSolid) return vector;
                if (Physics.Raycast(vector4 - Origin.position, Vector3.down, out var raycastHit, 1.02f, 1082195968))
                {
                    vector4.y = raycastHit.point.y + Origin.position.y;
                    return vector4;
                }

                if (block2.IsElevator((int)block.rotation))
                {
                    vector4.y = vector.y;
                    return vector4;
                }
            }

            return vector;
        }

        public static bool CheckForClosedDoor(Context _context)
        {
            // If you are not blocked, don't bother processing.
            //  if (!(_context.Self.moveHelper.BlockedTime >= 0.1f)) return false;
            if (!_context.Self.moveHelper.IsBlocked) return false;


            var blockPos = _context.Self.moveHelper.HitInfo.hit.blockPos;
            var block = GameManager.Instance.World.GetBlock(blockPos);

            if (!Block.list[block.type].HasTag(BlockTags.Door) || BlockDoor.IsDoorOpen(block.meta)) return false;

            var canOpenDoor = true;
            if (GameManager.Instance.World.GetTileEntity(0, blockPos) is TileEntitySecureDoor tileEntitySecureDoor)
            {
                if (tileEntitySecureDoor.IsLocked())
                {
                    canOpenDoor = false;
                    if (tileEntitySecureDoor.GetOwner() == PlatformManager.InternalLocalUserIdentifier)

                        canOpenDoor = false;
                }
                //if (tileEntitySecureDoor.IsLocked() && tileEntitySecureDoor.GetOwner() == "") // its locked, and you are not the owner.
                //                    canOpenDoor = false;
            }

            if (!canOpenDoor) return false;


            SphereCache.AddDoor(_context.Self.entityId, blockPos);
            EntityUtilities.OpenDoor(_context.Self.entityId, blockPos);
            Task task = Task.Delay(2000)
                .ContinueWith(t => CloseDoor(_context, blockPos));

            //  We were blocked, so let's clear it.
            _context.Self.moveHelper.ClearBlocked();
            return true;
        }

        public static void CloseDoor(Context _context, Vector3i doorPos)
        {
            EntityUtilities.CloseDoor(_context.Self.entityId, doorPos);
            SphereCache.RemoveDoor(_context.Self.entityId, doorPos);
        }

        public static bool IsAlly(Context _context, EntityAlive targetEntity)
        {
            // Do I have a leader?
            var myLeader = EntityUtilities.GetLeaderOrOwner(_context.Self.entityId);
            if (!myLeader) return false;

            // Is the target my leader?
            if (targetEntity.entityId == myLeader.entityId)
                return true;

            // Does my target have the same leader as me?
            var targetLeader = EntityUtilities.GetLeaderOrOwner(targetEntity.entityId);
            if (!targetLeader)
                return false;

            return targetLeader.entityId == myLeader.entityId;
        }

        public class NearestPathSorter : IComparer<Vector3>
        {
            public NearestPathSorter(Entity _self)
            {
                this.self = _self;
            }

            public int Compare(Vector3 _obj1, Vector3 _obj2)
            {
                var distanceSq = this.self.GetDistanceSq(_obj1);
                var distanceSq2 = this.self.GetDistanceSq(_obj2);
                if (distanceSq < distanceSq2)
                    return -1;

                if (distanceSq > distanceSq2)
                    return 1;

                return 0;
            }

            private Entity self;
        }

        public static void GetItemFromContainer(Context _context, TileEntityLootContainer tileLootContainer)
        {
            var blockPos = tileLootContainer.ToWorldPos();
            if (string.IsNullOrEmpty(tileLootContainer.lootListName))
                return;
            if (tileLootContainer.bTouched)
                return;

            tileLootContainer.bTouched = true;
            tileLootContainer.bWasTouched = true;

            // Nothing to loot.
            if (tileLootContainer.items == null) return;

            _context.Self.SetLookPosition(blockPos.ToVector3());
            _context.Self.MinEventContext.TileEntity = tileLootContainer;
            _context.Self.FireEvent(MinEventTypes.onSelfOpenLootContainer);

            var lootContainer = LootContainer.GetLootContainer(tileLootContainer.lootListName);
            if (lootContainer == null)
                return;
            var gameStage = EffectManager.GetValue(PassiveEffects.LootGamestage, null, 10, _context.Self);
            var array = lootContainer.Spawn(_context.Self.rand, tileLootContainer.items.Length, gameStage, 0f, null, new FastTags());

            for (var i = 0; i < array.Count; i++)
                _context.Self.lootContainer.AddItem(array[i].Clone());

            _context.Self.FireEvent(MinEventTypes.onSelfLootContainer);
        }

        public static bool CheckContainer(Context _context, Vector3 _vector)
        {
            if (!_context.Self.onGround)
                return false;

            _context.Self.SetLookPosition(_vector);

            var lookRay = new Ray(_context.Self.position, _context.Self.GetLookVector());
            if (!Voxel.Raycast(_context.Self.world, lookRay, 3f, false, false))
                return false; // Not seeing the target.

            if (!Voxel.voxelRayHitInfo.bHitValid)
                return false; // Missed the target. Overlooking?

            // Still too far away.
            var sqrMagnitude2 = (_vector - _context.Self.position).sqrMagnitude;
            if (sqrMagnitude2 > 2f)
                return false;

            var tileEntity = _context.Self.world.GetTileEntity(Voxel.voxelRayHitInfo.hit.clrIdx, new Vector3i(_vector));
            switch (tileEntity)
            {
                // if the TileEntity is a loot container, then loot it.
                case TileEntityLootContainer tileEntityLootContainer:
                    GetItemFromContainer(_context, tileEntityLootContainer);
                    break;
            }

            // Stop the move helper, so the entity does not slide around.
            EntityUtilities.Stop(_context.Self.entityId);
            return true;
        }
    }
}