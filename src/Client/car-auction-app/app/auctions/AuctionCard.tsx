import React from 'react'
import CountdownTimer from './CountdownTimer'
import CarImage from './CarImage'
import { Auction } from '@/types'

type Props = {
    auction: Auction
}

export default function AuctionCard({auction} : Props)
{
    return(
        <a href="#" className='group'>
            <div className='relative w-full bg-gray-200 aspect-w-16 aspect-h-10 rounded-lg overflow-hidden'>
                <div>
                    <CarImage auction={auction}/>
                    <div className='absolute bottom-6 left-6'>
                        <CountdownTimer auctionEnd={auction.auctionEnd}/>
                    </div>
                </div>
            </div>

            <div className='flex justify-between items-center mt-4'>
                <h3 className='text-gray-700'>
                   <strong>{auction.make} {auction.model}</strong> 
                </h3>
                <p className='font-semibold text-sm'>
                   <strong>
                        {auction.year}
                   </strong> 
                </p>
            </div>
        </a>
    )  
}


