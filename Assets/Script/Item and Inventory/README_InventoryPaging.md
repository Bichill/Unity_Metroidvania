# 分頁背包系統設置指南

## 概述
這個分頁背包系統允許背包在容量不足時自動創建新的頁面，並提供頁面切換功能。

## 主要功能
1. **自動擴展**：當背包槽不夠時，自動創建新的背包頁面
2. **頁面切換**：通過UI按鈕切換不同的背包頁面
3. **頁面指示器**：顯示當前頁面位置和總頁面數量
4. **無限制拾取**：移除拾取物品的容量限制

## 設置步驟

### 1. 準備背包頁面預製體
- 創建一個包含14個ItemSlot的背包頁面預製體
- 確保所有ItemSlot都有UI_ItemSlot腳本
- 將預製體保存為 `InventoryPage.prefab`

### 2. 修改Inventory腳本設置
在Inventory腳本中設置以下參數：
- `inventoryPagePrefab`：拖入背包頁面預製體
- `inventoryPagesParent`：拖入背包頁面的父物體Transform

### 3. 創建UI分頁控制器
1. 在背包UI中添加以下元素：
   - 上一頁按鈕 (Previous Button)
   - 下一頁按鈕 (Next Button)
   - 頁面信息文本 (Page Info Text)
   - 頁面指示器容器 (Page Indicators Parent)

2. 添加UI_InventoryPaging腳本到背包UI
3. 設置腳本參數：
   - `previousPageButton`：拖入上一頁按鈕
   - `nextPageButton`：拖入下一頁按鈕
   - `pageInfoText`：拖入頁面信息文本
   - `pageIndicatorsParent`：拖入頁面指示器容器

### 4. 創建頁面指示器預製體
1. 創建一個簡單的UI Image作為頁面指示器
2. 添加UI_PageIndicator腳本
3. 設置適當的大小和樣式
4. 保存為 `PageIndicator.prefab`

### 5. 設置頁面指示器
在UI_InventoryPaging腳本中：
- `pageIndicatorPrefab`：拖入頁面指示器預製體

## 使用方法

### 自動擴展
- 當玩家拾取物品時，如果當前背包已滿，系統會自動創建新的背包頁面
- 新頁面會自動設置為非激活狀態，只有當前頁面可見

### 頁面切換
- 使用上一頁/下一頁按鈕切換背包頁面
- 頁面信息會顯示當前頁面和總頁面數量
- 頁面指示器會高亮顯示當前頁面

### 直接跳轉
- 點擊頁面指示器可以直接跳轉到指定頁面

## 腳本說明

### Inventory.cs 新增功能
- `CreateNewInventoryPage()`：創建新的背包頁面
- `SwitchToLast()`：切換到上一個頁面
- `SwitchToNext()`：切換到下一個頁面
- `CanAddItem()`：修改為總是返回true，支持自動擴展

### UI_InventoryPaging.cs
- 管理頁面切換UI
- 更新頁面信息顯示
- 控制頁面指示器

### UI_PageIndicator.cs
- 單個頁面指示器的行為
- 支持點擊跳轉到指定頁面

## 注意事項
1. 確保背包頁面預製體包含正確數量的ItemSlot
2. 每頁默認14個槽位，可在Inventory腳本中修改 `slotsPerPage` 變數
3. 頁面切換時會自動更新UI顯示
4. 所有頁面的物品數據都存儲在同一個inventory列表中

## 故障排除
- 如果背包頁面不顯示，檢查 `inventoryPagePrefab` 和 `inventoryPagesParent` 設置
- 如果頁面切換不工作，確保UI_InventoryPaging腳本正確設置
- 如果物品不顯示，檢查ItemSlot腳本是否正確附加 