using System.IO;

///<summary>
/// A .NET equivalent to Be's BPoint class
///</summary>
///
namespace muscle.support {
    public class Point : Flattenable {
    
        private float _x = 0.0f;
        private float _y = 0.0f;
    
        ///<summary>
        /// Default constructor, sets the point to be (0, 0)
        ///</summary>
        ///
        public Point() { /* empty */ }

        ///<summary>
        /// Constructor where you specify the initial value of the point 
        /// <param name="x>Initial X position</param>
        /// <param name="y>Initial y position</param>
        ///</summary>
        ///
        public Point(float X, float Y) {
            set(X, Y);
        }

        ///<summary>
        /// Returns a clone of this object
        ///</summary>
        ///
        public override Flattenable cloneFlat() {
            return new Point(x, y);
        }

        ///<summary>
        /// Should set this object's state equal to that of (setFromMe), or throw an UnflattenFormatException if it can't be done.
        /// <param name="setFromMe">The object we want to be like.</param>
        /// <exception cref="ClassCastException">if (setFromMe) isn't a Point</exception>
        ///</summary>
        ///
        public override void setEqualTo(Flattenable setFromMe)
        {
            Point p = (Point) setFromMe; 
            set(p.x, p.y);
        }

        ///<summary>
        /// Sets a new value for the point.
        /// <param name="X">The new X value.</param>
        /// <param name="Y">The new Y value.</param>
        /// <exception cref="ClassCastException">if (setFromMe) isn't a Point</exception>
        ///</summary>
        ///
        public void set(float X, float Y) {
            _x = X; _y = Y;
        }
    
        /// <summary>
        /// If the point is outside the rectangle specified by the two arguments, 
        /// it will be moved horizontally and/or vertically until it falls inside the rectangle.
        /// </summary>
        /// <param name="topLeft">Minimum values acceptable for X and Y</param>
        /// <param name="bottomRight">Maximum values acceptable for X and Y</param>
        ///
        public void constrainTo(Point topLeft, Point bottomRight)
        {
            if (x < topLeft.x) 
                _x = topLeft.x;
        
            if (y < topLeft.y) 
                _y = topLeft.y;
      
            if (x > bottomRight.x) 
                _x = bottomRight.x;
      
            if (y > bottomRight.y) 
                _y = bottomRight.y;
        }
    
        public override string ToString() {
            return "Point: " + x + " " + y;
        }

        /** Returns another Point whose values are the sum of this point's values and (p)'s values. 
        *  @param rhs Point to add
        *  @return Resulting point
        */
        public Point add(Point rhs) {
            return new Point(_x + rhs.x, _y + rhs.y);
        }

        /** Returns another Point whose values are the difference of this point's values and (p)'s values. 
        *  @param rhs Point to subtract from this
        *  @return Resulting point
        */
        public Point subtract(Point rhs) {
            return new Point(_x - rhs.x, _y - rhs.y);
        }

        /** Adds (p)'s values to this point's values */
        public void addToThis(Point p)
        {
            _x += p.x;
            _y += p.y;
        }

        /** Subtracts (p)'s values from this point's values */
        public void subtractFromThis(Point p)
        {
            _x -= p.x;
            _y -= p.y;
        }

        /** Returns true iff (p)'s values match this point's values */
        public override bool Equals(object o)
        {
            if (o is Point) {
                Point p = (Point) o;
                return ((_x == p.x) && (_y == p.y));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    
        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool isFixedSize() {return true;}
    
        /// <summary>
        /// Returns B_POINT_TYPE.
        /// </summary>
        public override int typeCode() {
            return TypeConstants.B_POINT_TYPE;
        }
    
        /// <summary>
        /// Returns 8 (2*sizeof(float))
        /// </summary>
        public override int flattenedSize() {
            return 8;
        }
    
        public override void flatten(BinaryWriter writer)
        {
            writer.Write((float) _x);
            writer.Write((float) _y);
        }

        /// <summary> 
        /// Returns true iff (code) is B_POINT_TYPE.
        /// </summary>
        public override bool allowsTypeCode(int code) {
            return (code == B_POINT_TYPE);
        }

        /** 
*  Should attempt to restore this object's state from the given buffer.
*  @param in The stream to read the object from.
*  @throws IOException if there is a problem reading in the input bytes.
*  @throws UnflattenFormatException if the bytes that were read in weren't in the expected format.
*/
        public override void unflatten(BinaryReader reader, int numBytes)
        {
            _x = reader.ReadSingle();
            _y = reader.ReadSingle();
        }

        /*   public void unflatten(DataInput in, int numBytes)
        {
            x = in.readFloat();
            y = in.readFloat();
        }
   
        public void unflatten(ByteBuffer in, int numBytes)
        {
            x = in.getFloat();
            y = in.getFloat();
        }*/
    }
}
