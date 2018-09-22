using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Driver;

namespace Assignment5 {

	public abstract class Filter<P,T> {
		protected MemberInfo _field;
		protected System.Type _fieldType;
		protected System.Type _objectType;
		protected string _fieldName;
		protected T _value;

		protected Filter (string fieldName, T value) {
			_objectType = typeof(P);
			_fieldType = typeof(T);
			MemberInfo info = _objectType.GetField(fieldName) as MemberInfo ??
			                  _objectType.GetProperty(fieldName) as MemberInfo;
			_fieldName = fieldName;
			_value = value;
		}

		protected T GetFieldValue (P obj) {
			if (_field.MemberType == MemberTypes.Field) {
				return (T)((FieldInfo)_field).GetValue(obj);
			} else if (_field.MemberType == MemberTypes.Property) {
				return (T)((PropertyInfo)_field).GetValue(obj);
			}

			return default(T);
		}

		public abstract bool Satisfy(P obj);

		public abstract FilterDefinition<P> GetMongoDbFilter();
	}

	public class EqFilter<To, Tv> : Filter<To, Tv> {

		public EqFilter(string fieldName, Tv value) : base(fieldName, value) {

		}

		public override bool Satisfy(To obj) {
			return EqualityComparer<Tv>.Default.Equals(GetFieldValue(obj), _value);
		}

		public override FilterDefinition<To> GetMongoDbFilter() {
			return Builders<To>.Filter.Eq(_fieldName, _value);
		}
	}

	public class MoreThanFilter<To, Tv> : Filter<To, Tv> {

		public MoreThanFilter (string fieldName, Tv value) : base(fieldName, value) {

		}

		public override bool Satisfy (To obj) {
			dynamic x = GetFieldValue(obj);
			dynamic y = _value;
			return x > y;
		}

		public override FilterDefinition<To> GetMongoDbFilter () {
			return Builders<To>.Filter.Gt(_fieldName, _value);
		}
	}

	public class LessThanFilter<To, Tv> : Filter<To, Tv> {

		public LessThanFilter (string fieldName, Tv value) : base(fieldName, value) {

		}

		public override bool Satisfy (To obj) {
			dynamic x = GetFieldValue(obj);
			dynamic y = _value;
			return x < y;
		}

		public override FilterDefinition<To> GetMongoDbFilter () {
			return Builders<To>.Filter.Lt(_fieldName, _value);
		}
	}
}
