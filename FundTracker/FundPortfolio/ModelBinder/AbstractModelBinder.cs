using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;

namespace FundPortfolio.ModelBinder
{
	public class AbstractModelBinder : DefaultModelBinder
	{
		private readonly string _typeNameKey;

		public AbstractModelBinder(string typeNameKey = null)
		{
			_typeNameKey = typeNameKey ?? "__type__";
		}

		public override object BindModel
		(
		  ControllerContext controllerContext,
		  ModelBindingContext bindingContext
		)
		{
			var providerResult =
			bindingContext.ValueProvider.GetValue(_typeNameKey);

			if (providerResult != null)
			{
				var modelTypeName = providerResult.AttemptedValue;

				var modelType =
				  BuildManager.GetReferencedAssemblies()
					.Cast<Assembly>()
					.SelectMany(x => x.GetExportedTypes())
					.Where(type => !type.IsInterface)
					.Where(type => !type.IsAbstract).FirstOrDefault(type =>
					  string.Equals(type.FullName, modelTypeName,
						StringComparison.OrdinalIgnoreCase));

				if (modelType != null)
				{
					var metaData =	ModelMetadataProviders.Current.GetMetadataForType(null, modelType);

					bindingContext.ModelMetadata = metaData;
				}
			}

			// Fall back to default model binding behavior
			return base.BindModel(controllerContext, bindingContext);
		}
	}
}