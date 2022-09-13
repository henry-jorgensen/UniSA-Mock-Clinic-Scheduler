﻿using Firebase.Auth;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
//using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
//using Google.Rpc;
//using System.Linq;
using System.Text.RegularExpressions;
//using Newtonsoft.Json.Linq;
//using System;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseSharedFunctions
    {
        FirestoreDb db;
        FirebaseAuthProvider auth;
        public FirebaseEncryptor encryptor;

        public FirestoreDb DB()
        {
            return db;
        }
       
        public FirebaseAuthProvider Auth()
        {
            return auth;
        }

        public FirebaseSharedFunctions(HttpContext context)
        {
            //TODO hide this in the future with ENV
            db = new FirestoreDbBuilder { ProjectId = "unisa-rt-mock-clinic", JsonCredentials = "{ 'type': 'service_account',   'project_id': 'unisa-rt-mock-clinic',   'private_key_id': '6aa0d9f7dc80f52392dd906a85d0f4f462432f52',   'private_key': '-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDP+Pn9NZkcvnxs\n7sTgbHBZJqlqK/wv5qVSdyQeUZuC4Dp1uFffRcGS2OSrq+v0CMgRxaBtF5ox4oca\nrk5+B0AqtDpuWLoKVOlAbzEjbdZgcV2WZccfxb1VqMMeNJRFF1g3O+idQIAzI0ax\nDH+RQoPaco6pc8LjfTG5NAfcyZQcSoYGyUgFVaMfmbay7DQwyvIsyY9uZ52ZPYWG\n3N2KZI69uKYVPKflgqOtzZuedAtDG2MMnyHe8Majjt/MtGFLKeI/hE+TpYvpn5c6\ncV33BpFyN7D1LP2pVCP83rtmCswWbZPfZL+Fpjnc/QANqzVVJb3SLIBWOmeEkuIi\n8dw60uaNAgMBAAECggEAAd04dD9Q9bibYYRSjXGkbj8ZCN65ihdVd/A9iaOvn2C8\nzcGV5wIhGjAq5qRrYe2CDIqO0NPcxESSc3J363iqH31vB8bJeVTBbL2EfsKsGx7Z\nth5858iRIxEeOsli0lKAl/eKlyTb82dOdN0dr29VU1An/hMxFJP5hvM8OcNSLxvD\nC8a+uiSWl0OBGnDNBulkmffZsdjiphPVooY/khLGpTxUN8ec7HZEhhlGvaPvvBPW\n6ywKxtjssblc22CkIkexVsnFqtf1ZkUSAKggnmv8aKImzKkkG+8PQcPiJNAp6FC9\nZ9bdXwrFDtrPU+V3gFCS4Z7yogU1fcaTM9wI0tjPoQKBgQD+poIAr4BYmVIaO86S\nSFifnurNfYOsTBEa8cn//i8W125LyV93Ee4ThCV4zgP9sT7HblnxT6U+mB+Hmkz7\nNeIfbYfL3RR+R7GFqkbYs8P5GPUP8FSsTMzyLvdMHF1TQz4G0Qbn+r1AgwXZcVdL\nuvNyE8MYKft2xAz8uCSKwL0ahQKBgQDREyOvKOKE0JadMLrE1wCaapUamHmLZiAO\nJsYzQjz/186FwKKyQyLAZSAbGdkzLK2U8ZSosoQZnrokeDxfmXoZniKqcFbuFIJi\ngyFhpfp6mK4GPhyD+bzsIoT3WX9Xwtrmv2OfudalY1hd6fNaapYomyYqTdoWUA88\nvxdLe/zOaQKBgQCaD2B9S7ApafC7AE3UQEKlpz5EvdfIiGic1YUxA7W3avRGk3jX\nD5jqY7tL38+YTwA9JWzyyg2d1ejVYCuMm6fG/bv3QTRhxbwHsuGTvwYkEM5KK0r+\nxqQDLRjeChcIBZlkBFfaRt7yRZJnX+PBZEReUshoORXyX1/AESPCciK2BQKBgD64\nCyBkl29YU5ZcI+sgxGGOT6Rm0S9sN3mHUDXYTQxC5QViwGvRj/8/Vt5KZsnfQUNJ\nJVtmEhLNdvGx0Aqts98zfRq8EJfjNynuRHlSnU1ht/LPdyZwKKh9wn2hL35YSeqm\nx3AHA8khgETMBeC90MXlpRFTwXSoF6oVeRt/2lrhAoGBAL8IsbiE9hKv7MUK1xR3\nrW194MfKocnFuAi+GdbhJjw6jG88/3l3nnVE+unz8ORP5Kj6m9XwISWrU9/H0KJ7\n/lJ8pafeBWsc8i+B+x2/vwZNjxGVzwpWzBLnHwjgsZ3qT1w2h3I7boAnn3Ezouwv\n5w2vKJXCIjteoyLxZ83Zao3w\n-----END PRIVATE KEY-----\n',   'client_email': 'firebase-adminsdk-vjevh@unisa-rt-mock-clinic.iam.gserviceaccount.com',   'client_id': '103711456653250716580',   'auth_uri': 'https://accounts.google.com/o/oauth2/auth',   'token_uri': 'https://oauth2.googleapis.com/token',   'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs',   'client_x509_cert_url': 'https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-vjevh%40unisa-rt-mock-clinic.iam.gserviceaccount.com' }"}.Build();
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyByVk8XwhbQGoeeqkcxr5vRJWhOjep5Ulc"));
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

        public string VerifyVerificationToken(HttpContext context)
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

        public async Task<Boolean> VerifyAnonymousLoggedIn(string UserName)
        {
            CollectionReference usersRef = db.Collection("Students");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == UserName) return true;
            }

            return false;
        }

        //TODO THIS IS SLOW AND BEING CALLED MULTIPLE TIMES PER PAGE CHANGE - WORK ON OPTIMISATION
        public async Task<bool> VerifyLoggedInSession(HttpContext context)
        {
            //Check the database whether the user token is valid
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                //Return true if it exists in the database
                if (document.Id == VerifyVerificationToken(context)) return true;
            }
            return false;
        }

        public async Task<bool> VerifyLoggedInCoordinator(HttpContext context)
        {
            //Check the database whether the user token is valid
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                //Return true if it exists in the database
                if (document.Id == VerifyVerificationToken(context))
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string CCCode = documentDictionary["CCCode"].ToString();

                    return VerifyCoordinatorCode(CCCode).Result;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserToken"></param>
        /// <returns></returns>
        public async Task<Boolean> LoggedInAsStudent(string UserName)
        {
            CollectionReference usersRef = db.Collection("Students");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == UserName) return true;
            }

            return false;
        }

        public async Task<UserModel> GenerateUserModel(HttpContext context)

        {
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == VerifyVerificationToken(context))
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string FirstName = documentDictionary["FirstName"].ToString();
                    string LastName = documentDictionary["LastName"].ToString();
                    string CCCode = documentDictionary["CCCode"].ToString();

                    return new UserModel(document.Id, FirstName, LastName, CCCode);
                }
            }

            return null;
        }

        public async Task<UserModel?> GetAnonymousUserModelAsync(HttpContext context, string UserName)
        {
            //Retrieve the verification token and verify it is valid to the device
            var UserToken = context.Session.GetString("VerificationToken");

            DocumentReference anonRef = db.Collection("Students").Document(UserName);
            DocumentSnapshot snapshot = await anonRef.GetSnapshotAsync();

            if(!snapshot.Exists)
            {
                return null;
            }

            try
            {
                string firstName = snapshot.GetValue<string>("FirstName");
                string lastName = snapshot.GetValue<string>("LastName");
                return new UserModel(UserToken, firstName, lastName, "Student");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<UserModel> LoginAccountAsync(AccountModel accountModel)
        {
            var firebaseAuth = await auth.SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
            string token = firebaseAuth.User.LocalId;

            Debug.WriteLine(token);
            if (token != null)
            {
                CollectionReference usersRef = db.Collection("Users");
                QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Id == token)
                    {
                        Dictionary<string, object> documentDictionary = document.ToDictionary();
                        string FirstName = documentDictionary["FirstName"].ToString();
                        string LastName = documentDictionary["LastName"].ToString();
                        string CCCode = documentDictionary["CCCode"].ToString();

                        return new UserModel(token, FirstName, LastName, CCCode);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Using a supplied username and password search the database for a matching student
        /// document. If there is a matching record use Firebase's anonymous function to create
        /// a semi-temporary account.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<StudentModel?> LoginAnonymousAccountAsync(string username, string password)
        {
            DocumentReference docRef = db.Collection("Students").Document(username);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if(!snapshot.Exists)
            {
                return null;
            }

            StudentModel temp = snapshot.ConvertTo<StudentModel>();

            if(!temp.Password.Equals(password))
            {
                return null;
            }

            var firebaseAuth = await auth.SignInAnonymouslyAsync();
            string token = firebaseAuth.User.LocalId;

            //Add the token to the student model
            temp.Token = token;

            if (token != null)
            {
                return temp;
            }

            return null;
        }

        public async Task<Boolean> VerifyCoordinatorCode(string code)
        {
            CollectionReference usersRef = db.Collection("CoordinatorCodes");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == code) return true;
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
        public async Task<string?> CreateNewScheduleAsync(HttpContext context, string className, ScheduleModel scheduleModel)
        {
            string token = VerifyVerificationToken(context);

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

                //For each student update the schedule code list
                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentStudent = documentSnapshot.ConvertTo<StudentModel>();
                    DocumentReference studentRef = db.Collection("Students").Document(currentStudent.Username);
                    batch.Update(studentRef, "ScheduleCode", FieldValue.ArrayUnion(scheduleModel.ScheduleCode));
                }
                    
                //Write all database changes in one go
                await batch.CommitAsync();

                //Return the model as a response
                return JsonConvert.SerializeObject(scheduleModel);
            }

            return null;
        }

        /// <summary>
        /// Create a new class document with the provided name as the document id under the current users in firebase. 
        /// If there is no Classes it will create a new collection.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="classModel">An object contained all the required information for the new entry</param>
        /// <returns>A string representing the ID(name) of the new document</returns>
        public async Task<string?> CreateNewClassAsync(string token, ClassModel classModel)
        {
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
        /// Collect all class details, excluding the student and schedules lists that belong to a
        /// particular course coordinator.
        /// <param name="token">A string representing the current user signed in</param>
        /// </summary>
        public async Task<List<ClassModel>?> CollectAllClassAsync(string token)
        {
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
        public async Task<ClassModel?> CollectClassAsync(string token, string className)
        {
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
                List<string> students = new List<string>();

                foreach (DocumentSnapshot documentSnapshot in allClassesQuerySnapshot.Documents)
                {
                    var currentStudent = documentSnapshot.ConvertTo<StudentModel>();

                    Console.WriteLine(currentStudent.ToString());
                    students.Add(JsonConvert.SerializeObject(currentStudent));
                }

                //Join the students with a unique character as to easily separate them in javascript
                classDetails.Students = string.Join("|", students.ToArray());

                return classDetails;
            }

            return null;
        }

        /// <summary>
        /// Save a new class string to under the currently selected class.
        /// </summary>
        /// <param name="token">A string representing the current user signed in</param>
        /// <param name="className">A string representing the ID of the class to save to</param>
        /// <param name="studentList">A string of student values, each value separated by a ':' and each student
        ///                             separated by a ','
        /// <returns>A boolean representing if the operation was a success</returns>
        public async Task<bool> SaveAClassListAsync(string token, string classCode, string className, string[] studentList)
        {
            if (token != null)
            {
                HashSet<string> existingStudents = new HashSet<string>();

                //Collect the student list as to detect if a student already exists
                CollectionReference colRef = db.Collection("Students");
                QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

                foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                {
                    StudentModel student = documentSnapshot.ConvertTo<StudentModel>();
                    existingStudents.Add(student.Username);
                }

                WriteBatch batch = db.StartBatch();

                foreach (string student in studentList)
                {
                    //Need to check if student exists and add the new code or create a new student entry
                    var studentObject = JsonConvert.DeserializeObject<StudentModel>(student);

                    if (studentObject == null) return false;

                    if (existingStudents.Contains(studentObject.Username))
                    {
                        ModifyCurrentStudentEntry(studentObject, classCode, ref batch);
                    } else
                    {
                        createNewStudentEntry(studentObject, classCode, ref batch);
                    }
                }

                await batch.CommitAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a new student entry for the firebase. Add the class code to its ClassCode array before assigning
        /// it to the batch write.
        /// </summary>
        /// <param name="student">A StudentModel object containing information about a student</param>
        /// <param name="classCode">A string representing the unqiue code of the class the student is linked with</param>
        /// <param name="batch">A reference to a batch write</param>
        private void createNewStudentEntry(StudentModel student, string classCode, ref WriteBatch batch) {
            student.ClassCode.Add(classCode);

            DocumentReference docRef = db.Collection("Students").Document(student.Username);
            batch.Set(docRef, student);
        }

        private void ModifyCurrentStudentEntry(StudentModel student, string classCode, ref WriteBatch batch)
        {
            student.ClassCode.Add(classCode);

            DocumentReference docRef = db.Collection("Students").Document(student.Username);
            batch.Update(docRef, "ClassCode", FieldValue.ArrayUnion(classCode));
        }


        //TEMP USE OF FUNCTION FOR DEMO

        public async Task<UserModel> GetUserModelAsync(string UserToken)
        {
            CollectionReference usersRef = db.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == UserToken)
                {
                    Console.WriteLine("User: {0}", document.Id);
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string FirstName = documentDictionary["FirstName"].ToString();
                    string LastName = documentDictionary["LastName"].ToString();
                    string CCCode = documentDictionary["CCCode"].ToString();

                    return new UserModel(UserToken, FirstName, LastName, CCCode);
                }
            }

            return null;
        }


        public async Task<List<AppointmentModel>> CollectAllAppointmentsAsync(HttpContext context)
        {
            string token = VerifyVerificationToken(context);

            if (token != null)
            {
                Query allAppointmentsQuery = db.Collection("Appointments")
                                                .OrderByDescending("Date");
                QuerySnapshot allAppointmentsQuerySnapshot = await allAppointmentsQuery.GetSnapshotAsync();
                List<AppointmentModel> appointments = new List<AppointmentModel>();

                foreach (DocumentSnapshot documentSnapshot in allAppointmentsQuerySnapshot.Documents)
                {
                    AppointmentModel currentAppointment = documentSnapshot.ConvertTo<AppointmentModel>();

                    currentAppointment.Date = currentAppointment.Date.AddHours(9.5);

                    UserModel userPatient = await GetUserModelAsync(currentAppointment.Patient);
                    currentAppointment.Patient = userPatient.FirstName + " " + userPatient.LastName;

                    UserModel userRT1 = await GetUserModelAsync(currentAppointment.RadiationTherapist1);
                    currentAppointment.RadiationTherapist1 = userRT1.FirstName + " " + userRT1.LastName;

                    UserModel userRT2 = await GetUserModelAsync(currentAppointment.RadiationTherapist2);
                    currentAppointment.RadiationTherapist2 = userRT2.FirstName + " " + userRT2.LastName;

                    appointments.Add(currentAppointment);
                }

                return appointments;
            }

            return null;

        }

        public async Task<List<AppointmentModel>> CollectStudentsAppointmentsAsync(HttpContext context)
        {
            string token = VerifyVerificationToken(context);

            if (token != null)
            {
                Query allAppointmentsQuery = db.Collection("Appointments")
                                               .OrderByDescending("Date");
                QuerySnapshot allAppointmentsQuerySnapshot = await allAppointmentsQuery.GetSnapshotAsync();
                List<AppointmentModel> appointments = new List<AppointmentModel>();

                foreach (DocumentSnapshot documentSnapshot in allAppointmentsQuerySnapshot.Documents)
                {
                    AppointmentModel currentAppointment = documentSnapshot.ConvertTo<AppointmentModel>();
                    currentAppointment.Date = currentAppointment.Date.AddHours(9.5);
                    if (currentAppointment.Patient == token || currentAppointment.RadiationTherapist1 == token || currentAppointment.RadiationTherapist2 == token)
                    {
                        UserModel userPatient = await GetUserModelAsync(currentAppointment.Patient);
                        currentAppointment.Patient = userPatient.FirstName + " " + userPatient.LastName;

                        UserModel userRT1 = await GetUserModelAsync(currentAppointment.RadiationTherapist1);
                        currentAppointment.RadiationTherapist1 = userRT1.FirstName + " " + userRT1.LastName;

                        UserModel userRT2 = await GetUserModelAsync(currentAppointment.RadiationTherapist2);
                        currentAppointment.RadiationTherapist2 = userRT2.FirstName + " " + userRT2.LastName;

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
                UserModel user = GenerateUserModel(context).Result;
                string token = VerifyVerificationToken(context);

                CollectionReference RequestsRef = db.Collection("DataRequests");
                QuerySnapshot snapshot = await RequestsRef.GetSnapshotAsync();

                bool found = false;
                
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    string typeDict = documentDictionary["Type"].ToString();
                    string uid = documentDictionary["UID"].ToString();
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
    }
}