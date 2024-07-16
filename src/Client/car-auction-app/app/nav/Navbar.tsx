import React from "react"
import { RiAuctionFill } from "react-icons/ri"
import Search from "./Search"
import Logo from "./Logo"
import LoginButton from "./LoginButton"
import { getCurrentUser } from "../actions/authActions"
import UserActions from "./UserActions"

export default async function Navbar()
{
    const user = await getCurrentUser();
    return(
        <header 
            className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
            <Logo/>
            <Search/>
            {user ? (
                <UserActions user = {user}/>
            ):(
                <LoginButton/>
            )}
        </header>
    )

}