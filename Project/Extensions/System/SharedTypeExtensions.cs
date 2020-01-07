using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System
{
	/// <summary>
	/// ����������չ
	/// </summary>
	[DebuggerStepThrough]
	internal static class SharedTypeExtensions
	{
		private static readonly Dictionary<Type, object> _commonTypeDictionary = new Dictionary<Type, object>
		{
			{ typeof(int),				0						},
			{ typeof(Guid),				default(Guid)			},
			{ typeof(DateTime),			default(DateTime)		},
			{ typeof(DateTimeOffset),	default(DateTimeOffset) },
			{ typeof(long),				0L						},
			{ typeof(bool),				false					},
			{ typeof(double),			0.0d					},
			{ typeof(short),			(short)0				},
			{ typeof(float),			0f						},
			{ typeof(byte),				(byte)0					},
			{ typeof(char),				'\0'					},
			{ typeof(uint),				0u						},
			{ typeof(ushort),			(ushort)0				},
			{ typeof(ulong),			0uL						},
			{ typeof(sbyte),			(sbyte)0				}
		};

		/// <summary>
		/// չ���ɿ�����
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static Type UnwrapNullableType(this Type type)
		{
			return Nullable.GetUnderlyingType(type) ?? type;
		}

		/// <summary>
		/// �Ƿ�ǿ�����
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static bool IsNullableType(this Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			if (typeInfo.IsValueType)
			{
				if (typeInfo.IsGenericType)
				{
					return typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// �Ƿ���Ч��ʵ������
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static bool IsValidEntityType(this Type type)
		{
			return type.GetTypeInfo().IsClass;
		}

		/// <summary>
		/// �����ǿ�����
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="nullable">�Ƿ�Ϊ��</param>
		/// <returns></returns>

		public static Type MakeNullable(this Type type, bool nullable = true)
		{
			if (type.IsNullableType() != nullable)
			{
				if (!nullable)
				{
					return type.UnwrapNullableType();
				}
				return typeof(Nullable<>).MakeGenericType(type);
			}
			return type;
		}

		/// <summary>
		/// �Ƿ�����
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static bool IsInteger(this Type type)
		{
			type = type.UnwrapNullableType();
			if (!(type == typeof(int))    && 
				!(type == typeof(long))   && 
				!(type == typeof(short))  && 
				!(type == typeof(byte))   && 
				!(type == typeof(uint))   && 
				!(type == typeof(ulong))  && 
				!(type == typeof(ushort)) && 
				!(type == typeof(sbyte)))
			{
				return type == typeof(char);
			}
			return true;
		}

		/// <summary>
		/// �������������Ϣ
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="name">����</param>
		/// <returns></returns>
		public static PropertyInfo GetAnyProperty(this Type type, string name)
		{
			List<PropertyInfo> list = 
			    (from p in RuntimeReflectionExtensions.GetRuntimeProperties(type)
				where p.Name == name
			   select p).ToList();
			if (list.Count > 1)
			{
				throw new AmbiguousMatchException();
			}
			return list.SingleOrDefault();
		}

		/// <summary>
		/// �ǿ�ʵ������
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static bool IsInstantiable(this Type type)
		{
			return IsInstantiable(type.GetTypeInfo());
		}

		private static bool IsInstantiable(TypeInfo type)
		{
			if (!type.IsAbstract && !type.IsInterface)
			{
				if (type.IsGenericType)
				{
					return !type.IsGenericTypeDefinition;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// �Ƿ����
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static bool IsGrouping(this Type type)
		{
			return IsGrouping(type.GetTypeInfo());
		}

		private static bool IsGrouping(TypeInfo type)
		{
			if (type.IsGenericType)
			{
				if (!(type.GetGenericTypeDefinition() == typeof(IGrouping<, >)))
				{
					return type.GetGenericTypeDefinition() == typeof(IAsyncGrouping<, >);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// չ��ö������
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static Type UnwrapEnumType(this Type type)
		{
			bool flag = type.IsNullableType();
			Type type2 = flag ? type.UnwrapNullableType() : type;
			if (!type2.GetTypeInfo().IsEnum)
			{
				return type;
			}
			Type underlyingType = Enum.GetUnderlyingType(type2);
			if (!flag)
			{
				return underlyingType;
			}
			return underlyingType.MakeNullable();
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static Type GetSequenceType(this Type type)
		{
			Type type2 = type.TryGetSequenceType();
			if (type2 == null)
			{
				throw new ArgumentException();
			}
			return type2;
		}

		/// <summary>
		/// ���Ի����������
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static Type TryGetSequenceType(this Type type)
		{
			return type.TryGetElementType(typeof(IEnumerable<>)) ?? type.TryGetElementType(typeof(IAsyncEnumerable<>));
		}

		/// <summary>
		/// ���Ի��Ԫ������
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="interfaceOrBaseType">�ӿڻ����</param>
		/// <returns></returns>
		public static Type TryGetElementType(this Type type, Type interfaceOrBaseType)
		{
			if (type.GetTypeInfo().IsGenericTypeDefinition)
			{
				return null;
			}
			IEnumerable<Type> genericTypeImplementations = type.GetGenericTypeImplementations(interfaceOrBaseType);
			Type type2 = null;
			foreach (Type item in genericTypeImplementations)
			{
				if (!(type2 == null))
				{
					type2 = null;
					break;
				}
				type2 = item;
			}
			return type2?.GetTypeInfo().GenericTypeArguments.FirstOrDefault();
		}

		/// <summary>
		/// ��ȡ��������ʵ��
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="interfaceOrBaseType">�ӿڻ����</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetGenericTypeImplementations(this Type type, Type interfaceOrBaseType)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			if (!typeInfo.IsGenericTypeDefinition)
			{
				IEnumerable<Type> enumerable = interfaceOrBaseType.GetTypeInfo().IsInterface ? typeInfo.ImplementedInterfaces : type.GetBaseTypes();
				foreach (Type item in enumerable)
				{
					if (item.GetTypeInfo().IsGenericType && item.GetGenericTypeDefinition() == interfaceOrBaseType)
					{
						yield return item;
					}
				}
				if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == interfaceOrBaseType)
				{
					yield return type;
				}
			}
		}

		/// <summary>
		/// ��û�������
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetBaseTypes(this Type type)
		{
			type = type.GetTypeInfo().BaseType;
			while (type != null)
			{
				yield return type;
				type = type.GetTypeInfo().BaseType;
			}
		}

		/// <summary>
		/// ��ȡ��νṹ�е�����
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetTypesInHierarchy(this Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.GetTypeInfo().BaseType;
			}
		}

		/// <summary>
		/// ��ȡ�����Ĺ��캯��
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="types">��������</param>
		/// <returns></returns>
		public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] types)
		{
			types = (types ?? Array.Empty<Type>());
			return type.GetTypeInfo().DeclaredConstructors.SingleOrDefault((ConstructorInfo c) => !c.IsStatic && (from p in c.GetParameters()
				select p.ParameterType).SequenceEqual(types));
		}

		/// <summary>
		/// ��ȡ��νṹ�е����� 
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="name">����</param>
		/// <returns></returns>
		public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(this Type type, string name)
		{
			do
			{
				TypeInfo typeInfo = type.GetTypeInfo();
				PropertyInfo declaredProperty = typeInfo.GetDeclaredProperty(name);
				if (declaredProperty != null && !(declaredProperty.GetMethod ?? declaredProperty.SetMethod).IsStatic)
				{
					yield return declaredProperty;
				}
				type = typeInfo.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		///  ��ȡ��νṹ�еĳ�Ա 
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static IEnumerable<MemberInfo> GetMembersInHierarchy(this Type type)
		{
			do
			{
				foreach (PropertyInfo item in from pi in RuntimeReflectionExtensions.GetRuntimeProperties(type)
					where !(pi.GetMethod ?? pi.SetMethod).IsStatic
					select pi)
				{
					yield return item;
				}
				foreach (FieldInfo item2 in from f in RuntimeReflectionExtensions.GetRuntimeFields(type)
					where !f.IsStatic
					select f)
				{
					yield return item2;
				}
				type = type.BaseType;
			}
			while (type != null);
		}

		/// <summary>
		///  ��ȡ��νṹ�еĳ�Ա
		/// </summary>
		/// <param name="type">����</param>
		/// <param name="name">����</param>
		/// <returns></returns>
		public static IEnumerable<MemberInfo> GetMembersInHierarchy(this Type type, string name)
		{
			return from m in type.GetMembersInHierarchy()
				where m.Name == name
				select m;
		}

		/// <summary>
		/// ���Ĭ��ֵ
		/// </summary>
		/// <param name="type">����</param>
		/// <returns></returns>
		public static object GetDefaultValue(this Type type)
		{
			if (!type.GetTypeInfo().IsValueType)
			{
				return null;
			}
			if (!_commonTypeDictionary.TryGetValue(type, out object value))
			{
				return Activator.CreateInstance(type);
			}
			return value;
		}

		/// <summary>
		/// ��ȡ�ɹ�������
		/// </summary>
		/// <param name="assembly">����</param>
		/// <returns></returns>
		public static IEnumerable<TypeInfo> GetConstructibleTypes(this Assembly assembly)
		{
			return from t in assembly.GetLoadableDefinedTypes()
				where !t.IsAbstract && !t.IsGenericTypeDefinition
				select t;
		}

		/// <summary>
		/// ��ȡ�ɼ��صĶ�������
		/// </summary>
		/// <param name="assembly">����</param>
		/// <returns></returns>
		public static IEnumerable<TypeInfo> GetLoadableDefinedTypes(this Assembly assembly)
		{
			try
			{
				return assembly.DefinedTypes;
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types.Where((Type t) => t != null).Select(IntrospectionExtensions.GetTypeInfo);
			}
		}
	}
}
