

using System;
using System.Collections.Generic;

public class TaskManagerModel
{
    private static TaskManagerModel _instance;
    public static TaskManagerModel Instance => _instance ??= new TaskManagerModel();

    // 🔹 所有已加载的月份数据（键是 "2025-07"）
    private Dictionary<string, MonthMissionData> monthMap = new();

    // 🔹 当前激活的日期（如 "2025-07-28"），用于 UI 显示、自动跳转
    public string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

    // 🔹 当前选中的月份（如 "2025-07"），用于月历 UI 高亮
    public string currentMonth => currentDate.Substring(0, 7);



    // 🔹 当前是否有数据被修改（用于保存提示）
    private bool isDirty = false;

    // 🔹 自动保存标志（可控制是否实时保存）
    public bool autoSave = true;
}
