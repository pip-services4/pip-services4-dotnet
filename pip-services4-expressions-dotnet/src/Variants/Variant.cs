using System;

namespace PipServices4.Expressions.Variants
{
    /// <summary>
    /// Defines container for variant values.
    /// </summary>
    public class Variant
    {
        private VariantType _type;
        private object _value;

        public static readonly Variant Empty = new Variant();

        /// <summary>
        /// Constructs an empty variant
        /// </summary>
        public Variant()
        {
            _type = VariantType.Null;
            _value = null;
        }

        /// <summary>
        /// Constructs this class and assignes object value.
        /// </summary>
        /// <param name="value">a value to be assigned to this variant.</param>
        public Variant(object value)
        {
            _type = VariantType.Null;
            _value = null;
            AsObject = value;
        }

        /// <summary>
        /// Defines a type of this variant value.
        /// </summary>
        public VariantType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Provides access to variant value as integer
        /// </summary>
        public int AsInteger
        {
            get { return (int)_value; }
            set
            {
                _type = VariantType.Integer;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as long
        /// </summary>
        public long AsLong
        {
            get { return (long)_value; }
            set
            {
                _type = VariantType.Long;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as boolean
        /// </summary>
        public bool AsBoolean
        {
            get { return (bool)_value; }
            set
            {
                _type = VariantType.Boolean;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as float
        /// </summary>
        public float AsFloat
        {
            get { return (float)_value; }
            set
            {
                _type = VariantType.Float;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as double
        /// </summary>
        public double AsDouble
        {
            get { return (double)_value; }
            set
            {
                _type = VariantType.Double;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as string
        /// </summary>
        public string AsString
        {
            get { return (string)_value; }
            set
            {
                _type = VariantType.String;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as character
        /// </summary>
        public DateTime AsDateTime
        {
            get { return (DateTime)_value; }
            set
            {
                _type = VariantType.DateTime;
                _value = value;
            }
        }

        public TimeSpan AsTimeSpan
        {
            get { return (TimeSpan)_value; }
            set
            {
                _type = VariantType.TimeSpan;
                _value = value;
            }
        }

        /// <summary>
        /// Provides access to variant value as object
        /// </summary>
        public object AsObject
        {
            get { return _value; }
            set
            {
                _value = value;

                if (value == null)
                {
                    _type = VariantType.Null;
                }
                else if (value is System.Int32)
                {
                    _type = VariantType.Integer;
                }
                else if (value is System.Int64)
                {
                    _type = VariantType.Long;
                }
                else if (value is System.Single)
                {
                    _type = VariantType.Float;
                }
                else if (value is System.Double)
                {
                    _type = VariantType.Double;
                }
                else if (value is System.Boolean)
                {
                    _type = VariantType.Boolean;
                }
                else if (value is System.DateTime)
                {
                    _type = VariantType.DateTime;
                }
                else if (value is System.TimeSpan)
                {
                    _type = VariantType.TimeSpan;
                }
                else if (value is System.String)
                {
                    _type = VariantType.String;
                }
                else if (value is Variant[])
                {
                    _type = VariantType.Array;
                }
                else if (value is Variant)
                {
                    var temp = value as Variant;
                    _type = temp._type;
                    _value = temp._value;
                }
                else
                {
                    _type = VariantType.Object;
                }
            }
        }

        /// <summary>
        /// Provides access to variant value as variant array
        /// </summary>
        public Variant[] AsArray
        {
            get { return (Variant[])_value; }
            set
            {
                _type = VariantType.Array;
                if (value != null)
                {
                    _value = new Variant[value.Length];
                    Array.Copy(value, 0, (object[])_value, 0, value.Length);
                }
                else
                {
                    _value = null;
                }
            }
        }

        /// <summary>
        /// Provides access to array length
        /// </summary>
        public int Length
        {
            get
            {
                if (_value != null && _type == VariantType.Array)
                {
                    return ((Variant[])_value).Length;
                }
                return 0;
            }
            set
            {
                if (_type == VariantType.Array)
                {
                    Variant[] array = new Variant[value];
                    if (_value != null)
                    {
                        Array.Copy((object[])_value, 0, array, 0, ((object[])_value).Length);
                    }
                    _value = array;
                }
                else
                {
                    throw new NotSupportedException("Cannot set array length for non-array data type.");
                }
            }
        }

        /// <summary>
        /// Gets an array element by its index.
        /// </summary>
        /// <param name="index">an element index</param>
        /// <returns>a requested array element</returns>
        public Variant GetByIndex(int index)
        {
            if (_type == VariantType.Array)
            {
                if (_value != null && ((object[])_value).Length > index)
                {
                    return ((Variant[])_value)[index];
                }
                else
                {
                    throw new IndexOutOfRangeException("Requested element of array is not accessible.");
                }
            }
            else
            {
                throw new NotSupportedException("Cannot access array element for none-array data type.");
            }
        }

        /// <summary>
        /// Sets an array element by its index.
        /// </summary>
        /// <param name="index">an element index</param>
        /// <param name="element">an element value</param>
        public void SetByIndex(int index, Variant element)
        {
            if (_type == VariantType.Array)
            {
                if (_value != null && ((Variant[])_value).Length > index)
                {
                    ((Variant[])_value)[index] = (Variant)element;
                }
                else
                {
                    throw new IndexOutOfRangeException("Requested element of array is not accessible.");
                }
            }
            else
            {
                throw new NotSupportedException("Cannot access array element for none-array data type.");
            }
        }

        /// <summary>
        /// Checks is this variant value Null.
        /// </summary>
        /// <returns> <code>true</code> if this variant value is Null.
        /// </returns>
        public bool IsNull()
        {
            return _type == VariantType.Null;
        }

        /// <summary>
        /// Checks is this variant value empty.
        /// </summary>
        /// <returns> <code>true</code< is this variant value is empty.
        /// </returns>
        public bool IsEmpty()
        {
            return _value == null;
        }

        /// <summary>
        /// Assignes a new variant value to this object.
        /// </summary>
        /// <param name="value">A new variant value to be assigned.</param>
        public void Assign(Variant value)
        {
            if (value != null)
            {
                _type = value._type;
                _value = value._value;
            }
            else
            {
                _type = VariantType.Null;
                _value = null;
            }
        }

        /// <summary>
        /// Clears this object and assignes a VariantType.Null type.
        /// </summary>
        public void Clear()
        {
            _type = VariantType.Null;
            _value = null;
        }

        /// <summary>
        /// Returns a string value for this object.
        /// </summary>
        /// <returns> a string value for this object.</returns>
        public override string ToString()
        {
            return _value == null ? "null" : _value.ToString();
        }

        /// <summary>
        /// Compares this object to the specified one.
        /// </summary>
        /// <param name="obj">An object to be compared.</param>
        /// <returns><code>true</code> if objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Variant)
            {
                Variant variantObj = (Variant)obj;
                if (AsObject == null || variantObj.AsObject == null)
                {
                    return AsObject == variantObj.AsObject;
                }
                return (Type == variantObj.Type) && AsObject.Equals(variantObj.AsObject);
            }
            return false;
        }

        /// <summary>
        /// Generates a unique hash value for this object.
        /// </summary>
        /// <returns>A generated hash value (code).</returns>
        public override int GetHashCode()
        {
            return AsObject != null ? AsObject.GetHashCode() : 0;
        }

        /// <summary>
        /// Cloning the variant value
        /// </summary>
        /// <returns>The cloned value of this variant</returns>
        public Variant Clone() {
            var result = new Variant();
            result.Assign(this);
            return result;
        }

        /// <summary>
        /// Creates a new variant from Integer value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromInteger(int value)
        {
            var result = new Variant();
            result.AsInteger = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from Long value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromLong(long value)
        {
            var result = new Variant();
            result.AsLong = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from Boolean value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromBoolean(bool value)
        {
            var result = new Variant();
            result.AsBoolean = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from Float value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromFloat(float value)
        {
            var result = new Variant();
            result.AsFloat = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from Double value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromDouble(double value)
        {
            var result = new Variant();
            result.AsDouble = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from String value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromString(string value)
        {
            var result = new Variant();
            result.AsString = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from DateTime value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromDateTime(DateTime value)
        {
            var result = new Variant();
            result.AsDateTime = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from TimeSpan value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromTimeSpan(TimeSpan value)
        {
            var result = new Variant();
            result.AsTimeSpan = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from Object value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromObject(object value)
        {
            var result = new Variant();
            result.AsObject = value;
            return result;
        }

        /// <summary>
        /// Creates a new variant from Array value.
        /// </summary>
        /// <param name="value">a variant value.</param>
        /// <returns>A created variant object</returns>
        public static Variant FromArray(Variant[] value)
        {
            var result = new Variant();
            result.AsArray = value;
            return result;
        }

    }

}
