
CREATE TABLE IF NOT EXISTS "Theme" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(30) UNIQUE NOT NULL
);


CREATE TABLE IF NOT EXISTS "Role" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(30) NOT NULL
);


CREATE TABLE IF NOT EXISTS "User" (
    "Id" SERIAL PRIMARY KEY,
    "RoleId" INT NOT NULL,
    "Login" VARCHAR(30) UNIQUE NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    FOREIGN KEY ("RoleId") REFERENCES "Role"("Id")
);


CREATE TABLE IF NOT EXISTS "Action" (
    "Id" SERIAL PRIMARY KEY,
    "PublicId" INT UNIQUE NOT NULL,
    "Name" VARCHAR(60) UNIQUE NOT NULL
);


CREATE TABLE IF NOT EXISTS "Log" (
    "ActionId" INT NOT NULL,
    "UserId" INT NOT NULL,
    "Date" TIMESTAMPTZ NOT NULL,
    PRIMARY KEY ("ActionId", "UserId"),
    FOREIGN KEY ("ActionId") REFERENCES "Action"("Id"),
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Channel" (
    "Id" SERIAL PRIMARY KEY,
    "OwnerId" INT NOT NULL,
    "Name" VARCHAR(30) UNIQUE NOT NULL,
    "Description" VARCHAR(255),
    "SubscribersCount" BIGINT NOT NULL DEFAULT 0,
    FOREIGN KEY ("OwnerId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Video" (
    "Id" SERIAL PRIMARY KEY,
    "ChannelId" INT NOT NULL,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(255),
    "Date" TIMESTAMPTZ NOT NULL,
    "Url" VARCHAR(255) NOT NULL,
    "IsPublic" BOOLEAN DEFAULT TRUE,
    "ViewersCount" BIGINT DEFAULT 0,
    "Rating" REAL DEFAULT 0,
    FOREIGN KEY ("ChannelId") REFERENCES "Channel"("Id")
);


CREATE TABLE IF NOT EXISTS "Subscription" (
    "ChannelId" INT NOT NULL,
    "UserId" INT NOT NULL,
    PRIMARY KEY ("ChannelId", "UserId"),
    FOREIGN KEY ("ChannelId") REFERENCES "Channel"("Id"),
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Comment" (
    "Id" SERIAL PRIMARY KEY,
    "VideoId" INT NOT NULL,
    "UserId" INT NOT NULL,
    "Text" VARCHAR(255) NOT NULL,
    "Date" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id"),
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Mark" (
    "Id" SERIAL PRIMARY KEY,
    "VideoId" INT NOT NULL,
    "UserId" INT NOT NULL,
    "IsLike" BOOLEAN,
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id"),
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "ThemeVideo" (
    "ThemeId" INT NOT NULL,
    "VideoId" INT NOT NULL,
    PRIMARY KEY ("ThemeId", "VideoId"),
    FOREIGN KEY ("ThemeId") REFERENCES "Theme"("Id"),
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id")
);


CREATE TABLE IF NOT EXISTS "Playlist" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Name" VARCHAR(30) NOT NULL,
    "VideoCount" INT NOT NULL DEFAULT 0,
    "Duration" INT NOT NULL DEFAULT 0,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "PlaylistVideo" (
    "PlaylistId" INT NOT NULL,
    "VideoId" INT NOT NULL,
    PRIMARY KEY ("PlaylistId", "VideoId"),
    FOREIGN KEY ("PlaylistId") REFERENCES "Playlist"("Id"),
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id")
);


CREATE TABLE IF NOT EXISTS "WatchHistory" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "VideoId" INT NOT NULL,
    "WatchedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id"),
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id")
);


CREATE TABLE IF NOT EXISTS "Favorite" (
    "UserId" INT NOT NULL,
    "VideoId" INT NOT NULL,
    "AddedAt" TIMESTAMPTZ NOT NULL,
    PRIMARY KEY ("UserId", "VideoId"),
    FOREIGN KEY ("UserId") REFERENCES "User"("Id"),
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id")
);


CREATE TABLE IF NOT EXISTS "Notification" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Message" VARCHAR(255) NOT NULL,
    "IsRead" BOOLEAN DEFAULT FALSE,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Report" (
    "Id" SERIAL PRIMARY KEY,
    "ReporterId" INT NOT NULL,
    "VideoId" INT,
    "CommentId" INT,
    "Reason" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("ReporterId") REFERENCES "User"("Id"),
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id"),
    FOREIGN KEY ("CommentId") REFERENCES "Comment"("Id")
);


CREATE TABLE IF NOT EXISTS "Tag" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) UNIQUE NOT NULL
);


CREATE TABLE IF NOT EXISTS "VideoTag" (
    "VideoId" INT NOT NULL,
    "TagId" INT NOT NULL,
    PRIMARY KEY ("VideoId", "TagId"),
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id"),
    FOREIGN KEY ("TagId") REFERENCES "Tag"("Id")
);


CREATE TABLE IF NOT EXISTS "VideoHistory" (
    "Id" SERIAL PRIMARY KEY,
    "VideoId" INT NOT NULL,
    "ChangedBy" INT NOT NULL,
    "ChangeDescription" VARCHAR(255) NOT NULL,
    "ChangedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("VideoId") REFERENCES "Video"("Id"),
    FOREIGN KEY ("ChangedBy") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Ban" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Reason" VARCHAR(255) NOT NULL,
    "BannedAt" TIMESTAMPTZ NOT NULL,
    "ExpiresAt" TIMESTAMPTZ,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "Session" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Token" VARCHAR(255) UNIQUE NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "ExpiresAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE TABLE IF NOT EXISTS "SearchHistory" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Query" VARCHAR(255) NOT NULL,
    "SearchedAt" TIMESTAMPTZ NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);


CREATE INDEX IF NOT EXISTS idx_user_login ON "User" ("Login");
CREATE INDEX IF NOT EXISTS idx_channel_name ON "Channel" ("Name");
CREATE INDEX IF NOT EXISTS idx_action_name_publicId ON "Action" ("Name", "PublicId");



CREATE OR REPLACE FUNCTION update_rating() RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'DELETE' THEN
        UPDATE "Video"
        SET "Rating" = (
            SELECT SUM(CASE WHEN "IsLike" IS TRUE THEN 1.0 ELSE 0 END) / COUNT(*) * 5.0
            FROM "Mark"
            WHERE "VideoId" = OLD."VideoId"
        )
        WHERE "Id" = OLD."VideoId";
    ELSE 
        UPDATE "Video"
        SET "Rating" = (
            SELECT COALESCE(SUM(CASE WHEN "IsLike" IS TRUE THEN 1.0 ELSE 0 END) / COUNT(*) * 5.0, 0)
            FROM "Mark"
            WHERE "VideoId" = NEW."VideoId"
        )
        WHERE "Id" = NEW."VideoId";
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER update_rating_trigger
AFTER INSERT OR UPDATE OR DELETE ON "Mark"
FOR EACH ROW
EXECUTE FUNCTION update_rating();


DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'mark_unique'
    ) THEN
        ALTER TABLE "Mark"
        ADD CONSTRAINT mark_unique UNIQUE ("VideoId", "UserId");
    END IF;
END$$;

