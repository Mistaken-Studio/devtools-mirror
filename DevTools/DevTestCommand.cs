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
using Exiled.API.Features.Items;
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
            if (player.Group?.KickPower == 255)
                return new string[] { "This command is used for testing, allowed only for users with kickpower 255" };
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
                case "spawn":
                    {
                        if (this.keycard != null)
                            this.keycard.Destroy();
                        var basePos = player.CurrentRoom.Position;
                        var offset = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        offset = (player.CurrentRoom.transform.forward * -offset.x) + (player.CurrentRoom.transform.right * -offset.z) + (Vector3.up * offset.y);
                        basePos += offset;
                        this.keycard = new Item(ItemType.KeycardFacilityManager).Spawn(basePos, Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]))));
                        this.keycard.Base.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        this.keycard.Scale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
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
                            this.keycard.Destroy();
                        this.keycard = new Item(ItemType.KeycardFacilityManager).Spawn(pos - new Vector3(1.65f, 0, 0), Quaternion.Euler(new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]))));
                        this.keycard.Base.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        this.keycard.Scale = new Vector3(float.Parse(args[7]) * 9, float.Parse(args[8]) * 410, float.Parse(args[9]) * 2);
                        return new string[] { this.door.transform.position.x + string.Empty, this.door.transform.position.y + string.Empty, this.door.transform.position.z + string.Empty };
                    }

                case "spawn3":
                    {
                        if (this.keycard != null)
                            this.keycard.Destroy();
                        var absolute = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        this.keycard = new Item(ItemType.KeycardFacilityManager).Spawn(absolute, Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]))));
                        this.keycard.Base.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        this.keycard.Scale = new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9]));
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

                case "spawn7":
                    {
                        var pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        var scale = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                        Vector3 basePos;

                        if (player.Position.y > 900)
                            basePos = pos;
                        else
                        {
                            basePos = player.CurrentRoom.Position;
                            pos = player.CurrentRoom.transform.forward * -pos.x + player.CurrentRoom.transform.right * -pos.z + Vector3.up * pos.y;
                            basePos += pos;
                        }

                        var obj = new GameObject();
                        obj.transform.position = basePos;
                        obj.transform.localScale = scale;

                        List<(Vector3, Vector3, Vector3)> box = new List<(Vector3, Vector3, Vector3)>()
                        {
                            (new Vector3(basePos.x + (scale.x / 2), basePos.y, basePos.z), new Vector3(4.4f * scale.z, 123f * scale.y, 0.05f), new Vector3(0, -90, 0)),
                            (new Vector3(basePos.x - (scale.x / 2), basePos.y, basePos.z), new Vector3(4.4f * scale.z, 123f * scale.y, 0.05f), new Vector3(0, 90, 0)),
                            (new Vector3(basePos.x, basePos.y, basePos.z + (scale.z / 2)), new Vector3(4.4f * scale.x, 123f * scale.y, 0.05f), new Vector3(0, 180, 0)),
                            (new Vector3(basePos.x, basePos.y, basePos.z - (scale.z / 2)), new Vector3(4.4f * scale.x, 123f * scale.y, 0.05f), new Vector3(0, 0, 0)),
                            (new Vector3(basePos.x, basePos.y + (scale.y / 2), basePos.z), new Vector3(4.4f * scale.x, 123f * scale.z, 0.05f), new Vector3(-90, 0, 0)),
                            (new Vector3(basePos.x, basePos.y - (scale.y / 2), basePos.z), new Vector3(4.4f * scale.x, 123f * scale.z, 0.05f), new Vector3(90, 0, 0)),
                        };

                        foreach (var card in box)
                        {
                            this.keycard = new Item(ItemType.KeycardNTFCommander).Spawn(card.Item1, Quaternion.Euler(card.Item3));
                            this.keycard.Base.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                            this.keycard.Scale = card.Item2;
                        }

                        return new string[] { $"Spawned boxcollider at position {obj.transform.position}" };
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
                case "spect":
                    return new string[] { Player.Get(player.ReferenceHub.spectatorManager.CurrentSpectatedPlayer).ToString() };
            }

            success = true;
            return new string[] { "HMM" };
        }

        private Pickup keycard;
        private DoorVariant door;
    }
}
