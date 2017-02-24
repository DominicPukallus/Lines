function Rand()
{
	this.c = -1.99999999999999;
	this.x = this.c;
    	this.range = this.c * -1;

	this.Get = get
}

function get()
{
	this.x = (this.x * this.x) + this.c;

	return (this.x / this.range);
}
