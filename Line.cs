using System;
using System.Collections.Generic;
using System.Text;

namespace Lines2_1
{
    class Line
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        public float positionOX;
        public float positionOY;
        public float positionOZ;
        public float speedX;
        public float speedY;
        public float speedZ;
        public int red;
        public int green;
        public int blue;
        public float attraction;
        public float velocity;

        public Line()
        {
        
        }

        public void normalizeVelocity()
        {
            float hypotenuse;

            hypotenuse = (speedX * speedX) + (speedY * speedY) + (speedZ * speedZ);
            hypotenuse = (float)System.Math.Sqrt(hypotenuse);

            speedX = (speedX / hypotenuse) * velocity;
            speedY = (speedY / hypotenuse) * velocity;
            speedZ = (speedZ / hypotenuse) * velocity;
        }

    }
    
}
