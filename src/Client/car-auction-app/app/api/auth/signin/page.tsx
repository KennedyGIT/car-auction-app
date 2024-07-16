import EmptyFilter from "@/app/components/EmptyFilter";
import React from "react";


type searchParamsProp = {
    searchParams : {
        callbackUrl : string
    }
}


export default function Page({searchParams} : searchParamsProp){
    return(
       <EmptyFilter
        title="You need to be logged in to do that"
        subtitle="please click the link below to sign in"
        showLogin
        callbackUrl={searchParams.callbackUrl}
       />
    )
}