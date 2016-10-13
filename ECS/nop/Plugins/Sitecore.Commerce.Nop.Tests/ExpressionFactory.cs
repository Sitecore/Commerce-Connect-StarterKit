// -----------------------------------------------------------------
// <copyright file="ExpressionFactory.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ExpressionFactory class.
// </summary>
// -----------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -----------------------------------------------------------------
namespace Sitecore.Commerce.Nop.Tests
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;


  class ExpressionFactory<T>
  {
    private Type _type;
    private Expression _typeExpression;

    private ParameterExpression _source;
    private ParameterExpression _flags;
    private ParameterExpression _initializers;
    private ParameterExpression _target;

    private LabelTarget _return;

    public ExpressionFactory()
    {
      _type = typeof(T);
      _typeExpression = Expression.Constant(_type, typeof(Type));

      Initialize();
    }

    public Expression<Func<T, CloningFlags, IDictionary<Type, Func<object, object>>, T>> CloneExpression { get; private set; }

    internal Func<T, CloningFlags, IDictionary<Type, Func<object, object>>, T> GetCloneFunc()
    {
      return CloneExpression == null ? null : CloneExpression.Compile();
    }

    public void Initialize()
    {
      // generate simple return expression for primitive, knows immutable types and delegates
      if (_type.IsPrimitiveOrKnownImmutable() || typeof(Delegate).IsAssignableFrom(_type))
      {
        CloneExpression = GetPrimitiveTypeExpression();
      }
      else
      {
        CloneExpression = GetNonPrimitiveTypeExpression();
      }
    }

    private Expression<Func<T, CloningFlags, IDictionary<Type, Func<object, object>>, T>> GetPrimitiveTypeExpression()
    {
      return (s, fl, kt) => s;
    }

    private Expression<Func<T, CloningFlags, IDictionary<Type, Func<object, object>>, T>> GetNonPrimitiveTypeExpression()
    {
      _source = Expression.Parameter(_type, "source");
      _flags = Expression.Parameter(typeof(CloningFlags), "flags");
      _initializers = Expression.Parameter(typeof(IDictionary<Type, Func<object, object>>), "initializers");
      _target = Expression.Variable(_type, "target");

      // prepare return expression
      _return = Expression.Label(_type);

      var expressions = new List<Expression>();

      // array clone
      if (typeof(Array).IsAssignableFrom(_type))
      {
        InitializeArrayCloneExpressionParts(expressions);
      }
      // nullable<T> clone
      else if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        InitializeNullableCloneExpressionParts(expressions);
      }
      // KeyValuePair<TKey, TValue>
      else if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
      {
        InitializeKeyValuePairCloneExpressionParts(expressions);
      }
      // Tuple
      else if (_type.IsGenericType &&
          (_type.GetGenericTypeDefinition() == typeof(Tuple<>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,,>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,,,>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,,,,>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,,,,,>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,,,,,,>)
          || _type.GetGenericTypeDefinition() == typeof(Tuple<,,,,,,,>)))
      {
        InitializeTupleCloneExpressionParts(expressions);
      }
      // unknow type - just initialize and copy what's necessary
      else
      {
        InitializeComplexTypeExpressionParts(expressions);
      }

      // generate return statement
      expressions.Add(Expression.Label(_return, _target));

      var block = Expression.Block(
              new[] { _target },
              expressions.ToArray());

      return Expression.Lambda<Func<T, CloningFlags, IDictionary<Type, Func<object, object>>, T>>(
              block,
              new[] { _source, _flags, _initializers });
    }

    private void InitializeComplexTypeExpressionParts(List<Expression> expressions)
    {
      // generate if(source == null) return null;
      if (!_type.IsValueType)
        expressions.Add(GetIfNullReturnNullExpression());

      // generate initialzation expression
      expressions.Add(GetInitializationExpression());

      // generate expressions for fields and properties
      expressions.Add(Expression.IfThen(
          GetCloningFlagsExpression(CloningFlags.Fields),
          GetFieldsCloneExpression()
      ));
      expressions.Add(Expression.IfThen(
          GetCloningFlagsExpression(CloningFlags.Properties),
          GetPropertiesCloneExpression()
      ));

      // generate foreach(TItem item in source) { target.Add(item); }
      // whe T implements ICollection<TItem>
      var collectionType = _type.GetInterfaces()
                                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
      if (collectionType != null)
        expressions.Add(Expression.IfThen(
            GetCloningFlagsExpression(CloningFlags.CollectionItems),
            GetForeachAddExpression(collectionType)));
    }

    private void InitializeNullableCloneExpressionParts(List<Expression> expressions)
    {
      var structType = _type.GetGenericArguments().First();
      expressions.Add(Expression.IfThenElse(
          Expression.Equal(Expression.Property(_source, "HasValue"), Expression.Constant(false)),
          Expression.Assign(_target, Expression.Constant(null, _type)),
          Expression.Assign(_target, Expression.New(_type.GetConstructor(new[] { structType }), GetCloneMethodCall(structType, Expression.Property(_source, "Value"))))));
    }

    private void InitializeArrayCloneExpressionParts(List<Expression> expressions)
    {
      var itemType = _type.GetInterfaces()
                          .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                          .GetGenericArguments().First();
      var arrayLength = Expression.Property(_source, "Length");

      expressions.Add(Expression.Assign(_target, Expression.NewArrayBounds(itemType, arrayLength)));

      expressions.Add(GetForAssignExpression(itemType, arrayLength));
    }

    private void InitializeKeyValuePairCloneExpressionParts(List<Expression> expressions)
    {
      var constructor = _type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 2);
      var keyType = _type.GetGenericArguments()[0];
      var valueType = _type.GetGenericArguments()[1];

      expressions.Add(
          Expression.Assign(
              _target,
              Expression.New(
                  constructor,
                  GetCloneMethodCall(keyType, Expression.Property(_source, "Key")),
                  GetCloneMethodCall(valueType, Expression.Property(_source, "Value")))));
    }

    private void InitializeTupleCloneExpressionParts(List<Expression> expressions)
    {
      var genericTypes = _type.GetGenericArguments();
      var itemsCloneExpressions = new Expression[genericTypes.Length];

      // Can't loop to 8, because instead of Item8 the last one is called Rest
      var loopCount = Math.Min(genericTypes.Length, 7);
      for (int i = 0; i < loopCount; i++)
      {
        itemsCloneExpressions[i] = GetCloneMethodCall(
                                    genericTypes[i],
                                    Expression.Property(
                                        _source,
                                        "Item" + (i + 1).ToString(CultureInfo.InvariantCulture)));
      }

      // add Rest expression if it's necessary
      if (genericTypes.Length == 8)
        itemsCloneExpressions[7] = GetCloneMethodCall(
                                    genericTypes[7],
                                    Expression.Property(
                                        _source,
                                        "Rest"));

      var constructor = _type.GetConstructors()[0];

      expressions.Add(
          Expression.Assign(
              _target,
              Expression.New(
                  constructor,
                  itemsCloneExpressions
              )));
    }

    private Expression GetInitializationExpression()
    {
      // initializers.ContainsKey method call
      var containsKeyCall = Expression.Call(_initializers, "ContainsKey", null, _typeExpression);

      // initializer delegate invoke
      var dictIndex = Expression.Property(_initializers, "Item", _typeExpression);
      var funcInvokeCall = Expression.Call(dictIndex, "Invoke", null, Expression.Convert(_source, typeof(object)));
      var initializerCall = Expression.Convert(funcInvokeCall, _type);

      var constructor = _type.GetConstructor(new Type[0]);

      return Expression.IfThenElse(
          containsKeyCall,
          Expression.Assign(_target, initializerCall),
          (_type.IsAbstract || _type.IsInterface || (!_type.IsValueType && constructor == null)) ?
              GetThrowInvalidOperationExceptionExpression(_type) :
              Expression.Assign(
                  _target,
                  _type.IsValueType ? (Expression)_source : Expression.New(_type)
              )
      );
    }

    private Expression GetIfNullReturnNullExpression()
    {
      return Expression.IfThen(
              Expression.Equal(_source, Expression.Constant(null, _type)),
              Expression.Return(_return, Expression.Constant(null, _type), _type)
          );
    }

    private Expression GetFieldsCloneExpression()
    {
      var fields = from f in _type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                   //                   where f.GetCustomAttributes(typeof(NonClonedAttribute), true).Length == 0
                   where !f.IsInitOnly
                   select new Member(f, f.FieldType);

      return GetMembersCloneExpression(fields.ToArray());
    }

    private Expression GetPropertiesCloneExpression()
    {
      // get all public properties with public setter and getter, which are not indexed properties
      var properties = from p in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       let setMethod = p.GetSetMethod(false)
                       let getMethod = p.GetGetMethod(false)
                       //                          where p.GetCustomAttributes(typeof(NonClonedAttribute), true).Length == 0
                       where setMethod != null && getMethod != null && !p.GetIndexParameters().Any()
                       select new Member(p, p.PropertyType);

      return GetMembersCloneExpression(properties.ToArray());
    }

    private Expression GetMembersCloneExpression(Member[] members)
    {
      if (!members.Any())
        return Expression.Empty();

      return Expression.Block(
          members.Select(m =>
              Expression.Assign(
                  Expression.MakeMemberAccess(_target, m.Info),
                  GetCloneMethodCall(m.Type, Expression.MakeMemberAccess(_source, m.Info))
                  )));
    }

    private Expression GetForeachAddExpression(Type collectionType)
    {
      var collection = Expression.Variable(collectionType);
      var itemType = collectionType.GetGenericArguments().First();
      var enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
      var enumeratorType = typeof(IEnumerator<>).MakeGenericType(itemType);
      var enumerator = Expression.Variable(enumeratorType);
      var getEnumeratorCall = Expression.Call(Expression.Convert(_source, enumerableType), "GetEnumerator", null);
      var assignToEnumerator = Expression.Assign(enumerator, Expression.Convert(getEnumeratorCall, enumeratorType));
      var assignToCollection = Expression.Assign(collection, Expression.Convert(_target, collectionType));
      var moveNextCall = Expression.Call(enumerator, typeof(IEnumerator).GetMethod("MoveNext"));
      var currentProperty = Expression.Property(enumerator, "Current");
      var breakLabel = Expression.Label();

      return Expression.Block(
          new[] { enumerator, collection },
          assignToEnumerator,
          assignToCollection,
          Expression.Loop(
              Expression.IfThenElse(
                  Expression.NotEqual(moveNextCall, Expression.Constant(false, typeof(bool))),
                  Expression.Call(collection, "Add", null,
                      GetCloneMethodCall(itemType, currentProperty)),
                  Expression.Break(breakLabel)
              ),
              breakLabel
          )
      );
    }

    private Expression GetForAssignExpression(Type itemType, MemberExpression arrayLength)
    {
      var counter = Expression.Variable(typeof(int));
      var breakLabel = Expression.Label();

      return Expression.Block(
          new[] { counter },
          Expression.Assign(counter, Expression.Constant(0)),
          Expression.Loop(
              Expression.IfThenElse(
                  Expression.LessThan(counter, arrayLength),
                  Expression.Block(
                      Expression.Assign(
                          Expression.ArrayAccess(_target, counter),
                          GetCloneMethodCall(itemType, Expression.ArrayAccess(_source, counter))
                      ),
                      Expression.AddAssign(counter, Expression.Constant(1))
                  ),
                  Expression.Break(breakLabel)
              ),
              breakLabel
          )
      );
    }

    private Expression GetCloningFlagsExpression(CloningFlags cloningFlags)
    {
      var flagExpression = Expression.Convert(Expression.Constant(cloningFlags, typeof(CloningFlags)), typeof(byte));
      return Expression.Equal(
          Expression.And(
              Expression.Convert(_flags, typeof(byte)),
              flagExpression
          ),
          flagExpression
      );
    }

    private Expression GetThrowInvalidOperationExceptionExpression(Type type)
    {
      var message = string.Format("You have to provide initialization expression for {0}.", type.FullName);

      return Expression.Throw(
          Expression.New(
              typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) }),
              Expression.Constant(message, typeof(string))
          )
      );
    }

    private Expression GetCloneMethodCall(Type type, Expression source)
    {
      return Expression.Call(typeof(CloneFactory), "GetClone", new[] { type }, source, _flags, _initializers);
    }
  }
}
