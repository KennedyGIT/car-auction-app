'use client';

import React from "react";
import { RiAuctionFill } from "react-icons/ri";
import { useParamsStore } from "../hooks/useParamsStore";

export default function Logo()
{
    const reset = useParamsStore(state => state.reset);
    return(
        <div>
            <div onClick={reset} className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-blue-500">
                <RiAuctionFill size={34}/>
                Car Auction
            </div>
        </div>
    )    
}