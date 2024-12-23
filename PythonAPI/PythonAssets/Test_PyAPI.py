import sys
from PyAPI import APIIn, APIOut
import time
import os


if __name__ == "__main__":
    os.chdir(os.path.dirname(os.path.abspath(__file__)))

    print("AAA")

    input = APIIn()
    if input is None: # Noneチェックを追加
        sys.exit(1) # エラー終了

    input["Battery"] += 10
    input["Memory"] = "16 GB"

    # time.sleep(5)

    APIOut(input)