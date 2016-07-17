///////////////////////////////////////////////////////////////////////////////////
///    NHibernate Class Generator                                               ///
///    Copyright(C) 2016 Lucas Teske                                            ///
///                                                                             ///
///    This program is free software: you can redistribute it and/or modify     ///
///    it under the terms of the GNU General Public License as published by     ///
///    the Free Software Foundation, either version 3 of the License, or        ///
///    any later version.                                                       ///
///                                                                             ///
///    This program is distributed in the hope that it will be useful,          ///
///    but WITHOUT ANY WARRANTY; without even the implied warranty of           ///
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the              ///
///    GNU General Public License for more details.                             ///
///                                                                             ///
///    You should have received a copy of the GNU General Public license        ///
///    along with this program.If not, see<http://www.gnu.org/licenses/>.       ///
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;

namespace NHClassGen {
  public class Column {
    #region Constants
    private static readonly Dictionary<string, Type> dbTypes = new Dictionary<string, System.Type>() {
      {"bigint", typeof(Int64)},
      {"binary", typeof(Byte[])},
      {"bit", typeof(Boolean)},
      {"char", typeof(String)},
      {"date", typeof(DateTime)},
      {"datetime", typeof(DateTime)},
      {"datetimeoffset", typeof(DateTimeOffset)},
      {"decimal", typeof(Decimal)},
      {"float", typeof(Double)},
      {"image", typeof(Byte[])},
      {"int", typeof(Int32)},
      {"money", typeof(Decimal)},
      {"nchar", typeof(String)},
      {"ntext", typeof(String)},
      {"numeric", typeof(Decimal)},
      {"nvarchar", typeof(String)},
      {"real", typeof(Single)},
      {"smalldatetime", typeof(DateTime)},
      {"smallint", typeof(Int16)},
      {"text", typeof(String)},
      {"time", typeof(TimeSpan)},
      {"timestamp", typeof(Byte[])},
      {"tinyint", typeof(Byte)},
      {"varbinary", typeof(Byte[])},
      {"varchar", typeof(String)}
    };
    #endregion
    #region Private Fields
    private int _idx;
    private string _name;
    private Type _type;
    private byte _numericPrecision;
    private short _numericPrecisionRadix;
    private int _numericScale;
    private bool _isNullable;
    private int _characterMaximumLength;
    private bool _isId;
    private bool _isAutoIncrement;
    #endregion
    #region Properties
    public int CharacterMaximumLength {
      get { return _characterMaximumLength; }
      set { _characterMaximumLength = value; }
    }

    public int Index {
      get { return _idx; }
      set { _idx = value; }
    }

    public string Name {
      get { return _name; }
      set { _name = value; }
    }

    public Type Type {
      get { return _type; }
      set { _type = value; }
    }

    public byte NumericPrecision {
      get { return _numericPrecision; }
      set { _numericPrecision = value; }
    }

    public short NumericPrecisionRadix {
      get { return _numericPrecisionRadix; }
      set { _numericPrecisionRadix = value; }
    }

    public int NumericScale {
      get { return _numericScale; }
      set { _numericScale = value; }
    }

    public bool IsNullable {
      get { return _isNullable; }
      set { _isNullable = value; }
    }

    public bool IsId {
      get { return _isId; }
      set { _isId = value; }
    }

    public bool IsAutoIncrement {
      get { return _isAutoIncrement; }
      set { _isAutoIncrement = value; }
    }
    #endregion
    #region Constructors
    public Column(DataRow d) {
      this.Index = d["ORDINAL_POSITION"].GetType() != typeof(DBNull) ? (int)d["ORDINAL_POSITION"] : 0;
      this.Name = d["COLUMN_NAME"].GetType() != typeof(DBNull) ? (string)d["COLUMN_NAME"] : null; // Just Sanity Checking
      string t = d["DATA_TYPE"].GetType() != typeof(DBNull) ? (string)d["DATA_TYPE"] : null;
      if (t != null) {
        if (dbTypes.ContainsKey(t.ToLower())) {
          this.Type = dbTypes[t.ToLower()];
        } else {
          throw new UnknownTypeException(t);
        }
      } else {
        throw new UnknownTypeException("Null Type");
      }
      this.NumericPrecision = d["NUMERIC_PRECISION"].GetType() != typeof(DBNull) ? (byte)d["NUMERIC_PRECISION"] : (byte)0;
      this.NumericPrecisionRadix = d["NUMERIC_PRECISION_RADIX"].GetType() != typeof(DBNull) ? (short)d["NUMERIC_PRECISION_RADIX"] : (short)0;
      this.NumericScale = d["NUMERIC_SCALE"].GetType() != typeof(DBNull) ? (int)d["NUMERIC_SCALE"] : 0;
      this.IsNullable = d["IS_NULLABLE"].GetType() != typeof(DBNull) ? !((string)d["IS_NULLABLE"]).Equals("NO") : false;
      this.CharacterMaximumLength = d["CHARACTER_MAXIMUM_LENGTH"].GetType() != typeof(DBNull) ? (int)d["CHARACTER_MAXIMUM_LENGTH"] : 0;
      this.IsAutoIncrement = false;
      this.IsId = false;
    }
    #endregion
  }

  public class UnknownTypeException : Exception {
    public UnknownTypeException(string type) : base(string.Format("Unknown Column Type {0}", type)) { }
  }
}
