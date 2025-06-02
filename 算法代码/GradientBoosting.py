import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, StandardScaler
from sklearn.ensemble import GradientBoostingClassifier, RandomForestClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.neighbors import KNeighborsClassifier
from sklearn.svm import SVC
from econml.dml import CausalForestDML

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
    "GradientBoosting": GradientBoostingClassifier(n_estimators=100, random_state=42),
    "RandomForest": RandomForestClassifier(n_estimators=200, random_state=42),
    "LogisticRegression": LogisticRegression(max_iter=1000, random_state=42),
    "KNN": KNeighborsClassifier(n_neighbors=5),
    "SVC": SVC(probability=True, random_state=42)
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

def process_row_no_cf(row, model_name):
    """未利用反事实解释，直接预测"""
    row_features = pd.DataFrame([{feature: row[feature] for feature in features}])
    pred_label = int(models[model_name].predict(row_features)[0])
    pred_name = label_encoder.inverse_transform([pred_label])[0]
    return pred_label, pred_name

def process_row_with_cf_advanced(row, model_name):
    """结合因果效应和动态扰动的反事实优化"""
    row_features = pd.DataFrame([{feature: row[feature] for feature in features}])
    pred_proba = models[model_name].predict_proba(row_features)[0]
    pred_label = int(np.argmax(pred_proba))
    best_proba = pred_proba[pred_label]
    best_label = pred_label

    # 1. 计算每个特征的反事实效应（绝对值最大者优先）
    row_scaled = scaler.transform(row_features)
    effects = []
    for feature in features:
        model_cf = causal_forest_models[feature]
        T = row_scaled[0, features.index(feature)]
        X_temp = np.delete(row_scaled, features.index(feature), axis=1)
        effect = model_cf.effect(T0=T, T1=T, X=X_temp)
        effects.append(abs(effect[0]))
    sorted_features = [features[i] for i in np.argsort(effects)[::-1]]  # 按效应绝对值降序

    # 2. 对效应最大的前2个特征，尝试不同幅度扰动
    for feature in sorted_features[:2]:
        for delta in [-2, -1, -0.5, 0.5, 1, 2]:
            row_cf = row_features.copy()
            row_cf[feature] = row_cf[feature] + delta
            proba_cf = models[model_name].predict_proba(row_cf)[0]
            label_cf = int(np.argmax(proba_cf))
            if proba_cf[label_cf] > best_proba:
                best_proba = proba_cf[label_cf]
                best_label = label_cf

    pred_name = label_encoder.inverse_transform([best_label])[0]
    return best_label, pred_name

def evaluate_accuracy():
    _, X_test, _, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
    X_test = X_test.copy()
    X_test["Name"] = range(len(X_test))

    for model_name in models:
        correct_no_cf = 0
        correct_with_cf_advanced = 0
        total = len(X_test)
        for i, (_, row) in enumerate(X_test.iterrows()):
            label_no_cf, _ = process_row_no_cf(row, model_name)
            label_with_cf_advanced, _ = process_row_with_cf_advanced(row, model_name)
            if label_no_cf == int(y_test.iloc[i]):
                correct_no_cf += 1
            if label_with_cf_advanced == int(y_test.iloc[i]):
                correct_with_cf_advanced += 1
        print(f"{model_name} 未利用反事实解释的准确率: {correct_no_cf / total:.4f}")
        print(f"{model_name} 结合因果效应和动态扰动的反事实优化准确率: {correct_with_cf_advanced / total:.4f}")

if __name__ == "__main__":
    evaluate_accuracy()