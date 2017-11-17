﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Parquet.Data
{
   public class DictionarySchemaElement : SchemaElement
   {
      internal SchemaElement Key { get; private set; }

      internal SchemaElement Value { get; private set; }

      public DataType KeyType => Key.DataType;

      public DataType ValueType => Value.DataType;

      //todo: add overload for CLR generics

      public DictionarySchemaElement(string name, DataType keyDataType, DataType valueDataType)
         : base(name, DataType.Dictionary)
      {
         Key = new SchemaElement("key", keyDataType, false, true);
         Value = new SchemaElement("value", valueDataType, true, true);
      }

      internal DictionarySchemaElement(string name)
         : base(name, DataType.Dictionary)
      {
      }

      internal override void Assign(SchemaElement se)
      {
         if(Key == null)
         {
            Key = se;
         }
         else if(Value == null)
         {
            Value = se;
         }
         else
         {
            throw new InvalidOperationException($"'{Name}' already has key and value assigned");
         }
      }

      internal IDictionary CreateCellValue(IDictionary<string, IList> pathToValues, int index)
      {
         IList keys = (IList)(pathToValues[Key.Path][index]);
         IList values = (IList)(pathToValues[Value.Path][index]);

         Type gt = typeof(Dictionary<,>);
         Type masterType = gt.MakeGenericType(Key.ClrType, Value.ClrType);
         IDictionary result = (IDictionary)Activator.CreateInstance(masterType);

         for (int i = 0; i < keys.Count; i++)
         {
            result.Add(keys[i], values[i]);
         }

         return result;
      }
   }
}