import os
import re

# 获取当前目录下所有jpg文件
files = [f for f in os.listdir('.') if f.endswith('.jpg')]

# 按文件名排序
files.sort()

# 重命名文件
for i, filename in enumerate(files):
    # 匹配10001.jpg, 10002-1.jpg等格式
    if re.match(r'^100\d\d+(-1)?\.jpg$', filename):
        new_name = f"{i+10}.jpg"
        os.rename(filename, new_name)
        print(f"已重命名: {filename} -> {new_name}")