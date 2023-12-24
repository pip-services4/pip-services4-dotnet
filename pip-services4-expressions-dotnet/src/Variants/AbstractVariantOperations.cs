using System;

namespace PipServices4.Expressions.Variants
{
    /// <summary>
    /// Implements an abstractd variant operations manager object.
    /// </summary>
    public abstract class AbstractVariantOperations : IVariantOperations
    {

        /// <summary>
        /// Convert variant type to string representation
        /// </summary>
        /// <param name="value">a variant type to be converted.</param>
        /// <returns>a string representation of the type.</returns>
        protected string TypeToString(VariantType value)
        { 
            switch (value)
            {
                case VariantType.Null:
                    return "Null";
                case VariantType.Integer:
                    return "Integer";
                case VariantType.Long:
                    return "Long";
                case VariantType.Float:
                    return "Float";
                case VariantType.Double:
                    return "Double";
                case VariantType.String:
                    return "String";
                case VariantType.Boolean:
                    return "Boolean";
                case VariantType.DateTime:
                    return "DateTime";
                case VariantType.TimeSpan:
                    return "TimeSpan";
                case VariantType.Object:
                    return "Object";
                case VariantType.Array:
                    return "Array";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Converts variant to specified type
        /// </summary>
        /// <param name="value">A variant value to be converted.</param>
        /// <param name="newType">A type of object to be returned.</param>
        /// <returns> A converted Variant value.</returns>
        public abstract Variant Convert(Variant value, VariantType newType);

        /// <summary>
        /// Performs '+' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Add(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger + value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong + value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value1.AsFloat + value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value1.AsDouble + value2.AsDouble;
                    return result;
                case VariantType.TimeSpan:
                    result.AsTimeSpan = value1.AsTimeSpan.Add(value2.AsTimeSpan);
                    return result;
                case VariantType.String:
                    result.AsString = value1.AsString + value2.AsString;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '+' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '-' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Sub(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger - value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong - value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value1.AsFloat - value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value1.AsDouble - value2.AsDouble;
                    return result;
                case VariantType.TimeSpan:
                    result.AsTimeSpan = value1.AsTimeSpan.Subtract(value2.AsTimeSpan);
                    return result;
                case VariantType.DateTime:
                    result.AsTimeSpan = value1.AsDateTime.Subtract(value2.AsDateTime);
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '-' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '*' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Mul(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger * value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong * value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value1.AsFloat * value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value1.AsDouble * value2.AsDouble;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '*' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '/' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Div(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger / value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong / value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsFloat = value1.AsFloat / value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsDouble = value1.AsDouble / value2.AsDouble;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '/' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '%' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Mod(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger % value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong % value2.AsLong;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '%' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '^' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Pow(Variant value1, Variant value2)
        {
            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return Variant.Empty;
            }

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                case VariantType.Long:
                case VariantType.Float:
                case VariantType.Double:
                    // Converts second operant to the type of the first operand.
                    value1 = Convert(value1, VariantType.Double);
                    value2 = Convert(value2, VariantType.Double);
                    return new Variant(System.Math.Pow(value1.AsDouble, value2.AsDouble));
            }
            throw new NotSupportedException(string.Format("Operation '^' is not supported for type {0}", value1.Type));
        }

        /// <summary>
        /// Performs AND operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant And(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger & value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong & value2.AsLong;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value1.AsBoolean && value2.AsBoolean;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation AND is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs OR operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Or(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger | value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong | value2.AsLong;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value1.AsBoolean || value2.AsBoolean;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation OR is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs XOR operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Xor(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger ^ value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong ^ value2.AsLong;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = (value1.AsBoolean && !value2.AsBoolean) || (!value1.AsBoolean && value2.AsBoolean);
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation XOR is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs << operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Lsh(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, VariantType.Integer);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger << value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong << value2.AsInteger;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '<<' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs >> operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Rsh(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, VariantType.Integer);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = value1.AsInteger >> value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = value1.AsLong >> value2.AsInteger;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '>>' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs NOT operation for a variant.
        /// </summary>
        /// <param name="value">The operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Not(Variant value)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value.Type == VariantType.Null)
            {
                return result;
            }

            // Performs operation.
            switch (value.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = ~value.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = ~value.AsLong;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = !value.AsBoolean;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation NOT is not supported for type {0}", TypeToString(value.Type)));
        }

        /// <summary>
        /// Performs unary '-' operation for a variant.
        /// </summary>
        /// <param name="value">The operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Negative(Variant value)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value.Type == VariantType.Null)
            {
                return result;
            }

            // Performs operation.
            switch (value.Type)
            {
                case VariantType.Integer:
                    result.AsInteger = -value.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsLong = -value.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsFloat = -value.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsDouble = -value.AsDouble;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation unary '-' is not supported for type {0}", TypeToString(value.Type)));
        }

        /// <summary>
        /// Performs '=' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Equal(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null && value2.Type == VariantType.Null)
            {
                result.AsBoolean = true;
                return result;
            }
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsBoolean = value1.AsInteger == value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsBoolean = value1.AsLong == value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsBoolean = value1.AsFloat == value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsBoolean = value1.AsDouble == value2.AsDouble;
                    return result;
                case VariantType.String:
                    result.AsBoolean = value1.AsString == value2.AsString;
                    return result;
                case VariantType.TimeSpan:
                    result.AsBoolean = value1.AsTimeSpan == value2.AsTimeSpan;
                    return result;
                case VariantType.DateTime:
                    result.AsBoolean = value1.AsDateTime == value2.AsDateTime;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value1.AsBoolean == value2.AsBoolean;
                    return result;
                case VariantType.Object:
                    result.AsBoolean = value1.AsObject == value2.AsObject;
                    return result;
                //case VariantType.Array:
                //    return new Variant(Array.Equals(value1.AsObject, value2.AsObject));
            }
            throw new NotSupportedException(String.Format("Operation '=' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '<>' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant NotEqual(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null && value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = true;
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsBoolean = value1.AsInteger != value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsBoolean = value1.AsLong != value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsBoolean = value1.AsFloat != value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsBoolean = value1.AsDouble != value2.AsDouble;
                    return result;
                case VariantType.String:
                    result.AsBoolean = value1.AsString != value2.AsString;
                    return result;
                case VariantType.TimeSpan:
                    result.AsBoolean = value1.AsTimeSpan != value2.AsTimeSpan;
                    return result;
                case VariantType.DateTime:
                    result.AsBoolean = value1.AsDateTime != value2.AsDateTime;
                    return result;
                case VariantType.Boolean:
                    result.AsBoolean = value1.AsBoolean != value2.AsBoolean;
                    return result;
                case VariantType.Object:
                    result.AsBoolean = value1.AsObject != value2.AsObject;
                    return result;
                //case VariantType.Array:
                //    return new Variant(!Array.Equals(value1.AsObject, value2.AsObject));
            }
            throw new NotSupportedException(String.Format("Operation '<>' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '>' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant More(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsBoolean = value1.AsInteger > value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsBoolean = value1.AsLong > value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsBoolean = value1.AsFloat > value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsBoolean = value1.AsDouble > value2.AsDouble;
                    return result;
                case VariantType.String:
                    result.AsBoolean = String.Compare(value1.AsString, value2.AsString) > 0;
                    return result;
                case VariantType.TimeSpan:
                    result.AsBoolean = value1.AsTimeSpan > value2.AsTimeSpan;
                    return result;
                case VariantType.DateTime:
                    result.AsBoolean = value1.AsDateTime > value2.AsDateTime;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '>' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '<' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Less(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsBoolean = value1.AsInteger < value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsBoolean = value1.AsLong < value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsBoolean = value1.AsFloat < value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsBoolean = value1.AsDouble < value2.AsDouble;
                    return result;
                case VariantType.String:
                    result.AsBoolean = String.Compare(value1.AsString, value2.AsString) < 0;
                    return result;
                case VariantType.TimeSpan:
                    result.AsBoolean = value1.AsTimeSpan < value2.AsTimeSpan;
                    return result;
                case VariantType.DateTime:
                    result.AsBoolean = value1.AsDateTime < value2.AsDateTime;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '<' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '>=' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant MoreEqual(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsBoolean = value1.AsInteger >= value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsBoolean = value1.AsLong >= value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsBoolean = value1.AsFloat >= value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsBoolean = value1.AsDouble >= value2.AsDouble;
                    return result;
                case VariantType.String:
                    result.AsBoolean = String.Compare(value1.AsString, value2.AsString) >= 0;
                    return result;
                case VariantType.TimeSpan:
                    result.AsBoolean = value1.AsTimeSpan >= value2.AsTimeSpan;
                    return result;
                case VariantType.DateTime:
                    result.AsBoolean = value1.AsDateTime >= value2.AsDateTime;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '>=' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs '<=' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant LessEqual(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Converts second operant to the type of the first operand.
            value2 = Convert(value2, value1.Type);

            // Performs operation.
            switch (value1.Type)
            {
                case VariantType.Integer:
                    result.AsBoolean = value1.AsInteger <= value2.AsInteger;
                    return result;
                case VariantType.Long:
                    result.AsBoolean = value1.AsLong <= value2.AsLong;
                    return result;
                case VariantType.Float:
                    result.AsBoolean = value1.AsFloat <= value2.AsFloat;
                    return result;
                case VariantType.Double:
                    result.AsBoolean = value1.AsDouble <= value2.AsDouble;
                    return result;
                case VariantType.String:
                    result.AsBoolean = String.Compare(value1.AsString, value2.AsString) <= 0;
                    return result;
                case VariantType.TimeSpan:
                    result.AsBoolean = value1.AsTimeSpan <= value2.AsTimeSpan;
                    return result;
                case VariantType.DateTime:
                    result.AsBoolean = value1.AsDateTime <= value2.AsDateTime;
                    return result;
            }
            throw new NotSupportedException(String.Format("Operation '<=' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs IN operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant In(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Processes null arrays.
            if (value1.AsObject == null)
            {
                result.AsBoolean = false;
                return result;
            }

            if (value1.Type == VariantType.Array)
            {
                Variant[] array = value1.AsArray;
                foreach (Variant element in array)
                {
                    var eq = this.Equal(element, value2);
                    if (eq.Type == VariantType.Boolean && eq.AsBoolean)
                    {
                        result.AsBoolean = true;
                        return result;
                    }
                }
                result.AsBoolean = false;
                return result;
            }
            return Equal(value1, value2);
        }

        /// <summary>
        /// Performs [] operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant GetElement(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                return result;
            }

            value2 = Convert(value2, VariantType.Integer);

            if (value1.Type == VariantType.Array)
            {
                return (Variant)value1.GetByIndex(value2.AsInteger);
            }
            else if (value1.Type == VariantType.String)
            {
                result.AsString = value1.AsString[value2.AsInteger].ToString();
                return result;
            }
            throw new NotSupportedException(String.Format("Operation '[]' is not supported for type {0}", TypeToString(value1.Type)));
        }

        /// <summary>
        /// Performs LIKE operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        public virtual Variant Like(Variant value1, Variant value2)
        {
            var result = new Variant();

            // Processes VariantType.Null values.
            if (value1.Type == VariantType.Null || value2.Type == VariantType.Null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Processes null arrays.
            if (value1.AsObject == null)
            {
                result.AsBoolean = false;
                return result;
            }

            // Converts second operant to the string.
            value2 = Convert(value2, VariantType.String);

            if (value1.Type == VariantType.String)
            {
                result.AsBoolean = value1.AsString.Contains(value2.AsString);
                return result;
            }
            throw new NotSupportedException(String.Format("Operation '<=' is not supported for type {0}", TypeToString(value1.Type)));
        }
    }
}
