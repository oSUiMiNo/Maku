import sys
import json
import os
from inspect import stack
from pathlib import Path


# 更新するファイルのパス
LogPath = ""
RootPah = ""


def APInit():
    global LogPath, RootPah
    # Assets 直下にログ用txtファイルがある
    LogPath = f"{Path.cwd()}/Assets/PyLog.txt"
    RootPah = Path.cwd()
    # 自分が配置されているディレクトリに移動
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    


def APIn():
    APInit()
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



def APOut(outputJobj):
     outputJson = json.dumps(outputJobj, ensure_ascii=False)
     Log(f"JSON_OUTPUT_START{outputJson}JSON_OUTPUT_END") # プレフィックスとサフィックスで囲む



def Log(msg):
    if os.path.exists(LogPath):
        # スタックの呼び出し元情報を取得（スタックの1つ上）
        caller_frame = stack()[1]
        # 呼び出し元ファイルの相対パス
        caller_file = Path(caller_frame.filename).relative_to(RootPah).as_posix()
        # 呼び出し元の行番号
        caller_line = caller_frame.lineno
        # ファイルに追記
        with open(LogPath, "a", encoding="utf-8") as file:
            file.write( 
                        f"___\n{msg}\n"
                        f"(at ./{caller_file}:{caller_line})")
    else:
        # ファイルが存在しない場合はコンソールに出力
        print(msg)


