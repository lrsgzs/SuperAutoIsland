d > dropdown
dd > dynamic dropdown
date > @blockly/field-date

# 「档案」分类

## 通用

空 GUID
保存档案

## 科目

科目GUID [dd]
科目GUID [string]
科目GUID[string] 存在?

科目名称 科目GUID[string]

## 时间表

时间表GUID [dd]
时间表GUID [string]
时间表GUID[string] 存在?

时间表名称 时间表GUID[string]

## 课表

课表GUID [dd]
课表GUID [string]
课表GUID[string] 存在?

课表名称 课表GUID[string]
当前启用的课表GUID
当前为第几节课

获取 课表GUID[string] 的时间表GUID
获取 课表GUID[string] 第[int]节课 的科目GUID

(data)复制课表 原课表GUID[string]
(data)创建临时层 原课表GUID[string]
(data)创建临时层 原课表GUID[string] 启用日期[date]

设置 课表GUID[string] 名称[string]
设置 课表GUID[string] 时间表为 时间表GUID[string]
设置 课表GUID[string] 课表群为 课表群GUID[string]

设置 课表GUID[string] 触发规则为每周 每[int]周 的每周启用 且今天是星期[int]
设置 课表GUID[string] 触发规则为每周 每[int]周 的第[int]周 且今天是星期[int]
设置 课表GUID[string] 触发规则为某天 [list[date]]
设置 课表GUID[string] 触发日期范围为 自[date] 到[date]

修改 课表GUID[string] 第[int]节课 为 科目GUID[string]
交换 课表GUID[string] 第[int]节课 和 第[int]节课 课程
删除 课表GUID[string]

启用临时课表 [string] 启用日期[date]
清除临时课表 启用日期[date]

(rule)正在使用课表 课表GUID[string]
(rule)课表GUID[string] 第[int]节课 是 科目GUID[string]
(rule)课表GUID[string] 是临时层

## 课表群

课表群GUID [dd]
课表群GUID [string]
课表群GUID[string] 存在?

课表群名称 课表群GUID[string]
当前启用的课表群GUID

切换为课表群 [GUID]
