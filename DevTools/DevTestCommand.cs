// -----------------------------------------------------------------------
// <copyright file="DevTestCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdminToys;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
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
            if (player.Group?.KickPower != 255 && !player.UserId.IsDevUserId())
                 return new string[] { $"This command is used for testing, allowed only for users with kickpower 255, you have {player.Group?.KickPower}" };
            switch (args[0].ToLower())
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
                        Vector3 pos;
                        Quaternion rot;
                        Vector3 scale;
                        Room room = null;
                        switch (args[1].ToLower())
                        {
                            case "relative":
                                room = player.CurrentRoom;
                                pos = room.Position;
                                var offset = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
                                offset = (room.transform.forward * -offset.x) + (room.transform.right * -offset.z) + (Vector3.up * offset.y);
                                pos += offset;
                                rot = Quaternion.Euler(room.transform.eulerAngles + new Vector3(float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7])));
                                scale = new Vector3(float.Parse(args[8]), float.Parse(args[9]), float.Parse(args[10]));
                                break;
                            case "absolute":
                                pos = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
                                rot = Quaternion.Euler(float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]));
                                scale = new Vector3(float.Parse(args[8]), float.Parse(args[9]), float.Parse(args[10]));
                                break;
                            default:
                                return new string[] { $"Unknown spawn type ({args[1].ToLower()}):", "- relative", "- absolute" };
                        }

                        switch (args[11].ToLower())
                        {
                            case "door":
                                {
                                    DoorUtils.DoorType type;
                                    switch (args[12].ToLower())
                                    {
                                        case "lcz":
                                            type = DoorUtils.DoorType.LCZ_BREAKABLE;
                                            break;
                                        case "hcz":
                                            type = DoorUtils.DoorType.HCZ_BREAKABLE;
                                            break;
                                        case "ez":
                                            type = DoorUtils.DoorType.EZ_BREAKABLE;
                                            break;
                                        default:
                                            return new string[] { $"Unknown door type ({args[12].ToLower()}):", "- lcz", "- hcz", "- ez" };
                                    }

                                    if (this.door != null)
                                        NetworkServer.Destroy(this.door.gameObject);
                                    this.door = DoorUtils.SpawnDoor(type, pos, rot.eulerAngles, scale);
                                    if (!(args.Length > 13 && args[13].ToLower() == "breakable"))
                                        (this.door as BreakableDoor)._brokenPrefab = null;

                                    return new string[]
                                    {
                                        $"Spawned {type}",
                                        $"In   : {(room == null ? "Absolute" : room.Type.ToString())}",
                                        $"Pos  : {pos}",
                                        $"Rot  : {rot}",
                                        $"Scale: {scale}",
                                    };
                                }

                            case "toy":
                                {
                                    PrimitiveType type;
                                    Color color;
                                    switch (args[12].ToLower())
                                    {
                                        case "light":
                                            {
                                                if (!this.lightSources.ContainsKey(player))
                                                    this.lightSources[player] = GlobalHandler.GetLightSourceObject(player);

                                                if (!ColorUtility.TryParseHtmlString(args[13], out color))
                                                    color = Color.gray;

                                                this.lightSources[player].transform.position = pos;
                                                this.lightSources[player].transform.rotation = rot;
                                                this.lightSources[player].transform.localScale = scale;
                                                this.lightSources[player].NetworkLightColor = color;
                                                this.lightSources[player].NetworkLightIntensity = float.Parse(args[14]);
                                                this.lightSources[player].NetworkLightRange = float.Parse(args[15]);
                                                this.lightSources[player].NetworkLightShadows = bool.Parse(args[16]);

                                                return new string[]
                                                {
                                                    $"Spawned light source",
                                                    $"In       : {(room == null ? "Absolute" : room.Type.ToString())}",
                                                    $"Pos      : {pos}",
                                                    $"Rot      : {rot}",
                                                    $"Scale    : {scale}",
                                                    $"Color    : {color}",
                                                    $"Intensity: {this.lightSources[player].NetworkLightIntensity}",
                                                    $"Range    : {this.lightSources[player].NetworkLightRange}",
                                                    $"Shadows  : {this.lightSources[player].NetworkLightShadows}",
                                                };
                                            }

                                        case "cube":
                                            type = PrimitiveType.Cube;
                                            break;
                                        case "plane":
                                            type = PrimitiveType.Plane;
                                            break;
                                        case "quad":
                                            type = PrimitiveType.Quad;
                                            break;
                                        case "sphere":
                                            type = PrimitiveType.Sphere;
                                            break;
                                        case "capsule":
                                            type = PrimitiveType.Capsule;
                                            break;
                                        case "cylinder":
                                            type = PrimitiveType.Cylinder;
                                            break;
                                        default:
                                            return new string[] { $"Unknown toy type ({args[12].ToLower()}):", "- light", "- cube", "- plane", "- quad", "- sphere", "- capsule", "- cylinder" };
                                    }

                                    if (!this.primitiveObjects.ContainsKey(player))
                                        this.primitiveObjects[player] = GlobalHandler.GetPrimitiveObject(player);

                                    if (!ColorUtility.TryParseHtmlString(args[13], out color))
                                        color = Color.gray;

                                    this.primitiveObjects[player].NetworkPrimitiveType = type;
                                    this.primitiveObjects[player].transform.position = pos;
                                    this.primitiveObjects[player].transform.rotation = rot;
                                    this.primitiveObjects[player].transform.localScale = scale;
                                    this.primitiveObjects[player].NetworkMaterialColor = color;

                                    return new string[]
                                    {
                                        $"Spawned {type}",
                                        $"In   : {(room == null ? "Absolute" : room.Type.ToString())}",
                                        $"Pos  : {pos}",
                                        $"Rot  : {rot}",
                                        $"Scale: {scale}",
                                        $"Color: {color}",
                                    };
                                }

                            case "workstation":
                                {
                                    var workStation = GameObject.Instantiate(CustomNetworkManager.singleton.spawnPrefabs.First(x => x.name == "Work Station"));
                                    workStation.transform.position = pos;
                                    workStation.transform.rotation = rot;
                                    workStation.transform.localScale = scale;
                                    NetworkServer.Spawn(workStation);
                                    return new string[]
                                    {
                                        $"Spawned workstation",
                                        $"In   : {(room == null ? "Absolute" : room.Type.ToString())}",
                                        $"Pos  : {pos}",
                                        $"Rot  : {rot}",
                                        $"Scale: {scale}",
                                    };
                                }

                            default:
                                return new string[] { $"Unknown spawn item ({args[11].ToLower()}):", "- door", "- toy", "- workstation" };
                        }
                    }

                case "spawn2":
                    {
                        if (args[1].ToLower() == "remove")
                        {
                            if (this.primitiveObjectsList[player].Count != 0)
                            {
                                NetworkServer.Destroy(this.primitiveObjectsList[player].Last().gameObject);
                                this.primitiveObjectsList[player].Remove(this.primitiveObjectsList[player].Last());
                                foreach (var primObj in this.primitiveObjectsList[player])
                                    player.SendConsoleMessage($"{primObj.netId}", "gray");

                                return new string[] { "Object removed successfully!" };
                            }

                            return new string[] { "Failed to remove object!" };
                        }

                        var pos = player.CurrentRoom.Position;
                        var offset = new Vector3(float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]));
                        offset = (player.CurrentRoom.transform.forward * -offset.x) + (player.CurrentRoom.transform.right * -offset.z) + (Vector3.up * offset.y);
                        pos += offset;

                        var obj = GlobalHandler.GetPrimitiveObject(player);

                        if (!System.Enum.TryParse<PrimitiveType>(args[1], true, out var type))
                            type = PrimitiveType.Sphere;
                        if (!ColorUtility.TryParseHtmlString(args[2], out var color))
                            color = Color.gray;

                        obj.NetworkPrimitiveType = type;
                        obj.transform.position = pos;
                        obj.transform.rotation = Quaternion.Euler(player.CurrentRoom.transform.eulerAngles + new Vector3(float.Parse(args[6]), float.Parse(args[7]), float.Parse(args[8])));
                        obj.transform.localScale = new Vector3(float.Parse(args[9]), float.Parse(args[10]), float.Parse(args[11]));
                        obj.NetworkMaterialColor = color;
                        if (!this.primitiveObjectsList.ContainsKey(player))
                            this.primitiveObjectsList.Add(player, new List<PrimitiveObjectToy>());
                        this.primitiveObjectsList[player].Add(obj);

                        return new string[] { $"Spawned {type} at {pos} with color {color}. Use \".test spawn2 remove\" to remove last spawned object." };
                    }

                case "spawn3":
                    {
                        if (args[1].ToLower() == "remove")
                        {
                            if (this.absolutePrimitiveObjectsList[player].Count != 0)
                            {
                                NetworkServer.Destroy(this.absolutePrimitiveObjectsList[player].Last().gameObject);
                                this.absolutePrimitiveObjectsList[player].Remove(this.absolutePrimitiveObjectsList[player].Last());
                                foreach (var primObj in this.absolutePrimitiveObjectsList[player])
                                    player.SendConsoleMessage($"{primObj.netId}", "gray");

                                return new string[] { "Object removed successfully!" };
                            }

                            return new string[] { "Failed to remove object!" };
                        }

                        var obj = GlobalHandler.GetPrimitiveObject(player);

                        if (!System.Enum.TryParse<PrimitiveType>(args[1], true, out var type))
                            type = PrimitiveType.Sphere;
                        if (!ColorUtility.TryParseHtmlString(args[2], out var color))
                            color = Color.gray;

                        var pos = new Vector3(float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]));

                        obj.NetworkPrimitiveType = type;
                        obj.transform.position = pos;
                        obj.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(args[6]), float.Parse(args[7]), float.Parse(args[8])));
                        obj.transform.localScale = new Vector3(float.Parse(args[9]), float.Parse(args[10]), float.Parse(args[11]));
                        obj.NetworkMaterialColor = color;
                        if (!this.absolutePrimitiveObjectsList.ContainsKey(player))
                            this.absolutePrimitiveObjectsList.Add(player, new List<PrimitiveObjectToy>());
                        this.absolutePrimitiveObjectsList[player].Add(obj);

                        return new string[] { $"Spawned {type} at {pos} with color {color}. Use \".test spawn3 remove\" to remove last spawned object." };
                    }

                case "spawn5":
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

                case "attp":
                    {
                        if (args[1].ToLower() == "remove")
                        {
                            var rematt = RealPlayers.Get(args[2]);
                            if (rematt is null)
                                rematt = player;
                            if (this.playerAttachedObjects[rematt].Count != 0)
                            {
                                NetworkServer.Destroy(this.playerAttachedObjects[rematt].Last().gameObject);
                                this.playerAttachedObjects[rematt].Remove(this.playerAttachedObjects[rematt].Last());
                                foreach (var primObj in this.absolutePrimitiveObjectsList[rematt])
                                    player.SendConsoleMessage($"{primObj.netId}", "gray");

                                return new string[] { "Object removed successfully!" };
                            }

                            return new string[] { "Failed to remove object!" };
                        }

                        var obj = GlobalHandler.GetPrimitiveObject(player);
                        var player2 = RealPlayers.Get(args[1]);
                        if (player2 is null)
                            player2 = player;
                        if (!System.Enum.TryParse<PrimitiveType>(args[2], true, out var type))
                            type = PrimitiveType.Sphere;
                        if (!ColorUtility.TryParseHtmlString(args[3], out var color))
                            color = Color.gray;
                        var pos = player.Position;
                        var offset = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                        offset = (player.CurrentRoom.transform.forward * -offset.x) + (player.CurrentRoom.transform.right * -offset.z) + (Vector3.up * offset.y);
                        pos += offset;

                        obj.NetworkPrimitiveType = type;
                        obj.transform.position = pos;
                        obj.transform.rotation = Quaternion.Euler(new Vector3(float.Parse(args[7]), float.Parse(args[8]), float.Parse(args[9])));
                        obj.transform.localScale = new Vector3(float.Parse(args[10]), float.Parse(args[11]), float.Parse(args[12])) * -1f;
                        obj.NetworkMaterialColor = color;
                        obj.transform.parent = player.GameObject.transform;
                        if (!this.playerAttachedObjects.ContainsKey(player))
                            this.playerAttachedObjects.Add(player, new List<PrimitiveObjectToy>());
                        this.playerAttachedObjects[player].Add(obj);

                        return new string[] { $"Attached {type} to {player.Nickname} with offset {pos} and color {color}. Use \".test attp remove [player id]\" to remove last attached object." };
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

                case "unit":
                    player.UnitName = args[1];
                    break;
                case "spect":
                    return new string[] { Player.Get(player.ReferenceHub.spectatorManager.CurrentSpectatedPlayer).ToString() };
                case "list":
                    return NetworkManager.singleton.spawnPrefabs.Select(x =>
                    {
                        var tmp = GameObject.Instantiate(x, player.Position, Quaternion.identity);
                        NetworkServer.Spawn(tmp);
                        return x.name;
                    }).ToArray();
                case "hint":
                    player.ShowHint(string.Join(" ", args.Skip(1)), 20);
                    break;
                case "hint_test":
                    player.SetGUI("test", PseudoGUIPosition.MIDDLE, string.Join(" ", args.Skip(1)));
                    break;
                case "hint_ignore":
                    PseudoGUIHandler.Ignore(player);
                    break;
                case "hint_stop_ignore":
                    PseudoGUIHandler.StopIgnore(player);
                    break;

                case "perms":
                    return Exiled.Permissions.Extensions.Permissions.Groups.Select(x => string.Join("\n", x.Value.Permissions.Select(perm => $"{x.Key}: {perm}"))).ToArray();

                case "my_perms":
                    return Exiled.Permissions.Extensions.Permissions.Groups.Where(x => x.Key == player.GroupName).First().Value.CombinedPermissions.ToArray();
                case "my_perms2":
                    return Exiled.Permissions.Extensions.Permissions.Groups.Where(x => x.Key == player.GroupName).First().Value.Permissions.ToArray();
                case "my_perms3":
                    return Exiled.Permissions.Extensions.Permissions.Groups.Where(x => x.Key == player.GroupName).First().Value.Inheritance.ToArray();
                case "cr":
                    CustomRole.Get(int.Parse(args[1])).AddRole(RealPlayers.Get(args[2]));
                    break;

                case "networkprefabs":
                    return CustomNetworkManager.singleton.spawnPrefabs.Select(x => x.name).ToArray();
            }

            success = true;
            return new string[] { "HMM" };
        }

        private readonly Dictionary<Player, PrimitiveObjectToy> primitiveObjects = new Dictionary<Player, PrimitiveObjectToy>();
        private readonly Dictionary<Player, LightSourceToy> lightSources = new Dictionary<Player, LightSourceToy>();
        private readonly Dictionary<Player, List<PrimitiveObjectToy>> absolutePrimitiveObjectsList = new Dictionary<Player, List<PrimitiveObjectToy>>();
        private readonly Dictionary<Player, List<PrimitiveObjectToy>> primitiveObjectsList = new Dictionary<Player, List<PrimitiveObjectToy>>();
        private readonly Dictionary<Player, List<PrimitiveObjectToy>> playerAttachedObjects = new Dictionary<Player, List<PrimitiveObjectToy>>();
        private DoorVariant door;
    }
}
