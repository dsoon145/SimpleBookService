using Newtonsoft.Json;
using SimpleBookService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SimpleBookService.Controllers
{
    public class BookController : ApiController
    {
        /// <summary>
        /// Used to reutrn all books currently in the library.json
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/book")]
        public HttpResponseMessage GetAllBooks()
        {
            //Using library.json to store the books
            var path = string.Format("{0}Data\\library.json", AppDomain.CurrentDomain.BaseDirectory);

            if (File.Exists(path))
            {
                var currentBooks = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(path)) ?? new List<Book>();
                return Request.CreateResponse(HttpStatusCode.OK, currentBooks);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new List<Book>());
        }

        /// <summary>
        /// Used to get a single book by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/book/{id}")]
        public HttpResponseMessage GetById([FromUri] int id)
        {
            var path = string.Format("{0}Data\\library.json", AppDomain.CurrentDomain.BaseDirectory);

            if (File.Exists(path))
            {
                //return the book with a matching ID
                var currentBooks = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(path)) ?? new List<Book>();
                var selectedBook = currentBooks.Where(x => x.BookId == id).FirstOrDefault();


                if (selectedBook != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, selectedBook);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new Book() { BookId = 0 });
        }

        /// <summary>
        /// Used to create a new book
        /// </summary>
        /// <param name="bookRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/book/create")]
        public HttpResponseMessage Post([FromBody] Book bookRequest)
        {
            var path = string.Format("{0}Data\\library.json", AppDomain.CurrentDomain.BaseDirectory);

            if (File.Exists(path))
            {
                var currentBooks = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(path)) ?? new List<Book>();

                //Set ID to [last bookID + 1] unless 1st book
                if (currentBooks.Count > 0)
                {
                    bookRequest.BookId = currentBooks.Count + 1;
                }
                else
                {
                    bookRequest.BookId = 1;
                }

                //Set the registration date
                bookRequest.TimeStamp = DateTime.UtcNow.ToString("d");
                currentBooks.Add(bookRequest);

                //Serialize and write to file
                var updatedJson = JsonConvert.SerializeObject(currentBooks, Formatting.Indented);
                File.WriteAllText(path, updatedJson);
            }
            else
            {
                bookRequest.BookId = 1;
                bookRequest.TimeStamp = DateTime.UtcNow.ToString("d"); ;

                var updatedJson = JsonConvert.SerializeObject(new List<Book>() { bookRequest }, Formatting.Indented);
                File.WriteAllText(path, updatedJson);
            }

            return Request.CreateResponse(HttpStatusCode.OK, $"CREATED NEW BOOK => BOOKID: {bookRequest.BookId}");
        }

        /// <summary>
        /// Used to update an existing book
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="updateBookRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/book/{bookId}/update")]
        public HttpResponseMessage Put([FromUri] int bookId, [FromBody] Book updateBookRequest)
        {
            var path = string.Format("{0}Data\\library.json", AppDomain.CurrentDomain.BaseDirectory);

            if (File.Exists(path))
            {
                var currentBooks = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(path)) ?? new List<Book>();
                var updatedBook = currentBooks.Where(x => x.BookId == bookId).FirstOrDefault();

                //Update values if book found
                if (updatedBook != null)
                {
                    updatedBook.BookName = updateBookRequest.BookName;
                    updatedBook.Author = updateBookRequest.Author;
                    updatedBook.Category = updateBookRequest.Category;
                    updatedBook.Description = updateBookRequest.Description;

                    var updatedJson = JsonConvert.SerializeObject(currentBooks, Formatting.Indented);
                    File.WriteAllText(path, updatedJson);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, $"Could not find the library.json");
            }

            return Request.CreateResponse(HttpStatusCode.OK, $"UPDATED BOOK => BOOKID: {bookId}");
        }
    }
}
