const { MongoClient, GridFSBucket } = require('mongodb');
const fs = require('fs');
const https = require('https');
const path = require('path');

const imageUrl = 'https://upload.wikimedia.org/wikipedia/en/4/4d/Shrek_%28character%29.png';
const imagePath = '/tmp/Shrek.png';
const mongoUrl = 'mongodb://localhost:27017';
const dbName = 'blank';

const downloadImage = (url, path) => {
  return new Promise((resolve, reject) => {
    const file = fs.createWriteStream(path);
    console.log("Initiating HTTPS GET request");
    https.get(url, (response) => {
      console.log(`Response status code: ${response.statusCode}`);
      response.pipe(file);
      file.on('finish', () => {
        console.log("File download finished");
        file.close(resolve);
      });
    }).on('error', (err) => {
      console.error("Error during HTTPS GET request:", err);
      fs.unlink(path, () => reject(err));
    });
  });
};

const main = async () => {
  console.log("\n\n\n\n\nScript started");

  try {
    // Ensure the directory exists
    const dir = path.dirname(imagePath);
    if (!fs.existsSync(dir)) {
      fs.mkdirSync(dir, { recursive: true });
    }

    const client = await MongoClient.connect(mongoUrl, { useNewUrlParser: true, useUnifiedTopology: true });
    console.log("Connected to MongoDB");

    const db = client.db(dbName);
    const bucket = new GridFSBucket(db);

    console.log("Starting image download");
    await downloadImage(imageUrl, imagePath);
    console.log("Image downloaded successfully");

    console.log("Creating collection");
    const collection = await db.createCollection('lesson');
    console.log("Collection created");

    console.log("Inserting document");
    await collection.insertOne({
      lesson_details: {
        title: 'Test Lesson',
        description: 'If you see this... nice.',
        tags: ['test', 'lesson']
      },
      description: 'Yeeaaahhh',
      file_metadata: [{
        filename: imagePath,
        file_type: 'image/png',
      }],
      owner_id: 1
    });
    console.log("Document inserted");

    console.log("Starting file upload");

    // ✅ Fix: Ensure upload finishes before proceeding
    await new Promise((resolve, reject) => {
      const uploadStream = bucket.openUploadStream(path.basename(imagePath));
      fs.createReadStream(imagePath)
        .pipe(uploadStream)
        .on('error', (err) => {
          console.error("Error uploading file:", err);
          reject(err);
        })
        .on('finish', () => {
          console.log("File uploaded successfully");
          resolve();
        });
    });

    console.log("Closing MongoDB connection");
    await client.close();
    console.log("MongoDB connection closed");
  } catch (err) {
    console.error("Error:", err);
  }

  console.log("Script completed\n\n\n\n\n");
};

main();
