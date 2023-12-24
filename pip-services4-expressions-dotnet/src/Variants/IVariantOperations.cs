namespace PipServices4.Expressions.Variants
{
    /// <summary>
    /// Defines an interface for variant operations manager.
    /// </summary>
    public interface IVariantOperations
    {
        /// <summary>
        /// Converts variant to specified type
        /// </summary>
        /// <param name="value">A variant value to be converted.</param>
        /// <param name="newType">A type of object to be returned.</param>
        /// <returns> A converted Variant value.</returns>
        Variant Convert(Variant value, VariantType newType);

        /// <summary>
        /// Performs '+' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Add(Variant value1, Variant value2);

        /// <summary>
        /// Performs '-' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Sub(Variant value1, Variant value2);

        /// <summary>
        /// Performs '*' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Mul(Variant value1, Variant value2);

        /// <summary>
        /// Performs '/' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Div(Variant value1, Variant value2);

        /// <summary>
        /// Performs '%' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Mod(Variant value1, Variant value2);

        /// <summary>
        /// Performs '^' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Pow(Variant value1, Variant value2);

        /// <summary>
        /// Performs AND operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant And(Variant value1, Variant value2);

        /// <summary>
        /// Performs OR operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Or(Variant value1, Variant value2);

        /// <summary>
        /// Performs XOR operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Xor(Variant value1, Variant value2);

        /// <summary>
        /// Performs << operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Lsh(Variant value1, Variant value2);

        /// <summary>
        /// Performs >> operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Rsh(Variant value1, Variant value2);

        /// <summary>
        /// Performs NOT operation for a variant.
        /// </summary>
        /// <param name="value">The operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Not(Variant value);

        /// <summary>
        /// Performs unary '-' operation for a variant.
        /// </summary>
        /// <param name="value">The operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Negative(Variant value);

        /// <summary>
        /// Performs '=' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Equal(Variant value1, Variant value2);

        /// <summary>
        /// Performs '<>' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant NotEqual(Variant value1, Variant value2);

        /// <summary>
        /// Performs '>' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant More(Variant value1, Variant value2);

        /// <summary>
        /// Performs '<' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Less(Variant value1, Variant value2);

        /// <summary>
        /// Performs '>=' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant MoreEqual(Variant value1, Variant value2);

        /// <summary>
        /// Performs '<=' operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant LessEqual(Variant value1, Variant value2);

        /// <summary>
        /// Performs IN operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant In(Variant value1, Variant value2);

        /// <summary>
        /// Performs [] operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant GetElement(Variant value1, Variant value2);

        /// <summary>
        /// Performs LIKE operation for two variants.
        /// </summary>
        /// <param name="value1">The first operand for this operation.</param>
        /// <param name="value2">The second operand for this operation.</param>
        /// <returns>A result variant object.</returns>
        Variant Like(Variant value1, Variant value2);
    }

}
