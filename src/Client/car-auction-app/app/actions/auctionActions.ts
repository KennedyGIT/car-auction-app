'use server'

import { Auction, PagedResult } from "@/types";
import { fetchWrapper } from "../lib/fetchWrapper";
import { FieldValues } from "react-hook-form";
import { revalidatePath } from "next/cache";

export async function getData(query : string) : Promise<PagedResult<Auction>>
{
    return await fetchWrapper.get(`search${query}`);
}

export async function createAuction(data: FieldValues){
    return await fetchWrapper.post('auctions', data)
}

export async function getDetails(id: string) : Promise<Auction>{
    return await fetchWrapper.get(`auctions/${id}`)
}

export async function updateAuction(id: string, data:FieldValues) {
    const res =  await fetchWrapper.put(`auctions/${id}`, data);
    revalidatePath(`/auctions/details/${id}`);
    return res;
}

export async function deleteAuction(id : string ){
    const res = await fetchWrapper.del(`auctions/${id}`);
    revalidatePath('/');
    return res;
}