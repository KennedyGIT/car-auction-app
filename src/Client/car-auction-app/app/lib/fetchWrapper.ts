import { getTokenCustom } from "../actions/authActions";

const baseUrl = 'http://localhost:6001/';

async function get(url: string){
    const requestOptions = {
        method : 'GET',
        //headers: {} 
    }

    const response = await fetch(baseUrl + url, requestOptions);
    return await handleResponse(response)
}

async function post(url: string, body: {}){
    const requestOptions = {
        method : 'POST',
        headers: await getHeaders(),
        body: JSON.stringify(body)
    }

    const response = await fetch(baseUrl + url, requestOptions);
    return await handleResponse(response)
}

async function put(url: string, body: {}){
    const requestOptions = {
        method : 'PUT',
        headers: await getHeaders(),
        body: JSON.stringify(body)
    }

    const response = await fetch(baseUrl + url, requestOptions);
    return await handleResponse(response)
}

async function del(url: string){
    const requestOptions = {
        method : 'DELETE',
        headers: await getHeaders()
    }

    const response = await fetch(baseUrl + url, requestOptions);
    return await handleResponse(response)
}

async function handleResponse(response: Response) {
    const text = await response.text();
    const data = text && JSON.parse(text);

    if(response.ok)
    {
        return data || response.statusText;
    } 
    else
    {
        const error = {
            status: response.status,
            message: response.statusText
        };

        return {error};
    }
}

async function getHeaders(){
    const token = await getTokenCustom();
    const headers = {'Content-type': 'application/json'} as any;

    if(token){
        headers.Authorization = 'Bearer '+ token.access_token
    }

    return headers;
}

export const fetchWrapper = {
    get,
    post,
    put,
    del
}
