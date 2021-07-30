// -----------------------------------------------------------------------
// <copyright file="DevTestCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.CustomClasses;
using UnityEngine;

namespace Mistaken.DevTools.Commands
{
    /// <inheritdoc/>
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class DevTestCommand : IBetterCommand
    {
        public override string Description => "DEV STUFF";

        public override string Command => "test";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            if (player.GroupName != "dev")
                return new string[] { "This command is used for testing, allowed only for users with \"dev\" group" };
            switch (args[0])
            {
                case "sound":
                    GameObject.FindObjectOfType<AmbientSoundPlayer>().RpcPlaySound(int.Parse(args[1]));
                    break;
                case "tfc":
                    player.ChangeAppearance(RealPlayers.Get(args[1]), (RoleType)sbyte.Parse(args[2]));
                    break;
                case "fc":
                    player.ChangeAppearance((RoleType)sbyte.Parse(args[1]));
                    break;
                case "nick":
                    player.TargetSetNickname(RealPlayers.Get(args[1]), args[2]);
                    break;
                case "badge":
                    player.TargetSetBadge(RealPlayers.Get(args[1]), args[2], args[3]);
                    break;
                case "give":
                    Inventory.SyncItemInfo info;
                    switch (args[1].ToLower())
                    {
                        case "taser":
                            info = new Inventory.SyncItemInfo
                            {
                                id = ItemType.GunUSP,
                                durability = 501000f + int.Parse(args[2]),
                            };
                            break;
                        case "impact":
                            info = new Inventory.SyncItemInfo
                            {
                                id = ItemType.GrenadeFrag,
                                durability = 001000f,
                            };
                            break;
                        case "armor":
                            info = new Inventory.SyncItemInfo
                            {
                                id = ItemType.Coin,
                                durability = 001000f + int.Parse(args[2]),
                            };
                            break;
                        case "snav-3000":
                            info = new Inventory.SyncItemInfo
                            {
                                id = ItemType.WeaponManagerTablet,
                                durability = 301000f,
                            };
                            break;
                        case "snav-ultimate":
                            info = new Inventory.SyncItemInfo
                            {
                                id = ItemType.WeaponManagerTablet,
                                durability = 401000f,
                            };
                            break;
                        case "scp-1499":
                            info = new Inventory.SyncItemInfo
                            {
                                id = ItemType.GrenadeFlash,
                                durability = 149000f,
                            };
                            break;
                        default:
                            return new string[] { "Avaiable items:", "Taser", "Impact", "Armor", "SNav-3000", "SNav-Ultimate", "SCP-1499" };
                    }

                    if (player.Items.Count > 7)
                        info.Spawn(player.Position);
                    else
                        player.AddItem(info);
                    break;
                case "spawn":
                    {
                        if (this.keycard != null)
                            this.keycard.Delete();
                        var basePos = player.CurrentRoom.Position;
                        var offset = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        offset = (player.CurrentRoom.transform.forward * -offset.x) + (player.CurrentRoom.transform.right * -offset.z) + (Vector3.up * offset.y);
                        basePos += offset;
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);
                        gameObject.transform.position = basePos;
                        gameObject.transform.localScale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
                        gameObject.transform.rotation = Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])));
                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        Mirror.NetworkServer.Spawn(gameObject);
                        this.keycard = gameObject.GetComponent<Pickup>();
                        this.keycard.SetupPickup(ItemType.KeycardFacilityManager, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
                        return new string[] { player.CurrentRoom.Type + string.Empty, basePos.x + string.Empty, basePos.y + string.Empty, basePos.z + string.Empty, player.CurrentRoom.Type.ToString() + string.Empty };
                    }

                case "spawn2":
                    {
                        if (this.door != null)
                            NetworkServer.Destroy(this.door.gameObject);
                        var pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        this.door = DoorUtils.SpawnDoor(DoorUtils.DoorType.HCZ_BREAKABLE, pos, new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])), new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9])), name: "tmp_door");
                        (this.door as BreakableDoor)._brokenPrefab = null;
                        if (this.keycard != null)
                            this.keycard.Delete();
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);
                        gameObject.transform.position = pos - new Vector3(1.65f, 0, 0);
                        gameObject.transform.localScale = new Vector3(float.Parse(args[7]) * 9, float.Parse(args[8]) * 410, float.Parse(args[9]) * 2);
                        gameObject.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])));
                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        Mirror.NetworkServer.Spawn(gameObject);
                        this.keycard = gameObject.GetComponent<Pickup>();
                        this.keycard.SetupPickup(ItemType.KeycardFacilityManager, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
                        return new string[] { this.door.transform.position.x + string.Empty, this.door.transform.position.y + string.Empty, this.door.transform.position.z + string.Empty };
                    }

                case "spawn3":
                    {
                        if (this.keycard != null)
                            this.keycard.Delete();
                        var absolute = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);
                        gameObject.transform.position = absolute;
                        gameObject.transform.localScale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
                        gameObject.transform.rotation = Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])));
                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        Mirror.NetworkServer.Spawn(gameObject);
                        this.keycard = gameObject.GetComponent<Pickup>();
                        this.keycard.SetupPickup(ItemType.KeycardFacilityManager, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
                        return new string[] { player.CurrentRoom.Type + string.Empty, absolute.x + string.Empty, absolute.y + string.Empty, absolute.z + string.Empty, player.CurrentRoom.Type.ToString() + string.Empty };
                    }

                case "spawn4":
                    {
                        if (this.door != null)
                            NetworkServer.Destroy(this.door.gameObject);
                        var pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        this.door = DoorUtils.SpawnDoor(DoorUtils.DoorType.HCZ_BREAKABLE, pos, new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])), new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9])), name: "tmp_door");
                        (this.door as BreakableDoor)._brokenPrefab = null;
                        return new string[] { this.door.transform.position.x + string.Empty, this.door.transform.position.y + string.Empty, this.door.transform.position.z + string.Empty };
                    }

                case "spawn5":
                    {
                        if (this.keycard != null)
                            this.keycard.Delete();
                        var absolute = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);
                        gameObject.transform.position = absolute;
                        gameObject.transform.localScale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
                        gameObject.transform.rotation = Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])));
                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        Mirror.NetworkServer.Spawn(gameObject);
                        this.keycard = gameObject.GetComponent<Pickup>();
                        this.keycard.SetupPickup(ItemType.SCP018, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 0), gameObject.transform.position, gameObject.transform.rotation);
                        return new string[] { this.keycard.transform.position.x + string.Empty, this.keycard.transform.position.y + string.Empty, this.keycard.transform.position.z + string.Empty };
                    }

                case "spawn6":
                    {
                        if (this.door != null)
                            NetworkServer.Destroy(this.door.gameObject);
                        var pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        var rot = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                        var scale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
                        var basePos = player.CurrentRoom.Position;
                        pos = (player.CurrentRoom.transform.forward * -pos.x) + (player.CurrentRoom.transform.right * -pos.z) + (Vector3.up * pos.y);
                        basePos += pos;
                        this.door = UnityEngine.Object.Instantiate(DoorUtils.GetPrefab(DoorUtils.DoorType.LCZ_BREAKABLE), basePos, Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + rot));
                        GameObject.Destroy(this.door.GetComponent<DoorEventOpenerExtension>());
                        if (this.door.TryGetComponent<Scp079Interactable>(out var scp079Interactable))
                            GameObject.Destroy(scp079Interactable);
                        this.door.transform.localScale = scale;
                        if (this.door is BasicDoor basicdoor)
                            basicdoor._portalCode = 2;
                        this.door.NetworkActiveLocks |= (ushort)DoorLockReason.AdminCommand;
                        (this.door as BreakableDoor)._brokenPrefab = null;
                        this.door.gameObject.SetActive(false);
                        MethodInfo sendSpawnMessage = Server.SendSpawnMessage;
                        if (sendSpawnMessage != null)
                        {
                            if (player.ReferenceHub.networkIdentity.connectionToClient == null)
                                return new string[] { "Player left" };
                            sendSpawnMessage.Invoke(
                                null,
                                new object[] { this.door.netIdentity, player.Connection, });
                        }

                        return new string[] { this.door.transform.position.x + string.Empty, this.door.transform.position.y + string.Empty, this.door.transform.position.z + string.Empty };
                    }

                case "light":
                    {
                        if (this.keycard != null)
                            this.keycard.Delete();
                        var absolute = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Server.Host.Inventory.pickupPrefab);
                        gameObject.transform.position = absolute;
                        gameObject.transform.localScale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
                        gameObject.transform.rotation = Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6])));
                        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        Mirror.NetworkServer.Spawn(gameObject);
                        this.keycard = gameObject.GetComponent<Pickup>();
                        this.keycard.SetupPickup(ItemType.GunE11SR, 0, Server.Host.Inventory.gameObject, new Pickup.WeaponModifiers(true, 0, 0, 4), gameObject.transform.position, gameObject.transform.rotation);
                        return new string[] { player.CurrentRoom.Type + string.Empty, absolute.x + string.Empty, absolute.y + string.Empty, absolute.z + string.Empty, player.CurrentRoom.Type.ToString() + string.Empty };
                    }

                case "builder":
                    {
                        var pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        var rotation = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                        var size = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
                        DoorVariant doorVariant = UnityEngine.Object.Instantiate(DoorUtils.GetPrefab(DoorUtils.DoorType.HCZ_BREAKABLE), pos, Quaternion.Euler(rotation));
                        doorVariant.gameObject.transform.localScale = size;
                        GameObject.Destroy(doorVariant);
                        NetworkServer.Spawn(doorVariant.gameObject);
                        break;
                    }

                case "aaa":
                    string arg = args[1] == "null" ? null : args[1];
                    player.ReferenceHub.characterClassManager.NetworkCurSpawnableTeamType = (byte)(arg == null ? 0 : 1);
                    player.ReferenceHub.characterClassManager.NetworkCurUnitName = arg;
                    break;
                case "wall":
                    {
                        var pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        var rot = new Vector3(float.Parse(args[4]) - 180, float.Parse(args[5]) - 180, float.Parse(args[6]));
                        var width = float.Parse(args[7]);
                        var height = float.Parse(args[8]);
                        this.SpawnWorkStation(pos - (Vector3.right * 0.05f), rot, new Vector3(width, height, 0.1f));
                        this.SpawnWorkStation(pos + (Vector3.right * 0.05f), new Vector3(rot.x, rot.y + 180, rot.z), new Vector3(width, height, 0.1f));

                        this.SpawnWorkStation(pos - (Vector3.right * 0.05f), new Vector3(rot.x + 180, rot.y, rot.z), new Vector3(width, height, 0.1f));
                        this.SpawnWorkStation(pos + (Vector3.right * 0.05f), new Vector3(rot.x + 180, rot.y + 180, rot.z), new Vector3(width, height, 0.1f));
                        break;
                    }

                case "cc_gc":
                    CustomClass.CustomClasses.First(i => i.ClassSessionVarType == SessionVarType.CC_GUARD_COMMANDER).Spawn(player);
                    break;
                case "cc_zm":
                    CustomClass.CustomClasses.First(i => i.ClassSessionVarType == SessionVarType.CC_ZONE_MANAGER).Spawn(player);
                    break;
                case "cc_dfm":
                    CustomClass.CustomClasses.First(i => i.ClassSessionVarType == SessionVarType.CC_DEPUTY_FACILITY_MANAGER).Spawn(player);
                    break;
                case "cc":
                    CustomClass.CustomClasses.First(i => i.ClassName.ToLower() == string.Join(" ", args.Skip(1)).ToLower()).Spawn(player);
                    break;
                case "unit":
                    player.UnitName = args[1];
                    break;
                case "dequip":
                    Log.Debug(player.CurrentItemIndex == -1 ? ItemType.None : player.CurrentItem.id);
                    player.Inventory.Network_curItemSynced = ItemType.None;
                    player.Inventory.NetworkitemUniq = 0; // 0 not -1
                    break;
                case "break":
                    new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        player.Kill(new DamageTypes.DamageType("*Can not define what killed him"));
                        player.Broadcast(5, $"<color=red>You have been killed by admin</color>");
                    }).Start();
                    break;
                case "equip":
                    player.Inventory.Network_curItemSynced = player.Inventory.items[0].id;
                    player.Inventory.NetworkitemUniq = player.Inventory.items[0].uniq;
                    break;
                case "equip2":
                    var oldItem = player.CurrentItem;
                    var targetItem = player.Inventory.items[player.Inventory.items.Count - 1];
                    player.CurrentItem = targetItem;
                    var newItem = player.CurrentItem;
                    return new string[]
                    {
                        $"Old item: Id: {oldItem.id} | Uniq: {oldItem.uniq}",
                        $"Target item: Id: {targetItem.id} | Uniq: {targetItem.uniq}",
                        $"New item: Id: {newItem.id} | Uniq: {newItem.uniq}",
                    };
                case "look":
                    if (Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out var hit, 20, PlayerStats._singleton.weaponManager.raycastMask))
                        return new string[] { hit.collider.gameObject.name };
                    break;
            }

            success = true;
            return new string[] { "HMM" };
        }

        private Pickup keycard;
        private DoorVariant door;
        private WorkStation prefab;

        private void SpawnWorkStation(Vector3 pos, Vector3 rot, Vector3 size)
        {
            if (this.prefab == null)
            {
                foreach (var item in NetworkManager.singleton.spawnPrefabs)
                {
                    var ws = item.GetComponent<WorkStation>();
                    if (ws)
                    {
                        Log.Debug(item.name);
                        this.prefab = ws;
                    }
                }
            }

            var spawned = GameObject.Instantiate(this.prefab.gameObject, pos, Quaternion.Euler(rot));
            spawned.transform.localScale = size;
            Log.Debug("Spawning");
            NetworkServer.Spawn(spawned);
        }
    }
}
