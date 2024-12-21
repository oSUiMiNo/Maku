import sys
import json
import time

arg = sys.argv[1]
inputJObj = json.loads(arg)
inputJObj["Battery"] += 10
inputJObj["Memory"] = "16 GB" # "Memory"キーを追加するだけ
time.sleep(5)
print(json.dumps(inputJObj, ensure_ascii=False))