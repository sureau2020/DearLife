import json
import os

input_dir = "./Items"      # 你的 JSON 文件夹
output_file = "all_items_list.json"

merged_list = []

for filename in os.listdir(input_dir):
    if filename.endswith(".json"):
        path = os.path.join(input_dir, filename)
        try:
            with open(path, "r", encoding="utf-8") as f:
                data = json.load(f)
                merged_list.append(data)
        except json.JSONDecodeError as e:
            print(f"❌ JSON 解析失败: {filename}")
            raise e

# 写成一个大 list
with open(output_file, "w", encoding="utf-8") as f:
    json.dump(merged_list, f, ensure_ascii=False, indent=2)

print(f"✅ 合完成，共 {len(merged_list)} 个物品")
