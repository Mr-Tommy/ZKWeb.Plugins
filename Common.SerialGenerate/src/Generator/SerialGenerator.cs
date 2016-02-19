﻿using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Plugins.Common.SerialGenerate.src.Model;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Plugins.Common.SerialGenerate.src.Generator {
	/// <summary>
	/// 序列号生成器
	/// </summary>
	public static class SerialGenerator {
		/// <summary>
		/// 给指定的数据生成序列号
		/// 默认序列号是 年月日+8位随机数字，共16位
		/// </summary>
		/// <param name="data">数据</param>
		/// <returns></returns>
		public static string GenerateFor(object data) {
			var now = DateTime.UtcNow.ToLocalTime();
			var serial = now.ToString("yyyyMMdd") + RandomUtils.RandomString(8, "0123456789");
			var callbacks = Application.Ioc.ResolveMany<ISerialGenerateCallback>();
			callbacks.ForEach(c => c.OnGenerate(data, ref serial);
			return serial;
		}
	}
}