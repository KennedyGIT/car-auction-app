'use client';

import React from "react";
import { RiAuctionFill } from "react-icons/ri";
import { useParamsStore } from "../hooks/useParamsStore";
import { useRouter, usePathname } from "next/navigation";

export default function Logo()
{
    const router = useRouter();
    const pathName = usePathname();
    const reset = useParamsStore(state => state.reset)

    function doReset(){
        if(pathName !== '/') router.push('/')
        reset();
    }

    return(
        <div>
            <div onClick={doReset} className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-blue-500">
                <RiAuctionFill size={34}/>
                Car Auction
            </div>
        </div>
    )    
}