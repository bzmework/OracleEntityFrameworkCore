using JetBrains.Annotations;
using System.Diagnostics;
using System.Linq;

namespace System.Reflection
{
	/// <summary>
	/// PropertyInfo扩展
	/// </summary>
	[DebuggerStepThrough]
	internal static class PropertyInfoExtensions
	{
		/// <summary>
		/// 是否静态属性
		/// </summary>
		/// <param name="property">属性信息</param>
		/// <returns></returns>
		public static bool IsStatic(this PropertyInfo property)
		{
			return (property.GetMethod ?? property.SetMethod).IsStatic;
		}

		/// <summary>
		/// 是否候选属性
		/// </summary>
		/// <param name="propertyInfo">属性信息</param>
		/// <param name="needsWrite">需要写入</param>
		/// <param name="publicOnly">是否是Public方法</param>
		/// <returns></returns>
		public static bool IsCandidateProperty(this PropertyInfo propertyInfo, bool needsWrite = true, bool publicOnly = true)
		{
			if (!propertyInfo.IsStatic() && propertyInfo.CanRead && (!needsWrite || propertyInfo.FindSetterProperty() != null) && propertyInfo.GetMethod != null && (!publicOnly || propertyInfo.GetMethod.IsPublic))
			{
				return propertyInfo.GetIndexParameters().Length == 0;
			}
			return false;
		}

		/// <summary>
		/// 查找Getter属性 
		/// </summary>
		/// <param name="propertyInfo">属性信息</param>
		/// <returns></returns>
		public static PropertyInfo FindGetterProperty([NotNull] this PropertyInfo propertyInfo)
		{
			return propertyInfo.DeclaringType.GetPropertiesInHierarchy(propertyInfo.Name).FirstOrDefault((PropertyInfo p) => p.GetMethod != null);
		}

		/// <summary>
		/// 查找Setter属性 
		/// </summary>
		/// <param name="propertyInfo">属性信息</param>
		/// <returns></returns>
		public static PropertyInfo FindSetterProperty([NotNull] this PropertyInfo propertyInfo)
		{
			return propertyInfo.DeclaringType.GetPropertiesInHierarchy(propertyInfo.Name).FirstOrDefault((PropertyInfo p) => p.SetMethod != null);
		}
	}
}
