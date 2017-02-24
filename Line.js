function Line()
{
	this.positionX;
	this.positionY;
	this.positionZ;
	this.positionOX;
	this.positionOY;
	this.positionOZ;
	this.speedX;
	this.speedY;
	this.speedZ;
	this.red;
    this.green;
    this.blue;
    this.attraction;
    this.velocity;

	this.NormalizeVelocity = normalizeVelocity;
}

function normalizeVelocity()
{
	var hypotenuse = (this.speedX * this.speedX) + (this.speedY * this.speedY) + (this.speedZ * this.speedZ);
	hypotenuse = Math.sqrt(hypotenuse);
	
    this.speedX = (this.speedX / hypotenuse) * this.velocity;
    this.speedY = (this.speedY / hypotenuse) * this.velocity;
    this.speedZ = (this.speedZ / hypotenuse) * this.velocity;
}
