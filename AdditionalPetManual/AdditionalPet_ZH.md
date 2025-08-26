--由 错数螺线 制作

附属宠物和本体自带的宠物一样，需要继承自`BasicTouhouPet`类
除此之外还需要宠物对应的Buff和宠物对应的物品
鉴于Buff和物品结构上相对简单，这里优先进行介绍

# Buff

宠物对应的Buff建议继承`BasicPetBuff`类，直接继承`ModBuff`可以但是**不推荐**
继承之后需要重写如下两个属性

- `PetType`
用于指定这个buff对应的宠物的**弹幕ID**

- `LightPet`
用于标记这个buff是否属于发光宠物类型

# 物品

宠物对应的物品与普通宠物的写法完全一致

# BasicTouhouPet

以下是一些常用的成员的介绍与使用例
完整见源码内的XML注释

# 重写函数：

## 初始化类

- `PetSetStaticDefaults()`
`SetStaticDefaults`时执行，用于补充设置一些静态默认值(如`Main.projFrames` `Main.projPet`等)
可以重写`SetStaticDefaults`但是**不建议**

以下是使用例
```csharp
public override void PetStaticDefaults()
{
    Main.projFrames[Type] = 18; // 标记宠物有18帧
    Main.projPet[Type] = true;  // 将弹幕标记为宠物
    // ProjectileID.Sets.LightPet[Type] = true;
    // ↑将弹幕标记为发光宠物
    // 通过普通宠物和发光宠物我们就能让一些宠物成对存在，比如古明地姐妹
}
```

- `PetDefaults()`
在`SetDefaults`时执行，用于补充设置一些默认值，可以重写`SetDefaults`但是**不建议**
以下是使用例
```csharp
public override void PetDefaults()
{
    Projectile.width = Projectile.height = 30; // 设置宠物宽高
    Projectile.tileCollide = true; // 令宠物有物块碰撞判定
}
```

## 绘制类

- `VisualEffectForPreview()`
用于进行视觉效果相关字段的更新

- `DrawPetSelf(ref Color color)`
在`PreDraw`时执行，可重写`PreDraw`但是**不建议**

以下是它们的使用例
```csharp
private int clothFrame, clothFrameCounter;
private void UpdateClothFrame()
{
    // 在这个函数里面我们对clothFrame和clothFrameCounter进行更新
}
public override void VisualEffectForPreview()
{
    // 这个函数里面我们执行视觉效果的一些更新
    // 比如衣服当前帧的序数
    UpdateClothFrame();
	// 有需要可以写更多更新某方面视觉效果的更新函数
}
public override bool DrawPetSelf(ref Color lightColor)
{
	// 在这个函数里面我们用clothFrame进行绘制
}
```

## 对话类
- `ChatKeyToRegister(string name ,int index)`
对话文本的本地化键，默认为
```csharp
$"Mods.{Mod.Name}.Chat_{name}.Chat{index}"
```
此处`name`用作标识，一般是当前宠物的名字，不应为空字符串或null
`index`则是和`ChatDictionary`对应的索引
比如`ChatDictionary[1]`的值就是此处`index`为1时的`LocalizedText`
需要注意的是，默认**不会自动**注册本地化键
如有需要可以在`PetSetStaticDefaults`中调用`Language.GetOrRegister`进行注册
或者干脆在这个函数里先获得result，`Language.GetOrRegister(result)`之后再return

- `RegisterChat(ref string name, ref Vector2 indexRange)`
在这个函数给刚刚上面的`name`和`index`传值，这里通过一个二维向量给出索引的范围
比如`indexRange = new(0,10)`
那么上面的`index`就会从0取到10，每次间隔1，注意这里是闭区间，0和10都会取到
尝试访问范围外的对话文本会出错

它们的使用例如下：
```csharp
/*
* 假设我们的模组叫MyPetMod
* 此时我们的本地化文件里会自动注册以下三个键
* Mods.MyPetMod.Chat_Yabusame.RegulaChat0
* Mods.MyPetMod.Chat_Yabusame.RegulaChat1
* Mods.MyPetMod.Chat_Yabusame.RegulaChat2
* 分别对应
* ChatDictionary[0]
* ChatDictionary[1]
* ChatDictionary[2]
*/
public override void RegisterChat(ref string name,ref Vector2 indexRange)
{
	name = "Yabusame"; // 注册键的name参数为"Yabusame"
	indexRange = new Vector2(0, 2); // 注册键的index范围为[0,2]
}
public override string ChatKeyToRegister(string name,int index)
{
	var result = $"Mods.{Mod.Name}.Chat_{name}.RegulaChat{index}";
	Language.GetOrRegister(result);
	return result;
}
```

- `SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)`
用于设置常规聊天的行为
`timePerDialog`用于指定每次尝试对话的间隔
`chance`用于指定每次尝试时发生概率的倒数
`whenShouldStop`用于手动控制哪些场合不应该发生聊天

使用例如下
```csharp
public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
{
    timePerDialog = 721; // 默认对话频率下每721帧尝试一次
    chance = 6; // 每次尝试有1/6概率触发对话
    whenShouldStop = !Main.dayTime; // 不在白天时不进行对话
}
```

- `RegularDialogText()`
用于设置该宠物会自然进行的对话，以及每项对话发生可能的权重

使用例如下
```csharp
public override WeightedRandom<LocalizedText> RegularDialogText()
{
    WeightedRandom<LocalizedText> chat = new();
    for (int n = 0; n < 3; n++)
        chat.Add(ChatDictionary[n],n + 1);
    // 将三条对话分别以1 2 3的权重加入随机池
    // 即分别对应1/6 2/6 3/6的发生概率
    return chat;
}
```

- `RegisterChatRoom()`
用于注册聊天室

使用例如下
```csharp
// 该段示例来自恋恋的代码
public override List<List<ChatRoomInfo>> RegisterChatRoom()
{
    return new()
    {
        Chatting1(),
    };
}

// 用于获取第一组对话
private List<ChatRoomInfo> Chatting1()
{
    TouhouPetID koishi = TouhouPetID.Koishi;
    TouhouPetID satori = TouhouPetID.Satori;
    // 对于模组宠物可以使用如下代码获取唯一ID
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
* ChatRoomInfo第一个参数表示是谁进行这句对话
* 第二个参数是对话内容
* 第三个参数是当前对话回合次序，从-1开始递增
* 需要注意的是，每组的第一个对话内容要写在常规对话随机池中才会触发后续内容
* 因此，如果需要由模组内置宠物与附属宠物进行对话，且是内置宠物发起，需要使用TouhouPets.CrossModSupport的PetDialog和PetChatRoom
*/
```
## 其它

- `AI()`
在这个函数里你需要检测玩家的状态来决定宠物行为，比如角色死亡时宠物消失、关闭聊天室
以及宠物对玩家的跟随
跟随部分常用的函数有`ChangeDir`用于调整宠物朝向、`MoveToPoint`用于宠物平滑移动等

使用例如下
```csharp
if (!CheckActive(Owner)) // 用于判定宠物是否应当继续存在以及timeLeft的锁定等
{
    currentChatRoom?.CloseChatRoom(); // 如果应当消失就关闭聊天室
    return;
}
ControlMovement(); // 用于更新宠物的移动
UpdateState(); // 用于更新宠物自身状态
```


其它原本ModProjectile常用的函数就不加以介绍了

属性：
- ChatSettingConfig
用于管理对话时文本的行为，比如文本颜色、打印所有文本所需时长等，详见结构体内XML注释

使用例如下
```csharp
public override ChatSettingConfig ChatSettingConfig => new ChatSettingConfig() with
{
    TextColor = Color.LightGray // 将对话颜色设为亮灰色
    TextBoardColor = Main.DiscoColor // 将文本边框设为彩色
};
```

注：UniqueID**一般不需要**重写，附属宠物用`UniqueIDExtended`进行编号，如果不放心可以重写，将`UniqueIDExtended`强转成`TouhouPetID`类型
像这样
```csharp
public override TouhouPetID UniqueID => (TouhouPetID)UniqueIDExtended;
```

# 函数

## 移动类

- `MoveToPoint`
将宠物平滑移动到指定位置
point参数表示相对于center的位置
speed参数表示平滑过程的速度，值越大越快完成移动过程
center参数表示目标中心，默认为(坐骑上)玩家中心

有一个变种`MoveToPoint2`
用法和`MoveToPoint`基本相同，中心固定为玩家中心
具体运动过程会和`MoveToPoint`有一定差异

使用例如下
```csharp
MoveToPoint(
	Main.GlobalTimeWrappedHourly.ToRotationVector2() * 64, 
	13f,
	Main.MouseWorld);
// 以鼠标为中心进行旋转
```

- ChangeDir
将宠物朝向与玩家同步，默认需要在100像素(6.25格)范围内才进行朝向同步，可以另外指定

使用例如下
```csharp
ChangeDir(1000); // 在一千像素范围内和玩家朝向保持同步
```
## 其它
- `FindPet`
用于查找属于当前玩家的某个宠物，一般用于当前宠物与那个宠物间的互动
使用例如下
```csharp
FindPet(out Projectile master, ProjectileType<Remilia>(), a, b, false);
// 这里查找状态介于a和b之间的蕾米，可取端点
// false表示不需要检查是否能进行对话
// master即为查找到的弹幕实例
// 如果没找到目标，master为null，函数返回false
```
除此之外还有两个变种
`FindPetByUniqueID`和`FindPetByPetType`
区别在**第二个参数**

`FindPet`的第二个参数填写弹幕ID, 类型为int
`FindPetByUniqueID`的第二个参数填写宠物独特ID, 类型为TouhouPetID
`FindPetByPetType`的第二个参数填写宠物拓展独特ID, 类型为int


# 字段与属性

- `PetState`
用于表示当前宠物所处状态序数，一般与枚举搭配使用
使用例如下
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
宠物所属的玩家

## 对话类

- `ChatDictionary`
宠物注册的对话所在的字典

- `IsChatRoomActive`
某文本开头的聊天室是否处于开启状态




# 完整示例

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
    public override int PetType => ModContent.ProjectileType<MyPet>(); // 设为与之相应的宠物
    public override bool LightPet => true; // 标记为照明宠物
}
public class MyPetItem : ModItem
{

    public override void SetDefaults()
    {
        Item.DefaultToVanitypet(ProjectileType<MyPet>(), BuffType<MyPetBuff>());    // 将物品与宠物和Buff绑定
        Item.DefaultToVanitypetExtra(26, 34);                                       // 设置物品宽高
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!player.HasBuff(Item.buffType))         // 发射宠物的同时添加buff
            player.AddBuff(Item.buffType, 2);
        return false;                               // Buff的更新函数负责生成宠物，所以这里是false
    }
}
public class MyPet : BasicTouhouPet
{
    const int FrameCount = 4;
    #region 初始化
    public override void PetStaticDefaults()
    {
        Main.projPet[Type] = true;                      // 标记为宠物
        Main.projFrames[Type] = FrameCount;             // 设置最大帧数
        ProjectileID.Sets.TrailCacheLength[Type] = 4;   // 记录四帧
        ProjectileID.Sets.LightPet[Type] = true;        // 标记为照明宠物
        base.PetStaticDefaults();
    }

    public override void PetDefaults()
    {
        Projectile.width = Projectile.height = 32;  // 设置宠物宽高
        Projectile.tileCollide = true;              // 添加物块判定
    }
    #endregion

    #region 视觉效果类
    public override void VisualEffectForPreview()
    {
        // 每10帧切换一次帧图
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 10) 
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            Projectile.frame %= FrameCount;
        }

        // 根据状态记录数据
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
        // 绘制，按你喜好来
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

    #region 宠物更新
    enum State
    {
        Idle,    // 闲置
        Rolling, // 滚动
        Shaking  // 抖动
    }
    // 给PetState套层壳
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
    // 检测并更新宠物是否应该继续存在
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
    // 控制宠物移动
    // 这里我让它绕着鼠标转圈
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
    // 更新状态
    private void UpdateState()
    {
        // 计时器自增
        Timer++;
        switch (CurrentState)
        {
            case State.Idle:
                {
                    if (Timer >= 600)
                    {
                        // 在滚动和抖动间二选一
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
                        // 在所有状态中进行加权随机
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
                        // 固定切换至滚动
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
        if (!CheckActive())                     // 用于判定宠物是否应当继续存在以及timeLeft的锁定等
        {
            currentChatRoom?.CloseChatRoom();   // 如果应当消失就关闭聊天室
            return;
        }
        ControlMovement();                      // 用于更新宠物的移动
        UpdateState();                          // 用于更新宠物自身状态
    }
    #endregion

    #region 对话设置
    public override ChatSettingConfig ChatSettingConfig => base.ChatSettingConfig with
    {
        TextBoardColor = Main.DiscoColor,       // 实际对话颜色是按开始对话时获取的颜色
        TextColor = Color.Black                 // 文本内部全黑
    };
    public override void RegisterChat(ref string name, ref Vector2 indexRange)
    {
        name = nameof(MyPet);
        indexRange = new(0, 3);
    }
    protected override string ChatKeyToRegister(string name, int index)
    {
        var result = this.GetLocalizationKey($"ChatText_{index}");  // 通过弹幕实例获取完整本地化键
        Language.GetOrRegister(result);                             // 自动注册本地化键，不需要可以省略
        return result;
    }
    public override void SetRegularDialog(ref int timePerDialog, ref int chance, ref bool whenShouldStop)
    {
        timePerDialog = 180;            // 每180帧尝试发起对话
        chance = 2;                     // 每次有1/2概率说话
        whenShouldStop = Main.dayTime;  // 白天不说话
    }
    public override WeightedRandom<LocalizedText> RegularDialogText()
    {
        var result = new WeightedRandom<LocalizedText>();
        for (int n = 0; n <= 3; n++)
            result.Add(ChatDictionary[n], n * n + 1);    // 权重写着玩的
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
        // 仅作示例
    }
    #endregion
}

```