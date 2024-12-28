import sys
from PyAPI import APIIn, APIOut
import time
import os


if __name__ == "__main__":
    # 自分が配置されているディレクトリに移動
    os.chdir(os.path.dirname(os.path.abspath(__file__)))

    print("AAA")

    input = APIIn()
    input["Battery"] += 10
    input["Memory"] = "16 GB"

    time.sleep(2)

    APIOut(input)