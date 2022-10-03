using Firebase.Auth;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
//using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
//using Google.Rpc;
//using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Firebase.Storage;
//using Newtonsoft.Json.Linq;
//using System;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseSharedFunctions
    {
        FirestoreDb db;
        FirebaseAuthProvider auth;
        FirebaseStorage storage;

        public FirebaseEncryptor encryptor;

        public FirestoreDb DB()
        {
            return db;
        }
       
        public FirebaseAuthProvider Auth()
        {
            return auth;
        }

        public FirebaseStorage Storage()
        {
            return storage;
        }

        public FirebaseSharedFunctions(HttpContext context)
        {
            //TODO hide this in the future with ENV
            db = new FirestoreDbBuilder { ProjectId = "unisa-rt-mock-clinic", JsonCredentials = "{ 'type': 'service_account',   'project_id': 'unisa-rt-mock-clinic',   'private_key_id': '6aa0d9f7dc80f52392dd906a85d0f4f462432f52',   'private_key': '-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDP+Pn9NZkcvnxs\n7sTgbHBZJqlqK/wv5qVSdyQeUZuC4Dp1uFffRcGS2OSrq+v0CMgRxaBtF5ox4oca\nrk5+B0AqtDpuWLoKVOlAbzEjbdZgcV2WZccfxb1VqMMeNJRFF1g3O+idQIAzI0ax\nDH+RQoPaco6pc8LjfTG5NAfcyZQcSoYGyUgFVaMfmbay7DQwyvIsyY9uZ52ZPYWG\n3N2KZI69uKYVPKflgqOtzZuedAtDG2MMnyHe8Majjt/MtGFLKeI/hE+TpYvpn5c6\ncV33BpFyN7D1LP2pVCP83rtmCswWbZPfZL+Fpjnc/QANqzVVJb3SLIBWOmeEkuIi\n8dw60uaNAgMBAAECggEAAd04dD9Q9bibYYRSjXGkbj8ZCN65ihdVd/A9iaOvn2C8\nzcGV5wIhGjAq5qRrYe2CDIqO0NPcxESSc3J363iqH31vB8bJeVTBbL2EfsKsGx7Z\nth5858iRIxEeOsli0lKAl/eKlyTb82dOdN0dr29VU1An/hMxFJP5hvM8OcNSLxvD\nC8a+uiSWl0OBGnDNBulkmffZsdjiphPVooY/khLGpTxUN8ec7HZEhhlGvaPvvBPW\n6ywKxtjssblc22CkIkexVsnFqtf1ZkUSAKggnmv8aKImzKkkG+8PQcPiJNAp6FC9\nZ9bdXwrFDtrPU+V3gFCS4Z7yogU1fcaTM9wI0tjPoQKBgQD+poIAr4BYmVIaO86S\nSFifnurNfYOsTBEa8cn//i8W125LyV93Ee4ThCV4zgP9sT7HblnxT6U+mB+Hmkz7\nNeIfbYfL3RR+R7GFqkbYs8P5GPUP8FSsTMzyLvdMHF1TQz4G0Qbn+r1AgwXZcVdL\nuvNyE8MYKft2xAz8uCSKwL0ahQKBgQDREyOvKOKE0JadMLrE1wCaapUamHmLZiAO\nJsYzQjz/186FwKKyQyLAZSAbGdkzLK2U8ZSosoQZnrokeDxfmXoZniKqcFbuFIJi\ngyFhpfp6mK4GPhyD+bzsIoT3WX9Xwtrmv2OfudalY1hd6fNaapYomyYqTdoWUA88\nvxdLe/zOaQKBgQCaD2B9S7ApafC7AE3UQEKlpz5EvdfIiGic1YUxA7W3avRGk3jX\nD5jqY7tL38+YTwA9JWzyyg2d1ejVYCuMm6fG/bv3QTRhxbwHsuGTvwYkEM5KK0r+\nxqQDLRjeChcIBZlkBFfaRt7yRZJnX+PBZEReUshoORXyX1/AESPCciK2BQKBgD64\nCyBkl29YU5ZcI+sgxGGOT6Rm0S9sN3mHUDXYTQxC5QViwGvRj/8/Vt5KZsnfQUNJ\nJVtmEhLNdvGx0Aqts98zfRq8EJfjNynuRHlSnU1ht/LPdyZwKKh9wn2hL35YSeqm\nx3AHA8khgETMBeC90MXlpRFTwXSoF6oVeRt/2lrhAoGBAL8IsbiE9hKv7MUK1xR3\nrW194MfKocnFuAi+GdbhJjw6jG88/3l3nnVE+unz8ORP5Kj6m9XwISWrU9/H0KJ7\n/lJ8pafeBWsc8i+B+x2/vwZNjxGVzwpWzBLnHwjgsZ3qT1w2h3I7boAnn3Ezouwv\n5w2vKJXCIjteoyLxZ83Zao3w\n-----END PRIVATE KEY-----\n',   'client_email': 'firebase-adminsdk-vjevh@unisa-rt-mock-clinic.iam.gserviceaccount.com',   'client_id': '103711456653250716580',   'auth_uri': 'https://accounts.google.com/o/oauth2/auth',   'token_uri': 'https://oauth2.googleapis.com/token',   'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs',   'client_x509_cert_url': 'https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-vjevh%40unisa-rt-mock-clinic.iam.gserviceaccount.com' }"}.Build();
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyByVk8XwhbQGoeeqkcxr5vRJWhOjep5Ulc"));
            storage = new FirebaseStorage("unisa-rt-mock-clinic.appspot.com");
            encryptor = new FirebaseEncryptor("b14ca5898a4e4133bbce2ea2315a1916");
        }

        //********************************************************************************************************//
        //                                        User Verification Section                                       //
        //********************************************************************************************************//
        public void SetVerificationToken(HttpContext context, string UserToken, bool setCookie = false)
        {
            var verificationToken = GenerateVerificationToken(context, UserToken);
            context.Session.SetString("VerificationToken", verificationToken);

            if (setCookie)
                context.Response.Cookies.Append("VerificationToken", verificationToken, new CookieOptions { Expires = DateTime.Now.AddDays(15) });
        }

        public string GenerateVerificationToken(HttpContext context, string UserToken)
        {
            //Generate info headers for the device and combine them with the user token
            var headers = context.Request.Headers["User-Agent"];
            var verificationToken = headers + "\n" + UserToken;

            //Encrypt the combination string using AES encryption
            return encryptor.SymmetricEnctyption(verificationToken);
        }

        public string? VerifyVerificationToken(HttpContext context)
        {
            try
            {
                //Retrieve the verification token and verify it is valid to the device
                var StoredToken = context.Session.GetString("VerificationToken");

                //If session token is null, check cookies
                if (StoredToken == null)
                {
                    var cookieToken = context.Request.Cookies["VerificationToken"];

                    if (cookieToken != null)
                    {
                        //Set the session token if the cookie token exists
                        StoredToken = cookieToken;
                        context.Session.SetString("VerificationToken", cookieToken);
                    }
                    else
                    {
                        return null;
                    }
                }

                //Decrypt the token stored in session or cookies
                var decrypted = encryptor.SymmetricDecryption(StoredToken);
                if (decrypted == null) return null;

                string[] storedToken = Regex.Split(decrypted, "\n");

                if (storedToken.Length == 2)
                {
                    var userAgent = storedToken[0];
                    var userToken = storedToken[1];

                    //If current device matches, return the stored user token
                    if (userAgent == context.Request.Headers["User-Agent"]) return userToken;
                }
            }
            catch { }

            //The stored token is not valid to the current current browser / device / user
            return null;
        }

        public async Task<bool> VerifyLoggedInSession(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if(token == null) return false;

            //Course Coordinators
            DocumentReference courseRef = db.Collection("Users").Document(token);
            DocumentSnapshot courseSnapshot = await courseRef.GetSnapshotAsync();

            if (courseSnapshot.Exists)
            {
                return true;
            }

            //Students
            DocumentReference studentRef = db.Collection("Students").Document(token);
            DocumentSnapshot studentSnapshot = await studentRef.GetSnapshotAsync();

            if (studentSnapshot.Exists)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> VerifyLoggedInCoordinator(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token == null) return false;

            DocumentReference courseRef = db.Collection("Users").Document(token);
            DocumentSnapshot courseSnapshot = await courseRef.GetSnapshotAsync();

            if (courseSnapshot.Exists)
            {
                string? CCCode = courseSnapshot.GetValue<string>("CCCode");
                return VerifyCoordinatorCode(CCCode).Result;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserToken"></param>
        /// <returns></returns>
        public async Task<Boolean> LoggedInAsStudent(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token == null) return false;

            DocumentReference studentRef = db.Collection("Students").Document(token);
            DocumentSnapshot studentSnapshot = await studentRef.GetSnapshotAsync();

            if (studentSnapshot.Exists)
            {
                return true;
            }

            return false;
        }

        public async Task<UserModel?> GenerateUserModel(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token == null) return null;

            //Course Coordinators
            DocumentReference courseRef = db.Collection("Users").Document(token);
            DocumentSnapshot courseSnapshot = await courseRef.GetSnapshotAsync();

            if (courseSnapshot.Exists)
            {
                string? FirstName = courseSnapshot.GetValue<string>("FirstName");
                string? LastName = courseSnapshot.GetValue<string>("LastName");
                string? CCCode = courseSnapshot.GetValue<string>("CCCode");

                if (FirstName == null || LastName == null) return null;
                return new UserModel(token, FirstName, LastName, CCCode);
            }

            //Students
            DocumentReference studentRef = db.Collection("Users").Document(token);
            DocumentSnapshot studentSnapshot = await studentRef.GetSnapshotAsync();

            if (studentSnapshot.Exists)
            {
                string? FirstName = courseSnapshot.GetValue<string>("FirstName");
                string? LastName = courseSnapshot.GetValue<string>("LastName");

                if (FirstName == null || LastName == null) return null;
                return new UserModel(token, FirstName, LastName);
            }

            return null;
        }

        public async Task<Boolean> VerifyCoordinatorCode(string? code)
        {
            if(code == null) return false;

            DocumentReference codeRef = db.Collection("CoordinatorCodes").Document(code);
            DocumentSnapshot snapshot = await codeRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a new class document with the provided name as the document id under the current users in firebase. 
        /// If there is no Classes it will create a new collection.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="classModel">An object contained all the required information for the new entry</param>
        /// <returns>A string representing the ID(name) of the new document</returns>
        public async Task<string?> CreateNewClinicAsync(HttpContext context, string className, ScheduleModel scheduleModel)
        {
            string? token = VerifyVerificationToken(context);

            //Insert this into cloud firestore database
            if (token != null)
            {
                WriteBatch batch = db.StartBatch();

                //Store the new schedule as a document
                DocumentReference docRef = db.Collection("Schedules").Document(scheduleModel.ScheduleCode);
                batch.Set(docRef, scheduleModel);

                //Update the course coordinators account with the new schedule code
                DocumentReference ccRef = db.Collection("Users").Document(token).Collection("Classes").Document(className);
                batch.Update(ccRef, "ScheduleCode", FieldValue.ArrayUnion(scheduleModel.ScheduleCode));

                //Get the class reference so that we can get all the associated students
                DocumentSnapshot snapshot = await ccRef.GetSnapshotAsync();
                ClassModel tempModel = snapshot.ConvertTo<ClassModel>();

                //Update all related students with the new schedule code
                //Retrieve the list of students that have the class code
                Query colRef = db.CollectionGroup("Students").WhereArrayContains("ClassCode", tempModel.ClassCode);
                QuerySnapshot allClassesQuerySnapshot = await colRef.GetSnapshotAsync();

                //Keep track of the firebase ID associated with each student username as to create appointments with
                Dictionary<string, string> firebaseEntries = new Dictionary<string, string>();

                //For each student update the schedule code list
                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                    DocumentReference studentRef = db.Collection("Students").Document(currentStudent.Username); //TODO change to documentSnapshot.id when finished testing
                    batch.Update(studentRef, "ScheduleCode", FieldValue.ArrayUnion(scheduleModel.ScheduleCode));

                    if (currentStudent.Username != null)
                    {
                        firebaseEntries.Add(currentStudent.Username, documentSnapshot.Id);
                    }
                }

                //Write all database changes in one go
                await batch.CommitAsync();

                //Create new appointments
                bool success = await createAssociatedAppointments(scheduleModel);

                if (!success) return null;

                //Return the model as a response
                return JsonConvert.SerializeObject(scheduleModel);
            }

            return null;
        }

        /// <summary>
        /// Create appointment entries from a given schedule model, the schedule holds the time table as a json but this
        /// allows invidiual appointments to be loaded by a student or course coordinator.
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <returns>A bool representing if the function was successful</returns>
        public async Task<bool> createAssociatedAppointments(ScheduleModel scheduleModel)
        {
            //Create new appointments
            CollectionReference appointmentRef = db.Collection("Appointments");
            if (scheduleModel.Schedule == null) return false;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            JObject? scheduleArray = (JObject)JsonConvert.DeserializeObject(scheduleModel.Schedule);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            if (scheduleArray == null) return false;

            //Loop through each location in the array
            foreach (var schedule in scheduleArray)
            {
                if (schedule.Value == null) continue;

                //Loop through each appointment in a schedule
                foreach (var appointment in schedule.Value)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    Dictionary<string, object> data = new Dictionary<string, object>
                        {
                            { "Date", scheduleModel.Date },
                            { "Time", appointment.Value<string>("Time") },
                            { "Patient", appointment.Value<string>("Patient") },
                            { "Infectious", appointment.Value<string>("Infectious") },
                            { "RadiationTherapist1", appointment.Value<string>("RT1") },
                            { "RadiationTherapist2", appointment.Value<string>("RT2") },
                            { "Room", schedule.Key },
                            { "Site", appointment.Value<string>("Site") },
                            { "ScheduleCode", scheduleModel.ScheduleCode }
                        };
#pragma warning restore CS8604 // Possible null reference argument.

                    //Create firebase appointment entry
                    await appointmentRef.AddAsync(data);
                }
            }

            return true;
        }

        /// <summary>
        /// Delete a particular schedule and any associated appointments while removing the schedule code from
        /// any students.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheduleCode"></param>
        /// <returns></returns>
        public async Task<bool> DeleteScheduleAsync(HttpContext context, string scheduleCode)
        {
            string? token = VerifyVerificationToken(context);

            if (token != null) return false;

            //Start a large batch write.
            WriteBatch batch = db.StartBatch();

            //Collect any assoicated appointments
            Query appointmentRef = db.CollectionGroup("Appointments").WhereArrayContains("ScheduleCode", scheduleCode);
            QuerySnapshot allAppointmentQuerySnapshot = await appointmentRef.GetSnapshotAsync();

            //Remove each appointment
            foreach (DocumentSnapshot documentSnapshot in allAppointmentQuerySnapshot.Documents)
            {
                batch.Delete(documentSnapshot.Reference);
            }

            //Update all related students by removing the schedule code
            Query studentScheduleRef = db.CollectionGroup("Students").WhereArrayContains("ScheduleCode", scheduleCode);
            QuerySnapshot allStudentScheduleQuerySnapshot = await studentScheduleRef.GetSnapshotAsync();

            //For each student update the schedule code list
            foreach (DocumentSnapshot documentSnapshot in allStudentScheduleQuerySnapshot.Documents)
            {
                var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                DocumentReference studentRef = db.Collection("Students").Document(currentStudent.Username); //TODO change to documentSnapshot.id when finished testing
                batch.Update(studentRef, "ScheduleCode", FieldValue.ArrayRemove(scheduleCode));
            }

            //Write all database changes in one go
            await batch.CommitAsync();

            //Remove the schedule from the list
            DocumentReference scheduleRef = db.Collection("Schedules").Document(scheduleCode);

            //Delete the class reference
            WriteResult success = await scheduleRef.DeleteAsync();

            //Return if the write was successful
            return success.Equals(true);
        }

        /// <summary>
        /// Create a new class document with the provided name as the document id under the current users in firebase. 
        /// If there is no Classes it will create a new collection.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="classModel">An object contained all the required information for the new entry</param>
        /// <returns>A string representing the ID(name) of the new document</returns>
        public async Task<string?> CreateNewClassAsync(HttpContext context, ClassModel classModel)
        {
            string? token = VerifyVerificationToken(context);

            //Insert this into cloud firestore database
            if (token != null)
            {
                DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(classModel.Name);
                await docRef.SetAsync(classModel);
                return JsonConvert.SerializeObject(classModel);
            }

            return null;
        }

        /// <summary>
        /// Delete a saved class list, this removes the entry from Firebase and removes the ClassCode from any assoicated 
        /// student entry, deletes any assoicated schedules and appointments.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="className"></param>
        /// <returns>A bool representing if the action was completed successfully</returns>
        public async Task<bool> DeleteClassAsync(HttpContext context, string className, string classCode)
        {
            string? token = VerifyVerificationToken(context);

            if (token != null) return false;

            //Get a reference to the class
            DocumentReference classRef = db.Collection("Users").Document(token).Collection("Classes").Document(className);
            DocumentSnapshot snapshot = await classRef.GetSnapshotAsync();
            var classDetails = snapshot.ConvertTo<ClassModel>();

            //Start a large batch write.
            WriteBatch batch = db.StartBatch();

            //Check if there are any schedules created
            if (classDetails.ScheduleCode != null)
            {
                foreach (string scheduleCode in classDetails.ScheduleCode)
                {
                    //Collect any assoicated appointments
                    Query appointmentRef = db.CollectionGroup("Appointments").WhereArrayContains("ScheduleCode", scheduleCode);
                    QuerySnapshot allAppointmentQuerySnapshot = await appointmentRef.GetSnapshotAsync();

                    //Remove each appointment
                    foreach (DocumentSnapshot documentSnapshot in allAppointmentQuerySnapshot.Documents)
                    {
                        batch.Delete(documentSnapshot.Reference);
                    }

                    //Update all related students by removing the schedule code
                    Query studentScheduleRef = db.CollectionGroup("Students").WhereArrayContains("ScheduleCode", scheduleCode);
                    QuerySnapshot allStudentScheduleQuerySnapshot = await studentScheduleRef.GetSnapshotAsync();

                    //For each student update the schedule code list
                    foreach (DocumentSnapshot documentSnapshot in allStudentScheduleQuerySnapshot.Documents)
                    {
                        var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                        DocumentReference studentRef = db.Collection("Students").Document(currentStudent.Username); //TODO change to documentSnapshot.id when finished testing
                        batch.Update(studentRef, "ScheduleCode", FieldValue.ArrayRemove(scheduleCode));
                    }

                    //Remove the schedule from firebase
                    DocumentReference scheduleRef = db.Collection("Schedules").Document(scheduleCode);
                    batch.Delete(scheduleRef);
                }
            }

            //Update all related studentsby removing the class code
            Query colRef = db.CollectionGroup("Students").WhereArrayContains("ClassCode", classCode);
            QuerySnapshot allStudentQuerySnapshot = await colRef.GetSnapshotAsync();

            //For each student update the class code list
            foreach (DocumentSnapshot documentSnapshot in allStudentQuerySnapshot.Documents)
            {
                var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                DocumentReference studentRef = db.Collection("Students").Document(currentStudent.Username); //TODO change to documentSnapshot.id when finished testing
                batch.Update(studentRef, "ClassCode", FieldValue.ArrayRemove(classCode));
            }

            //Write all database changes in one go
            await batch.CommitAsync();

            //Delete the class reference
            WriteResult success = await classRef.DeleteAsync();
            
            //Return if the write was successful
            return success.Equals(true);
        }

        /// <summary>
        /// Collect all class details, excluding the student and schedules lists that belong to a
        /// particular course coordinator.
        /// <param name="token">A string representing the current user signed in</param>
        /// </summary>
        public async Task<List<ClassModel>?> CollectAllClassAsync(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token != null)
            {
                Query allClassesQuery = db.Collection("Users").Document(token).Collection("Classes");
                QuerySnapshot allClassesQuerySnapshot = await allClassesQuery.GetSnapshotAsync();
                List<ClassModel> classes = new List<ClassModel>();

                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentClass = documentSnapshot.ConvertTo<ClassModel>();
                    classes.Add(currentClass);
                }

                return classes;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a saved classes information from firebase. The retrieved data depends on the provided user token
        /// and class name.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="className">A string representing the ID of the class to retrieve</param>
        /// <returns>A ClassModel object containing the information that had been saved previously</returns>
        public async Task<ClassModel?> CollectClassAsync(HttpContext context, string className)
        {
            string? token = VerifyVerificationToken(context);

            if (token != null)
            {
                //Collect the class details
                DocumentReference docRef = db.Collection("Users").Document(token).Collection("Classes").Document(className);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists) return null;

                var classDetails = snapshot.ConvertTo<ClassModel>();

                //Retrieve the list of students that have the class code
                Query colRef = db.CollectionGroup("Students").WhereArrayContains("ClassCode", classDetails.ClassCode);
                QuerySnapshot allClassesQuerySnapshot = await colRef.GetSnapshotAsync();
                List<StudentModel> students = new List<StudentModel>();

                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentStudent = documentSnapshot.ConvertTo<StudentModel>();

                    Console.WriteLine(currentStudent.ToString());
                    //students.Add(JsonConvert.SerializeObject(currentStudent));
                    students.Add(currentStudent);
                }

                //Join the students with a unique character as to easily separate them in javascript
                //classDetails.Students = string.Join("|", students.ToArray());
                classDetails.Students = students;

                return classDetails;
            }

            return null;
        }

        /// <summary>
        /// Create new student accounts if necessary and save a new class string under the currently selected class.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="classCode">A string representing the ID of the class to save to</param>
        /// <param name="studentList">A string of student values, each value separated by a ':' and each student
        ///                             separated by a ','
        /// <returns>A boolean representing if the operation was a success</returns>
        public async Task<bool> SaveAClassListAsync(HttpContext context, string classCode, string[] studentList)
        {
            string? token = VerifyVerificationToken(context);

            if (token == null) return false;

            List<string> students = new List<string>();

            foreach (string student in studentList)
            {
                //Need to check if student exists and add the new code or create a new student entry
                var studentObject = JsonConvert.DeserializeObject<StudentModel>(student);
                if (studentObject == null) continue;
                if (studentObject.Username == null) continue;
                students.Add(studentObject.Username);
            }

            //DETECT IF A STUDENT HAS BEEN REMOVED
            //Update all related students by removing the class code (incase they have been removed from the class
            Query existingStudentRef = db.CollectionGroup("Students").WhereArrayContains("ClassCode", classCode);
            QuerySnapshot allStudentQuerySnapshot = await existingStudentRef.GetSnapshotAsync();

            if(allStudentQuerySnapshot.Documents.Count != 0) {
                Console.WriteLine("HERE");

                WriteBatch clearBatch = db.StartBatch();
                //For each student update the class code list
                foreach (DocumentSnapshot documentSnapshot in allStudentQuerySnapshot.Documents)
                {
                    var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                    Console.WriteLine(currentStudent.Username);

                    //If the student is in the new student list don't do anything
                    if (currentStudent.Username == null) continue;
                    if (students.Contains(currentStudent.Username)) continue;

                    Console.WriteLine(currentStudent.Username);

                    //If the student is not in the new list then they have been removed from the class
                    DocumentReference studentRef = db.Collection("Students").Document(currentStudent.Username); //TODO change to documentSnapshot.id when finished testing
                    clearBatch.Update(studentRef, "ClassCode", FieldValue.ArrayRemove(classCode));
                }

                await clearBatch.CommitAsync();
            }




            Dictionary<string, string> existingStudents = new Dictionary<string, string>();

            //Collect the student list as to detect if a student already exists
            CollectionReference colRef = db.Collection("Students");
            QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                StudentModel student = documentSnapshot.ConvertTo<StudentModel>();
                if (student.Username != null)
                {
                    existingStudents.Add(student.Username, documentSnapshot.Id);
                }
            }

            WriteBatch batch = db.StartBatch();

            foreach (string student in studentList)
            {
                //Need to check if student exists and add the new code or create a new student entry
                var studentObject = JsonConvert.DeserializeObject<StudentModel>(student);

                if (studentObject == null) return false;
                if (studentObject.Username == null) return false;

                //If the key is not present create a new account and entry
                if (!existingStudents.ContainsKey(studentObject.Username))
                {
                    //Create a new student account
                    //UNCOMMENT FOR AUTOMATIC STUDENT ACCOUNT CREATION
                    //BEWARE THIS WILL CREATE AN ACCOUNT FOR EVERYY SINGLE ENTRY IF UNCOMMENTED
                    //string? Id = await RegisterNewStudentAccount(studentObject.Username);

                    //USE THIS FOR TESTING AT THE MOMENT
                    string? Id = studentObject.Username;

                    if (Id == null) return false;
                    //Notify the user that an account has not been created

                    //Create a new student entry
                    CreateNewStudentEntry(Id, studentObject, classCode, ref batch);
                } else
                {
                    string id = existingStudents[studentObject.Username];
                    ModifyCurrentStudentEntry(id, studentObject, classCode, ref batch);
                }
            }

            await batch.CommitAsync();

            return true;
        }

        /// <summary>
        /// Create a new student entry for the firebase. Add the class code to its ClassCode array before assigning
        /// it to the batch write.
        /// </summary>
        /// <param name="student">A StudentModel object containing information about a student</param>
        /// <param name="classCode">A string representing the unqiue code of the class the student is linked with</param>
        /// <param name="batch">A reference to a batch write</param>
        private void CreateNewStudentEntry(string firebaseId, StudentModel student, string classCode, ref WriteBatch batch) {
            if (student.ClassCode == null) return;
            student.ClassCode.Add(classCode);

            DocumentReference docRef = db.Collection("Students").Document(firebaseId);
            batch.Set(docRef, student);
        }

        private void ModifyCurrentStudentEntry(string firebaseId, StudentModel student, string classCode, ref WriteBatch batch)
        {
            if(student.ClassCode == null) return;
            student.ClassCode.Add(classCode);

            DocumentReference docRef = db.Collection("Students").Document(firebaseId);
            batch.Update(docRef, "ClassCode", FieldValue.ArrayUnion(classCode));
        }

        private async Task<string?> RegisterNewStudentAccount(string userName)
        {
            //TODO this only works for unisa students at the moment
            //string email = userName + "@mymail.unisa.edu.au";

            string email = userName + "@gmail.com";

            try
            {
                //Create a new account with the email and a random password
                FirebaseAuthLink firebaseAuth = await Auth().CreateUserWithEmailAndPasswordAsync(email, CreateRandomPassword(12));

                //Send an email for the student to reset their password
                ResetPassword(email);

                //Return the LocalId for to create the student details entry
                return firebaseAuth.User.LocalId;
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                if (firebaseEx == null)
                {
                    Console.WriteLine("Unknown Firebase Error has occurred");
                }
                else if (firebaseEx.error != null)
                {
                    Console.WriteLine(firebaseEx.error.message);
                }
                
                return null;
            }
        }

        private string CreateRandomPassword(int CodeLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ!@#$%^&*()[]{}";
            Random randNum = new Random();
            char[] chars = new char[CodeLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < CodeLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        /// <summary>
        /// Collect all the classes associated with a particular course coordinator and populate each
        /// ClassModel with the connected students.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A list of ClassModels with Students arrays integrated.</returns>
        public async Task<Dictionary<string, ClassModel>?> GetAllClassesAndStudents(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token == null) return null;

            //Collect all the classes associated with a course coordinator
            Query allClassesQuery = db.Collection("Users").Document(token).Collection("Classes");
            QuerySnapshot allClassesQuerySnapshot = await allClassesQuery.GetSnapshotAsync();

            Dictionary<string, ClassModel> classes = new Dictionary<string, ClassModel>();
            Dictionary<string, List<StudentModel>> codeGroup = new Dictionary<string, List<StudentModel>>();

            foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
            {
                var currentClass = documentSnapshot.ConvertTo<ClassModel>();
                if (currentClass.ClassCode != null)
                {
                    classes[currentClass.ClassCode] = currentClass;
                    //Initiate empty lists for the students
                    codeGroup[currentClass.ClassCode] = new List<StudentModel>();
                }
            }

            //Collect all the students
            CollectionReference usersRef = db.Collection("Students");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            //Cycle through students and add them to the code groupings
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                var currentStudent = document.ConvertTo<StudentModel>();

                //Look at each class code
                foreach (string code in document.GetValue<List<string>>("ClassCode"))
                {
                    if(code == null) continue;
                    //codeGroup[code].Add(JsonConvert.SerializeObject(currentStudent));
                    codeGroup[code].Add(currentStudent);
                }
            }

            //Cycle through the classes and add the student lists to them
            foreach (KeyValuePair<string, ClassModel> entry in classes)
            {
                //entry.Value.Students = string.Join("|", codeGroup[entry.Key].ToArray());
                entry.Value.Students = codeGroup[entry.Key];
            }

            return classes;
        }

        //TODO this collects course coordinators as well, add check to see if CCCode is null for just students later on.
        public async Task<Dictionary<string, Array>?> GetStudentsAsync()
        {
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            Dictionary<string, Array> students = new Dictionary<string, Array>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                students[document.Id] = new string[] { document.GetValue<string>("FirstName"), document.GetValue<string>("LastName") };
            }

            return students;
        }

        public async Task<List<AppointmentModel>?> CollectAllAppointmentsAsync(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token != null)
            {
                //Get the student information
                Dictionary<string, Array>? studentInformation = await GetStudentsAsync();
                if (studentInformation == null) return null;

                Query allAppointmentsQuery = db.Collection("Appointments")
                                                .OrderByDescending("Date");
                QuerySnapshot allAppointmentsQuerySnapshot = await allAppointmentsQuery.GetSnapshotAsync();
                List<AppointmentModel> appointments = new List<AppointmentModel>();

                foreach (DocumentSnapshot documentSnapshot in allAppointmentsQuerySnapshot.Documents)
                {
                    AppointmentModel currentAppointment = documentSnapshot.ConvertTo<AppointmentModel>();

                    if (currentAppointment.Patient == null || currentAppointment.RadiationTherapist1 == null || currentAppointment.RadiationTherapist2 == null) return null;

                    //currentAppointment.Date = currentAppointment.Date.AddHours(9.5);
                    Array userPatient = studentInformation[currentAppointment.Patient];
                    if (userPatient == null) return null;
                    currentAppointment.Patient = userPatient.GetValue(0) + " " + userPatient.GetValue(1);

                    Array userRT1 = studentInformation[currentAppointment.RadiationTherapist1];
                    if (userRT1 == null) return null;
                    currentAppointment.RadiationTherapist1 = userRT1.GetValue(0) + " " + userRT1.GetValue(1);

                    Array userRT2 = studentInformation[currentAppointment.RadiationTherapist2];
                    if (userRT2 == null) return null;
                    currentAppointment.RadiationTherapist2 = userRT2.GetValue(0) + " " + userRT2.GetValue(1);

                    appointments.Add(currentAppointment);
                }

                return appointments;
            }

            return null;

        }

        public async Task<List<AppointmentModel>?> CollectStudentsAppointmentsAsync(HttpContext context)
        {
            string? token = VerifyVerificationToken(context);

            if (token != null)
            {
                //Get the student information
                Dictionary<string, Array>? studentInformation = await GetStudentsAsync();
                if (studentInformation == null) return null;

                Query allAppointmentsQuery = db.Collection("Appointments")
                                               .OrderByDescending("Date");
                QuerySnapshot allAppointmentsQuerySnapshot = await allAppointmentsQuery.GetSnapshotAsync();
                List<AppointmentModel> appointments = new List<AppointmentModel>();

                foreach (DocumentSnapshot documentSnapshot in allAppointmentsQuerySnapshot.Documents)
                {
                    AppointmentModel currentAppointment = documentSnapshot.ConvertTo<AppointmentModel>();
                    currentAppointment.AppointmentID = documentSnapshot.Id;

                    if (currentAppointment.Patient == token || currentAppointment.RadiationTherapist1 == token || currentAppointment.RadiationTherapist2 == token)
                    {
                        if (currentAppointment.Patient == null || currentAppointment.RadiationTherapist1 == null || currentAppointment.RadiationTherapist2 == null) return null;

                        Array userPatient = studentInformation[currentAppointment.Patient];
                        if (userPatient == null) return null;
                        currentAppointment.Patient = userPatient.GetValue(0) + " " + userPatient.GetValue(1);

                        Array userRT1 = studentInformation[currentAppointment.RadiationTherapist1];
                        if (userRT1 == null) return null;
                        currentAppointment.RadiationTherapist1 = userRT1.GetValue(0) + " " + userRT1.GetValue(1);

                        Array userRT2 = studentInformation[currentAppointment.RadiationTherapist2];
                        if (userRT2 == null) return null;
                        currentAppointment.RadiationTherapist2 = userRT2.GetValue(0) + " " + userRT2.GetValue(1);

                        appointments.Add(currentAppointment);
                    }

                }
                return appointments;

            }
            return null;
        }

        public async void DataRequest(string type, HttpContext context)
        {
            try
            {
                UserModel? user = GenerateUserModel(context).Result;
                string? token = VerifyVerificationToken(context);

                //TODO Add a message in here so the user knows that something has not loaded properly
                if (token == null || user == null) return;

                CollectionReference RequestsRef = db.Collection("DataRequests");
                QuerySnapshot snapshot = await RequestsRef.GetSnapshotAsync();

                bool found = false;
                
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string? typeDict = documentDictionary["Type"].ToString();
                    string? uid = documentDictionary["UID"].ToString();
                    if (uid == token && typeDict == type)
                    {
                        found = true;
                        Debug.WriteLine("ALREADY EXIST");
                    } 
                    
                }
                if (found == false)
                {
                    DocumentReference docRef = db.Collection("DataRequests").Document();
                    Dictionary<string, object> dataRequest = new Dictionary<string, object>
                        {
                            { "UID", token },
                            { "Name",  user.FirstName + " " + user.LastName},
                            { "Type", type },
                            { "Date", DateTime.Now.ToString() }
                        };
                    await docRef.SetAsync(dataRequest);
                }

            } catch(Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async void DeleteUserData(HttpContext context)
        {
            try
            {
                var token = VerifyVerificationToken(context);
                DocumentReference userRef = db.Collection("Users").Document(token);
                await userRef.DeleteAsync();
                
            } catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async void ResetPassword(string email)
        {
            try
            {
                await auth.SendPasswordResetEmailAsync(email);
            } catch(Exception e)
            {
                Debug.WriteLine(e);
            }           
        }

        /// <summary>
        /// Upload a file to the connected Firebase Cloud Storage.
        /// </summary>
        /// <param name="treatmentID">The Firebase key of the appointment that the document is associated with</param>
        /// <param name="stream">A file stream representing the document to be uploaded</param>
        /// <returns>A string representing the URL of the uploaded PDF for viewing purposes</returns>
        public async Task<string?> UploadPDF(FileModel file)
        {
            if(file.Document == null)
            {
                return null;
            }

            //LOOK INTO THIS LATER FOR AUTHENTICATION
            //Add allow read, write: if request.auth != null; to firebase cloud storage rules

            //FirebaseStorage storage = new FirebaseStorage("unisa-rt-mock-clinic.appspot.com",
            //    new FirebaseStorageOptions
            //    {
            //        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
            //        ThrowOnCancel = true,
            //    }
            //);


            //Child is the name - can also add folders i.e testing/test.pdf
            await Storage().Child(file.ID).PutAsync(file.Document.OpenReadStream());

            string link = await Storage().Child(file.ID).GetDownloadUrlAsync();

            return link;
        }

        /// <summary>
        /// Retrieve the URL of this document so that a user can view it within a webpage.
        /// </summary>
        /// <param name="treatmentID">The Firebase key of the appointment that the document is associated with</param>
        /// <returns>A string of the URL pointing towards this document</returns>
        public async Task<string> RetrievePDF(FileModel file)
        {
            string link = await Storage().Child(file.ID).GetDownloadUrlAsync();

            return link;
        }

        /// <summary>
        /// Delete a pdf document from the Firebase Cloud Storage
        /// </summary>
        /// <param name="treatmentID">The Firebase key of the appointment that the document is associated with</param>
        public async Task<bool> DeletePDF(FileModel file)
        {
            await Storage().Child(file.ID).DeleteAsync();

            return true;
        }
    }
}