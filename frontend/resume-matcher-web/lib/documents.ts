import { apiFetch } from "@/lib/apiFetch";
import { BACKEND_URL } from "@/lib/config";
import { error } from "console";
import { useState } from "react";

export type DocumentKind = "Resume" | "CoverLetter" | "Other";

export type UserDocument = {
    id: string;
    originalFileName: string;
    kind: DocumentKind;
    isDefault: boolean; 
    uploadedAt: string;
};


export async function fetchUserDocuments(): Promise<UserDocument[]>{

    const token = sessionStorage.getItem("accessToken");

 
    const res = await apiFetch(`${BACKEND_URL}/documents/user/user-documents`, {
                    method: "GET",
                    cache: "no-store",
                });
    if (!res.ok){
        throw new Error(await res.text());
    }

    return (await res.json() as UserDocument[]);

}