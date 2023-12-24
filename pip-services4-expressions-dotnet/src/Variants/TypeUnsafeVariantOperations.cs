using System;

using PipServices4.Commons.Convert;

namespace PipServices4.Expressions.Variants
{
    /// <summary>
    /// Implements a type unsafe variant operations manager object.
    /// </summary>
    public class TypeUnsafeVariantOperations : AbstractVariantOperations
    {
        /// <summary>
        /// Constructs operations object
        /// </summary>
        /// <param name="rules">Rules for data conversion</param>
        public TypeUnsafeVariantOperations()
        {
        }

        /// <summary>
        /// Converts variant to specified type
        /// </summary>
        /// <param name="value">A variant value to be converted.</param>
        /// <param name="newType">A type of object to be returned.</param>
        /// <returns> A converted Variant value.</returns>
        public override Variant Convert(Variant value, VariantType newType)
        {
            if (newType == VariantType.Null)
            {
                var result = new Variant();
                return result;
            }
            if (newType == value.Type || newType == VariantType.Object)
            {
                return value;
            }
            if (newType == VariantType.String)
            {
                var result = new Variant();
                result.AsString = StringConverter.ToString(value.AsObject);
                return result;
            }

            switch (value.Type)
            {
                case VariantType.Null:
                    return ConvertFromNull(newType);
                case VariantType.Integer:
                    return ConvertFromInteger(value, newType);
                case VariantType.Long:
                    return ConvertFromLong(value, newType);
                case VariantType.Float:
                    return ConvertFromFloat(value, newType);
                case VariantType.Double:
                    return ConvertFromDouble(value, newType);
                case VariantType.DateTime:
                    return ConvertFromDateTime(value, newType);
                case VariantType.TimeSpan:
                    return ConvertFromTimeSpan(value, newType);
                case VariantType.String:
                    return ConvertFromString(value, newType);
                case VariantType.Boolean:
                    return ConvertFromBoolean(value, newType);
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromNull(VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = 0;
                    return result;
                case VariantType.Long:
                    result.AsLong = 0;
                    return result;
                case VariantType.Float:
                    result.AsFloat = 0;
                    return result;
                case VariantType.Double:
                    result.AsDouble = 0;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = false;
                    return result;
                case VariantType.DateTime:
                    result.AsDateTime = new DateTime();
                    return result;
                case VariantType.TimeSpan:
                    result.AsTimeSpan = new TimeSpan(0);
                    return result;
                case VariantType.String:
                    result.AsString = "null";
                    return result;
                case VariantType.Object:
                    result.AsObject = null;
                    return result;
                case VariantType.Array:
                    result.AsArray = null;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from Null "
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromInteger(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Long:
                    result.AsLong = value.AsInteger;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value.AsInteger;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value.AsInteger;
                    return result;
                case VariantType.DateTime:
                    result.AsDateTime = new DateTime(value.AsInteger);
                    return result;
                case VariantType.TimeSpan:
                    result.AsTimeSpan = new TimeSpan(value.AsInteger);
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value.AsInteger != 0;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromLong(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = (int)value.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value.AsLong;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value.AsLong;
                    return result;
                case VariantType.DateTime:
                    result.AsDateTime = new DateTime(value.AsLong);
                    return result;
                case VariantType.TimeSpan:
                    result.AsTimeSpan = new TimeSpan(value.AsLong);
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value.AsLong != 0;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromFloat(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = (int)Math.Truncate(value.AsFloat);
                    return result;
                case VariantType.Long:
                    result.AsLong = (long)Math.Truncate(value.AsFloat);
                    return result;
                case VariantType.Double:
                    result.AsDouble = value.AsFloat;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value.AsFloat != 0;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromDouble(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = (int)Math.Truncate(value.AsDouble);
                    return result;
                case VariantType.Long:
                    result.AsLong = (long)Math.Truncate(value.AsDouble);
                    return result;
                case VariantType.Float:
                    result.AsFloat = (float)value.AsDouble;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value.AsDouble != 0;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromString(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = IntegerConverter.ToInteger(value.AsString);
                    return result;
                case VariantType.Long:
                    result.AsLong = LongConverter.ToLong(value.AsString);
                    return result;
                case VariantType.Float:
                    result.AsFloat = FloatConverter.ToFloat(value.AsString);
                    return result;
                case VariantType.Double:
                    result.AsDouble = DoubleConverter.ToDouble(value.AsString);
                    return result;
                case VariantType.DateTime:
                    result.AsDateTime = DateTimeConverter.ToDateTime(value.AsString);
                    return result;
                case VariantType.TimeSpan:
                    result.AsTimeSpan = TimeSpanConverter.ToTimeSpan(value.AsString);
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = BooleanConverter.ToBoolean(value.AsString);
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromBoolean(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = value.AsBoolean ? 1 : 0;
                    return result;
                case VariantType.Long:
                    result.AsLong = value.AsBoolean ? 1 : 0;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value.AsBoolean ? 1 : 0;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value.AsBoolean ? 1 : 0;
                    return result;
                case VariantType.String:
                    result.AsString = value.AsBoolean ? "true" : "false";
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromDateTime(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = (int)value.AsDateTime.Ticks;
                    return result;
                case VariantType.Long:
                    result.AsLong = value.AsDateTime.Ticks;
                    return result;
                case VariantType.String:
                    result.AsString = StringConverter.ToString(value.AsDateTime);
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromTimeSpan(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Integer:
                    result.AsInteger = (int)Math.Truncate(value.AsTimeSpan.TotalMilliseconds);
                    return result;
                case VariantType.Long:
                    result.AsLong = (long)Math.Truncate(value.AsTimeSpan.TotalMilliseconds);
                    return result;
                case VariantType.String:
                    result.AsString = StringConverter.ToString(value.AsTimeSpan);
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

    }
}
