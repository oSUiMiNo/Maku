import sys
import json
import os


# 更新するファイルのパス
LogPath = ""


def APInit():
    global LogPath
    # Assets 直下にログ用txtファイルがある
    LogPath = f"{os.getcwd()}/Assets/PyLog.txt"
    Log(f"カレント {os.getcwd()}")
    # 自分が配置されているディレクトリに移動
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    Log(f"カレント {os.getcwd()}")
    


def APIIn():
    if len(sys.argv) > 1:
            try:
                arg = sys.argv[1]
                inputJObj = json.loads(arg)
                return inputJObj
            except json.JSONDecodeError:
                Log("JSON形式の引数ではありません。")
            except Exception as e:
                Log(f"エラーが発生しました: {e}")
    else:
        Log("外部からの引数無し")



def APIOut(outputJobj):
     outputJson = json.dumps(outputJobj, ensure_ascii=False)
     Log(f"JSON_OUTPUT_START{outputJson}JSON_OUTPUT_END") # プレフィックスとサフィックスで囲む



def Log(msg):
    if os.path.exists(LogPath):
        # print("ある")
        # ファイルに追記
        with open(LogPath, "a", encoding="utf-8") as file:
            file.write(f"___\n{msg}\n")
    else:
        # ファイルが存在しない場合はコンソールに出力
        print(msg)
