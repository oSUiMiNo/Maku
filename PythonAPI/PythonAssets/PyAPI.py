import sys
import json
import os
import threading
import time
from inspect import stack
from pathlib import Path


# 更新するファイルのパス
LogPath = ""
RootPah = ""
OutPath = ""


def APInit():
     # UTF-8 再設定 (必要に応じて)
    if hasattr(sys.stdin, "reconfigure"):
        sys.stdin.reconfigure(encoding='utf-8')
        sys.stdout.reconfigure(encoding='utf-8')
        
    global LogPath, RootPah, OutPath
    # ルートパス一応保存しとく
    RootPah = Path.cwd()
    # Assets 直下にログ用txtファイルがある
    LogPath = f"{Path.cwd()}/Assets/PyLog.txt"
    # 自分が配置されているディレクトリに移動
    os.chdir(os.path.dirname(os.path.abspath(__file__)))



def APIn():
    global OutPath
    # アウトプット用にC#によって呼び出し元.pyと同じディレクトリに [自分のファイル名.json] が作成されている。
    OutPath = f"{Path(stack()[1].filename).with_suffix(".txt")}"
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



# def APOut(outputJobj):
#      outputJson = json.dumps(outputJobj, ensure_ascii=False)
#      Log(f"JSON_OUTPUT_START{outputJson}JSON_OUTPUT_END") # プレフィックスとサフィックスで囲む
def APOut(outJO):
    if os.path.exists(OutPath):
        outJ = json.dumps(outJO, ensure_ascii=False)
        with open(OutPath, "a", encoding="utf-8") as file:
            file.write( f"___\n{outJ}\n")
    else:
        return outJO



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



# 新しいスレッドを作成して関数を割り当て
def Idle(fnc):
    threading.Thread(target=idle, args=[fnc]).start()
def idle(fnc):
    # メインループ（C#からの入力を処理）
    for arg in sys.stdin:
        if arg.strip() == "Close":
            break
        try:
            inJO = json.loads(arg.strip())
            # Log(f"受け取ったデータ: {inJO}")
            fnc(inJO)
        except json.JSONDecodeError:
            Log("JSON形式の引数ではありません。")
        except Exception as e:
            # errorPos = ""
            # for callerFrame in stack():
            #     callerFile = Path(callerFrame.filename).relative_to(RootPah).as_posix()
            #     callerLine = callerFrame.lineno
            #     errorPos += f"\n(at ./{callerFile}:{callerLine})"
            Log(f"エラーが発生しました: {e}")
        time.sleep(0.001)