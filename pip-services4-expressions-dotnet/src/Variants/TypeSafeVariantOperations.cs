using System;

namespace PipServices4.Expressions.Variants
{
    /// <summary>
    /// Implements a strongly typed (type safe) variant operations manager object.
    /// </summary>
    public class TypeSafeVariantOperations : AbstractVariantOperations
    {
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

            switch (value.Type)
            {
                case VariantType.Integer:
                    return ConvertFromInteger(value, newType);
                case VariantType.Long:
                    return ConvertFromLong(value, newType);
                case VariantType.Float:
                    return ConvertFromFloat(value, newType);
                case VariantType.Double:
                    break;
                case VariantType.String:
                    break;
                case VariantType.Boolean:
                    break;
                case VariantType.Object:
                    return value;
                case VariantType.Array:
                    return ConvertFromArray(value, newType);
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
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
                case VariantType.Float:
                    result.AsFloat = value.AsLong;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value.AsLong;
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
                case VariantType.Double:
                    result.AsDouble = value.AsFloat;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

        private Variant ConvertFromArray(Variant value, VariantType newType)
        {
            var result = new Variant();
            switch (newType)
            {
                case VariantType.Null:
                    result.AsArray = null;
                    return result;
            }
            throw new InvalidCastException(
                "Variant convertion from " + TypeToString(value.Type)
                + " to " + TypeToString(newType) + " is not supported.");
        }

    }
}
