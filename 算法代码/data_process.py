import matplotlib.pyplot as plt
import seaborn as sns

# 假设 evaluate_accuracy 函数已经运行，并且我们得到了以下准确率数据
# 这些数据应该从 evaluate_accuracy 函数的输出中提取
accuracy_data = {
    "GradientBoosting": {"No CF": 0.6667, "With CF": 0.6250},
    "RandomForest": {"No CF": 0.4583, "With CF": 0.6250},
    "LogisticRegression": {"No CF": 0.5000, "With CF": 0.5000},
    "KNN": {"No CF": 0.5417, "With CF": 0.5417},
    "SVC": {"No CF": 0.4167, "With CF": 0.5000}
}

# 将数据转换为适合绘图的格式
models = list(accuracy_data.keys())
no_cf_accuracies = [accuracy_data[model]["No CF"] for model in models]
with_cf_accuracies = [accuracy_data[model]["With CF"] for model in models]

# 创建柱状图
fig, ax = plt.subplots(figsize=(10, 6))

# 设置柱状图的位置和宽度
bar_width = 0.35
index = range(len(models))

# 绘制柱状图
bar1 = ax.bar(index, no_cf_accuracies, bar_width, label='No CF')
bar2 = ax.bar([i + bar_width for i in index], with_cf_accuracies, bar_width, label='With CF')

# 添加标题和标签
ax.set_xlabel('Model')
ax.set_ylabel('Accuracy')
ax.set_title('Model Accuracy with and without Counterfactual Explanation')
ax.set_xticks([i + bar_width / 2 for i in index])
ax.set_xticklabels(models, rotation=45, ha='right')
ax.legend()

# 显示准确率值
for i, v in enumerate(no_cf_accuracies):
    ax.text(i, v + 0.01, f'{v:.2f}', ha='center', va='bottom')
for i, v in enumerate(with_cf_accuracies):
    ax.text(i + bar_width, v + 0.01, f'{v:.2f}', ha='center', va='bottom')

# 显示图表
plt.tight_layout()
plt.show()