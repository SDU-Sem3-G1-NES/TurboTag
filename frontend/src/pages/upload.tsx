import React, { useState } from 'react'
import Tags from '../components/tags'
import {
  UploadClient,
  LessonClient,
  UploadDto,
  LessonDto,
  LessonDetailsDto,
  FileMetadataDto,
  FileClient,
  UploadChunkDto,
  FinaliseUploadDto,
  GenerationClient
} from '../api/apiClient.ts'
import { Button, Form, Input, notification, Progress, Upload, UploadProps, Spin } from 'antd'
import { UploadOutlined } from '@ant-design/icons'
import TextArea from 'antd/es/input/TextArea'

const UploadPage: React.FC = () => {
  const [uploading, setUploading] = useState<boolean>(false)
  const [generating, setGenerating] = useState<boolean>(false)
  const [uploadProgress, setUploadProgress] = useState<number>(0)
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()
  const fileClient = new FileClient()
  const lessonClient = new LessonClient()
  const contentGenerationClient = new GenerationClient()
  const CHUNK_SIZE = 1048576 * 15 // 15MB Chunk size
  const [form] = Form.useForm()

  const ownerId = Number(localStorage.getItem('userId'))
  const ownerName = localStorage.getItem('userName')

  const handleFileChange: UploadProps['beforeUpload'] = (file) => {
    setFile(file)
    return false // prevent auto-upload
  }

  const getFileDuration = (file: File): Promise<number | null> => {
    return new Promise((resolve, reject) => {
      if (!file.type.startsWith('audio/') && !file.type.startsWith('video/')) {
        reject(new Error('Unsupported file type. Only audio and video files are allowed.'))
        return
      }

      const mediaElement = document.createElement(
        file.type.startsWith('audio/') ? 'audio' : 'video'
      )
      const url = URL.createObjectURL(file)

      mediaElement.src = url

      mediaElement.onloadedmetadata = () => {
        resolve(mediaElement.duration)
        URL.revokeObjectURL(url)
      }

      mediaElement.onerror = () => {
        URL.revokeObjectURL(url)
        reject(new Error('Unable to load media file for duration calculation'))
      }
    })
  }

  const blobToBase64 = (blob: Blob) =>
    new Promise<string>((resolve, reject) => {
      const reader = new FileReader()
      reader.onloadend = () => {
        const result = reader.result as string
        if (result.includes(',')) {
          resolve(result.split(',')[1])
        } else {
          reject(new Error('Invalid base64 format'))
        }
      }
      reader.onerror = reject
      reader.readAsDataURL(blob)
    })

  const handleChunkedUpload = async (file: File) => {
    const uploadId = crypto.randomUUID()
    const totalChunks = Math.ceil(file.size / CHUNK_SIZE)

    setUploading(true)

    for (let i = 0; i < totalChunks; i++) {
      const chunk = file.slice(i * CHUNK_SIZE, (i + 1) * CHUNK_SIZE)
      const base64Chunk = await blobToBase64(chunk)

      const uploadChunkDto = new UploadChunkDto()
      uploadChunkDto.init({
        chunk: base64Chunk,
        uploadId: uploadId,
        chunkNumber: i
      })

      await fileClient.uploadChunk(uploadChunkDto)

      setUploadProgress(Math.round(((i + 1) / totalChunks) * 100))
    }

    const finalizeUploadDto = new FinaliseUploadDto()
    finalizeUploadDto.init({
      uploadId: uploadId,
      fileName: file.name
    })

    const response = await fileClient['instance'].post('/File/FinalizeUpload', finalizeUploadDto)
    const fileId = response.data as string

    setUploading(false)
    return fileId
  }

  const generateContention = async (/*file: File*/) => {
    setGenerating(true)
    const text =
      " Hi. Welcome to Music Theory in One Lesson.  This is our musical alphabet.  You may be asking yourself why there are different spacings between each note. That introduces  the idea of musical distance, which is extremely important in music. The answer is quite  simple. There are more notes than what I initially put on the screen. Let's take a look  at what's called our chromatic scale. Much better to note to the next the distance of  one half step. Let's take a listen to each note one after another.  There are actually two ways to spell this alphabet. Using sharps and also using flats.  Flats are shown below the sharps. In A sharp and a B flat, yes, they are the same note.  That sounds ridiculous now, but later in the course you're going to learn that this is very useful  and practical. Let's take a listen to the same chromatic scale note by note.  One more thing to keep in mind is that this alphabet will repeat in both directions  essentially into an affinity, but we generally limit how far that alphabet goes.  Take a listen to our alphabet in two octaves. An octave basically just means where the alphabet repeats.  Scales are incredibly important in music and they really need to be thoroughly understood.  That being said, they are also incredibly easy. A scale basically is just a pattern of whole and half steps.  We're going to build what is called the A major scale.  But first, let's take a moment to think about that name, A major. A will be our starting note,  and major will be the pattern. We will cover other patterns later in this section.  The major pattern goes as follows. Starting on A, whole step to B, whole step to C sharp, half step to D, whole step to E, whole step to F sharp,  another whole step to G sharp, then another half step brings us back to A again.  Take a listen to the A major scale.  Pretty simple, right? Next, we're going to build the F major scale. That is the major pattern starting on the note F.  F, whole step to G, whole step to A, half step to A sharp, whole step to C, whole step to D, another whole step to E, and another half step to F.  You're going to notice something peculiar about this scale. We have two forms of A and no B.  Well, let's do this whole exercise over again, but this time let's use that version of the chromatic scale that we spelled with flats.  F, whole step to G, whole step to A, half step to B flat, there's our B, whole step to C, whole step to D, whole step to E, and another half step to F.  Now, let's take a listen to this scale.  The major is not the only scale type that we're going to use. Next, let's examine the minor pattern starting on the note A.  So, starting on A, whole step to B, half step to C, whole step to D, whole step to E, half step to F, whole step to G, and another whole step brings us back to A.  So, our pattern now is whole step, half step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step, whole step  on D, we would have D minor. Let's take a listen to the A minor scale now.  In this section, we're going to further explore scales by looking at the idea of scale degrees.  Scale degrees sound like they are very, very complicated and difficult, but they aren't just  like everything else in music theory. Quite simply, the note that you start on, an example  I had our A minor scale, that note A is our one. The note right after, you guessed it,  is two, that's our B, three, four, five, six, seven and back to one. It's pretty simple.  The note you start on is a one and the rest fall in line.  Next, let's make the harmonic minor scale. This is pretty simple.  You just sharp the seven, but let's talk about sharpening and flattening scale degrees really fast.  In this case, the seven is G, so we'll just make it G sharp. But in other scales, we have flats.  So if you need to sharp a scale degree that has a flat, you don't put a sharp after it,  the sharp just cancels out the flat. It's really important to think of sharps and flats as operators.  The sharp will raise the given note by a half step. The flat will lower the note by a half step.  But that does not mean sharpening a B flat will make a B sharp. In fact, it'll just make a regular  B. On the screen, you'll see both the a harmonic minor and a natural minor scales.  Any time in music you run into the word natural, that's really just code speak for normal  or regular, not sharp, not flat, as it occurs from the original pattern.  Notice the G sharp in the harmonic minor scale. That is our sharp seven.  Now let's take a listen to this scale.  Now let's take a listen to what is called the melodic minor scale.  This one is a little bit different than the harmonic minor because it is different on the way up  and the way down. First, on the way up, you're going to sharp the sixth scale degree  and the seventh scale degree. Giving us at least an A minor F sharp and G sharp.  After that, on the way down, you're going to play the natural minor scale as it was originally derived.  Let's take a listen to this scale.  Intervals are just the measurement of musical distance.  First, let's take a look at the C major scale shown on the screen with its scale degrees above it  and examine each distance. C to D is a second. C to E is a third.  C to F, a fourth. C to G, a fifth. C to E is a sixth and C to B is a seventh. C to C is our  octave. OCT, the prefix meaning eight.  But if you were to examine the C minor scale,  you'll see that we have seconds, thirds, fourths, fifths, sixths and sevenths, but the notes are different.  So we are going to need multiple interval types in order to describe these intervals.  What we have are major intervals, minor intervals, augmented, diminished,  and perfect. Let's start by examining all of the intervals in our C major scale.  We will then compare these notes and intervals to the C minor scale and begin to name the intervals.  First, C to D. This is what is called a major second. As you can see from the chromatic scale  below, a major second is built off of just one whole step. Also notice that the major second  belongs to both C major and C minor. This interval will not change between the scales.  The next one however will. This one C to E is you guessed it a major third.  But notice that C to E flat is a half step smaller than C to E.  This is what's called a minor third. That could have something to do with why this is called the minor scale.  But what is really important here is the logic that this introduces.  Anytime you have a major interval and you want to make it minor,  just lower the higher note by one half step, making the overall distance from the first note  to the second a half step smaller. This works for all major intervals. If you make that distance  one half smaller, it will become a minor interval. Let's take a look at C to F.  C to F is known as a perfect fourth. Notice that this perfect fourth belongs to both  major and minor. Next we're going to take a look at C to G.  This is called a perfect fifth. Also notice that this belongs to the minor scale as well.  In fact, each major and minor counterpart, assuming they have the same starting note or root note,  we'll have their major second, perfect fourth, and perfect fifth in common.  And of course the octave. The other intervals are the ones that vary between the two scales.  Next let's take a listen to C to A.  This belonging to the major scale of course is a major sixth.  Since we have to differentiate between major and minor here, they are not the same between the two scales.  Let's take a look at what is in the minor scale and A flat.  Now knowing an A flat is a half a step lower than A. We have lowered the size of that interval  by a half step. Essentially turning that major interval into a minor interval.  And it also goes to show that this new minor sixth C to A flat belongs to the C minor scale.  Next let's take a listen to our seventh.  My astute students probably would have guessed that this is a major seventh, just completely correct.  Now examining the minor scale, we move down to a B flat.  That being a half step smaller than the previous interval is a minor seventh.  Now I would like to examine the chromatic scale which is our scale done entirely in half steps  and look at how each interval progresses to the next.  So starting on C and going up to D flat we have a half step.  This is called a minor second.  Knowing that a minor interval is one half step lower than a major.  It goes to follow that C to D is a major second.  C to E flat our minor third bringing it up a half step C to E our major third.  Next C to E perfect for C to G flat is a very special interval called a tri-tone.  We will be discussing this in its own section.  Next C to G a perfect fifth C to A flat a minor sixth.  C to A a major sixth C to B flat a minor seventh and C to B a major seventh of course  we have C to C our octave after that.  As you will find out in this course intervals have different functions.  So now we need to take a look at a bit of a more theoretical application of these intervals.  First let's start with our major interval and alter it to minor.  Here we have a C to an E.  Next let's lower that E to E flat giving us our minor third.  Next we're going to lower that E again but we're not going to lower it in call it D.  We're actually going to call it E double flat.  Yes I know that that's a little bit strange but now it becomes what's called a diminished third  and it will actually act differently because of this fact.  Calling it a D would make it a major second and then it would act like a major second.  Just like with our chromatic scale there are multiple ways to spell things in music and in fact  most of the things you're going to run into will have more than two names.  But always remember the name will imply the function.  Let's go the other way with this interval.  C to E our major third.  Well if you make that an E sharp and yes I know that's silly because that's really enough but  make it an E sharp we now have an augmented third.  So looking at the logic of this major a half step larger will make it augmented.  A half step smaller will make it minor and a half step smaller further will make it diminished.  Perfect intervals work essentially the same.  Here we have C and G our perfect fifth.  If we lower that G to a G flat we go directly to diminished.  And if we raise it to a G sharp we go directly to augmented.  The logic any perfect interval lowered by a half step is diminished.  Any perfect interval raised a half step is augmented.  Melody is also quite simple.  The most important thing to keep in mind about Melody is that we use scales  as our building blocks.  Melody is quite simply a single line of music.  You can imagine someone humming or someone singing.  Even playing a single line on the guitar.  This defines Melody.  Later we'll talk about harmony which is basically multiple melodies played at once that work together.  First let's derive the F major scale and create a melody.  First our root note of F then a whole step to G.  A whole step to B. A half step to B flat.  Whole step to C, whole step to D, whole step to E and another half step brings us back to F.  Next you'll see the scale we just derived F major listed with its scale degrees.  Melody is fairly easy to sculpt out of our scale.  For the purposes of this series I'm not really teaching artistic tastes.  I'm more or less teaching you how to spell things out.  It will be up to you to figure out what you like to hear.  But that being said let's pick a couple scale degrees and piece together a melody.  We're going to start with F hour one.  Next we're going to move to B flat hour four.  We're going to go ahead and play that B flat twice.  Then it'll move down to A hour three.  C hour five, then back to one.  Next let's take the C major scale and play the same succession of scale degrees.  You'll see the C major scale labeled with its scale degrees above it.  One, four, four, three, five, one.  This produced a similar sound just starting on a different pitch.  Now let's try it using A minor.  One, four, four, three, five, one.  When you use a scale to build a melody or chord progression,  that's called playing in a key.  So our first example was in the key of F major.  Then the key of C major for a second example, and A minor for our third.  Cords, like everything else in music theory,  are quite simple, and they're just based off of patterns.  The first chord we're going to build is called the A major chord.  And you guessed it, it's built off of the A major scale,  which you now see on the screen with its scale degrees.  Cords are incredibly simple to build.  You take the root note in this case A, and then you do every other note  until you have three notes.  For triads, there are other chords that have more notes than three.  This section is focusing on what's called A triad.  This triad is called A major.  Take a listen.  This works exactly the same when you use the A minor scale,  which is the same thing as in the key of A minor.  This pattern is movable to all scales.  So if I wanted a B minor chord, all I have to know is the B minor scale.  Next, let's take a look at the key difference between major and minor chords.  On the screen, transposed over the chromatic scale this time.  You see the A minor chord, as well as the A major chord.  Notice the only difference is that middle note, our C, and C sharp.  If you remember when we were talking about intervals,  we pointed out that the 2, 4, and 5 remain consistent between major and minor scales  as long as they start on the same root note in this key.  A.  This is also reflected in these chords.  The spacing in the chords give it its flavor.  This relationship holds true for all major and minor triads.  If you have a major triad, you want to make it minor.  Just lower that middle note by a half step.  This also works the other way around, making minor into major, you raise it by half step.  Now we're going to work through chord progressions.  A chord progressions pretty simple.  We're going to be building multiple chords based off of the same scale.  There we're going to be progressing from one chord to the next.  This example we're going to use A major, which you'll see on the screen,  arranged four times in a grid like pattern.  Above it are the scale degrees.  Let's build our first chord.  You've probably guessed what that chord's going to be.  That is our A major chord.  In this case, we're going to call it our one chord.  Because, quite simply, it's built off of the first scale degree.  Cords when named in context of their scale are generally named for the root note.  In this case, A, or our first scale degree.  Let's build a chord off of the fourth scale degree, D.  We're going to do this the same way.  We're going to take every other note after the root note.  So we have D, F sharp, and you'll notice we've kind of run out of room here.  But A does come after G sharp.  So we'll use A, and it's totally fine to use the one back there at the beginning.  As long as we recognize that four is our root.  Take a listen.  Next, let's use the second scale degree to build our next chord.  This would use the notes B, our root, the second scale degree, D, and F sharp.  Again, just taking every other note until we have three.  Take a listen.  Our next chord is going to be built off of the fifth scale degree.  E.  G sharp.  And again, we're out of room, so after G sharp is A, and that's not, you know, every other note.  So we'll actually have to take the B, so let's take that B as well.  Now let's take a listen to the whole progression.  There are a multitude of ways to play a chord progression like this.  And something that's really fun is to take a look at these types of grids and pull some melodies out of them.  I'm going to set up a three part harmony using this chord progression between a cello, a viola, and a violin.  The cello's line will be in red, and he'll take up the notes to the left.  The viola will be in green, and the viola will take the notes in the middle, and the violin will be in black, and it'll take the notes to the right.  I'm going to have the first melody play, then the first two, then the first three.  Let's take a closer look at the Roman numerals we used in the last video.  Remember, we were using the scale A major, and we have the chord one, four, two, and five.  First I'd like to point out that two is lower case.  That just means it's minor, and logic would lead us to upper case meaning major, which is actually the case.  So let's take a look at these chords over the chromatic scale and examine the spacings, which are what make them major and minor.  Notice that the two chord, which is minor, has a note in the middle that is a half a step closer to the root in terms of musical distance.  This should remind you of our interval section. Remember, if you want to take a major third and make it minor, you just lower it a half step.  It works the same way with chords, so if you take a major chord and lower that middle note a half step, you'll get a minor chord.  It's also important to note that this pattern holds true for all major scales, so in all major scales the one is major, the two is minor, the three is minor, the four is major, the five is major, the six is minor, the seven is what's called diminished and we'll cover that later.  Minor scales also have their own patterns of Roman numerals, so if you build chords based off of a minor scale instead of a major, the one will be minor, the two will be diminished, the three will be major, the four will be minor, the five will be minor, the six will be major, the seven will be major.  Let's take a listen to our chord progression using the A minor scale instead of the A major scale. Then we're going to use a couple different scales after that.  The tritone is an incredibly interesting as well as dissonant interval. The tritone comes in the form of either an augmented fourth or a diminished fifth. Let's take a look at the C major scale above the chromatic scale.  Notice our tritone C to F sharp lies between the fourth and fifth of the C major scale. So if we took our perfect fourth C to F and augmented it a half step, we would get an augmented fourth.  Or if we took our perfect fifth C to G and spelled that F sharp as a G flat, we would have a diminished fifth. This is just the same interval, really just spelled two different ways.  Another way of looking at the tritone is to take any note and just quite simply go up three whole steps thus the word tritone three whole tones.  The tritone serves as a function. It's extremely unstable sound wants to move and either note will either move outwards or inwards. Let's take a look at the chromatic scale again and examine the two ways that this tritone can resolve.  In both cases the interval resolved where each note moved opposite of the other. So if the root note moved down then the upper note moved up or if the root note moved up the upper note moved down.  Another interesting note about the tritone is the fact that it is perfectly symmetrical.  First notice that from the first B to the next is six whole steps or whole tones and the tritone consists of three of those.  B to F. Notice that on either side B to F or F up to B we have the same amount of distance whereas all of the other intervals if I were to go from B to C sharp for example there's a different amount of distance on either side of the interval.  On the screen you will see the C major scale. Notice our tritone B to F is circle of black and belongs to the scale. In fact all major scales and actually all scales and modes contain one tritone.  First we're going to examine the tritone contained in C major and then we'll point out tritones as we move through the rest of this course.  So knowing that the tritone likes to resolve we have a clear path to resolution here there's a half step on either side of this tritone.  Let's take a listen to the resolution and then experiment with what kind of chords we can build with the scale degrees that the tritone leaves us.  On the screen you'll see the C major scale. Also I've circled the C into E after our tritone resolved it left us with C into E.  On the first line it's easy to complete the one chord by taking C E and G.  On the second line we can actually use the A as our root.  Doing every other note from A gives us A C and since we've run out of room here you know the next note up from C is D and a note after that is E.  So that gives us A C and E for our six chord.  In this section we're going to examine more chords and now we're going to actually examine their functions.  So the first chord we're going to look at is called a diminished chord.  We're going to get there by starting with a B major chord B D sharp and F sharp.  If you want to derive this chord on your own just write out the B major scale and do what we've done before taking every other note from B.  Next as we already know to make B minor we're going to lower that D sharp down to D B D F sharp.  Now to make diminished we're just going to take the F sharp the fifth of that chord and we're going to lower it to F natural.  Notice that we have that tri-tone from the last video here and we know that tri-tone resolves to C and E.  So let's resolve this B diminished chord to a C major chord.  Sounds great.  That is the function of the tri-tone within that diminished chord and this tri-tone is actually found in other chords.  Let's take a look at dominant chords.  On the screen you'll see the C major scale.  Please note that I'm actually starting this scale on the fifth scale degree and you'll see the scale degrees labeled up top there.  We are going to build our dominant chord in this key so first off a dominant chord is built off of the fifth scale degree.  And we're actually going to take four notes instead of three so let's do every other note until we have four notes.  As you can see we have the B and F our tri-tone again.  And a very clear resolution so the B wants to move a half step to C and the F wants to move a half step to E.  So let's take a listen to this chord.  Also note the seven over by the Roman numeral.  Quite simply if you look below this scale if we pretend G is our quote unquote new one just for a moment will notice that we have a seven up top there.  Any time you see a number in subscript to these Roman numerals that indicates the intervals above the lowest note or our base note.  Next let's examine the major seven chord.  Notice we are now using the G major scale and we're actually going to be using the one as our root note here.  So just like the dominant seventh chord that we just built we're going to every other note from the base which is our G until we have four notes.  This chord does not have a tritone in it and it has a much different sound than the dominant seventh chord.  You would notate this one one major seven because our root is G the one the root of the chord.  Notice that we have a major seventh in this chord from the G to the F sharp.  Whereas the other chord where we had an F natural we had a minor seventh knowing that taking a major interval G to F sharp and lowering it a half step makes it minor.  Next let's examine what is called the sus four chord.  The sus four chord is quite simple it's another three note chord just like our original triad except you trade that three for guess what the four the sus four.  Take a listen.  Sus two chords work basically the same way take that three and switch it out for a two take a listen.  Next let's examine augmented chords augmented chords act kind of like diminished chords except instead of lowering the third and the fifth we're actually just going to raise the fifth.  So using the G major chord we're going to take that D and raise it up a half step to D sharp neck gives us an augmented chord.  Next let's examine chords with notes added on past the octave.  So remember at the beginning of the course I mentioned that sometimes when you come back up to the one and octave later you call it an eight.  And you actually call it an eight so that the numbers past it can be called numbers greater than eight so two is now nine.  Three is now ten and so forth let's take a look at a one nine chord in G major.  Quite simply we're just going to add an A and but this A is going to be an octave above that base note.  Take a listen.  Now let's say we call it a flat nine chord well that would just mean we take that nine and we flat it.  Take a listen.  This logic can be used to make one thirteen chords flat thirteen chords and so forth.  Any number above eight can be added however you will not often see a ten chord because ten is the same is three and three already belong to the chord.  So you won't actually specify an extra note unless it does not belong to the original chord regardless of its octave.  Let's examine diminished chords with sevenths added on to them.  First we're going to start off with G minor you'll see the scale on the screen and also the G minor chord is circled.  We're going to add the seventh of this scale on to the chord so now we have a minor seven chord simple.  Next to make that minor chord diminished we're going to take the fifth.  And we're going to lower it a half step giving us G B flat and D flat.  This is a diminished seventh chord but it's only half diminished.  To make it fully diminished we're going to have to take that seventh and lower it still to an F flat.  Now you might think that's strange because E and F there's only a half step and really there's no such thing as an F flat but in this case.  Theoretically we have an F flat because E would be six over G and we could not call it a seven.  So what we have is our third the diminished fifth as well as the diminished seventh a diminished seventh being a half a step smaller than the minor seventh.  Inverting an interval is pretty simple.  Essentially you take the lower note of the pair and you make it the higher note so you move it an octave higher.  Let's use the C major scale to display this.  We have C to E our major third.  Let's take that C and move it up in octave.  Now we have something that sounds different.  This is actually a minor sixth.  If we were to pretend that the E is our new one you'll see that that C is actually six notes above the E.  And I happen to know that in the E minor scale we have the note C.  It follows that all major thirds become minor sixths.  In fact every interval has a kind of partner.  All major intervals will become minor.  Thirds will become sick.  seconds will become sevens, fourths will become fifths, and vice versa. All right, let's  invert some intervals. Starting with a major second, we're going to use the C major  scale. So C to D is our major second. If we invert that D to C, well, our major  interval should become minor, and our second should become a seventh. So let's check  the D minor scale for this minor seventh since D is now our technically our root. As you  can see, it in fact does belong to the D minor scale, inverting our major second gave us a  minor seventh. Now let's take the minor third D to F and invert it. So our minor should  become major, and our third should become a sixth, a minor third to a major sixth. Let's  check the F major scale F being our new root for our major sixth F to D. As you can see, that  relationship holds true. Now let's invert our fourth. Using this F major scale, we get F to be flat  our fourth. Now perfect intervals when they are inverted remain perfect. So we should get a  perfect fifth out of this one. Let's try it. Using the B flat major scale, we see that we have a  perfect fifth between B flat and F. So that inversion works too. Next let's invert a major sixth.  B flat to G is our major sixth. So if we take G and now make that our root note,  using the G minor scale, you'll see that our major sixth just became a minor third,  which holds true to the relationships we have previously examined.  Lastly, we're going to invert our minor seventh here in the G minor scale. So G to F is our minor seventh.  minor seventh belong to the minor scale. If we make F our root note and check the F major scale  for a major second, we'll see that our minor seventh minor became major seventh became a second.  minor seventh. Inverting chords is pretty simple. Now it works very similar to  inverting intervals. You take the lower note, the base note, and you move it up so that it's  no longer the lowest. Let's take a look at the C major scale as well as the C major chord,  our first triad. This is what's called root position. It's actually not inverted at all.  But if we take that C and we move it up an octave higher, the E will now be our base note.  The numbers on top represent the scale degrees and the numbers on bottom represent the distance  from the base note. So E is now our base note here and it's still the third scale degree.  Also notice that the Roman numeral one now has a sixth next to it. This is how you represent  first an inversion. And the sixth is actually simple. It's kind of the same concept as a seventh chord,  but there is an interval of a sixth from the base note E to C being sixth.  When we do second inversion, we're also going to take the E and move it up an octave.  Now our one has a four and a six. Take a listen.  Next let's invert a seven chord. Let's invert the five seven in this key.  Notice we're building this chord off of the fifth scale degree.  First let's take the G and move it up an octave.  Notice we have a sixth and a fifth above the base. Next let's take that B and move it an octave up.  When you put a seventh chord in second inversion, you're going to label it a  four three of course because we have four and three over the base note.  Now let's put this in third inversion where we move the D of an octave and the seventh of the  chord is now the base note. In this inversion we have a four and a two and our F is in the base.  This circle of fifth shown on the screen here is actually quite simple.  Starting on C we'll take the C major scale. We'll move to G because G is the five of C.  Then we move to D being the five of G, A which is the five of D, then E which is the five of  A, then B which is the five of E and so forth and eventually it'll take us all the way back around to C.  I'd like to start by deriving the C major scale which you'll see here on the screen.  Now we're going to take a look at what happens when you derive a scale, a fifth up from there.  So our five became our new one. That's always going to happen when you scale a fifth above using that fifth as the root note.  But also notice that what was once our four the F is now sharp and it became this seven.  That's going to happen every time I go to the next scale up in the circle of fifths.  The five of G is D. So let's take a look at the D major scale.  Notice just like last time what was once a four our C is now sharp and it's the seven.  A C sharp. We also kept the F sharp from the last scale.  The five of D is A. So let's take a look at the A major scale.  Notice our four in D which was G is now sharp as the seven in A and we also kept our F sharp in C sharp.  We're going to keep each sharp as we gather them and we're going to gather them one at a time.  The five of A is E. So let's take a look at the E major scale.  You'll notice our four which was D is now our seven as D sharp and we kept the accidentals from before.  Next we're going to move to the five of E which is B.  Notice our four which was A and E is now our A sharp which is seven in B and we've kept the accidentals from before.  Let's move on from B to the five of B which is F sharp.  Now our four which was E and B is now E sharp in the key of F sharp. Now I know E sharp's  kind of silly because that really is just an F right but if we spelled it as an F we would have two F's  in no E. So we're going to spell it as an E sharp which actually is a half step below F sharp.  Moving up to C sharp our old four which was B is now sharp B sharp and it's the same concept.  Now we have all of our sharps and if we're to move further we'd start getting double sharps and  that's just the headache. So let's not do that. We're going to actually rewrite C sharp as D flat  and take the D flat major scale. Now we have a much more manageable scale.  So the five of D flat is A flat and if we derive the A flat major scale that four is going to  be sharp and it will become the seven but notice that the flat is just cancel out so we don't get  a G sharp we get a G natural. Then the five of A flat is E flat and again the four which was our  D flat is now a D. It being sharp. The five of E flat is B flat and again our four is now our seven  and it's been sharp so we get from A flat to A natural. The five of B flat is F and of course the four  is now the sharp sevenths our E flat becomes an E. Next our five is C and that's right where we started.  Thus the circle of fifths. This introduces us to the most important thing key signatures.  Key signatures will always tell you what accidents belong in each scale. So first let's  start off with the order of sharps F sharp C sharp G sharp D sharp A sharp E sharp and B sharp.  To help with the use of acronym to memorize this I used to find classical guitarists demand  accurate execution because there's a million of them on the internet use whichever one you like.  So in order to figure out the key signature of any given key  take its root note. So in this case let's say at the key of D and go a half step lower you get a C sharp.  That C sharp will be the last sharp that you see in the key signature so the the key of D has an F sharp  and C sharp. If we pick E and go down a half step we'll get D sharp F sharp C sharp G sharp D sharp.  Okay let's take a look at the order of flats. If I want to find any key signature for a flat scale  so a scale with any sort of flat root note let's use B flat for this example. I'll find B flat  on my order of flats and that will be the second to last flat that I see. So I'll gather B and E flat.  So if I wanted to do this with A flat I'd have B flat E flat A flat it being the second to last  then D flat. The only exception to these two tricks is the F major scale which contains only a B flat  and since it's the odd man out that one's kind of easy to remember.  When playing or composing music you don't always use the same scale the whole time and in fact  it's very common to borrow from other scales. So let's start by using what's called the secondary  dominant. It's pretty simple. What happens is if you want to create more pull to its accord  that isn't necessarily your one chord you're going to borrow a tri-tone from another scale  and use that resolution to pull you to that chord. On the screen you'll see a C major chord  progression with all of the right inversions labeled next to the Roman numerals.  We're going to want to create more pull towards our five chord for whatever reason be  an artistic or just as an exercise. In order to do that we're going to make a tri-tone  resolve to our G chord and we're going to take that tri-tone from the G major scale.  The tri-tone in the G major scale is F sharp and C. Let's go ahead and put that F sharp and C  in the two chord and see what happens. The F sharp and C circled in black here  will resolve to G and B. The F sharp going a half step up and the C going a half step down.  Listen to the resolution. Now they wouldn't resolve to other notes because the notes on the other  side of these notes are more than a half step. So if we went F sharp down to E that's a whole step  and C up to D that's a whole step as well. So these things resolve by a half step.  Take a listen to the whole chord progression.  Beautiful. Also notice that the two's Roman numerals has changed to five seven of five.  And that works pretty simply. D being the root note of that two chord is actually the five of  G, which happens to be the five in our key. So if we were to pretend G is our new one just for a moment,  D would be the five. So the five of five creates a pull to the five. And it does so by using the  right tone from the five G major scale. Now let's harmonize a melody that borrows from another key  as opposed to creating a tritone for this purpose of resolution. You'll see our melody, C, D, E,  D borrows E flat from C minor. So we're in C major, but we're borrowing one note from C minor, the E flat.  Let's make that E flat harmonize with an E flat major. Notice the flat three Roman numerals.  That Roman numeral is capitalized, so we're going to be using E flat major.  The easiest way to derive this chord without thinking about it too much is to say, okay E flat major  chord, that means I'm going to build it off my E flat major scale.  At that point all I need to do is think about my E flat major key signature, which has a B flat  and E flat and an A flat. So taking every other note from the E flat, we have E flat  G B flat. I'm going to go ahead and fill in the rest of the notes of this chord progression.  Take a listen.  Another way to modulate or borrow is to use a common chord between two keys and use it as a pivoting chord.  So you look at the diagram on the screen. You'll notice that the first two rows are C major,  and the second two rows are G major, so we're actually trying to switch keys all together.  This is referred to as modulation. So in order to do that, we're going to use a chord that is  common between both keys. And in this case, that would be E minor. There's a couple other  common chords, but E minor is the one I'm picking. So by using E minor as our three chord in C,  but our six chord in G, we're going to pivot to the key of G major, and then do the B minor chord,  which does not belong in the original key of C major.  Take a listen.  Modes are pretty much just as simple as everything else. There are different kinds of scales,  but they're very easy to derive. So the easiest way to derive Modes is based off of  key signatures. So if I think about my G major key signature, it tells me all of the notes and G major,  and we have one F sharp in that key signature. So if I start an end on a different note,  other than G, I'll be playing Modes. On the screen, you'll see all of the Modes names listed next  to the scale degrees that they start on. The first one, G I O N is another example of how we have  multiple names for things in music. This is really just the G major scale just a much older name  to that scale. Take a listen. The next mode, the Dorian mode, is called A Dorian, because it's the  next, the Frigian pattern. This one is B Frigian. B is the third scale degree. And Frigian  is the mode you get when you start on the third scale degree of a major scale. Take a listen.  Next, my personal favorite, which is Lydian, starting on the fourth scale degree. In this case,  see Lydian. Take a listen. Next, this mix Lydian. Starting on the fifth scale degree,  this would be D mix Lydian. And actually mix Lydian is very popular in the blue style.  Moving on to A O Lydian, which actually is the same as natural minor. So, this is E A O Lydian,  but you'll find that this is exactly the same as E minor. Take a listen.  Next, a very dark mode, Lydian. This is F sharp Lydian. Lydian will always start on the seventh  scale degree of any major pattern. Take a listen.  As you can see, it's pretty easy to figure out any mode based off of key signatures,  but it's also very, very, very helpful to understand how to alter a major scale to become a mode.  So, if I'm playing in C major and I want to switch to C Dorian, for example,  there is something I will need to do to that scale to make it Dorian. And I can't necessarily  go find the right key signature if I have to think quickly. So, let's take all of the modes that we  just observed and then compare them to the major scale starting on the same root note.  We're going to leave Ionian in A O Lydian out because these are just major and minor and we already know  how to derive major and minor scales. But for the other modes, let's take a look at what we  alter in order to get a mode from its major scale. So, first, let's take a look at A Dorian.  First, notice A Dorian is on top. Second, A major is below.  And order to make A major A Dorian, we will have to flat the three the C sharp to see natural  as well as the seven the G sharp to G natural. So, to take any major, any major scale and make it Dorian.  When you compare B frigion to B major, you'll notice that you have a flat two C sharp to see natural,  flat three that D sharp down to D natural, as well as a flat six and a flat seven the G sharp  being flat it down to natural each as well. So, if you flat the two three six and seven,  you will get the frigion mode. In Lydian, you'll see that you have a sharp four  that F being raised up to F sharp for our mode. Mix a Lydian, starting on the fifth scale degree,  we have a flat seven. In this case, that C sharp coming down to a C natural.  And finally, with the  We see that we have to flat the 2, the 3, the 5, the 6, and the 7.  This is by far our most heavily changed mode, which is why it has such a dissonant sound.  Don't forget to text, I love music to 4, 4, 2, 2 to 2 to receive your free music and guitar  ebook.  Or visit music and guitar lessons.com for more awesome lessons."
    
    const result = await contentGenerationClient.generate(text)

    if (result != null) {
      const tagsList = (result.tags ?? '')
        .split(',')
        .map((tag) => tag.trim())
        .filter((tag) => tag !== '')

      setTags(tagsList)

      const description = result.description ?? ''
      setDescription(description)
      form.setFieldsValue({ description })

      notification.success({
        message: 'Content Generation successful',
        description: 'Your lecture content has been generated successfully',
        placement: 'topRight',
        duration: 2
      })
      console.log('Content Generation successful')
    } else {
      notification.error({
        message: 'Content Generation failed',
        description: 'Your lecture content could not be generated',
        placement: 'topRight',
        duration: 2
      })
      console.error('Content Generation failed')
    }

    setGenerating(false)
  }

  const handleSubmit = async () => {
    if (!file) return

    try {
      const fileId = await handleChunkedUpload(file)
      const duration = await getFileDuration(file)

      const uploadDTO = new UploadDto()
      uploadDTO.init({
        id: null,
        ownerId: ownerId,
        date: new Date(),
        type: file.type,
        libraryId: 1
      })

      const uploadID = await uploadClient.addUpload(uploadDTO)

      const lessonDetailsDTO = new LessonDetailsDto()
      lessonDetailsDTO.init({
        id: uploadID,
        title: title,
        description: description,
        tags: tags
      })

      const fileMetadataDTO = new FileMetadataDto()
      fileMetadataDTO.init({
        id: fileId,
        fileType: file.type,
        fileName: file.name,
        fileSize: file.size,
        duration: duration === null ? null : Math.round(duration),
        date: new Date(),
        checksum: null
      })

      const fileMetadataArray = Array.isArray(fileMetadataDTO) ? fileMetadataDTO : [fileMetadataDTO]

      const lessonDTO = new LessonDto()
      lessonDTO.init({
        uploadId: uploadID,
        lessonDetails: lessonDetailsDTO,
        fileMetadata: fileMetadataArray,
        ownerId: ownerId,
        ownerName: ownerName
      })

      await lessonClient.addLesson(lessonDTO)

      notification.success({
        message: 'Upload successful',
        description: 'Your lecture and file have been uploaded successfully',
        placement: 'topRight',
        duration: 2
      })

      console.log('Upload successful')
    } catch (error) {
      notification.error({
        message: 'Upload failed',
        description: 'Your lecture could not be uploaded',
        placement: 'topRight',
        duration: 2
      })

      console.error('Upload failed', error)
    }
  }

  return (
    <div className="form-container">
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        initialValues={{ description: description }}
      >
        <Form.Item
          label="Title"
          name="title"
          rules={[{ required: true, message: 'Please input your title!' }]}
        >
          <Input
            maxLength={100}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Enter Title Here"
          />
        </Form.Item>

        <Form.Item
          label="Upload File"
          name="file"
          rules={[{ required: true, message: 'Please upload a file!' }]}
        >
          <Upload.Dragger beforeUpload={handleFileChange} accept=".mp4,.mov,.avi,.wmv" maxCount={1}>
            <p className="ant-upload-drag-icon">
              <UploadOutlined />
            </p>
            <p className="ant-upload-text">Click or drag file to this area to upload</p>
            <p className="ant-upload-hint">Supported file formats: mp4, mov, avi, wmv</p>
          </Upload.Dragger>
        </Form.Item>

        <Form.Item
          label="Description"
          name="description"
          rules={[{ required: true, message: 'Please input your description!' }]}
        >
          <TextArea maxLength={100} disabled />
        </Form.Item>

        <Form.Item label="Tags" name="tags">
          <Tags tags={tags} setTags={setTags} />
        </Form.Item>

        <Form.Item label="Content Generation" name="contentGeneration">
          <div className="upload-button-wrapper">
            {generating ? (
              <Spin tip="Loading..." />
            ) : (
              //Need to put it in generateContention function after implentation of mp3 to text
              /*file as File*/
              <Button
                type="primary"
                className="upload-btn"
                icon={<UploadOutlined />}
                onClick={() => generateContention()}
              >
                Generate Content
              </Button>
              
            )}
          </div>
        </Form.Item>

        <Form.Item>
          <div className="upload-button-wrapper">
            {uploading ? (
              <Progress percent={uploadProgress} status="active" />
            ) : (
              <Button
                type="primary"
                htmlType="submit"
                className="upload-btn"
                icon={<UploadOutlined />}
              >
                Submit
              </Button>
            )}
          </div>
        </Form.Item>
      </Form>
    </div>
  )
}

export default UploadPage
