# Omni-CustomSquads
 Omni Custom Squads as a Standalone plugin!


## Omni Common Library
Omni-CommonLibrary is a small class library that features a couple of extensions & works as a main hub for generalized things I make for these plugins.

It contains one important class, *OverallRoleType*, which is critical but simple to understand.

The `RoleType` field can be one of three values: `BaseGameRole`, `CrRole` (EXILED Custom Role), or `UcrRole` (UncomplicatedCustomRoles).

The `RoleId` field can be any integer, which corresponds to the ID of the role in its respective system (BaseGame, EXILED Customrole, UCR). The `BaseGameRole` ID system can be found below.
```
| Id  | RoleTypeId     |
|-----|----------------|
| -1  | None           |
| 0   | Scp173         |
| 1   | ClassD         |
| 2   | Spectator      |
| 3   | Scp106         |
| 4   | NtfSpecialist  |
| 5   | Scp049         |
| 6   | Scientist      |
| 7   | Scp079         |
| 8   | ChaosConscript |
| 9   | Scp096         |
| 10  | Scp0492        |
| 11  | NtfSergeant    |
| 12  | NtfCaptain     |
| 13  | NtfPrivate     |
| 14  | Tutorial       |
| 15  | FacilityGuard  |
| 16  | Scp939         |
| 17  | CustomRole     |
| 18  | ChaosRifleman  |
| 19  | ChaosMarauder  |
| 20  | ChaosRepressor |
| 21  | Overwatch      |
| 22  | Filmmaker      |
| 23  | Scp3114        |
| 24  | Flamingo       |
| 25  | AlphaFlamingo  |
| 26  | ZombieFlamingo |
```
An example `OverallRoleType` corresponding to the Scientist class:
```
  role_id: 6
  role_type: BaseGameRole
```
Another example `OverallRoleType` corresponding to a UCR role with its ID set to 22:
```
  role_id: 22
  role_type: UcrRole
```
If there are any questions, do not be afraid to send me a DM!
