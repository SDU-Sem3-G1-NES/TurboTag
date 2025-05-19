-- Enable dblink extension
CREATE EXTENSION IF NOT EXISTS dblink;

-- Create Blank database to copy from
DO
$do$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'blank') THEN
      PERFORM dblink_exec('dbname=postgres', 'CREATE DATABASE blank');
END IF;
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'hangfire') THEN
      PERFORM dblink_exec('dbname=postgres', 'CREATE DATABASE hangfire');
END IF;
END
$do$;

\c blank

-- Create blank tables

CREATE TABLE IF NOT EXISTS user_types (
    user_type_id SERIAL PRIMARY KEY,
    user_type_name VARCHAR(255) UNIQUE NOT NULL,
    user_type_permissions JSONB NOT NULL
);

CREATE TABLE IF NOT EXISTS "users" (
    user_id SERIAL PRIMARY KEY,
    user_type_id INT REFERENCES user_types(user_type_id),
    user_name VARCHAR(255) UNIQUE NOT NULL,
    user_email VARCHAR(255) UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS libraries (
    library_id SERIAL PRIMARY KEY,
    library_name VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS uploads (
    upload_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "users"(user_id),
    -- library_id INT REFERENCES libraries(library_id),
    upload_date TIMESTAMP NOT NULL,
    upload_type VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS user_library_access (
    user_id INT REFERENCES "users"(user_id),
    library_id INT REFERENCES libraries(library_id),
    PRIMARY KEY (user_id, library_id)
);

CREATE TABLE IF NOT EXISTS library_uploads (
    library_id INT REFERENCES libraries(library_id),
    upload_id INT REFERENCES uploads(upload_id),
    PRIMARY KEY (library_id, upload_id)
);

CREATE TABLE IF NOT EXISTS settings (
    setting_id SERIAL PRIMARY KEY,
    setting_name VARCHAR(255) UNIQUE NOT NULL,
    setting_value VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS user_credentials (
    user_id INT REFERENCES "users"(user_id),
    user_password_hash BYTEA NOT NULL,
    user_password_salt BYTEA NOT NULL,
    PRIMARY KEY (user_id)
);

CREATE TABLE IF NOT EXISTS "refresh_tokens" (
    user_id SERIAL PRIMARY KEY,
    token VARCHAR(255) NOT NULL,
    expiry TIMESTAMP NOT NULL
    );

-- Insert default data into tables

DO
$do$
BEGIN
   IF EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'user_types') THEN
      INSERT INTO user_types (user_type_name, user_type_permissions)
      VALUES ('speedadmin', '{"godmode": true}'),
             ('superuser', '{"fullaccess": true}');
    END IF;
END
$do$;

DO
$do$
BEGIN
   IF EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'users') THEN
      INSERT INTO "users" (user_type_id, user_name, user_email)
      VALUES (1, 'Nick', 'nbrau23@student.sdu.dk'),
             (1, 'Oskar', 'osand23@student.sdu.dk'),
             (1, 'Niko', 'zuzha23@student.sdu.dk'),
             (1, 'Simonas', 'sitar23@student.sdu.dk'),
             (1, 'Benediktas', 'bbart23@student.sdu.dk'),
             (1, 'Dovydas', 'dogan23@student.sdu.dk');
    END IF;
END
$do$;

DO
$do$
BEGIN
   IF EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'libraries') THEN
      INSERT INTO libraries (library_name)
      VALUES ('Default');
    END IF;
END
$do$;

DO
$do$
BEGIN
   IF EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'uploads') THEN
      INSERT INTO uploads (user_id, upload_date, upload_type)
      VALUES (1, NOW(), 'image/png');
    END IF;
END
$do$;

DO
$do$
BEGIN
   IF EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'library_uploads') THEN
      INSERT INTO library_uploads (library_id, upload_id)
      VALUES (1, 1);
    END IF;
END
$do$;

DO
$do$
BEGIN
   IF EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'user_credentials') THEN
      INSERT INTO user_credentials (user_id, user_password_hash, user_password_salt)
      VALUES (1, decode('24326224313224417665436d4a6a586e70577a762e6448376f315045757079666b57704e6675744d576a74686f5a7630615664764e35315743706e57', 'hex'), decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575', 'hex')),
             (2, decode('24326224313224417665436d4a6a586e70577a762e6448376f315045752e4777416356377434576c39636e4d3868522e52414b7536767a54536a412e', 'hex'), decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575', 'hex')),
             (3, decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575502f36552e3677583858654869586b4a753753784a4d70362f4c587a735771', 'hex'), decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575', 'hex')),
             (4, decode('24326224313224417665436d4a6a586e70577a762e6448376f3150457544554a5a38485143526a6552677430364f636a6a4235576d4a66476c38502e', 'hex'), decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575', 'hex')),
             (5, decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575446a5651485331796f7835547450654f477579594c786357786c6c48692f65', 'hex'), decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575', 'hex')),
             (6, decode('24326224313224417665436d4a6a586e70577a762e6448376f3150457532724b4c66717233536872656f4b6a4e494d7136374e336a634f7359317343', 'hex'), decode('24326224313224417665436d4a6a586e70577a762e6448376f31504575', 'hex'));
    END IF;
END
$do$;