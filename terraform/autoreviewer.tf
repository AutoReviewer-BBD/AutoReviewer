
provider "aws" {
  region  = "eu-west-1"
}
 
resource "aws_vpc" "mainVPC" {
  cidr_block            = "10.0.0.0/16"
  enable_dns_hostnames  = true
 
  tags = {
    Name = "main"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}

resource "aws_internet_gateway" "gw" {
  vpc_id = aws_vpc.mainVPC.id
 
  tags = {
    Name = "mainGW"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}
 
resource "aws_route" "add_to_default_route" {
  route_table_id         = aws_vpc.mainVPC.default_route_table_id
  destination_cidr_block = "0.0.0.0/0" #where we want the traffic to go
  gateway_id             = aws_internet_gateway.gw.id
}
 
resource "aws_subnet" "subnet1" {
  vpc_id     = aws_vpc.mainVPC.id
  cidr_block = "10.0.1.0/24"
  availability_zone = "eu-west-1a"
 
  tags = {
    Name = "subnet1"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}

resource "aws_subnet" "subnet2" {
  vpc_id     = aws_vpc.mainVPC.id
  cidr_block = "10.0.2.0/24"
  availability_zone = "eu-west-1b"
 
  tags = {
    Name = "subnet2"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}
 
resource "aws_db_subnet_group" "database_subnet_group" {
  name         = "database subnet group"
  subnet_ids   = [aws_subnet.subnet1.id, aws_subnet.subnet2.id]
  description  = "The subnet group for our database"
 
  tags = {
    Name = "database_subnet_group"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}
 
resource "aws_security_group" "database_security_group" {
  name        = "database security group"
  description = "enable access on port 1433"
  vpc_id      = aws_vpc.mainVPC.id
 
  ingress {
    description      = "ms mysql access"
    from_port        = 1433
    to_port          = 1433
    protocol         = "tcp"
    cidr_blocks      = ["0.0.0.0/0"]
  }
 
  egress {
    from_port        = 0
    to_port          = 0
    protocol         = -1
    cidr_blocks      = ["0.0.0.0/0"]
  }
 
  tags = {
    Name = "database_security_group"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}
 

resource "aws_db_instance" "db_instance" {
  engine                  = "sqlserver-ex"
  engine_version          = "15.00.4355.3.v1"
  multi_az                = false
  identifier              = "autoreviewerdb" #the name of the instance
  username                = "AutoreviewerAdmin"
  password                = "Password12345"
  instance_class          = "db.t3.micro"
  allocated_storage       = 20
  db_subnet_group_name    = aws_db_subnet_group.database_subnet_group.name
  vpc_security_group_ids  = [aws_security_group.database_security_group.id]
  skip_final_snapshot     = true
  publicly_accessible     = true
 
  tags = {
    Name = "autoreviewerdbinstance"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}
 
/////////////////////////////////////////////////////////////////////
#Security group for our ec2 isntance
resource "aws_security_group" "ec2_security_group" {
  name        = "ec2 security group"
  description = "enable ssh access on port 22"
  vpc_id      = aws_vpc.mainVPC.id
 
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }
 
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
 
  tags = {
    Name = "ec2_security_group"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}

resource "aws_instance" "ec2_instance" {
  ami           = "ami-0f007bf1d5c770c6e"
  instance_type = "t3.micro"
  subnet_id     = aws_subnet.subnet1.id
  security_groups = [aws_security_group.ec2_security_group.id]
  key_name      = "AutoReviewerKey"
  associate_public_ip_address = true
 
  tags = {
    Name = "ec2_instance"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}

resource "aws_s3_bucket" "s3_bucket" {
  bucket = "autoreviewerbucket"
  acl    = "private"
 
  tags = {
    Name = "s3_bucket"
    owner = "Thapelo.Thoka@bbd.co.za"
    created-using = "terraform"
  }
}
 
resource "aws_s3_bucket_ownership_controls" "s3_bucket_acl_ownership" {
  bucket = aws_s3_bucket.s3_bucket.id
 
  rule {
    object_ownership = "ObjectWriter"
  }
}
 
resource "aws_s3_bucket_versioning" "s3_bucket_versioning" {
  bucket = aws_s3_bucket.s3_bucket.id
 
  versioning_configuration {
    status = "Enabled"
  }
}