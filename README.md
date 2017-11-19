# Social Network

## Prerequisites

* [MySQL ODBC Connector Driver v5.3 32-bit](https://dev.mysql.com/get/Downloads/Connector-ODBC/5.3/mysql-connector-odbc-5.3.9-win32.msi)
* An Internet Connection

## social_network File Structure

### User

#### Description

The User class is the data management for individual users, and handles the password verification, as well as friend and post management.

#### Public Variables

* userID
  * Primary Key and username
* passSalt
  * Cyptographically-secure, randomly-generated salt
* passHash
  * Cryptographically-secure password hash
* posts
  * Integer list of post IDs associated with the user
* friends
  * String list of other users that are friends with that user

#### Public Functions

* VerifyPassword
  * Parameters: string password
  * Return Value: boolean
  * Description: Calls PassHash verify password, passing the password, passSalt, and passHash to see if the entered password is correct
* AddFriend
  * Parameters: string friend
  * Return Value: none 
  * Description: Adds a user ID to the friends list if they are not already on it
* RemoveFriend
  * Parameters: string friend
  * Return Value: none
  * Description: Adds a user ID to the friends list if they are on it
* AddPost
  * Parameters: int postID
  * Return value: none
  * Description: Adds a post ID to the posts list if it is not already on there
* RemovePost
  * Parameters: string userID, int postID
  * Return value: none
  * Description: Removes a post ID to the posts list if it is on there

### Post

#### Description

The Post class is the data management for individual posts.

#### Public Variables

* postID
  * Primary Key and integer ID
* opID
  * Original Poster userID
* content
  * String content of the post
* timeStamp
  * String timestamp of the time the post was made (format: yyyy-MM-dd HH:mm:ss.ffffff)

### Server

#### Description

The Server class manages the remote MySQL database connection through ODBC. 
It contains the login credentials, and manages all read/write calls to the database. 

#### Public Variables

* conn
  * ODBC connection to the remote database

#### Public Functions

* GetAllUsers
  * Parameters: none
  * Return Value: List\<User>
  * Description: Connects to MySQL database and retrieves all users
* GetAllPosts
  * Parameters: none
  * Return Value: List\<Post>
  * Description: Connects to MySQL database and retrieves all posts
* WriteAllUsers
  * Parameters: List\<User> users
  * Return Value: none
  * Description: Inserts the current user list into the MySQL database, updating already existing users to have current post and friend lists
* WriteAllPosts
  * Parameters: List\<Post> posts
  * Return Value: none
  * Description: Inserts the current post list into the MySQL database, ignoring duplicate post IDs
* DeleteUser
  * Parameters: User user
  * Return Value: none
  * Description: Deletes specified user from MySQL database
* DeletePost
  * Parameters: Post post
  * Return Value: none
  * Description: Deletes specified post from MySQL database
* ResetData
  * Parameters: none
  * Return Value: none
  * Description: Deletes all data from database

### PassHash

#### Description

PassHash is a password hashing class used to register and login users. It implements PBKDF2 with SHA512

#### Private Variables

* SaltByteLength
  * The length of the salt byte array, 64
* DerivedKeyLength
  * The length of the password hash, 128

#### Public Functions

* CreatePassHash
  * Parameters: string password
  * Returns: string[]
  * Descriptions: Uses RNGCryptoServiceProvider to create secure salt, hashes the password with PBKDF2 SHA512 HMAC, and returns the salt and hash
* VerifyPass
  * Parameters: string password, string passSalt, string passHash
  * Returns: bool
  * Description: Hashes an inputted password with the user salt and verifies if the inputted password hash matches the correct hash

### PBKDF2

#### Description

The Password-Based Key Derivation Function 2 algorithm for password hashing to slow down brute force attack of encrypted keys. [More info here](https://en.wikipedia.org/wiki/PBKDF2)

I did not write this function, as there are plenty of people who have implemented it. Used implementation courtesy of [Medo](https://www.medo64.com), and can be found [here](https://www.medo64.com/2012/04/pbkdf2-with-sha-256-and-others/).

As such, I will leave how this works to the previous links. In short, PBKDF2 returns a byte array for SHA512 hashing, and PassHash converts it to a Base64 string.

### social_network

#### Description

The main program with all of the calls, separated into it's own class for ease of use.

#### Public Variables

* users
  * List of all users
* posts
  * List of all posts
* currentUser
  * The current logged-in user

#### Public Functions

* readUsers
  * Parameters: none 
  * Return Value: bool
  * Description: Calls the Server.ReadAllUsers()
* readPosts
  * Parameters: none 
  * Return Value: bool
  * Description: Calls the Server.ReadAllPosts()
* addUser
  * Parameters: string userID, string passSalt, string passHash, List\<int> posts (opt), List\<string> friends (opt) 
  * Return Value: bool
  * Description: Adds a user with the specified information
* addPost
  * Parameters: string userID, string content 
  * Return Value: bool
  * Description: Adds a post to the main post list, userID.posts list
* writeUsers
  * Parameters: none 
  * Return Value: bool
  * Description: Calls Server.WriteAllUsers()
* writePosts
  * Parameters: none 
  * Return Value: bool
  * Description: Calls Server.WriteAllPosts()
* writeAllData
  * Parameters: none 
  * Return Value: bool
  * Description: Calls writeUsers, writePosts
* deleteUser
  * Parameters: string userID 
  * Return Value: bool
  * Description: Deletes specified user
* deletePost
  * Parameters: int postID 
  * Return Value: bool
  * Description: Deletes specified post
* addFriend
  * Parameters: string userID, string friend 
  * Return Value: bool
  * Description: Adds friend to userID.friends, and vice versa
* removeFriend
  * Parameters: string userID, string friend 
  * Return Value: bool
  * Description: Removes friend from userID.friends, and vice versa
* getFriends
  * Parameters: string userID  
  * Return Value: List\<string>
  * Description: Gets the list of friends associated with the specified user
* getFriendsPosts
  * Parameters: string userID
  * Return Value: List\<int>
  * Description: Gets the list of posts made by a user's friends
* resetData
  * Parameters: none 
  * Return Value: bool
  * Description: Calls Server.ResetData()

## social_network_test File Structure

### social_network_test

#### Description

The testing class for the social_network.cs file. 

For each function in the social_network class, there is a corresponding test_function that asserts that it return either true, or not null for functions that return a non-boolean value.

Also holds the Main function to run the program with, though this could be moved anywhere needed.
