from PyAPI import APIn, APOut, Log 
import time
apIn = APIn()


if __name__ == "__main__":
    apIn["Battery"] += 10
    apIn["Memory"] = "16 GB"

    time.sleep(2)

    APOut(input)