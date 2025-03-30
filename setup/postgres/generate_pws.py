import bcrypt

# Generate a salt
salt = bcrypt.gensalt()

# Hash a password
passwords = [b"nbrau23", b"osand23", b"zuzha23", b"sitar23", b"bbart23", b"dogan23" ]

i = 1
for password in passwords:
    hashed_password = bcrypt.hashpw(password, salt)
    print("(" + str(i) + ", decode(\'" + hashed_password.hex() + "\', \'hex\'), decode(\'" + salt.hex() + "\', \'hex\')),")
    i += 1


