﻿using DotLiquid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.FastReflection;
using System.Reflection;
using ZKWeb.Plugins.Common.Base.src.UIComponents.Forms.Attributes;
using ZKWeb.Plugins.Common.Base.src.UIComponents.Forms.Extensions;
using ZKWeb.Plugins.Common.Base.src.UIComponents.Forms.Interfaces;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Plugins.Common.Base.src.UIComponents.Forms {
	/// <summary>
	/// 从模型构建表单的构建器
	/// <exmaple>
	/// public class TestForm : ModelFormBuilder {
	///		[Required]
	///		[TextBoxField("FieldA", "Please enter something")]
	///		public string FieldA { get; set; }
	///		[TextBoxField("FieldB", "Please enter something")]
	///		public string FieldB { get; set; }
	///		protected override void OnBind() {
	///			FieldA = "Default value";
	///		}
	///		protected override object OnSubmit() {
	///			return new { message = string.Format("{0}, {1}", FieldA, FieldB) };
	///		}
	/// }
	/// </exmaple>
	/// </summary>
	public abstract class ModelFormBuilder : IModelFormBuilder, ILiquidizable {
		/// <summary>
		/// 表单构建器
		/// </summary>
		[JsonIgnore]
		public FormBuilder Form { get; protected set; }
		/// <summary>
		/// 表单字段到成员信息
		/// </summary>
		[JsonIgnore]
		public IDictionary<FormField, Pair<object, PropertyInfo>> FieldToProperty { get; protected set; }
		/// <summary>
		/// 当前的Http上下文
		/// </summary>
		[JsonIgnore]
		public virtual IHttpContext Context { get { return HttpManager.CurrentContext; } }
		/// <summary>
		/// 当前的Http请求
		/// </summary>
		[JsonIgnore]
		public virtual IHttpRequest Request { get { return Context.Request; } }
		/// <summary>
		/// 当前的Http回应
		/// </summary>
		[JsonIgnore]
		public virtual IHttpResponse Response { get { return Context.Response; } }

		/// <summary>
		/// 初始化
		/// </summary>
		public ModelFormBuilder(FormBuilder form = null) {
			Form = form ?? Application.Ioc.Resolve<FormBuilder>();
			// 设置表单属性
			var typeInfo = GetType().GetTypeInfo();
			var formAttribute = typeInfo.GetAttribute<FormAttribute>();
			if (formAttribute != null) {
				Form.Attribute = formAttribute;
			}
			// 添加成员和验证属性
			Form.Fields.Clear();
			FieldToProperty = new Dictionary<FormField, Pair<object, PropertyInfo>>();
			AddFieldsFrom(this);
		}

		/// <summary>
		/// 从指定的对象添加字段
		/// 默认会从this添加，这个函数可以用于从其他对象扩展表单
		/// </summary>
		/// <param name="obj">对象</param>
		public void AddFieldsFrom(object obj) {
			// 先添加基类的字段，再添加子类的字段
			var types = new List<Type>();
			var type = obj.GetType();
			while (type != null) {
				types.Add(type);
				type = type.GetTypeInfo().BaseType;
			}
			types.Reverse();
			foreach (var fromType in types) {
				foreach (var property in fromType.FastGetProperties(
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
					var fieldAttribute = property.GetAttribute<FormFieldAttribute>();
					if (fieldAttribute != null) {
						var field = new FormField(fieldAttribute);
						field.ValidationAttributes.AddRange(property.GetAttributes<ValidationAttribute>());
						Form.Fields.Add(field);
						FieldToProperty[field] = Pair.Create(obj, property);
					}
				}
			}
		}

		/// <summary>
		/// 绑定时的处理
		/// </summary>
		protected abstract void OnBind();

		/// <summary>
		/// 提交时的处理，返回处理结果
		/// </summary>
		protected abstract object OnSubmit();

		/// <summary>
		/// 绑定表单
		/// </summary>
		public void Bind() {
			// 绑定值到模型
			OnBind();
			// 把模型中的值设置到字段
			Pair<object, PropertyInfo> property;
			foreach (var field in Form.Fields) {
				if (FieldToProperty.TryGetValue(field, out property)) {
					field.Value = property.Second.FastGetValue(property.First);
				}
			}
		}

		/// <summary>
		/// 提交表单，返回处理结果
		/// </summary>
		/// <returns></returns>
		public object Submit() {
			// 把提交的值设置到模型
			var submitValues = Request.GetAllDictionary();
			var values = Form.ParseValues(submitValues);
			Pair<object, PropertyInfo> property;
			foreach (var field in Form.Fields) {
				var value = values.GetOrDefault(field.Attribute.Name);
				if (FieldToProperty.TryGetValue(field, out property)) {
					value = value.ConvertOrDefault(property.Second.PropertyType, null);
					if (value != null) {
						property.Second.FastSetValue(property.First, value);
					}
				}
			}
			// 调用提交时的处理
			return OnSubmit();
		}

		/// <summary>
		/// 获取表单属性
		/// </summary>
		/// <returns></returns>
		FormAttribute IModelFormBuilder.GetFormAttribute() {
			return Form.Attribute;
		}

		/// <summary>
		/// 允许直接描画到模板
		/// </summary>
		/// <returns></returns>
		object ILiquidizable.ToLiquid() {
			return ((ILiquidizable)Form).ToLiquid();
		}

		/// <summary>
		/// 获取表单html
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Form.ToString();
		}
	}
}
