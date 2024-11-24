using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricDreams.Dreams.LofiHollow {
    public class SpecificPosition {
        public int X;
        public int Y;
        public int mX;
        public int mY;
        public string WorldArea = "";

        public SpecificPosition(int x, int y, int mx, int my, string wa) {
            X = x;
            Y = y;
            mX = mx;
            mY = my;
            WorldArea = wa;
        } 

        public Point GetPos() { return new Point(X, Y); }
        public Point GetMapPos() { return new Point(mX, mY); }

        public SpecificPosition But(string what, string where = "") {
            SpecificPosition output = Helper.Clone(this);

            if (what == "mup") {
                output.mY -= 1;
            } else if (what == "mdown") {
                output.mY += 1;
            } else if (what == "mleft") {
                output.mX -= 1;
            } else if (what == "mright") {
                output.mX += 1;
            }
            else if (what == "up") {
                output.Y -= 1;
            } else if (what == "down") {
                output.Y += 1;
            } else if (what == "left") {
                output.X -= 1;
            } else if (what == "right") {
                output.X += 1;
            }

            else if (what == "world") {
                output.WorldArea = where;
            }

            return output;
        }
    }
}
