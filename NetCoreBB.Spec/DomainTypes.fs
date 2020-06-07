module NetCoreBB.Spec.DomainTypes

// User

type Email = Email of string

type PasswordHash = PasswordHash of string


type ContactInfo =
    { Email: string }

type AdminUser =
    { ContactInfo: ContactInfo }

type ModUser = unit

type RegularUser = unit

type ArchivedUser = unit

type UnverifiedUser = unit

type BannedUser = unit

type User =
    | AdminUser
    | ModUser
    | RegularUser
    | UnverifiedUser
    | BannedUser


// Attachments

type Attachment = unit


// Private Messages

type PrivateMessage = unit


// Thread

type Title = Title of string

type Post =
    { Title: Title
      Author: User }

type Thread =
    { Title: Title
      Author: User }

type Category = unit


// Groups


// Notifications


// Content

type Language = unit

type Site =
    { Language: Language }


// Search

type SearchResult = unit


// Theming

type Theme = unit
