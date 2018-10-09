# インディアン・ポーカー・ソルバー

## 使い方

.NET Core 2.1 が必要です。

```
git clone git@github.com:tana-gh/indian_poker.git
cd indian_poker
dotnet run -p IndianPoker.App.Multiple/IndianPoker.App.Multiple.csproj -- 5 3
```

上記 `5 3` の部分には、それぞれ**カード枚数**、**プレイヤー人数**を指定してください。  
（大きな数を指定すると計算時間がかかります）

```
dotnet run -p IndianPoker.App.Multiple/IndianPoker.App.Multiple.csproj -- <カード枚数> <プレイヤー人数>
```

配るカードを指定する場合はこちらです。

```
dotnet run -p IndianPoker.App.Single/IndianPoker.App.Single.csproj -- <カード枚数> <プレイヤー人数> <カード1> <カード2> ...
```

## 解決対象となる問題

[リクルートコミュニケーションズのコーディング試験](https://www.rco.recruit.co.jp/career/engineer/entry/)で過去に出題された問題を、[@sumim](https://twitter.com/sumim)氏が改変したものです。

問題はこちら → https://twitter.com/sumim/status/1043665621433470977

### 問題の要点

- この問題におけるインディアン・ポーカー
  - 2人以上のプレイヤーで行なう。
  - 数字の書かれたカードを用意する（カード数はプレイヤー数以上）。
  - プレイヤーにカードを配るが、各プレイヤーは自分のカードの数字を見てはいけない。
  - 各プレイヤーはカードを額にかざす。カード数字は自分を除く他プレイヤーに見える状態となる。
  - この状態で、各プレイヤーは順番に、自分のカードの数字を予想し、宣言してゆく。
  - 宣言の内容は4種類。
    - 自分が1番小さい「MIN」
    - 自分が1番大きい「MAX」
    - 自分は最大と最小以外である「MID」
    - 分からない「?」
  - 1人でも答えが分かったらゲーム終了。
  - プレイヤーは嘘をつかず、最善を尽くす。

### 例

カード `{ 1, 2, 3, 4, 5 }`  
プレイヤー `{ A, B, C }`

- A=1, B=4, C=5 の場合
  - A=>MIN

- A=1, B=2, C=4 の場合
  - A=>?, B=>MID

## 謝辞

問題並びに解答の提供者である[@sumim](https://twitter.com/sumim)氏に感謝します。
ありがとうございました。
