import { DefaultSession } from "next-auth";

// Augmenting the Session interface from next-auth module
declare module 'next-auth' {
    // Extending the existing Session interface
    interface Session {
        // Adding a new property 'user' to the Session interface
        user: {
            // Specifying that 'user' object must have a 'username' property of type string
            username: string

        } & DefaultSession['user']; // Merging with existing 'user' type from DefaultSession
    }

    interface Profile {
        username: string;
    }

    interface User {
        username: string;
        id: string;
    }
}

declare module 'next-auth/jwt'{
    interface JWT{
        username : string;
        access_token?: string;
    }
}
