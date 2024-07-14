'use client'

import React, { useEffect, useState } from "react";
import AuctionCard from "./AuctionCard";
import { Auction, PagedResult } from "@/types";
import AppPagination from "../components/AppPagination";
import { getData } from "../actions/auctionActions";
import Filters from "./Filters";
import { useParamsStore } from "../hooks/useParamsStore";
import { shallow } from "zustand/shallow";
import qs from 'query-string';
import EmptyFilter from "../components/EmptyFilter";



export default function Listings(){
    
    const [data, setData] = useState<PagedResult<Auction>>();
    const params = useParamsStore(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm,
        orderBy: state.orderBy,
        filterBy: state.filterBy
    }), shallow)

    const setParams = useParamsStore(state => state.setParams);
    const url = qs.stringifyUrl({url: '', query:params})

    function setPageNumber(pageNumber: number)
    {
        setParams({pageNumber});
    }
    

    useEffect(() => {
        getData(url).then(data => {
            setData(data);
        })
    }, [url])


    if(!data) return <h3>Loading...</h3>

    return(
        <>
            <Filters/>
            {data.totalCount === 0 ? (
                <EmptyFilter showReset />
            ) : (
                <>
                    <div className='grid grid-cols-4 gap-6'>
                        {data.results.map(auction => (
                            <AuctionCard auction={auction} key={auction.id}/>
                        ))}
                    </div>
                    <div className='flex overflow-x-auto sm:justify-center '>
                        <AppPagination pageChanged={setPageNumber} currentPage={params.pageNumber} pageCount={data.pageCount}/>
                    </div>
                </>
            )}
           
        </>
        
    )
}
/*
    The Listings component maintains its own pageNumber state

    --> const [pageNumber, setPageNumber] = useState(1);

    It passes setPageNumber as the pageChanged prop to the AppPagination component

    --> <AppPagination pageChanged={setPageNumber} currentPage={pageNumber} pageCount={pageCount}/>

    Refer to AppPagination Component to see what is going on there.

    This pageChanged is actually the setPageNumber function from the Listings component, 

    so calling pageChanged updates the pageNumber state in the Listings component

    When pageNumber is updated, it triggers a re-render of the Listings component
    The useEffect hook in Listings detects the change in pageNumber and calls getData to fetch the data for the new page:
    useEffect(() => {
    getData(pageNumber).then(data => {
        setAuctions(data.results);
        setPageCount(data.pageCount);
    });
}, [pageNumber]);

*/

