# Galgame 編輯器
[English](README.md) | [繁體中文](README.zh.md)

一款基於 **Unity 6** 的視覺小說編輯器，讓你完全透過純文字腳本創作自己的 Galgame——無需任何程式設計經驗。

---

## 功能特色

- 腳本驅動的視覺小說引擎
- 角色立繪與情緒切換
- 背景管理
- 帶有選項的分支故事路線
- 多結局支援

---

## 開始

### 系統需求

- Unity 6+

### 安裝步驟

1. Clone 或下載此專案
2. 以 Unity 6 開啟專案
3. 將故事腳本放入 `Assets/StreamingAssets/Stories/`
4. 按下 **Play** 執行

> **注意：** 第一個故事腳本**必須**命名為 `StartingStory.txt`。  
> 若想使用其他名稱，請修改 `GameManager` 中的 `startingStory` 欄位。

---

## 撰寫你的故事

故事腳本是放置於 `Assets/StreamingAssets/Stories/` 的純 `.txt` 文字檔。

每一行可以是**指令**（以 `#` 開頭），或**路線標籤**（以 `*` 開頭），或**對話**。

### 對話範例

```
Mio: 你好，歡迎來到我的故事。
Alice: 很高興認識你！
```

### 指令

| 指令 | 說明 |
|---|---|
| `#bg <背景名稱>` | 切換背景圖片 |
| `#enter <角色名稱> <情緒>` | 將角色帶入畫面 |
| `#exit <角色名稱>` | 將角色移出畫面 |
| `#change <角色名稱> <情緒>` | 切換角色的情緒 |
| `#scene <場景名稱>` | 跳轉至指定場景 |
| `#jump <腳本名稱>` | 跳轉至另一個故事腳本 |
| `#end <結局名稱>` | 顯示結局圖片並結束故事 |
| `#choice` ... `#choice` | 向玩家呈現分支選項 |

> 每個腳本結尾須使用 `#jump` 或 `#end`。

---

## 指令參考

### `#bg <背景名稱>`

切換當前背景。

- 來源路徑：`Assets/Resources/Backgrounds/<背景名稱>.png`

```
#bg rooftop_night
```

---

### `#enter <角色名稱> <情緒>`

以指定情緒立繪將角色帶入畫面。

- 角色資料夾：`Assets/Resources/Characters/<角色名稱>/`
- 立繪檔案：`Assets/Resources/Characters/<角色名稱>/<情緒>.png`

```
#enter mio normal
```

---

### `#exit <角色名稱>`

將指定角色移出畫面。

```
#exit mio
```

---

### `#change <角色名稱> <情緒>`

切換已在畫面上的角色立繪。

- 立繪檔案：`Assets/Resources/Characters/<角色名稱>/<情緒>.png`

```
#change mio blush
```

---

### `#scene <場景名稱>`

**供開發者使用。** 暫停故事並啟動自訂 Unity 場景——適合小遊戲、互動橋段或任何超出對話範疇的玩法。自訂場景結束後，在你的程式碼中呼叫以下方法即可返回並從中斷處繼續故事：

```csharp
SceneController.Instance.ReturnScene();
```

在故事腳本中：

```
#scene MyMinigame
```

> `<場景名稱>` 必須與 Unity 場景名稱完全一致，並已加入 **Build Settings**。

---

### `#jump <腳本名稱>`

跳轉至另一個故事腳本檔案。

- 目標檔案：`Assets/StreamingAssets/Stories/<腳本名稱>.txt`

```
#jump Scene2
```

---

### `#end <結局名稱>`

顯示結局圖片並結束故事。

- 圖片檔案：`Assets/Resources/Endings/<結局名稱>.png`

```
#end demo_end
```

---

### `#bgm <背景音樂名稱>`

撥放一段背景音樂。

- 音樂檔案：`Assets/Resources/Audios/<背景音樂名稱>.mp3`

```
#end demo_end
```

---
### `#stopbgm`

停止播放背景音樂。

---

### `#choice`

向玩家呈現一組選項，每個選項分支至同一腳本中定義的路線標籤。將選項包夾在兩個 `#choice` 之間。

```
#choice
緊緊抱住她 => ending_hug
直接親下去 => ending_kiss
我該先立遺囑嗎？ => ending_joke
#choice
```

- `=>` 前的文字為顯示給玩家的按鈕文字
- `=>` 後的文字為跳轉目標的路線標籤（以 `*` 定義）

---

### `*<標籤>`

定義可供 `#choice` 選項跳轉的路線標籤，可放置在 `#choice` 區塊下方任意位置。

```
*ending_hug
#change mio blush
Mio: ……你這個笨蛋……突然做這種事……
#jump DemoEnd
```

---

## 腳本範例

```
#bg rooftop_night
#enter mio normal

Mio: ……又是一個人，一如往常。
Mio: ——嘿，新來的。要讓我「吃掉」你嗎？

#choice
逃跑 => route_run
留下來聊聊 => route_talk
#choice

*route_run
Mio: 哼……膽小鬼。
#jump EndingA

*route_talk
#change mio blush
Mio: ……你居然留下來了。
#jump EndingB
```

---

## 專案結構

```
Assets/
├── Resources/
│   ├── Backgrounds/        # 背景圖片（.png）
│   ├── Characters/
│   │   └── <角色名稱>/      # 每個角色一個資料夾
│   │       └── <情緒>.png
│   └── Endings/            # 結局圖片（.png）
└── StreamingAssets/
    └── Stories/            # 故事腳本（.txt）
        └── StartingStory.txt
```

---

## 授權

本專案為開源專案，詳見 [LICENSE](LICENSE)。