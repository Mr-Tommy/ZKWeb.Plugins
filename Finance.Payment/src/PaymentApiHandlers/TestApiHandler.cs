﻿using DryIocAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Core;
using ZKWeb.Plugins.Common.Base.src.Model;
using ZKWeb.Plugins.Finance.Payment.src.Database;
using ZKWeb.Plugins.Finance.Payment.src.Forms;
using ZKWeb.Plugins.Finance.Payment.src.Model;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Plugins.Finance.Payment.src.PaymentApiHandlers {
	/// <summary>
	/// 测试接口的处理器
	/// </summary>
	[ExportMany]
	public class TestApiHandler : IPaymentApiHandler {
		/// <summary>
		/// 接口类型
		/// </summary>
		public string Type { get { return "TestApi"; } }
		/// <summary>
		/// 支付密码
		/// </summary>
		[Required]
		[StringLength(100, MinimumLength = 5)]
		[PasswordField("PaymentPassword", "Password required to pay transactions")]
		public string PaymentPassword { get; set; }

		/// <summary>
		/// 后台编辑表单创建后的处理
		/// </summary>
		public void OnFormCreated(PaymentApiEditForm form) {
			form.AddFieldsFrom(this);
		}

		/// <summary>
		/// 后台编辑表单绑定时的处理
		/// </summary>
		public void OnFormBind(PaymentApiEditForm form, DatabaseContext context, PaymentApi bindFrom) {
			PaymentPassword = bindFrom.ExtraData.GetOrDefault<string>("PaymentPassword");
		}

		/// <summary>
		/// 后台编辑表单保存时的处理
		/// </summary>
		public void OnFormSubmit(PaymentApiEditForm form, DatabaseContext context, PaymentApi saveTo) {
			saveTo.ExtraData["PaymentPassword"] = PaymentPassword;
		}

		/// <summary>
		/// 获取支付Html
		/// </summary>
		public void GetPaymentHtml(PaymentTransaction transaction, ref HtmlString html) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 调用发货接口
		/// </summary>
		public void SendGoods(PaymentTransaction transaction, string logisticsName, string invoiceNo) {
			throw new NotImplementedException();
		}
	}
}