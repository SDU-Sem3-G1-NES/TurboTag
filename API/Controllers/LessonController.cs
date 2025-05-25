using API.DTOs;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LessonController(ILessonService lessonService) : ControllerBase
{
    [HttpPost("GetAllLessons")]
    public ActionResult<IEnumerable<LessonDto>> GetAllLessons([FromBody] LessonFilter? filter)
    {
        return Ok(lessonService.GetAllLessons(filter));
    }

    [HttpPost("StarLesson")]
    public ActionResult StarLesson([FromBody] int lessonId, int userId)
    {
        lessonService.StarLesson(lessonId, userId);
        return Ok();
    }

    [HttpPost("UnstarLesson")]
    public ActionResult UnstarLesson([FromBody] int lessonId, int userId)
    {
        lessonService.UnstarLesson(lessonId, userId);
        return Ok();
    }

    [HttpGet("GetLessonsByTags")]
    public ActionResult<IEnumerable<LessonDto>> GetLessonsByTags([FromQuery] string[] tags)
    {
        return Ok(lessonService.GetLessonsByTags(tags));
    }

    [HttpGet("GetLessonsByTitle")]
    public ActionResult<IEnumerable<LessonDto>> GetLessonsByTitle(string title)
    {
        return Ok(lessonService.GetLessonsByTitle(title));
    }

    [HttpGet("GetLessonById")]
    public ActionResult<LessonDto> GetLessonById(int lessonId)
    {
        var result = lessonService.GetLessonById(lessonId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("GetTranscriptionByObjectId")]
    public string GetTranscriptionByObjectId(string objectId)
    {
        var result = lessonService.GetTranscriptionByObjectId(objectId);
        if (result == null) return "";
        return result;
    }

    [HttpGet("GetLessonByObjectId")]
    public ActionResult<LessonDto> GetLessonByObjectId(string objectId)
    {
        var result = lessonService.GetLessonByObjectId(objectId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("GetLessonByUploadId")]
    public ActionResult<LessonDto> GetLessonByUploadId(int uploadId)
    {
        var result = lessonService.GetLessonByUploadId(uploadId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("AddLesson")]
    public ActionResult<int> AddLessonAndTriggerGeneration([FromBody] LessonDto lesson, string id)
    {
        lessonService.AddLesson(lesson);

        var transcription = lessonService.GetTranscriptionByObjectId(id);
        if (!string.IsNullOrWhiteSpace(transcription))
        {
            var httpClient = new HttpClient();
            var payload = new
            {
                uploadId = lesson.UploadId,
                text = " Hi guys, my name's Andy Crawley, thank you for joining me for day one of this 10-day guitar challenge where we're going to play guitar for 10 minutes a day for 10 days.  This day one is my absolute beginner's first lesson, so if you were taking a 1-to-1 private lesson with me and you'd never even held a guitar before or you'd tried a few bits in the past but it was a long time ago  and this is the guitar lesson that I would be showing you.  In this lesson, I'm going to show you how to play the easiest two chords on guitar and then get you playing your first song by the end of this video in under 10 minutes.  So if you've never even played a song before, this is the ideal place to start and let's get you straight in for a close-up and learn how to play these two easy chords.  So the first chord we're going to be learning is the E major chord, also known as an E chord and this is where we need to play our fingers to be able to play it.  Now if your guitar isn't in tune, it doesn't really matter where you put your fingers on the guitar, it's not going to sound right.  There is a video of mine in the description below where you'll find a video shown you exactly how to tune your guitar with and without a guitar tune.  So check out that video first if you suspect that something isn't sounding right here, but I'm going to talk you through a couple of other bits of the guitar before we get fully started.  So these metal strips going down the guitar here are your frets and going across the strings.  We number the strings thinnest to thickest, so 1, 2, 3, 4, 5, 6 and each fret is the area between the metal strips that we'd call fret 1, fret 2 and fret 3.  Anytime we place a finger on a fret, we really want it to be to this side of the fret, so the side nearest to you, up against the metal strip, but not on it.  So if I wanted to place my finger on first finger, first fret, it would be placed here rather than in the middle and we also want to be right on the tip of your finger like this.  For this first chord, the E major chord, we need that first finger to be on the third string at that first fret.  So that's just here, right on the tip of our finger, and we are pressing between our thumb and first finger.  My thumb is directly behind where my first finger is on the guitar just here.  The middle finger needs to be at the second fret of the fifth string, so string 5, 1, 2, 3, 4, 5.  And the third finger, otherwise known as the ring finger, goes here, directly underneath at the same fret on the string below.  So just one recap, these fingers are placed, finger number 1 or the index finger, middle finger and the third finger here.  The little finger, it's best to keep it as close to the third finger as possible rather than shooting off and doing its own thing.  This is where we want to place it here.  For this particular chord, we do want to strum all six strings and make sure that they're all heard.  So let's do that now and strum our E major chord.  Now that sounds like it should sound and hopefully you'll sound the same.  If not your guitar may be out of tune, so check out that tuning video.  However, we also want to make sure that all six strings are ringing out by which we want to pick the thickest to the thinnest string so string 6 to 1.  And if we all sound like that then we're doing it correctly.  Possibly, one of the strings sounds a little bit like this.  We've got a little bit of either buzzing or it's not ringing out at all like this.  Now there are two common reasons that that can be the case.  The first thing is you need to be at this side of the fret as I said at the start of the video.  If then in the middle of the fret, that may not ring out.  They all want to be at this side of the fret, not at the far side.  And then each finger also needs to be right on the tip, not flat.  Now this is common because whenever we hold things, say if I was just holding this guitar neck to pick it up, we'd grip it like this.  But this is not how we press down notes on a string instrument.  We need to be right on the tips of our fingers and make sure that this joint in our fingers  is bent and kind of curled over so that each tip of the finger is at 90 degrees to the fret.  And when those two things are the case, when we're at this side of the fret and on the tips of our fingers,  it's just the case of getting that right and out of pressure down.  But with the right finger placement, it may be a little bit less than you think.  Now even I at this point have some lines on the end of my fingers.  This is normal. We do have to press down, you know, a decent amount to get the note ringing out,  but try not to press down any more than you need to try and find that sweet spot.  The second chord we're going to cover in this video is the air major chord.  And with this chord, we can play any one of ten songs, as I say, there'll be a song at the end of this particular video,  but there are a number of songs, ten songs that are played only using these two chords on my website  and the link to that is also in the description.  So to play the air major chord, it's best to start off on the e major chord that we already know.  Now we need to keep this first finger down, but lift the other two away.  And this is really important. This is going to be our anchor finger, because when we're learning chords,  the hard thing isn't necessarily playing one chord, it's the change between.  So what we're going to do is keep this first finger down, but slide it over to the second fret.  And this time we want to be around in the middle of the fret, because we're going to place the middle finger above it  at that same second fret and the third finger directly below it.  And this is the air major chord. You may have seen this chord played like this.  However, if this is the first time you've ever picked up a guitar before, it's going to be incredibly difficult to change between any chords and these included.  So we're making this as easy as possible by keeping contact with the fretboard at all times and using that first finger as an anchor point.  So this is the air major chord that we're going for. Let's place this first finger at the third string, second fret, one, two.  Middle finger goes directly above it and third finger below it. And it's best to keep that first finger around in the middle of the fret this time,  and this time on, they really, so that we can fit the other two in.  And with this particular chord, we want to strum from string five.  And this is what the air major chord should sound like.  To check that all those strings are ringing out, we want to pick again from the thickest to the thinnest, but we're going to start from string five.  There is more opportunity for strings not ringing out on this one. So again, try and get them as far to towards you as possible and keep right on the tips.  That is the best way to get them ringing out, but you may have to press on just a little bit harder on this chord to get those strings ringing out.  One more time strum, pick each string and strum.  So to change between those chords, we need to keep that first finger down at all times and change between them.  So if we go back to the E major chord now, and just give it one single strum of all six strings.  We keep the first finger down, slide to the second fret, middle finger above, third finger below, and strum.  And then to change back, the first finger stays down, slide it back to that first fret, middle finger above, third finger below.  And strum.  Again, so we're on that first chord that we look at now the E chord, so we keep the first finger down.  We slide it to the second fret for the second chord that we looked at, middle finger above, third finger below, and strum.  Try and strum from the fifth string, so missing out the thickest string.  If you do accidentally catch that thickest string, it's okay for now, we're just learning.  First finger stays down, we move back to the first fret for our first chord, which is the E major and strum and that side E major.  Now you may wish to pause the video here so that you can have a little bit longer practice between those two chords, changing between the E and the E chord.  Remember to keep your first finger down at all times.  And the first chord we looked at has your first finger on the first fret.  And then chord number two, the E chord, first finger second fret, middle finger above, third finger below, and strum.  And you're wanting your fingers to take around a second to go to E chord, to move on to the second part.  Which is basically going straight for our first song, which is a song called for what it's worth by Buffalo Springfield.  This song has just these two chords, and we have to play each chord for a bar of each.  To do this, we need to understand about bars and beats.  The beat is whatever you would nod your head to when your favourite song comes on.  So when you're kind of grooving along to a song and tapping your foot and enjoying it, you're tapping your feet on nodding your head to the beat.  This is an even pulse throughout the song, and it generally goes to account of four.  So an even count of one, two, three, four.  And that is repeated throughout an entire song, even me.  It's that count of four that we call a bar.  So the simplest strumming pattern we can do for any song is just strumming on the beat,  and we're going to strum each chord in this particular song for times.  So we strum the E chord four times, one, two, three, four.  And keep that first finger down when we're going to change to the second chord, which is the E chord.  And then strum this four times, one, two, three, four.  And then we need to change back to the E chord, and do this in a loop, four around a minute.  And if you've never heard that particular song that we're going for, the link is in the description to a YouTube video of the song,  so you can have a quick listen to it to hear what we're going for.  And you should pretty distinctly hear this rhythm guitar part throughout the entire song.  Now, keeping the strumming hand going, while changing chord, is undoubtedly the trickiest part of learning songs on guitar.  However, this anchor finger, keeping the first finger down, makes it a much easier task,  and helps us going forward to learn the other chords that we're going to be learning in the coming days.  So drilling this change is so important, rather than learning more chords.  So many beginners get a bulk full of 10 chords or 100 chords and try and learn them,  but it's mastering the change between them that's going to get you the end goal of being able to play it real songs as soon as possible.  And hopefully this will be one that you can play just in a few minutes now by following me.  So let's start with the E major chord, the first chord we covered in this video, and we're going to strum this four times.  And soon as we've done that, we want as soon as we strum that fourth strum, we want to change immediately to the E chord as swiftly as possible.  Now, if this takes a few attempts and that is totally fine because this is the end goal of this first video of this day one of this 10-day guitar course.  So if it's a little struggle at first, that's understandable because it might be the first thing that you've ever done.  Go easy on yourself, allow this trickiest part of this, the chord changes to bed in and for you to get used to them before you put too much pressure on yourself.  So let's play along together really slowly now, and let's have a go at playing our first song for what it's worth by Buffalo Springfield.  So we get ready on our E chord, we press down and we begin strumming in two, three, four, one, two,  three, four, then change to the E chord and begin strumming again.  One, two, three, back to the E, changing back and one, two, three, four.  First fingers, there's down, we change to the E, one, two, three, four, and pause there.  So now's the time to take stock with how you're doing. If you need to pause the video just briefly one time again to just do some individual changes, which will drill back change a little more often to help you get used to it more.  Maybe take a quick break to rest your fingers a little bit because you might have big lines and they might be quite sore.  That's understandable and totally normal in the beginning, and when you feel up to it, play this video from this point one more time.  And this time, we're going to try and get those spaces between the chord changes as quick as possible.  And the goal is just to keep our right hand strumming evenly.  No matter what chord we're playing, however there's a change or not, we're just going to try and keep this chord and strumming.  Okay? So from the E chord, play along with me one last time in one, two, three, four.  B, two, three, then two, and two, three, four.  B, two, three, four, and eight, two, three, four.  B, two, three, four, and eight, two, three, one last time from E.  B, two, three, four, and eight, two, three, finish on E.  And that's how to play our first song, and that is the end of day one of this 10-day guitar challenge.  So thank you very much for making it this far. Your homework now is to practice these two chords, the E major and the E major.  The changes between them, and then try and play them for four strings of the E chord, and the four strings for the E chord,  and we want to keep that chord sequence going for around a minute, and in total that should be around 10 minutes worth of practice.  So if you do that straight away after watching this video, you'll be in the perfect position to join me for day two tomorrow,  where we'll learn a new guitar chord, and a new song, and this time the song will have a very easy lead guitar part,  which is a bit more of a melody, so we won't just be focusing on chords in this course.  We'll also focus on some single string playing as well. It's going to be really cool. It's going to sound just like the song and be really, really recognizable.  Something great to show your friends, and I hope you will join me there. Thank you very much for watching guys.  Please subscribe if you enjoy this course, and I'm sure I'll see you again in one of my videos. Take care of yourselves and bye for now."
            };
            httpClient.PostAsJsonAsync("http://localhost:8001/generate-async", payload);
        }

        return Ok(lesson.UploadId);
    }

    [AllowAnonymous]
    [HttpPost("CompleteGeneration")]
    public IActionResult CompleteGeneration([FromBody] LessonCompletionDto result)
    {
        try
        {
            var lesson = lessonService.GetLessonByUploadId(result.UploadId);
            if (lesson == null) return NotFound();

            lesson.LessonDetails.Description = result.Description ?? "";
            
            lesson.LessonDetails.Tags = result.Tags?.Split(',').Select(t => t.Trim()).ToList() ?? [];

            lessonService.UpdateLesson(lesson);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error completing generation: {ex.Message}");
        }
    }

    [HttpPut("UpdateLesson")]
    public ActionResult UpdateLesson([FromBody] LessonDto lesson)
    {
        lessonService.UpdateLesson(lesson);
        return Ok();
    }

    [HttpDelete("DeleteLessonById")]
    public ActionResult DeleteLessonById(int lessonId)
    {
        lessonService.DeleteLessonById(lessonId);
        return Ok();
    }

    [HttpDelete("DeleteLessonByObjectId")]
    public ActionResult DeleteLessonByObjectId(string objectId)
    {
        lessonService.DeleteLessonByObjectId(objectId);
        return Ok();
    }
}

public class LessonCompletionDto
{
    public int UploadId { get; set; }
    public string? Tags { get; set; }
    public string? Description { get; set; }
}