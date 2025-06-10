东方小伙伴 ~ Adventure with Cute Partner，一个专注于东方Project系列宠物的泰拉瑞亚模组

本模组包含两个可用的ModCall内容：

1、Call("MarisasReactionToBoss", int BossType, string ChatText)

魔理沙会在遭遇任意Boss生物时说出一句评价，此Call可以为您自己的模组添加指定Boss的相关对话

BossType 是被指定NPC的种类，该NPC必须属于一个Boss（.boss = true）；ChatText 是与之相关的对话文本

2、Call("ReimusReactionToOtherPet", int PetType, string ChatText)

灵梦偶尔会对同一玩家召唤的另一个宠物（一般是照明宠物）进行评价，此Call可以为您自己的模组添加指定宠物的相关对话

PetType 是被指定弹幕的种类，该弹幕必须属于一个宠物（Main.projPet[PetType] = true）；ChatText 是与之相关的对话文本
