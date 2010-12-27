using System;
using System.Runtime.InteropServices;

namespace Rhino.Geometry
{
  [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 32)]
  [Serializable()]
  public struct Quaternion
  {
    #region statics
    public static Quaternion Zero
    {
      get { return new Quaternion(); }
    }
    public static Quaternion Identity
    {
      get { return new Quaternion(1.0, 0.0, 0.0, 0.0); }
    }
    public static Quaternion I
    {
      get { return new Quaternion(0.0, 1.0, 0.0, 0.0); }
    }
    public static Quaternion J
    {
      get { return new Quaternion(0.0, 0.0, 1.0, 0.0); }
    }
    public static Quaternion K
    {
      get { return new Quaternion(0.0, 0.0, 0.0, 1.0); }
    }
    #endregion

    // quaternion = a + bi + cj + dk
    double m_a;
    double m_b;
    double m_c;
    double m_d;

    public Quaternion(double a, double b, double c, double d)
    {
      m_a = a;
      m_b = b;
      m_c = c;
      m_d = d;
    }

    public static bool operator ==(Quaternion a, Quaternion b)
    {
      return (a.m_a == b.m_a && a.m_b == b.m_b && a.m_c == b.m_c && a.m_d == b.m_d);
    }
    public static bool operator !=(Quaternion a, Quaternion b)
    {
      return (a.m_a != b.m_a || a.m_b != b.m_b || a.m_c != b.m_c || a.m_d != b.m_d);
    }

    public override bool Equals(object obj)
    {
      return (obj is Quaternion && this == (Quaternion)obj);
    }

    public override int GetHashCode()
    {
      // MSDN docs recommend XOR'ing the internal values to get a hash code
      return m_a.GetHashCode() ^ m_b.GetHashCode() ^ m_c.GetHashCode() ^ m_d.GetHashCode();
    }


    public double A
    {
      get { return m_a; }
      set { m_a = value; }
    }
    public double B
    {
      get { return m_b; }
      set { m_b = value; }
    }
    public double C
    {
      get { return m_c; }
      set { m_c = value; }
    }
    public double D
    {
      get { return m_d; }
      set { m_d = value; }
    }
    public void Set(double a, double b, double c, double d)
    {
      m_a = a;
      m_b = b;
      m_c = c;
      m_d = d;
    }

    public static Quaternion operator*(Quaternion q, int x)
    {
      return new Quaternion(q.m_a*x,q.m_b*x,q.m_c*x,q.m_d*x);
    }
    public static Quaternion operator*(Quaternion q, float x)
    {
      return new Quaternion(q.m_a*x,q.m_b*x,q.m_c*x,q.m_d*x);
    }
    public static Quaternion operator*(Quaternion q, double x)
    {
      return new Quaternion(q.m_a*x,q.m_b*x,q.m_c*x,q.m_d*x);
    }

    public static Quaternion operator/(Quaternion q, int y)
    {
      double x = (0!=y) ? 1.0/((double)y) : 0.0;
      return new Quaternion(q.m_a*x,q.m_b*x,q.m_c*x,q.m_d*x);
    }
    public static Quaternion operator/(Quaternion q, float y)
    {
      double x = (0.0f!=y) ? 1.0/((double)y) : 0.0;
      return new Quaternion(q.m_a*x,q.m_b*x,q.m_c*x,q.m_d*x);
    }
    public static Quaternion operator/(Quaternion q, double y)
    {
      double x = (0.0f!=y) ? 1.0/((double)y) : 0.0;
      return new Quaternion(q.m_a*x,q.m_b*x,q.m_c*x,q.m_d*x);
    }

    public static Quaternion operator+(Quaternion a, Quaternion b)
    {
      return new Quaternion(a.m_a+b.m_a, a.m_b+b.m_b, a.m_c+b.m_c, a.m_d+b.m_d);
    }
    public static Quaternion operator-(Quaternion a, Quaternion b)
    {
      return new Quaternion(a.m_a-b.m_a, a.m_b-b.m_b, a.m_c-b.m_c, a.m_d-b.m_d);
    }

    /// <summary>
    /// quaternion multiplication is not commutative
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Quaternion operator*(Quaternion a, Quaternion b)
    {
      return new Quaternion(a.m_a*b.m_a - a.m_b*b.m_b - a.m_c*b.m_c - a.m_d*b.m_d,
                            a.m_a*b.m_b + a.m_b*b.m_a + a.m_c*b.m_d - a.m_d*b.m_c,
                            a.m_a*b.m_c - a.m_b*b.m_d + a.m_c*b.m_a + a.m_d*b.m_b,
                            a.m_a*b.m_d + a.m_b*b.m_c - a.m_c*b.m_b + a.m_d*b.m_a);
    }

    /// <summary>
    /// returns true if a, b, c, and d are valid finite IEEE doubles.
    /// </summary>
    public bool IsValid
    {
      get
      {
        return RhinoMath.IsValidDouble(m_a) &&
          RhinoMath.IsValidDouble(m_b) &&
          RhinoMath.IsValidDouble(m_c) &&
          RhinoMath.IsValidDouble(m_d);
      }
    }

    /// <summary>
    /// Returns the conjugate of the quaternion = (a,-b,-c,-d)
    /// </summary>
    public Quaternion Conjugate
    {
      get
      {
        return new Quaternion(m_a, -m_b, -m_c, -m_d);
      }
    }

    /// <summary>
    /// Sets the quaternion to a/L2, -b/L2, -c/L2, -d/L2, where
    /// L2 = length squared = (a*a + b*b + c*c + d*d).  This is
    /// the multiplicative inverse, i.e.,
    /// (a,b,c,d)*(a/L2, -b/L2, -c/L2, -d/L2) = (1,0,0,0).
    /// </summary>
    /// <returns>
    /// True if successful.  False if the quaternion is zero and cannot be inverted.
    /// </returns>
    public bool Invert()
    {
      double x = m_a * m_a + m_b * m_b + m_c * m_c + m_d * m_d;
      if (x <= double.MinValue)
        return false;
      x = 1.0 / x;
      m_a *= x;
      x = -x;
      m_b *= x;
      m_c *= x;
      m_d *= x;
      return true;
    }

    /// <summary>
    /// Sets the quaternion to a/L2, -b/L2, -c/L2, -d/L2, where
    /// L2 = length squared = (a*a + b*b + c*c + d*d). This is
    /// the multiplicative inverse, i.e.,
    /// (a,b,c,d)*(a/L2, -b/L2, -c/L2, -d/L2) = (1,0,0,0).
    /// If "this" is the zero quaternion, then the zero quaternion is returned.
    /// </summary>
    public Quaternion Inverse
    {
      get
      {
        double x = m_a * m_a + m_b * m_b + m_c * m_c + m_d * m_d;
        x = (x > double.MinValue) ? 1.0 / x : 0.0;
        return new Quaternion(m_a * x, -m_b * x, -m_c * x, -m_d * x);
      }
    }

#if USING_V5_SDK
    /// <summary>
    /// Returns the length or norm of the quaternion sqrt(a*a + b*b + c*c + d*d).
    /// </summary>
    public double Length
    {
      get
      {
        return UnsafeNativeMethods.ON_Quaternion_Length(ref this);
      }
    }
#endif

    /// <summary>
    /// Returns a*a + b*b + c*c + d*d
    /// </summary>
    public double LengthSquared
    {
      get
      {
        return (m_a * m_a + m_b * m_b + m_c * m_c + m_d * m_d);
      }
    }

#if USING_V5_SDK
    /// <summary>
    /// The distance or norm of the difference between the two quaternions. = ("this" - q).Length.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public double DistanceTo(Quaternion q)
    {
      Quaternion pq = new Quaternion(q.m_a-m_a, q.m_b-m_b, q.m_c-m_c, q.m_d-m_d);
      return pq.Length;
    }

    /// <summary>
    /// Returns the distance or norm of the difference between the two quaternions.
    /// = (p - q).Length().
    /// </summary>
    /// <param name="p"></param>
    /// <param name="q"></param>
    /// <returns></returns>
    public static double Distance(Quaternion p, Quaternion q)
    {
      Quaternion pq = new Quaternion(q.m_a-p.m_a,q.m_b-p.m_b,q.m_c-p.m_c,q.m_d-p.m_d);
      return pq.Length;
    }
#endif

    /// <summary>
    /// Returns 4x4 real valued matrix form of the quaternion
    /// a  b  c  d
    /// -b  a -d  c
    /// -c  d  a -b
    /// -d -c  b  a
    /// which has the same arithmetic properties in as the quaternion. 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Do not confuse this with the rotation defined by the quaternion. This
    /// function will only be interesting to math nerds and is not useful in
    /// rendering or animation applications.
    /// </remarks>
    public Transform MatrixForm()
    {
      Transform rc = new Transform();
      rc.m_00 =  m_a; rc.m_01 =  m_b; rc.m_02 =  m_c; rc.m_03 =  m_d;
      rc.m_10 = -m_b; rc.m_11 =  m_a; rc.m_12 = -m_d; rc.m_13 =  m_c;
      rc.m_20 = -m_c; rc.m_21 =  m_d; rc.m_22 =  m_a; rc.m_23 = -m_b;
      rc.m_30 = -m_d; rc.m_31 = -m_c; rc.m_32 =  m_b; rc.m_33 =  m_a;
      return rc;
    }

#if USING_V5_SDK
    /// <summary>
    /// Scales the quaternion's coordinates so that a*a + b*b + c*c + d*d = 1.
    /// </summary>
    /// <returns>
    /// True if successful.  False if the quaternion is zero and cannot be unitized.
    /// </returns>
    public bool Unitize()
    {
      return UnsafeNativeMethods.ON_Quaternion_Unitize(ref this);
    }
#endif

    /// <summary>
    /// Sets the quaternion to cos(angle/2), sin(angle/2)*x, sin(angle/2)*y, sin(angle/2)*z
    /// where (x,y,z) is the unit vector parallel to axis.  This is the unit quaternion
    /// that represents the rotation of angle about axis.
    /// </summary>
    /// <param name="angle">in radians</param>
    /// <param name="axisOfRotation"></param>
    public void SetRotation(double angle, Vector3d axisOfRotation)
    {
      double s = axisOfRotation.Length;
      s = (s > 0.0) ? Math.Sin(0.5*angle)/s : 0.0;
      m_a = Math.Cos(0.5*angle);
      m_b = s*axisOfRotation.m_x;
      m_c = s*axisOfRotation.m_y;
      m_d = s*axisOfRotation.m_z;
    }

    /// <summary>
    /// Returns the unit quaternion
    /// cos(angle/2), sin(angle/2)*x, sin(angle/2)*y, sin(angle/2)*z
    /// where (x,y,z) is the unit vector parallel to axis.  This is the
    /// unit quaternion that represents the rotation of angle about axis.
    /// </summary>
    /// <param name="angle">in radians</param>
    /// <param name="axisOfRotation"></param>
    /// <returns></returns>
    public static Quaternion Rotation( double angle, Vector3d axisOfRotation )
    {
      double s = axisOfRotation.Length;
      s = (s > 0.0) ? Math.Sin(0.5*angle)/s : 0.0;
      return new Quaternion(Math.Cos(0.5*angle),s*axisOfRotation.m_x,s*axisOfRotation.m_y,s*axisOfRotation.m_z);
    }

#if USING_V5_SDK
    /// <summary>
    /// Sets the quaternion to the unit quaternion which rotates
    /// plane0.xaxis to plane1.xaxis, plane0.yaxis to plane1.yaxis,
    /// and plane0.zaxis to plane1.zaxis.
    /// </summary>
    /// <param name="plane0"></param>
    /// <param name="plane1"></param>
    /// <remarks>the plane origins are ignored</remarks>
    public void SetRotation( Plane plane0, Plane plane1 )
    {
      UnsafeNativeMethods.ON_Quaternion_SetRotation(ref this, ref plane0, ref plane1);
    }

    /// <summary>
    /// Returns the unit quaternion that represents the the rotation that maps
    /// plane0.xaxis to plane1.xaxis, plane0.yaxis to plane1.yaxis, and 
    /// plane0.zaxis to plane1.zaxis.
    /// </summary>
    /// <param name="plane0"></param>
    /// <param name="plane1"></param>
    /// <returns></returns>
    /// <remarks>the plane origins are ignored</remarks>
    public static Quaternion Rotation( Plane plane0, Plane plane1 )
    {
      Quaternion q = new Quaternion();
      q.SetRotation(plane0, plane1);
      return q;
    }

    /// <summary>
    /// Returns the rotation defined by the quaternion
    /// </summary>
    /// <param name="angle">in radians</param>
    /// <param name="axis">unit axis of rotation of 0 if (b,c,d) is the zero vector</param>
    /// <returns></returns>
    /// <remarks>
    /// If the quaternion is not unitized, the rotation of its unitized form is returned
    /// </remarks>
    public bool GetRotation(out double angle, out Vector3d axis)
    {
      double s = this.Length;
      angle = (s > double.MinValue) ? 2.0*Math.Acos(m_a/s) : 0.0;
      axis.m_x = m_b;
      axis.m_y = m_c;
      axis.m_z = m_d;
      return (axis.Unitize() && s > double.MinValue);
    }

    /// <summary>
    /// Returns the frame created by applying the quaternion's rotation
    /// to the canonical world frame (1,0,0),(0,1,0),(0,0,1).
    /// </summary>
    /// <param name="plane"></param>
    /// <returns></returns>
    public bool GetRotation(out Plane plane)
    {
      plane = new Plane();
      return UnsafeNativeMethods.ON_Quaternion_GetRotation(ref this, ref plane);
    }

    /// <summary>
    /// Rotate a 3d vector.  This operation is also called conjugation,
    /// because the result is the same as
    /// (q.Conjugate()*(0,x,y,x)*q/q.LengthSquared()).Vector()
    /// </summary>
    /// <param name="v"></param>
    /// <returns>
    /// R*v, where R is the rotation defined by the unit quaternion.
    /// This is mathematically the same as the values
    /// (Inverse(q)*(0,x,y,z)*q).Vector()
    /// and
    /// (q.Conjugate()*(0,x,y,x)*q/q.LengthSquared()).Vector()
    /// </returns>
    /// <remarks>
    /// If you need to rotate more than a dozen or so vectors,
    /// it will be more efficient to calculate the rotation
    /// matrix once and use it repeatedly.
    /// </remarks>
    public Vector3d Rotate(Vector3d v)
    {
      Vector3d vout = new Vector3d();
      UnsafeNativeMethods.ON_Quaternion_Rotate(ref this, v, ref vout);
      return vout;
    }
#endif
    /// <summary>
    /// The "vector" or "imaginary" part of the quaternion = (b,c,d)
    /// </summary>
    public Vector3d Vector
    {
      get { return new Vector3d(m_b, m_c, m_d); }
    }

    /// <summary>
    /// The "real" or "scalar" part of the quaternion = a.
    /// </summary>
    public double Scalar
    {
      get { return m_a; }
    }

    /// <summary>
    /// True if a, b, c, and d are all zero.
    /// </summary>
    public bool IsZero
    {
      get { return (0.0 == m_a && 0.0 == m_b && 0.0 == m_c && 0.0 == m_d); }
    }

    /// <summary>
    /// True if b, c, and d are all zero.
    /// </summary>
    public bool IsScalar
    {
      get { return (0.0 == m_b && 0.0 == m_c && 0.0 == m_d); }
    }

    /// <summary>
    /// True if a = 0 and at least one of b, c, or d is not zero.
    /// </summary>
    public bool IsVector
    {
      get { return (0.0 == m_a && (0.0 != m_b || 0.0 != m_c || 0.0 != m_d)); }
    }

    /// <summary>
    /// The quaternion product of p and q.  This is the same value as p*q.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="q"></param>
    /// <returns></returns>
    public static Quaternion Product( Quaternion p, Quaternion q )
    {
      return new Quaternion(p.m_a*q.m_a - p.m_b*q.m_b - p.m_c*q.m_c - p.m_d*q.m_d,
                            p.m_a*q.m_b + p.m_b*q.m_a + p.m_c*q.m_d - p.m_d*q.m_c,
                            p.m_a*q.m_c - p.m_b*q.m_d + p.m_c*q.m_a + p.m_d*q.m_b,
                            p.m_a*q.m_d + p.m_b*q.m_c - p.m_c*q.m_b + p.m_d*q.m_a);
    }

    /// <summary>
    /// The vector cross product of p and q = (0,x,y,z) where
    /// (x,y,z) = ON_CrossProduct(p.Vector(),q.Vector())
    /// This is NOT the same as the quaternion product p*q.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="q"></param>
    /// <returns></returns>
    public static Quaternion CrossProduct( Quaternion p, Quaternion q )
    {
      return new Quaternion(0.0, p.m_c * q.m_d - p.m_d * q.m_c, p.m_d * q.m_b - p.m_b * q.m_d, p.m_b * q.m_c - p.m_c * q.m_d);
    }
  }
}