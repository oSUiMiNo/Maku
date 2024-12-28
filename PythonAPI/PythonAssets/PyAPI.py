import sys
import json


def APIn():
    if len(sys.argv) > 1:
            try:
                arg = sys.argv[1]
                inputJObj = json.loads(arg)
                return inputJObj
            except json.JSONDecodeError:
                print("JSON形式の引数ではありません。")
            except Exception as e:
                print(f"エラーが発生しました: {e}")
    else:
        print("外部からの引数無し")


def APOut(outputJobj):
     outputJson = json.dumps(outputJobj, ensure_ascii=False)
     print(f"JSON_OUTPUT_START{outputJson}JSON_OUTPUT_END") # プレフィックスとサフィックスで囲む