﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Diagnostics;

using Nuclex.Math.Generic;
using Nuclex.Support;

namespace Nuclex.Math {

  /// <summary>Specialized 3x3 matrix class like used for 3D orientations</summary>
  /// <typeparam name="ScalarType">Type that the matrix uses for its components</typeparam>
  /// <typeparam name="MathType">Math routines that are to be used</typeparam>
  public struct Matrix33<ScalarType, MathType>
    where MathType :
      IAbsoluteValueProvider<ScalarType>,
      IArithmeticOperationsProvider<ScalarType>,
      IComparisonProvider<ScalarType>,
      ILimitProvider<ScalarType>,
      ILogarithmicsProvider<ScalarType>,
      IScalingProvider<ScalarType>,
      ITrigonometricsProvider<ScalarType>,
      new() {

    /// <summary>A matrix whose elements have been initialized to 0</summary>
    public static Matrix33<ScalarType, MathType> Zero {
      [DebuggerStepThrough]
      get {
        return new Matrix33<ScalarType, MathType>(
          math.Zero, math.Zero, math.Zero,
          math.Zero, math.Zero, math.Zero,
          math.Zero, math.Zero, math.Zero
        );
      }
    }

    /// <summary>A matrix whose elements have been initialized to 1</summary>
    public static Matrix33<ScalarType, MathType> One {
      [DebuggerStepThrough]
      get {
        return new Matrix33<ScalarType, MathType>(
          math.One, math.One, math.One,
          math.One, math.One, math.One,
          math.One, math.One, math.One
        );
      }
    }

    /// <summary>The identity matrix, doing nothing</summary>
    public static Matrix33<ScalarType, MathType> Identity {
      [DebuggerStepThrough]
      get {
        return new Matrix33<ScalarType, MathType>(
          math.One, math.Zero, math.Zero,
          math.Zero, math.One, math.Zero,
          math.Zero, math.Zero, math.One
        );
      }
    }

    /// <summary>Constructs a matrix with the specified rotation on the X axis</summary>
    /// <param name="phi">Rotation angle of the matrix in radians</param>
    /// <returns>The rotated matrix</returns>
    public static Matrix33<ScalarType, MathType> BuildXRotation(double phi) {
      ScalarType posSin = math.Sin(phi);
      ScalarType posCos = math.Cos(phi);
      ScalarType negSin = math.Negate(posSin);

      return new Matrix33<ScalarType, MathType>(
        math.One, math.Zero, math.Zero,
        math.Zero, posCos, negSin,
        math.Zero, posSin, posCos
      );
    }

    /// <summary>Constructs a matrix with the specified rotation on the Y axis</summary>
    /// <param name="phi">Rotation angle of the matrix in radians</param>
    /// <returns>The rotated matrix</returns>
    public static Matrix33<ScalarType, MathType> BuildYRotation(double phi) {
      ScalarType posSin = math.Sin(phi);
      ScalarType posCos = math.Cos(phi);
      ScalarType negSin = math.Negate(posSin);

      return new Matrix33<ScalarType, MathType>(
        posCos, math.Zero, posSin,
        math.Zero, math.One, math.Zero,
        negSin, math.Zero, posCos
      );
    }

    /// <summary>Constructs a matrix with the specified rotation on the Z axis</summary>
    /// <param name="phi">Rotation angle of the matrix in radians</param>
    /// <returns>The rotated matrix</returns>
    public static Matrix33<ScalarType, MathType> BuildZRotation(double phi) {
      ScalarType posSin = math.Sin(phi);
      ScalarType posCos = math.Cos(phi);
      ScalarType negSin = math.Negate(posSin);

      return new Matrix33<ScalarType, MathType>(
        posCos, negSin, math.Zero,
        posSin, posCos, math.Zero,
        math.Zero, math.Zero, math.One
      );
    }

    /// <summary>Builds a rotation matrix from a quaternion</summary>
    /// <param name="quaternion">Quaternion from which the orientation is taken</param>
    /// <returns>A matrix with the same orientation as the quaternion</returns>
    public static Matrix33<ScalarType, MathType> FromQuaternion(
      Quaternion<ScalarType, MathType> quaternion
    ) {
      Number<ScalarType, MathType> tX = math.Scale(quaternion.X, 2.0);
      Number<ScalarType, MathType> tY = math.Scale(quaternion.Y, 2.0);
      Number<ScalarType, MathType> tZ = math.Scale(quaternion.Z, 2.0);

      Number<ScalarType, MathType> tWX = tX * quaternion.W;
      Number<ScalarType, MathType> tWY = tY * quaternion.W;
      Number<ScalarType, MathType> tWZ = tZ * quaternion.W;
      Number<ScalarType, MathType> tXX = tX * quaternion.X;
      Number<ScalarType, MathType> tXY = tY * quaternion.X;
      Number<ScalarType, MathType> tXZ = tZ * quaternion.X;
      Number<ScalarType, MathType> tYY = tY * quaternion.Y;
      Number<ScalarType, MathType> tYZ = tZ * quaternion.Y;
      Number<ScalarType, MathType> tZZ = tZ * quaternion.Z;

      return new Matrix33<ScalarType, MathType>(
        math.One - (tYY + tZZ), tXY - tWZ, tXZ + tWY,
        tXY + tWZ, math.One - (tXX + tZZ), tYZ - tWX,
        tXZ - tWY, tYZ + tWX, math.One - (tXX + tYY)
      );
    }

    /// <summary>Constructs a new matrix as copy of an existing matrix</summary>
    /// <param name="other">Matrix to copy</param>
    [DebuggerStepThrough]
    public Matrix33(Matrix33<ScalarType, MathType> other) :
      this(
        other[0, 0], other[0, 1], other[0, 2],
        other[1, 0], other[1, 1], other[1, 2],
        other[2, 0], other[2, 1], other[2, 2]
      ) { }

    /// <summary>
    ///   Constructs a new matrix from three orthogonal vectors and a translation
    /// </summary>
    /// <param name="right">A vector pointing to the right</param>
    /// <param name="up">A vector pointing upwards</param>
    /// <param name="into">A vector pointing into the direction the matrix faces</param>
    [DebuggerStepThrough]
    public Matrix33(
      Vector3<ScalarType, MathType> right,
      Vector3<ScalarType, MathType> up,
      Vector3<ScalarType, MathType> into
    ) :
      this(
        right.X, right.Y, right.Z,
        up.X, up.Y, up.Z,
        into.X, into.Y, into.Z
      ) { }

    /// <summary>Constructs a new matrix from the specified components</summary>
    /// <param name="m00">The value for element 0, 0 of the matrix</param>
    /// <param name="m01">The value for element 0, 1 of the matrix</param>
    /// <param name="m02">The value for element 0, 2 of the matrix</param>
    /// <param name="m10">The value for element 1, 0 of the matrix</param>
    /// <param name="m11">The value for element 1, 1 of the matrix</param>
    /// <param name="m12">The value for element 1, 2 of the matrix</param>
    /// <param name="m20">The value for element 2, 0 of the matrix</param>
    /// <param name="m21">The value for element 2, 1 of the matrix</param>
    /// <param name="m22">The value for element 2, 2 of the matrix</param>
    [DebuggerStepThrough]
    public Matrix33(
      ScalarType m00, ScalarType m01, ScalarType m02,
      ScalarType m10, ScalarType m11, ScalarType m12,
      ScalarType m20, ScalarType m21, ScalarType m22
    ) {
      this.m = new Number<ScalarType, MathType>[3, 3];
      this.m[0, 0] = m00;
      this.m[0, 1] = m01;
      this.m[0, 2] = m02;
      this.m[1, 0] = m10;
      this.m[1, 1] = m11;
      this.m[1, 2] = m12;
      this.m[2, 0] = m20;
      this.m[2, 1] = m21;
      this.m[2, 2] = m22;
    }

    /// <summary>Access a component of the matrix by its index</summary>
    /// <param name="y">The row of the matrix to access</param>
    /// <param name="x">The column of the matrix to access</param>
    public ScalarType this[int y, int x] {
      [DebuggerStepThrough]
      get { return this.m[y, x]; }
      [DebuggerStepThrough]
      set { this.m[y, x] = value; }
    }

    /// <summary>Access a component of the matrix by its index</summary>
    /// <param name="row">The row of the matrix to access</param>
    public Vector3<ScalarType, MathType> this[int row] {
      [DebuggerStepThrough]
      get {
        switch(row) {
          case 0: { return Right; }
          case 1: { return Up; }
          case 2: { return Into; }
          default: {
            throw new ArgumentOutOfRangeException("Matrix row out of range", "row");
          }
        }
      }
      [DebuggerStepThrough]
      set {
        switch(row) {
          case 0: { Right = value; break; }
          case 1: { Up = value; break; }
          case 2: { Into = value; break; }
          default: {
            throw new ArgumentOutOfRangeException("Matrix row out of range", "row");
          }
        }
      }
    }

    /// <summary>Vector pointing to the right</summary>
    public Vector3<ScalarType, MathType> Right {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          this.m[0, 0], this.m[0, 1], this.m[0, 2]
        );
      }
      [DebuggerStepThrough]
      set {
        this.m[0, 0] = value.X;
        this.m[0, 1] = value.Y;
        this.m[0, 2] = value.Z;
      }
    }

    /// <summary>Vector pointing upwards</summary>
    public Vector3<ScalarType, MathType> Up {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          this.m[1, 0], this.m[1, 1], this.m[1, 2]
        );
      }
      [DebuggerStepThrough]
      set {
        this.m[1, 0] = value.X;
        this.m[1, 1] = value.Y;
        this.m[1, 2] = value.Z;
      }
    }

    /// <summary>Vector pointing inwards</summary>
    public Vector3<ScalarType, MathType> Into {
      [DebuggerStepThrough]
      get {
        return new Vector3<ScalarType, MathType>(
          this.m[2, 0], this.m[2, 1], this.m[2, 2]
        );
      }
      [DebuggerStepThrough]
      set {
        this.m[2, 0] = value.X;
        this.m[2, 1] = value.Y;
        this.m[2, 2] = value.Z;
      }
    }

    /// <summary>Calculates the product of two matrices</summary>
    /// <param name="matrix">First matrix for the multiplication</param>
    /// <param name="factor">Second matrix for the multiplication</param>
    /// <returns>The product of the multiplication of both vector's elements</returns>
    public static Matrix33<ScalarType, MathType> operator *(
      Matrix33<ScalarType, MathType> matrix, Matrix33<ScalarType, MathType> factor
    ) {
      return new Matrix33<ScalarType, MathType>(
        matrix.m[0, 0] * factor.m[0, 0] + matrix.m[0, 1] *
          factor.m[1, 0] + matrix.m[0, 2] * factor.m[2, 0],
        matrix.m[0, 0] * factor.m[0, 1] + matrix.m[0, 1] *
          factor.m[1, 1] + matrix.m[0, 2] * factor.m[2, 1],
        matrix.m[0, 0] * factor.m[0, 2] + matrix.m[0, 1] *
          factor.m[1, 2] + matrix.m[0, 2] * factor.m[2, 2],

        matrix.m[1, 0] * factor.m[0, 0] + matrix.m[1, 1] *
          factor.m[1, 0] + matrix.m[1, 2] * factor.m[2, 0],
        matrix.m[1, 0] * factor.m[0, 1] + matrix.m[1, 1] *
          factor.m[1, 1] + matrix.m[1, 2] * factor.m[2, 1],
        matrix.m[1, 0] * factor.m[0, 2] + matrix.m[1, 1] *
          factor.m[1, 2] + matrix.m[1, 2] * factor.m[2, 2],

        matrix.m[2, 0] * factor.m[0, 0] + matrix.m[2, 1] *
          factor.m[1, 0] + matrix.m[2, 2] * factor.m[2, 0],
        matrix.m[2, 0] * factor.m[0, 1] + matrix.m[2, 1] *
          factor.m[1, 1] + matrix.m[2, 2] * factor.m[2, 1],
        matrix.m[2, 0] * factor.m[0, 2] + matrix.m[2, 1] *
          factor.m[1, 2] + matrix.m[2, 2] * factor.m[2, 2]
      );
    }

    /// <summary>Transforms a vector by this matrix</summary>
    /// <param name="matrix">Matrix to transform the vector by</param>
    /// <param name="vector">Vector to be transformed</param>
    /// <returns>The transformed vector</returns>
    public static Vector3<ScalarType, MathType> operator *(
      Matrix33<ScalarType, MathType> matrix, Vector3<ScalarType, MathType> vector
    ) {
      return new Vector3<ScalarType, MathType>(
        matrix.m[0, 0] * vector[0] + matrix.m[0, 1] * vector[1] + matrix.m[0, 2] * vector[2],
        matrix.m[1, 0] * vector[0] + matrix.m[1, 1] * vector[1] + matrix.m[1, 2] * vector[2],
        matrix.m[2, 0] * vector[0] + matrix.m[2, 1] * vector[1] + matrix.m[2, 2] * vector[2]
      );
    }

    /// <summary>Determines if two matrices are equal</summary>
    /// <param name="first">First matrix to be compared</param>
    /// <param name="second">Second matrix to be compared</param>
    /// <returns>True if both matrices are equal</returns>
    [DebuggerStepThrough]
    public static bool operator ==(
      Matrix33<ScalarType, MathType> first, Matrix33<ScalarType, MathType> second
    ) {
      return
        (first.m[0, 0] == second.m[0, 0]) &&
        (first.m[0, 1] == second.m[0, 1]) &&
        (first.m[0, 2] == second.m[0, 2]) &&

        (first.m[1, 0] == second.m[1, 0]) &&
        (first.m[1, 1] == second.m[1, 1]) &&
        (first.m[1, 2] == second.m[1, 2]) &&

        (first.m[2, 0] == second.m[2, 0]) &&
        (first.m[2, 1] == second.m[2, 1]) &&
        (first.m[2, 2] == second.m[2, 2]);
    }

    /// <summary>Determines if two matrices are unequal</summary>
    /// <param name="first">First matrix to be compared</param>
    /// <param name="second">Second matrix to be compared</param>
    /// <returns>True if both matrices are unequal</returns>
    [DebuggerStepThrough]
    public static bool operator !=(
      Matrix33<ScalarType, MathType> first, Matrix33<ScalarType, MathType> second
    ) {
      return
        (first.m[0, 0] != second.m[0, 0]) ||
        (first.m[0, 1] != second.m[0, 1]) ||
        (first.m[0, 2] != second.m[0, 2]) ||

        (first.m[1, 0] != second.m[1, 0]) ||
        (first.m[1, 1] != second.m[1, 1]) ||
        (first.m[1, 2] != second.m[1, 2]) ||

        (first.m[2, 0] != second.m[2, 0]) ||
        (first.m[2, 1] != second.m[2, 1]) ||
        (first.m[2, 2] != second.m[2, 2]);
    }

    /// <summary>Determines if an object is identical to the matrix</summary>
    /// <param name="obj">Object to compare to</param>
    /// <returns>True if the object is identical to the matrix</returns>
    public override bool Equals(object obj) {
      if(obj is Matrix33<ScalarType, MathType>) {
        return (this == (Matrix33<ScalarType, MathType>)obj);
      } else {
        return false;
      }
    }

    /// <summary>Builds a hashing code for the instance</summary>
    /// <returns>The instance's hashing code</returns>
    public override int GetHashCode() {
      return this.m.GetHashCode();
    }

    /// <summary>Converts the matrix to a readable string representation</summary>
    /// <returns>The matrix as a string</returns>
    public override string ToString() {
      return
        "| " + this[0, 0] + " " + this[0, 1] + " " + this[0, 2] + " |\n" +
        "| " + this[1, 0] + " " + this[1, 1] + " " + this[1, 2] + " |\n" +
        "| " + this[2, 0] + " " + this[2, 1] + " " + this[2, 2] + " |";
    }

    /// <summary>Constructs a number wrapper around the generic data type</summary>
    /// <param name="value">Instance of the generic data type to wrap</param>
    /// <returns>A number wrapper that wraps the generic data type instance</returns>
    private static Number<ScalarType, MathType> N(ScalarType value) { return value; }

    /// <summary>Instance of the math package we are using</summary>
    private static MathType math = Shared<MathType>.Instance;

    /// <summary>The elements of the matrix</summary>
    private Number<ScalarType, MathType>[/*3*/, /*3*/] m;

  }

} // namespace Nuclex.Math
