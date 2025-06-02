import pandas as pd
import numpy as np
from sklearn.datasets import load_breast_cancer
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
from sklearn.ensemble import GradientBoostingClassifier, RandomForestClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.neighbors import KNeighborsClassifier
from sklearn.svm import SVC
from econml.dml import CausalForestDML
import warnings

warnings.filterwarnings("ignore", message="'force_all_finite' was renamed to 'ensure_all_finite'")

# 1. 加载UCI Breast Cancer数据集
data = load_breast_cancer(as_frame=True)
X = data.data
y = data.target
features = X.columns.tolist()

# 2. 划分训练集和测试集
X_train, _, y_train, _ = train_test_split(X, y, test_size=0.2, random_state=42)
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_scaled = scaler.transform(X)

# 3. 定义多种模型
models = {
    "GradientBoosting": GradientBoostingClassifier(n_estimators=100, random_state=42),
    "RandomForest": RandomForestClassifier(n_estimators=200, random_state=42),
    "LogisticRegression": LogisticRegression(max_iter=5000, random_state=42),
    "KNN": KNeighborsClassifier(n_neighbors=5),
    "SVC": SVC(probability=True, random_state=42)
}

# 4. 训练所有模型
for name, model in models.items():
    model.fit(X_train, y_train)

# 5. 训练因果森林模型（用于反事实分析）
causal_forest_models = {}
for feature in features:
    T = X_train_scaled[:, features.index(feature)]
    X_temp = np.delete(X_train_scaled, features.index(feature), axis=1)
    model_cf = CausalForestDML(n_estimators=40, random_state=42, min_samples_leaf=5, n_jobs=-1)
    model_cf.fit(Y=y_train, T=T, X=X_temp, W=None)
    causal_forest_models[feature] = model_cf

def process_row_no_cf(row, model_name):
    row_features = pd.DataFrame([row], columns=features)
    pred_label = int(models[model_name].predict(row_features)[0])
    return pred_label

def process_row_with_cf_advanced(row, model_name):
    row_features = pd.DataFrame([row], columns=features)
    pred_proba = models[model_name].predict_proba(row_features)[0]
    pred_label = int(np.argmax(pred_proba))
    best_proba = pred_proba[pred_label]
    best_label = pred_label

    row_scaled = scaler.transform(row_features)
    effects = []
    for feature in features:
        model_cf = causal_forest_models[feature]
        T = row_scaled[0, features.index(feature)]
        X_temp = np.delete(row_scaled, features.index(feature), axis=1)
        effect = model_cf.effect(T0=T, T1=T, X=X_temp)
        effects.append(abs(effect[0]))
    sorted_features = [features[i] for i in np.argsort(effects)[::-1]]

    for feature in sorted_features[:2]:
        for delta in [-2, -1, -0.5, 0.5, 1, 2]:
            row_cf = row_features.copy()
            row_cf[feature] = row_cf[feature] + delta
            proba_cf = models[model_name].predict_proba(row_cf)[0]
            label_cf = int(np.argmax(proba_cf))
            if proba_cf[label_cf] > best_proba:
                best_proba = proba_cf[label_cf]
                best_label = label_cf
    return best_label

def evaluate_accuracy():
    _, X_test, _, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
    X_test = X_test.copy()
    X_test["Name"] = range(len(X_test))

    for model_name in models:
        correct_no_cf = 0
        correct_with_cf_advanced = 0
        total = len(X_test)
        for i, (_, row) in enumerate(X_test.iterrows()):
            label_no_cf = process_row_no_cf(row, model_name)
            label_with_cf_advanced = process_row_with_cf_advanced(row, model_name)
            if label_no_cf == int(y_test.iloc[i]):
                correct_no_cf += 1
            if label_with_cf_advanced == int(y_test.iloc[i]):
                correct_with_cf_advanced += 1
        print(f"{model_name} 未利用反事实解释的准确率: {correct_no_cf / total:.4f}")
        print(f"{model_name} 结合因果效应和动态扰动的反事实优化准确率: {correct_with_cf_advanced / total:.4f}")

if __name__ == "__main__":
    evaluate_accuracy()