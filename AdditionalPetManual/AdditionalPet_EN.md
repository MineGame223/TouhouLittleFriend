--By LogSpiral

The pet is like the pet in Touhou Little friend, it need to inherit from the class `BasicTouhouPet`
You need the Buff and the Item correspond to the pet as well.
I'd like to introduce them first since the structure of them is simple.
# Buff

It is recommend that the Buff inherit fome the class `BasicPetBuff`, **Not Recommend** to inherit from `ModBuff` directly.
You need to override these Properties.

- `PetType`
Indicate the **Projectile Type** of the pet correspond to the buff

- `LightPet`
It is used to mark the pet is a light pet or not.
# Item

The same as the normal moded pet item.

# BasicTouhouPet

Here some introductions and examples to the commonly used members.
The full summary see the XML comment in the code.

# override methods:

## Defaults

- `PetSetStaticDefaults()`
Call in `SetStaticDefaults`. Used to set some static default value, like `Main.projFrames` or `Main.projPets`
You can override `SetStaticDefaults` but **Not Recommend**

Here is the showcase.
```csharp
public override void PetStaticDefaults()
{
    Main.projFrames[Type] = 18; // Indicate that the pet has 18 frames
    Main.projPet[Type] = true;  // Indicate that the proj is a pet
	// ProjectileID.Sets.LightPet[Type] = true; 
	// ↑ Indicate that the pet is a LightPet
	// By set LightPet, we can allow some paired-pet coexists, like Komeiji sisters
}
```

- `PetDefaults()`
Call in `SetDefaults`. Used to set some default value
You can override `SetDefaults` but **Not Recommend**

Here is the showcase.
```csharp
public override void PetDefaults()
{
    Projectile.width = Projectile.height = 30; // Set the width and height
    Projectile.tileCollide = true;             // let the pet has a tile collision
}
```

## Visuals

- `VisualEffectForPreview()`
Update the fields or properties that relate to the visuals

- `DrawPetSelf(ref Color color)`
Call in `PreDraw`.
You can override `PreDraw` but **Not Recommend**

Here is the showcase.
```csharp
private int clothFrame, clothFrameCounter;
private void UpdateClothFrame()
{
	// We update the clothFrame and clothFrameCounter in this method.
}
public override void VisualEffectForPreview()
{
	// We update some visual effect in this method.
	// Like the current frame-number of the cloth.
    UpdateClothFrame();
    // You can write more method to update visuals if needed.
}
public override bool DrawPetSelf(ref Color lightColor)
{
	// In this funciton we can draw with clothFrame.
}
```

## Chats
- `ChatKeyToRegister(string name ,int index)`
The localization key to register, the default as below
```csharp
$"Mods.{Mod.Name}.Chat_{name}.Chat{index}"
```
`name` is the pet's name by default, it shouldn't be empty or null.
`index` is the key of `ChatDictionary`
for example, the value of `ChatDictionary[1]` is the `LocalizedText` when the index here is 1.
You Need to Notice that, It **will not automatically** register in localizations file by default.
You can call `Language.GetOrRegister` in `PetSetStaticDefaults` to register if needed.
Or calculate the result in this method, then call `Language.GetOrRegister(result)`, and finally return the result.

- `RegisterChat(ref string name, ref Vector2 indexRange)`
This method is used to set the value of `name` and `index` of the previous method.
You need to give the range of index by a Vector2
For example, `indexRange = new(0,10)`
The `index` will be set from 0 to 10 and incrase 1 each time.
An exception will be thrown if you try to get the Text out of range.

Here are the showcases.
```csharp
/*
* Assume our mod is called MyPetMod
* Now the following keys will be registered in our localization files
* 
* Mods.MyPetMod.Chat_Yabusame.RegulaChat0
* Mods.MyPetMod.Chat_Yabusame.RegulaChat1
* Mods.MyPetMod.Chat_Yabusame.RegulaChat2
* 
* correspond to the following values
* 
* ChatDictionary[0]
* ChatDictionary[1]
* ChatDictionary[2]
*/
public override void RegisterChat(ref string name,ref Vector2 indexRange)
{
	name = "Yabusame";              // the "name" argument of key is "Yabusame"
	indexRange = new Vector2(0, 2); // the "index" argument's range is [0,2]
}
public override string ChatKeyToRegister(string name,int index)
{
	var result = $"Mods.{Mod.Name}.Chat_{name}.RegulaChat{index}";
	Language.GetOrRegister(result);
	return result;
}
```

- `SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)`
Set the behavior of regular chats
`timePerDialog` indicate the time interval between each chat 
`chance` the countdown of the chance to start a chat
`whenShouldStop` indicate when shouldn't occur a chat

Here is the showcase.
```csharp
public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
{
    timePerDialog = 721; // Try to start a chat each 721 ticks by default.
    chance = 6; // 1 / 6 chance to start a chat
    whenShouldStop = !Main.dayTime; // it will not chat when it is not dayTime
}
```

- `RegularDialogText()`
Set the pet's regular chat, and the weight of random to start that chat

Here is the showcase.
```csharp
public override WeightedRandom<LocalizedText> RegularDialogText()
{
    WeightedRandom<LocalizedText> chat = new();
    for (int n = 0; n < 3; n++)
        chat.Add(ChatDictionary[n],n + 1);
    // Add these three chats into random pool with the weight 1, 2 and 3
    // So the chance of each chat is 1/6, 2/6 and 3/6
    return chat;
}
```

- `RegisterChatRoom()`
It is used to register a chat room

Here is the showcase.
```csharp
// This showcase is from Koishi's code
public override List<List<ChatRoomInfo>> RegisterChatRoom()
{
    return new()
    {
        Chatting1(),
    };
}

// Used to get first chat group.
private List<ChatRoomInfo> Chatting1()
{
    TouhouPetID koishi = TouhouPetID.Koishi;
    TouhouPetID satori = TouhouPetID.Satori;
    // You can get the additional pet's Extended Unique ID in this way:
    // int koishi = ModTouhouPetLoader.UniqueID<Koishi>()
    // int satori = ModTouhouPetLoader.UniqueID<Satori>()
    List<ChatRoomInfo> list =
    [
        new ChatRoomInfo(koishi, ChatDictionary[5], -1), 
        new ChatRoomInfo(satori, GetChatText("Satori",4), 0),
    ];
    return list;
}

/*
* The first argument of ChatRoomInfo is who give this chat
* the second is the content
* the third is the index, starts from -1
* It needs to Notice that you NEED to put the first chat in regular chat to start
* Thus, if you need to add a chat room starts with the pet in touhou little friend
* You need to call PetDialog and PetChatRoom in TouhouPets.CrossModSupport
*/
```
## Misc

- `AI()`
In this method you need to decide the behavior of the pet via player's state
For example, the pet disappears and the chat room closes when the player died.
You need to handle the movement of the pet with player.
You can call `ChangeDir` to adjust direction of the pet,
or call `MoveToPoint` to handle the movement.

Here is the showcase.
```csharp
if (!CheckActive(Owner)) // Check whether the pet should exists or not.
{
    currentChatRoom?.CloseChatRoom(); // Close the chat room if not active
    return;
}
ControlMovement(); // Update the movement of the pet
UpdateState(); // Update the state of the pet
```


The other commonly used methods in `ModProjectile` are needn't to be given a introduction.

## Properties
- ChatSettingConfig
Handle the behavior of the text in chat, like color, time to print all the texts
See the XML comment in that struct for more details.

Here is the showcase.
```csharp
public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
{
    TextColor = Color.LightGray // Set the color of text to Light Gray
    TextBoardColor = Main.DiscoColor // Set the color of text's border to Rainbow
};
```

Notice: It is **commonly not required to** override the property `UniqueID`. The additional pet is encoded with `UniqueIDExtended`, You can override it and convert `UniqueIDExtended` into `TouhouPetID`.
Like this
```csharp
public override TouhouPetID UniqueID => (TouhouPetID)UniqueIDExtended;
```

# Methods

## Movements

- `MoveToPoint`
Let the pet move to the position smoothly
point argument give the offset of center argument
speed argument indicates the speed of the process, the bigger the faster.
center argument indicates the center of the position, it is Player's mounterd center by default

It has an variation, `MoveToPoint2`
The usage is like `MoveToPoint` , but the center is player's center
the process will be quite different with `MoveToPoint`

Here is the showcase.
```csharp
MoveToPoint(
	Main.GlobalTimeWrappedHourly.ToRotationVector2() * 64, 
	13f,
	Main.MouseWorld);
// Turning around with the center of mouse's coord in world.
```

- ChangeDir
Sync the pet's direction with the player, it require the distance is less than 100px by default.

Here is the showcase.
```csharp
ChangeDir(1000); // Sync the Direction with the owner in the distance of 1000
```
## Misc
- `FindPet`
Find the pet belong to the owner, used to process the interaction between pets

Here is the showcase.
```csharp
FindPet(out Projectile master, ProjectileType<Remilia>(), a, b, false);
// Find Remilia in the state between a and b
// false indicates that needn't check whether the pet can chat or not.
// master is the instance we find
// if the target not found, the master will be null, the return value will be false
```
It has two variations, `FindPetByUniqueID`and `FindPetByPetType`
THe difference is **The Second Argument**

`FindPet` the second argument indicates the projectile type of pet, Type is int
`FindPetByUniqueID` indicates the unique id of pet, Type is TouhouPetID
`FindPetByPetType`  indicates the extended unique id of pet, Type is int


# Fields and Properties

- `PetState`
Indicates the state number of current state. Usually use it with enums
Here is the showcase.
```csharp
private enum States
{
    Idle,
    Blink,
    Rolling,
    Swimming,
    Flying,
}
private States CurrentState
{
    get => (States)PetState;
    set => PetState = (int)value;
}
```

- Owner
The player holds the pet.

## Chats

- `ChatDictionary`
The Dictionary contains the content of chat.

- `IsChatRoomActive`
Indicates whether the chatRoom with the given key as the first chat is Active




# Full Show Case

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Utilities;
using TouhouPets;
using TouhouPets.Common.ModSupports.ModPetRegisterSystem;
using TouhouPets.Content.Buffs;
using TouhouPets.Content.Projectiles.Pets;

namespace MyPetMod.Pets.MyPet;

public class MyPetBuff : BasicPetBuff
{
    public override int PetType => ModContent.ProjectileType<MyPet>(); // Set to the correspond Pet
    public override bool LightPet => true; // Mark as Light Pet
}
public class MyPetItem : ModItem
{

    public override void SetDefaults()
    {
        Item.DefaultToVanitypet(ProjectileType<MyPet>(), BuffType<MyPetBuff>());    // Bind the item with Pet and Buff
        Item.DefaultToVanitypetExtra(26, 34);                                       // Set the width and height
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!player.HasBuff(Item.buffType))         // add the buff when shooting
            player.AddBuff(Item.buffType, 2);
        return false;                               // Buff's updates will shoot the pet, so we return false here
    }
}
public class MyPet : BasicTouhouPet
{
    const int FrameCount = 4;
    #region Initializes
    public override void PetStaticDefaults()
    {
        Main.projPet[Type] = true;                      // mark as pet
        Main.projFrames[Type] = FrameCount;             // max frames
        ProjectileID.Sets.TrailCacheLength[Type] = 4;   // record 4 frame
        ProjectileID.Sets.LightPet[Type] = true;        // Mark as Light Pet
        base.PetStaticDefaults();
    }

    public override void PetDefaults()
    {
        Projectile.width = Projectile.height = 32;  // set width and height
        Projectile.tileCollide = true;              // add tile collision
    }
    #endregion

    #region Visuals
    public override void VisualEffectForPreview()
    {
        // Change frame each 10 ticks
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 10) 
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            Projectile.frame %= FrameCount;
        }

        // record datas
        Vector2 offset = default;
        switch (CurrentState) 
        {
            case State.Rolling:
                Projectile.rotation += 0.2f;
                break;
            case State.Shaking:
                offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8);
                break;
        }
        for (int n = 3; n > 0; n--)
        {
            Projectile.oldPos[n] = Projectile.oldPos[n - 1];
            Projectile.oldRot[n] = Projectile.oldRot[n - 1];
        }
        Projectile.oldPos[0] = Projectile.Center + offset;
        Projectile.oldRot[0] = Projectile.rotation;
        
    }
    public override bool DrawPetSelf(ref Color lightColor)
    {
        // drawings, do what you like
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        for (int n = 0; n < 4; n++)
        {
            Main.EntitySpriteDraw(
                texture,
                Projectile.oldPos[n] - Main.screenPosition,
                texture.Frame(1, FrameCount, 0, (Projectile.frame + n) % FrameCount),
                Color.White * (1 - n * .25f),
                Projectile.oldRot[n],
                new Vector2(16),
                1,
                0,
                0);
        }
        return false;
    }
    #endregion

    #region Updates
    enum State
    {
        Idle,    
        Rolling, 
        Shaking  
    }
    // Proxy the PetState
    State CurrentState
    {
        get => (State)PetState;
        set => PetState = (int)value;
    }
    int Timer
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }
    // check and update the pet
    bool CheckActive()
    {
        Player player = Owner;
        Projectile.timeLeft = 2;

        bool noActiveBuff = !player.HasBuff(BuffType<MyPetBuff>());
        bool shouldInactiveNormally = noActiveBuff;

        if (shouldInactiveNormally || player.dead)
        {
            Projectile.velocity *= 0;
            Projectile.frame = 0;
            Projectile.Opacity -= 0.009f;
            if (Projectile.Opacity <= 0)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
            }
            return false;
        }
        return true;

    }
    // Handle movement
    // I let it turn around with the mouse
    private void ControlMovement()
    {
        Projectile.tileCollide = false;

        ChangeDir();

        if (!Owner.dead)
            MoveToPoint(
                (Main.GlobalTimeWrappedHourly * 4).ToRotationVector2() * 64,
                24,
                Main.MouseWorld);
    }
    // Upadte States
    private void UpdateState()
    {
        // Timer incrases
        Timer++;
        switch (CurrentState)
        {
            case State.Idle:
                {
                    if (Timer >= 600)
                    {
                        // Choose between Rolling and shaking
                        CurrentState = Main.rand.Next([State.Rolling, State.Shaking]);
                        Timer = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                }
            case State.Rolling: 
                {
                    if (Timer >= 450)
                    {
                        // weighted random in all states
                        var rand = new WeightedRandom<State>();
                        rand.Add(State.Idle, 2);
                        rand.Add(State.Rolling, 1);
                        rand.Add(State.Shaking, 4);
                        CurrentState = rand.Get();
                        Timer = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                }
            case State.Shaking:
                {
                    if (Timer >= 300)
                    {
                        // switch to rolling
                        CurrentState = State.Rolling;
                        Timer = 150;
                        Projectile.netUpdate = true;
                    }

                    break;
                }
        }
    }
    public override void AI()
    {
        if (!CheckActive())                     // check active and lock timeleft
        {
            currentChatRoom?.CloseChatRoom();   // Close the chatroom if need to disappear
            return;
        }
        ControlMovement();                      // Update Movements
        UpdateState();                          // Update States
    }
    #endregion

    #region Chats
    public override ChatSettingConfig ChatSettingConfig => base.ChatSettingConfig with
    {
        TextBoardColor = Main.DiscoColor,       // The real color is the value of Main.DiscoColor when the chat starts.
        TextColor = Color.Black                 // The inner is black
    };
    public override void RegisterChat(ref string name, ref Vector2 indexRange)
    {
        name = nameof(MyPet);
        indexRange = new(0, 3);
    }
    protected override string ChatKeyToRegister(string name, int index)
    {
        var result = this.GetLocalizationKey($"ChatText_{index}");  // Get key via projectile instance
        Language.GetOrRegister(result);                             // Register key autmomatically
        return result;
    }
    public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
    {
        timePerDialog = 180;            // Try to chat every 180 ticks
        chance = 2;                     // the chance is 1/2 each time
        whenShouldStop = Main.dayTime;  // not speak in dayTime
    }
    public override WeightedRandom<LocalizedText> RegularDialogText()
    {
        var result = new WeightedRandom<LocalizedText>();
        for (int n = 0; n <= 3; n++)
            result.Add(ChatDictionary[n], n * n + 1);    // Just for showcase
        return result;
    }

    public override List<List<ChatRoomInfo>> RegisterChatRoom()
    {
        var myPet = UniqueIDExtended;
        var satori = TouhouPetID.Satori;
        var koishi = ModTouhouPetLoader.UniqueID<Koishi>();
        return [
            [new(myPet,ChatDictionary[0],-1),new(satori, ModUtils.GetChatText("Satori", 4), 0),new(koishi, ModUtils.GetChatText("Koishi", 5), 1)],
            [new(myPet, ChatDictionary[1], -1),new(koishi, ModUtils.GetChatText("Koishi", 5), 0)]
            ];
        // Just for showcase
    }
    #endregion
}

```