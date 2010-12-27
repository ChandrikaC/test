using System;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.Geometry;
using System.Collections.Generic;

namespace Rhino.Geometry.Intersect
{
  /// <summary>
  /// Provides all the information for a single Curve Intersection event.
  /// </summary>
  public class IntersectionEvent
  {
    #region members
    internal int m_type; // 1 == ccx_point
                         // 2 == ccx_overlap
                         // 3 == csx_point
                         // 4 == csx_overlap

    // using same internal names as OpenNURBS so we can keep things straight
    internal Point3d m_A0; //Point on A (or first point on overlap)
    internal Point3d m_A1; //Point on A (or last point on overlap)
    internal Point3d m_B0; //Point on B (or first point on overlap)
    internal Point3d m_B1; //Point on B (or last point on overlap)
    internal double m_a0; //Parameter on A (or first parameter of overlap)
    internal double m_a1; //Parameter on A (or last parameter of overlap)
    internal double m_b0; //Parameter on B (or first parameter of overlap) (or first U parameter on surface)
    internal double m_b1; //Parameter on B (or last parameter of overlap) (or last U parameter on surface)
    internal double m_b2; //First V parameter on surface
    internal double m_b3; //Last V parameter on surface
    #endregion

    #region properties
    /// <summary>
    /// All curve intersection events are either a single point or an overlap
    /// </summary>
    public bool IsPoint
    {
      get { return (1 == m_type || 3 == m_type); }
    }

    /// <summary>
    /// All curve intersection events are either a single point or an overlap
    /// </summary>
    public bool IsOverlap
    {
      get { return !IsPoint; }
    }

    /// <summary>
    /// Gets the point on Curve A where the intersection occured. 
    /// If the intersection type is overlap, then this will return the 
    /// start of the overlap region.
    /// </summary>
    public Point3d PointA
    {
      get { return m_A0; }
    }
    /// <summary>
    /// Gets the end point of the overlap on Curve A. 
    /// If the intersection type is not overlap, this value is meaningless.
    /// </summary>
    public Point3d PointA2
    {
      get { return m_A1; }
    }

    /// <summary>
    /// Gets the point on Curve B (or Surface B) where the intersection occured. 
    /// If the intersection type is overlap, then this will return the 
    /// start of the overlap region.
    /// </summary>
    public Point3d PointB
    {
      get { return m_B0; }
    }
    /// <summary>
    /// Gets the end point of the overlap on Curve B (or Surface B). 
    /// If the intersection type is not overlap, this value is meaningless.
    /// </summary>
    public Point3d PointB2
    {
      get { return m_B1; }
    }

    /// <summary>
    /// Gets the parameter on Curve A where the intersection occured. 
    /// If the intersection type is overlap, then this will return the 
    /// start of the overlap region.
    /// </summary>
    public double ParameterA
    {
      get { return m_a0; }
    }
    /// <summary>
    /// Gets the parameter on Curve A where the intersection occured. 
    /// If the intersection type is overlap, then this will return the 
    /// start of the overlap region.
    /// </summary>
    public double ParameterB
    {
      get { return m_b0; }
    }

    /// <summary>
    /// Gets the interval on curve A where the overlap occurs. 
    /// If the intersection type is not overlap, this value is meaningless.
    /// </summary>
    public Interval OverlapA
    {
      get { return new Interval(m_a0, m_a1); }
    }
    /// <summary>
    /// Gets the interval on curve B where the overlap occurs. 
    /// If the intersection type is not overlap, this value is meaningless.
    /// </summary>
    public Interval OverlapB
    {
      get { return new Interval(m_b0, m_b1); }
    }

    /// <summary>
    /// If this instance records a Curve|Surface intersection event, 
    /// <i>and</i> the intersection type is <b>point</b>, then use this function 
    /// to get the U and V parameters on the surface where the intersection occurs.
    /// </summary>
    /// <param name="u">Parameter on surface u direction where the intersection occurs.</param>
    /// <param name="v">Parameter on surface v direction where the intersection occurs.</param>
    public void SurfacePointParameter(out double u, out double v)
    {
      // ?? should we throw exceptions for calling this with ccx intersection types
      if (m_type < 3)
      {
        u = v = 0.0;
      }
      else
      {
        u = m_b0;
        v = m_b1;
      }
    }
    /// <summary>
    /// If this instance records a Curve|Surface intersection event, 
    /// <i>and</i> the intersection type if <b>overlap</b>, then use this function 
    /// to get the U and V domains on the surface where the overlap occurs.
    /// </summary>
    /// <param name="uDomain">Domain along surface U direction for overlap event.</param>
    /// <param name="vDomain">Domain along surface V direction for overlap event.</param>
    public void SurfaceOverlapParameter(out Interval uDomain, out Interval vDomain)
    {
      if( m_type < 3 )
      {
        uDomain = new Interval(0,0);
        vDomain = new Interval(0,0);
      }
      else
      {
        uDomain = new Interval(m_b0, m_b1);
        vDomain = new Interval(m_b2, m_b3);
      }
    }
    #endregion
  }

  /// <summary>
  /// Maintains an ordered list of Curve Intersection results.
  /// </summary>
  public class CurveIntersections : IDisposable
  {
    #region members
    IntPtr m_ptr; //ON_SimpleArray<ON_X_EVENT>
    IntersectionEvent[] m_events; // = null; initialized by runtime
    int m_count;
    #endregion

    #region constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    internal CurveIntersections(IntPtr ptr, int count)
    {
      m_ptr = ptr;
      m_count = count;
      m_events = new IntersectionEvent[count];
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~CurveIntersections()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
      if (IntPtr.Zero != m_ptr)
      {
        UnsafeNativeMethods.ON_Intersect_IntersectArrayDelete(m_ptr);
        m_ptr = IntPtr.Zero;
        m_count = 0;
        m_events = null;
      }
    }
    #endregion

    #region properties
    /// <summary>
    /// Gets the number of recorded intersection events.
    /// </summary>
    public int Count
    {
      get { return m_count; }
    }

    /// <summary>
    /// Gets the intersection event data at the given index.
    /// </summary>
    /// <param name="index">Index of intersection event to retrieve.</param>
    public IntersectionEvent this[int index]
    {
      get
      {
        if (index < 0) { throw new IndexOutOfRangeException("index must be equal to or larger than zero"); }
        if (index >= m_count) { throw new IndexOutOfRangeException("index must be less than the size of the collection"); }

        if (m_events[index] == null)
        {
          IntersectionEvent x;
          if (!CacheCurveData(index, out x)) { return null; } // This really should never happen.
          m_events[index] = x;
        }

        return m_events[index];
      }
    }
    internal bool CacheCurveData(int index, out IntersectionEvent x)
    {
      x = new IntersectionEvent();
      IntPtr ptr = ConstPointer();
      return UnsafeNativeMethods.ON_Intersect_CurveIntersectData(ptr, index,
        ref x.m_type, ref x.m_A0, ref x.m_A1, ref x.m_B0, ref x.m_B1,
        ref x.m_a0, ref x.m_a1, ref x.m_b0, ref x.m_b1, ref x.m_b2, ref x.m_b3);
    }
    #endregion

    #region methods
    private IntPtr ConstPointer() { return m_ptr; }
    #endregion
  }
}