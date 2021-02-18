create table AppUser (
    name text primary key,
    lastSeen timestamp
);

create table Workspace (
    name text primary key
);

create table Channel (
    name text primary key,
    belongsToWorkspace text not null references Workspace(name)
);

create table Message (
    id uuid primary key,
    createdAt timestamp not null,
    belongsToChannel text not null references Channel(name),
    createdByUser text not null references AppUser(name),
    discriminator text,
    content text,
    starterMessageId uuid
);

create table ThreadResponses (
  threadId uuid references Message(id) not null,
  responseId uuid references Message(id) not null
);



