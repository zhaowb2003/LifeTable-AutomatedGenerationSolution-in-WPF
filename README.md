<h1 align="center">LifeTable-AutomatedGenerationSolution-in-WPF <br> 👟 Step to Step 👟 &&👌🏼手把手👌🏼使用教程</h1>

## 目录

- [🔍 这段程序有何用](https://github.com/zhaowb2003/LifeTable-AutomatedGenerationSolution-in-WPF#-%E8%BF%99%E6%AE%B5%E7%A8%8B%E5%BA%8F%E6%9C%89%E4%BD%95%E7%94%A8)
  
- [🔧 环境安装与依赖](https://github.com/zhaowb2003/LifeTable-AutomatedGenerationSolution-in-WPF#-%E7%8E%AF%E5%A2%83%E5%AE%89%E8%A3%85%E4%B8%8E%E4%BE%9D%E8%B5%96)
  
- [🚀 下载运行本程序](https://github.com/zhaowb2003/LifeTable-AutomatedGenerationSolution-in-WPF#-%E4%B8%8B%E8%BD%BD%E8%BF%90%E8%A1%8C%E6%9C%AC%E7%A8%8B%E5%BA%8F)
  
- [💻 轻松绘制生命表](https://github.com/zhaowb2003/LifeTable-AutomatedGenerationSolution-in-WPF#-%E8%BD%BB%E6%9D%BE%E7%BB%98%E5%88%B6%E7%94%9F%E5%91%BD%E8%A1%A8)

## 🔍 这段程序有何用

这是一个使用WPF (.net 8.0) 构建的生命表生成程序，可以完成

1. 🎰 自动生成生命表 🎰
2. 💼 保存表格 💼
3. 📉 生成散点及拟合曲线图 📉
     
等功能，希望大家喜欢。

## 🔧 环境安装与依赖
请安装.net 8.0 runtime (点击运行后会自动跳转)

目前只支持windows版本，在windows11，windows10上可以运行，在windows8.1以下的电脑可能有兼容性问题。
使用mac linux的同学可以向我要此算法的py版本

## 🚀 下载运行本程序

1. 点击右侧"release"链接 ➡️ 展开"Assets" ➡️ 点击"v1.1.0-LTW-net8.0-windows.zip"下载<br>
   或者直接点击[这里](https://github.com/zhaowb2003/LifeTable-AutomatedGenerationSolution-in-WPF/releases/download/release/v1.1.0-LTW-net8.0-windows.zip)进行下载

2. 点击进行解压，解压后在 📁 ./v1.1.0-LTW-net8.0-windows 📁 文件夹下找到 👨‍💻 start.bat 👨‍💻 ，双击运行。
3. （打开成功请跳过此步）如果你的电脑没有安装.net 8.0 runtime，会有弹窗提示去微软官网 (https://dotnet.microsoft.com/zh-cn/download) 下载该运行时。
   <br>点击✅ ["下载.Net SDK x64"](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/sdk-8.0.401-windows-x64-installer) ✅<br>
   当下载完成后，找到下载好的运行时安装软件“dotnet-sdk-8.0.401-win-x64.exe”并双击安装。跟随步骤点击确定。待.Net SDK x64安装好后，重新双击步骤②中的👨‍💻 start.bat 👨‍💻运行。

## 💻 轻松绘制生命表

### ⚡ quick Start ⚡

1. 点击“请选择生命表形式：”选项卡中的“生命表-对角线型”。
2. 在下方的“请输入需要的样本数...”中输入一个你喜欢的数字，我们推荐使用 1000 作为样本数。
3. 滑动右侧的滑条，选择判断数。默认为“3”.
4. 点击上方栏中的“ ➕ 新数据 ”按钮，下方的表格栏中出现内容。
5. 点击窗口上方栏中的“ 💾 保存 ”按钮，保存表格为csv文件。
6. 点击上方栏中的“ 📊 绘图 ”按钮，弹出有拟合曲线图的新窗口。
7. 点击有拟合曲线图的新窗口上方栏中的“ 💾 保存 ”按钮，保存图片。

### ❓more explain ❓

#### 🎚️ 控制条 🎚️

注意，控制条只在“生命表-对角线型”状态下可用。

#### ⚙️ json配置 ⚙️
  
在默认情况下，json文本框中的内容如下：

```json
{
"killNumber":[2,3,4],
"Rounds":[2,4,30]
}
```
这段json传递了两个数组 `"killNumber"` 和 `"Rounds"`。
其中  `"killNumber"` 指在某一回合中去除样本的限定值，例如 `killNumber[0]` == 2 时，上个回合生成的数列 `life_table_once_row[]` （数列中数的值均在1-6之间）内的数字，小于 `killNumber[0]` 的数都会被去除。生成新的数列 `life_table_once_row_save[]` ，保存列长作为 `nx` 。

参数组 `"Rounds"` 则描述了当达到`"Rounds[i]"`回合后，程序的行为：
1. 若`"Rounds[i]"`不为数组最后一个数，则当 `Rounds[i]` 回合时，使 `i` 自增，并作用到 `killNumber` 参数。此时 `killNumber` == `killNumber[i+1]` 。
2. 若`"Rounds[i]"`为数组最后一个数（我们为最后一个数起名为Rounds[last]），则进行到Rounds[last]回合时，程序强行退出。

##### 🌰举个例子🌰

假设json文本框中的文字如下：
```json
{
"killNumber":[2,3,4],
"Rounds":[2,4,30]
}
```

则程序行为如下：

程序初始化，i = 0;

0-1回合，killNumber[i] == killNumber[0]  == 2；

2回合时，Rounds[i] == 2 等于本回合数，因此i++，此时i == 1; 故killNumber[i] == killNumber[1] == 3;

3回合 i == 1; Rounds[i] == Rounds[i] == 4; killNumber[i] == killNumber[1] == 3;

4回合时，Rounds[i] == 4 等于本回合数，因此i++，此时i == 2; 故killNumber[i] == killNumber[1] == 4;

......

30回合时，Rounds[i] == 30 等于本回合数，因此提示循环过长，退出程序。

尝试自己配置json，创造不同的曲线！



