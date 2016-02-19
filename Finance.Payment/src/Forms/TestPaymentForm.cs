﻿using DryIoc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Localize;
using ZKWeb.Plugins.Common.Base.src.HtmlBuilder;
using ZKWeb.Plugins.Common.Base.src.Managers;
using ZKWeb.Plugins.Common.Base.src.Model;
using ZKWeb.Plugins.Common.Base.src.Repositories;
using ZKWeb.Plugins.Common.Currency.src.Config;
using ZKWeb.Plugins.Common.Currency.src.ListItemProviders;
using ZKWeb.Plugins.Finance.Payment.src.Database;
using ZKWeb.Server;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Plugins.Finance.Payment.src.Forms {
	/// <summary>
	/// 用于测试支付接口是否可以正常使用的表单
	/// </summary>
	public class TestPaymentForm : ModelFormBuilder {
		/// <summary>
		/// 接口名称
		/// </summary>
		[LabelField("ApiName")]
		public string ApiName { get; set; }
		/// <summary>
		/// 金额
		/// </summary>
		[Required]
		[RegularExpression(RegexUtils.Expressions.Decimal)]
		[TextBoxField("Amount")]
		public decimal Amount { get; set; }
		/// <summary>
		/// 货币
		/// </summary>
		[Required]
		[DropdownListField("Currency", typeof(CurrencyListItemProvider))]
		public string Currency { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		[Required]
		[TextAreaField("Description", 5)]
		public string Description { get; set; }

		/// <summary>
		/// 从请求中获取支付接口，不存在时抛出错误
		/// </summary>
		/// <returns></returns>
		protected PaymentApi GetApiFromRequest() {
			PaymentApi api = null;
			var id = HttpContext.Current.Request.GetParam<long>("id");
			UnitOfWork.ReadData<PaymentApi>(repository => api = repository.GetById(id));
			if (api == null) {
				throw new HttpException(404, new T("Payment api not exist"));
			}
			return api;
		}

		/// <summary>
		/// 绑定表单
		/// </summary>
		protected override void OnBind() {
			var api = GetApiFromRequest();
			var configManager = Application.Ioc.Resolve<GenericConfigManager>();
			var currencySettings = configManager.GetData<CurrencySettings>();
			ApiName = api.Name;
			Amount = 0.1M;
			Currency = currencySettings.DefaultCurrency;
			Description = new T("Test Payment Api");
		}

		/// <summary>
		/// 提交表单
		/// </summary>
		/// <returns></returns>
		protected override object OnSubmit() {
			throw new NotImplementedException();
		}
	}
}