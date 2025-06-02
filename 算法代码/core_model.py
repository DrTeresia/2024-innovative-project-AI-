import time
import os
import json
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, StandardScaler
from sklearn.ensemble import RandomForestClassifier, GradientBoostingClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.neighbors import KNeighborsClassifier
from sklearn.svm import SVC
from econml.dml import CausalForestDML
from concurrent.futures import ThreadPoolExecutor
import warnings
warnings.filterwarnings("ignore", message="'force_all_finite' was renamed to 'ensure_all_finite'")

# 加载数据
data = pd.read_csv("large.csv")
data.fillna(0, inplace=True)

features = ["兵力比归一化", "地形编码", "天气编码", "发生时间"]
target = "使用的计策"

# 只对目标编码
label_encoder = LabelEncoder()
data[target] = label_encoder.fit_transform(data[target].astype(str))

X = data[features]
y = data[target]

# 只用训练集 fit scaler
X_train, _, y_train, _ = train_test_split(X, y, test_size=0.2, random_state=42)
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_scaled = scaler.transform(X)

# 定义多种模型
models = {
    "RandomForest": RandomForestClassifier(n_estimators=200, random_state=42),
    "LogisticRegression": LogisticRegression(max_iter=1000, random_state=42),
    "KNN": KNeighborsClassifier(n_neighbors=5),
    "SVC": SVC(probability=True, random_state=42),
    "GradientBoosting": GradientBoostingClassifier(n_estimators=100, random_state=42)
}

# 训练所有模型
for name, model in models.items():
    model.fit(X_train, y_train)

# 训练因果森林模型（用于反事实分析）
causal_forest_models = {}
for feature in features:
    T = X_train_scaled[:, features.index(feature)]
    X_temp = np.delete(X_train_scaled, features.index(feature), axis=1)
    model = CausalForestDML(n_estimators=300, random_state=42, min_samples_leaf=5, n_jobs=-1)
    model.fit(Y=y_train, T=T, X=X_temp, W=None)
    causal_forest_models[feature] = model

def process_row(row, model_name="RandomForest"):
    # 标准化新样本
    row_features = pd.DataFrame([{feature: row[feature] for feature in features}])
    row_scaled = scaler.transform(row_features)
    # 主预测：用指定分类模型
    pred_label = int(models[model_name].predict(row_features)[0])
    pred_name = label_encoder.inverse_transform([pred_label])[0]

    # 反事实分析部分
    original_preds = {}
    for feature in features:
        model = causal_forest_models[feature]
        T = row_scaled[0, features.index(feature)]
        X_temp = np.delete(row_scaled, features.index(feature), axis=1)
        effect = model.effect(T0=T, T1=T, X=X_temp)
        original_preds[feature] = effect[0]

    counterfactual_results = []
    for feature in features:
        model = causal_forest_models[feature]
        T = row_scaled[0, features.index(feature)]
        X_temp = np.delete(row_scaled, features.index(feature), axis=1)
        std = 1  # 标准差扰动
        for delta in [-std, std]:
            T_cf = T + delta
            effect_cf = model.effect(T0=T, T1=T_cf, X=X_temp)
            effect_change = effect_cf[0] - original_preds[feature]
            counterfactual_results.append({
                "特征": feature,
                "方向": "增大" if delta > 0 else "减小",
                "处理效应变化量": float(effect_change),
                "新特征值": float(T_cf)
            })

    # 找到绝对值最大影响
    effects_df = pd.DataFrame(counterfactual_results)
    idx = effects_df["处理效应变化量"].abs().idxmax()
    most_influential = effects_df.loc[idx]

    return {
        "名字": str(row.get("Name", "")),
        "最有影响力特征": str(most_influential["特征"]),
        "预测的计策编号": int(pred_label),
        "预测的计策名称": str(pred_name)
    }

def check_and_process_file():
    new_battle_file_path = "C:\\Users\\mikusama\\\AppData\\LocalLow\\Sustech\\SanGuoTown\\General_Data\\generals_info.json"
    if not os.path.exists(new_battle_file_path):
        print("新战役数据文件不存在，跳过处理。")
        return
    with open(new_battle_file_path, "r", encoding="utf-8") as f:
        new_battle_data = json.load(f)
    new_battle_data = pd.DataFrame(new_battle_data)
    # 并行处理每一行
    with ThreadPoolExecutor() as executor:
        results = list(executor.map(process_row, [row for _, row in new_battle_data.iterrows()]))
    with open("../Assets/StreamingAssets/predicted_strategy.json", "w", encoding='utf-8') as f:
        json.dump(results, f, ensure_ascii=False, indent=4)
    print("预测结果已更新到文件：predicted_strategy.json")

def evaluate_accuracy():
    # 使用之前划分的测试集
    _, X_test, _, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
    X_test = X_test.copy()
    X_test["Name"] = range(len(X_test))  # 给每行加个Name，避免process_row报错

    for model_name in models:
        correct = 0
        total = len(X_test)
        for i, (_, row) in enumerate(X_test.iterrows()):
            result = process_row(row, model_name=model_name)
            if result["预测的计策编号"] == int(y_test.iloc[i]):
                correct += 1
        accuracy = correct / total
        print(f"{model_name} 在测试集上的准确率为: {accuracy:.4f}")

if __name__ == "__main__":
    evaluate_accuracy()
    while True:
        check_and_process_file()
        time.sleep(10)