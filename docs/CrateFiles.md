# Crate Files

Crate files are the files that contain the data for all the crates in a level. They use the following format:

| Name              | Data type      | Notes                                 |
|-------------------|----------------|---------------------------------------|
| Version           | `u32`          | Default version is 4.                 |
| Crate group count | `u16`          | Amount of crate groups in this level. |
| Crate groups      | `CrateGroup[]` | List of crate groups.                 |

### Crate Group

| Name           | Data type | Notes                                                                                                                                                                     |
|----------------|-----------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| World position | `f32 x3`  | Origin position in world space.                                                                                                                                           |
| Crate offset   | `u16`     | Index offset of the first crate; if the first 2 crate groups both contain 10 crates, this value will be 20 for the third group.                                           |
| Crate count    | `u16`     | Amount of crates in this group.                                                                                                                                           |
| Tilt           | `u16`     | Tilt of the crate group encoded as a 16-bit integer. This is a rotation around the Y axis. To convert the value to degrees, multiply it by 0.0054931640625 (360 / 65536). |
| Crates         | `Crate[]` | List of crates in this group.                                                                                                                                             |

### Crate

| Name                    | Data type   | Notes                                                                                                                                                            |
|-------------------------|-------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| World position          | `f32 x3`    | Origin position in world space (not affected by crate group position).                                                                                           |
| ?                       | `f32`       | Unused; this value is always 0. It might be the 4th component of the world position (translation part of a transformation matrix).                               |
| Local position          | `s16 x3`    | Local position of the crate, compared to the crate group origin position.                                                                                        |
| Crate type A            | `CrateType` | The default crate type.                                                                                                                                          |
| Crate type B            | `CrateType` | The crate type used for time trial.                                                                                                                              |
| Crate type C            | `CrateType` | For slot crates: The first option. For empty crates: The crate type that the empty crate will change into when the corresponding exclamation crate is triggered. |
| Crate type D            | `CrateType` | For slot crates: The second option.                                                                                                                              |
| ?                       | `u16`       | Some sort of crate index. These values are sometimes -1, but otherwise never exceed the amount of crates in a level.                                             |
| ?                       | `u16`       | Same as above.                                                                                                                                                   |
| ?                       | `u16`       | Same as above.                                                                                                                                                   |
| ?                       | `u16`       | Same as above.                                                                                                                                                   |
| ?                       | `u16`       | Same as above.                                                                                                                                                   |
| ?                       | `u16`       | Same as above.                                                                                                                                                   |
| Exclamation crate index | `u16`       | For empty crates: The index of the exclamation crate that will change the empty crate into crate type C.                                                         |

### Crate Types

Crate types are 1 byte in size. Note that type 18 does not appear in the game.

| Value | Value (hex) | Type             |
|-------|-------------|------------------|
| -1    | `0xFF`      | None             |
| 0     | `0x00`      | Empty            |
| 1     | `0x01`      | Default          |
| 2     | `0x02`      | Life             |
| 3     | `0x03`      | Aku Aku          |
| 4     | `0x04`      | Arrow            |
| 5     | `0x05`      | Question Mark    |
| 6     | `0x06`      | Bounce           |
| 7     | `0x07`      | Checkpoint       |
| 8     | `0x08`      | Slot             |
| 9     | `0x09`      | Tnt              |
| 10    | `0x0A`      | Time Trial One   |
| 11    | `0x0B`      | Time Trial Two   |
| 12    | `0x0C`      | Time Trial Three |
| 13    | `0x0D`      | Iron Arrow       |
| 14    | `0x0E`      | Exclamation      |
| 15    | `0x0F`      | Iron             |
| 16    | `0x10`      | Nitro            |
| 17    | `0x11`      | Nitro Switch     |
| 18    | `0x12`      | Proximity?       |
| 19    | `0x13`      | Locked           |
| 20    | `0x14`      | Invincibility    |
