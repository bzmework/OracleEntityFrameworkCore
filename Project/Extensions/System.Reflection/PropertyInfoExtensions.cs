using JetBrains.Annotations;
using System.Diagnostics;
using System.Linq;

namespace System.Reflection
{
	/// <summary>
	/// PropertyInfo��չ
	/// </summary>
	[DebuggerStepThrough]
	internal static class PropertyInfoExtensions
	{
		/// <summary>
		/// �Ƿ�̬����
		/// </summary>
		/// <param name="property">������Ϣ</param>
		/// <returns></returns>
		public static bool IsStatic(this PropertyInfo property)
		{
			return (property.GetMethod ?? property.SetMethod).IsStatic;
		}

		/// <summary>
		/// �Ƿ��ѡ����
		/// </summary>
		/// <param name="propertyInfo">������Ϣ</param>
		/// <param name="needsWrite">��Ҫд��</param>
		/// <param name="publicOnly">�Ƿ���Public����</param>
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
		/// ����Getter���� 
		/// </summary>
		/// <param name="propertyInfo">������Ϣ</param>
		/// <returns></returns>
		public static PropertyInfo FindGetterProperty([NotNull] this PropertyInfo propertyInfo)
		{
			return propertyInfo.DeclaringType.GetPropertiesInHierarchy(propertyInfo.Name).FirstOrDefault((PropertyInfo p) => p.GetMethod != null);
		}

		/// <summary>
		/// ����Setter���� 
		/// </summary>
		/// <param name="propertyInfo">������Ϣ</param>
		/// <returns></returns>
		public static PropertyInfo FindSetterProperty([NotNull] this PropertyInfo propertyInfo)
		{
			return propertyInfo.DeclaringType.GetPropertiesInHierarchy(propertyInfo.Name).FirstOrDefault((PropertyInfo p) => p.SetMethod != null);
		}
	}
}
